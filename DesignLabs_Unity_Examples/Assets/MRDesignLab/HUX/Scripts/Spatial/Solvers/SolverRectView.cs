//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Spatial
{
    [RequireComponent(typeof(SolverHandler))]

    /// <summary>
    ///   RectViewPoser solver locks a tag-along type object within a rectangular view pyramid...
    /// </summary>
    public class SolverRectView : Solver
	{
		#region public enums
		public enum ReferenceDirectionEnum
		{
			HeadOriented,
			HeadFacingWorldUp,
			HeadMoveDirection
		}
		#endregion

		#region public members
		[Tooltip("Which direction to position the element relative to: HeadOriented rolls with the head, HeadFacingWorldUp view dir but ignores head roll, and HeadMoveDirection uses the direction the head last moved without roll")]
		public ReferenceDirectionEnum ReferenceDirection = ReferenceDirectionEnum.HeadFacingWorldUp;

		[Tooltip("Min distance from eye to position element around, i.e. the sphere radius")]
		public float MinDistance = 3.5f;
		[Tooltip("Max distance from eye to element")]
		public float MaxDistance = 3.5f;

		[Tooltip("The element will stay at least this far away from the center of view")]
		public Vector2 MinViewDegrees = new Vector2(5f, 5f);
		[Tooltip("The element will stay at least this close to the center of view")]
		public Vector2 MaxViewDegrees = new Vector2(10f, 10f);
		[Tooltip("Offset the restricted rect by this many degrees")]
		public Vector2 OffsetViewDegrees = new Vector2(0f, 0f);

		[Tooltip("Option to ignore angle clamping")]
		public bool IgnoreAngleClamp = false;
		[Tooltip("Option to ignore distance clamping")]
		public bool IgnoreDistanceClamp = false;

		[Tooltip("If true, element will orient to ReferenceDirection, otherwise it will orient to ref pos (the head is the only option currently)")]
		public bool OrientToRefDir = false;
		[Tooltip("If true, element will zero out pitch while orienting")]
		public bool OrientVertical = false;
		#endregion

		#region private members
		private Transform head;
		#endregion

		/// <summary>
		///   ReferenceDirectoin is the direction of the pyramid.  Position to the view direction, or the movement direction
		/// </summary>
		/// <returns>Vector3, the forward direction to use for positioning</returns>
		Vector3 GetReferenceDirection()
		{
			Vector3 ret = Vector3.one;
			if (ReferenceDirection == ReferenceDirectionEnum.HeadOriented || ReferenceDirection == ReferenceDirectionEnum.HeadFacingWorldUp)
			{
				ret = head.forward;
			}
			else if (ReferenceDirection == ReferenceDirectionEnum.HeadMoveDirection)
			{
				//ret = Veil.Instance.MoveDirectionFiltered;
				ret = head.forward;
			}
			return ret;
		}

		/// <summary>
		///   Pyramid may roll with head, or not.
		/// </summary>
		/// <returns>Vector3, the up direction to use for orientation</returns>
		Vector3 GetReferenceUp()
		{
			Vector3 ret = Vector3.up;
			if (ReferenceDirection == ReferenceDirectionEnum.HeadOriented)
			{
				ret = head.up;
			}
			return ret;
		}

		/// <summary>
		///   Calculating the current view angles requires slightly different coord system
		/// </summary>
		/// <returns>Vector3, the up direction to use for calculating current angles</returns>
		Vector3 GetViewUp()
		{
			Vector3 ret = Vector3.up;
			if (ReferenceDirection != ReferenceDirectionEnum.HeadMoveDirection)
			{
				ret = head.up;
			}
			return ret;
		}

		/// <summary>
		///   Calculating the current view angles requires slightly different coord system
		/// </summary>
		/// <returns>Vector3, the right direction to use for calculating current angles</returns>
		Vector3 GetViewRight()
		{
			Vector3 ret = head.right;
			if (ReferenceDirection == ReferenceDirectionEnum.HeadMoveDirection)
			{
				ret = Vector3.Cross(GetReferenceUp(), GetReferenceDirection()).normalized;
			}
			return ret;
		}

		public override void SolverUpdate()
		{
            if (head == null)
            {
                head = Veil.Instance.HeadTransform;
            }

            if (head == null)
            {
                throw new System.Exception("Veild head transform needs to exists for the RectViewPoser to work.");
            }

			Vector3 desiredPos = WorkingPos;

			if (IgnoreAngleClamp)
			{
				if (IgnoreDistanceClamp)
				{
					//desiredPos = transform.position;
				}
				else
				{
					GetDesiredOrientation_DistanceOnly(ref desiredPos);
				}
			}
			else
			{
				GetDesiredOrientation(ref desiredPos);
			}

			// Element orientation
			Vector3 refDirUp = GetReferenceUp();
			Quaternion desiredRot = Quaternion.identity;
			if (OrientToRefDir)
			{
				desiredRot = Quaternion.LookRotation(GetReferenceDirection(), refDirUp);
			}
			else
			{
				Vector3 refPoint = Veil.Instance.HeadTransform.position;
				desiredRot = Quaternion.LookRotation(desiredPos - refPoint, refDirUp);
			}

			Vector3 euler = desiredRot.eulerAngles;
			euler.z = 0;
			if (OrientVertical)
			{
				euler.x = 0;
			}
			desiredRot.eulerAngles = euler;

			this.GoalPosition = desiredPos;
			this.GoalRotation = desiredRot;

			//UpdateWorkingToGoal();
			UpdateWorkingPosToGoal();
			UpdateWorkingRotToGoal();
		}

		/// <summary>
		///   Optimized version of GetDesiredOrientation.  There should be a different solver for distance contraint though
		/// </summary>
		/// <param name="desiredPos"></param>
		void GetDesiredOrientation_DistanceOnly(ref Vector3 desiredPos)
		{
			// Determine reference locations and directions
			Vector3 refPoint = /*Veil.Instance.HeadPositionFiltered;//*/head.transform.position;
			Vector3 elementPoint = WorkingPos;// transform.position;
			Vector3 elementDelta = elementPoint - refPoint;
			float elementDist = elementDelta.magnitude;
			Vector3 elementDir = elementDist > 0 ? elementDelta / elementDist : Vector3.one;

			// Clamp distance too
			float clampedDistance = Mathf.Clamp(elementDist, MinDistance, MaxDistance);

			//desiredPos = elementPoint;

			if (clampedDistance != elementDist)
			{
				desiredPos = refPoint + clampedDistance * elementDir;
			}
		}

		void GetDesiredOrientation(ref Vector3 desiredPos)
		{
			// Determine reference locations and directions
			Vector3 refDir = GetReferenceDirection();
			Vector3 refPoint = head.transform.position;// Veil.Instance.HeadPositionFiltered;
			Vector3 elementPoint = WorkingPos;// transform.position;
			Vector3 elementDelta = elementPoint - refPoint;
			float elementDist = elementDelta.magnitude;
			Vector3 elementDir = elementDist > 0 ? elementDelta / elementDist : Vector3.one;
			float flip = Vector3.Dot(elementDelta, refDir);

			//Transform viewTransform = head.transform;
			Vector3 viewRight = GetViewRight();
			Vector3 viewUp = GetViewUp();

			// Visualize the view transform
			//Debug.DrawLine(refPoint, refPoint + 5f * refDir, Color.red);
			//Debug.DrawLine(refPoint, refPoint + 5f * viewRight, Color.green);
			//Debug.DrawLine(refPoint, refPoint + 5f * viewUp, Color.blue);

			// Get up and right components
			Vector2 dots = new Vector2(Vector3.Dot(elementDir, viewRight), Vector3.Dot(elementDir, viewUp));
			Vector3 elementDirPerpRight = viewRight * dots.x;
			Vector3 elementDirPerpUp = viewUp * dots.y;

			// Generate the angle offset to apply to angles during clamping (the basis flips to do negative angles)
			Vector2 workingOffsetDegrees = OffsetViewDegrees;// new Vector2(OffsetViewDegrees.x * Mathf.Sign(dots.x), OffsetViewDegrees.y * Mathf.Sign(dots.y));

			// Get current angles and clamp
			Vector2 currentAngles = new Vector2(Vector3.Angle(refDir, refDir + elementDirPerpRight), Vector3.Angle(refDir, refDir + elementDirPerpUp));
			if (dots.x < 0)
			{
				currentAngles.x *= -1f;
				elementDirPerpRight *= -1f;
			}
			if (dots.y < 0)
			{
				currentAngles.y *= -1f;
				elementDirPerpUp *= -1f;
			}
			currentAngles -= workingOffsetDegrees;

			// Pull in
			Vector2 clampedAngles = currentAngles;
			clampedAngles.x = Mathf.Clamp(clampedAngles.x, -MaxViewDegrees.x, MaxViewDegrees.x);
			clampedAngles.y = Mathf.Clamp(clampedAngles.y, -MaxViewDegrees.y, MaxViewDegrees.y);

			// Push out along shortest axis, only if inside min box
			Vector2 absClamp = new Vector2(Mathf.Abs(clampedAngles.x), Mathf.Abs(clampedAngles.y));
			if (absClamp.x < MinViewDegrees.x && absClamp.y < MinViewDegrees.y)
			{
				if (Mathf.Abs(absClamp.x - MinViewDegrees.x) < Mathf.Abs(absClamp.y - MinViewDegrees.y))
				{
					clampedAngles.x = MinViewDegrees.x * Mathf.Sign(clampedAngles.x);
				}
				else
				{
					clampedAngles.y = MinViewDegrees.y * Mathf.Sign(clampedAngles.y);
				}
			}

			// Clamp distance too, if desired
			float clampedDistance = IgnoreDistanceClamp ? elementDist : Mathf.Clamp(elementDist, MinDistance, MaxDistance);


			// If the angle was clamped, do some special update stuff
			if (flip < 0)
			{
				desiredPos = refPoint + refDir;
			}
			else if (currentAngles != clampedAngles)
			{
				Vector2 anglesRad = (clampedAngles + workingOffsetDegrees) * Mathf.Deg2Rad;

				// Calculate new position
				//Vector3 newDir = elementDirPerpUp * Mathf.Sin(anglesRad.y) + Mathf.Cos(anglesRad.y) * (elementDirPerpRight * Mathf.Sin(anglesRad.x) + Mathf.Cos(anglesRad.x) * refDir);
				//newDir = elementDirPerpRight * Mathf.Sin(anglesRad.x) + Mathf.Cos(anglesRad.x) * (elementDirPerpUp * Mathf.Sin(anglesRad.y) + Mathf.Cos(anglesRad.y) * refDir);
				elementDirPerpRight.Normalize();
				elementDirPerpUp.Normalize();
				Vector3 newDir = elementDirPerpUp * Mathf.Sin(anglesRad.y) + elementDirPerpRight * Mathf.Sin(anglesRad.x) + refDir * Mathf.Cos(anglesRad.x) * Mathf.Cos(anglesRad.y);

				//Vector3 newDir = refDir * Mathf.Cos(anglesRad.x) + elementDirPerpRight * Mathf.Sin(anglesRad.x);

				desiredPos = refPoint + clampedDistance * newDir.normalized;
				// Visualize applied offset
				//Debug.DrawLine(elementPoint, refPoint + clampedDistance * newDir, Color.magenta);
				//Debug.DrawLine(refPoint + refDir * clampedDistance, refPoint + clampedDistance * newDir, Color.yellow);
			}
			else if (clampedDistance != elementDist)
			{
				// Only need to apply distance
				desiredPos = refPoint + clampedDistance * elementDir;
			}
		}
	}

}
