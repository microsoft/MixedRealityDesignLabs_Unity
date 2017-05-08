//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Interaction;

namespace HUX.Receivers
{
	/// <summary>
	/// Simplified Affordance Interactible for move/rotate/scale.
	/// </summary>
	public class TransformReceiver : InteractionReceiver
	{

		#region public members
		public enum TranslationTypeEnum
		{
			Move,
			Rotate,
			Scale
		}

		[Tooltip("Type of translation")]
		public TranslationTypeEnum TranslationType;

		[Tooltip("Dampening applied to transform translation")]
		public float damping = 0.9f;

		[Tooltip("Speed to apply translation updates")]
		public float speed = 1.0f;

		[Tooltip("Sensitivity used for transform translation")]
		public float sensitivity = 7.0f;

		[Tooltip("Vector filter for non uniform translations")]
		public Vector3 SpatialFilter = new Vector3(1.0f, 1.0f, 1.0f);

		[Tooltip("Target object to apply the translation on")]
		public GameObject TargetObject;
		#endregion

		#region private members
		private Vector3 vDown;
		private Vector3 vDrag;

		private bool bDragging;
		private bool bSelected;

		private float angularVelocity;
		private Vector3 rotationAxis;

		private Vector3 handOrigin;
		private Vector3 handPos;
		#endregion

		void Start()
		{
			bDragging = false;
			angularVelocity = 0;
			rotationAxis = Vector3.zero;
		}

		void Update()
		{
			// on mouse down
			if (bSelected)
			{
				Transform headTransform = Veil.Instance.HeadTransform;
				handPos = Veil.Instance.HandPosition;

				if (!bDragging)
				{
					// extract vDown from the RaycastHit
					vDown = handPos - handOrigin;

					// start dragging
					bDragging = true;
				}
				else
				{
					// extract vDrag from the RaycastHit
					vDrag = handPos - handOrigin;

					// Rotate the hand offset based on the head facing.
					Quaternion quatForward = headTransform.rotation;
					vDrag = quatForward * vDrag;

					switch (TranslationType)
					{
						case TranslationTypeEnum.Move:
							TargetObject.transform.position += vDrag;
							break;
						case TranslationTypeEnum.Rotate:
							// compute the rotation axis and angular velocity from vDown and vDrag
							rotationAxis = Vector3.Cross(vDrag, vDown);
							angularVelocity = Vector3.Angle(vDrag, vDown) * speed;

							// apply the angular velocity
							if (angularVelocity > 0)
							{
								TargetObject.transform.Rotate(rotationAxis, angularVelocity * Time.deltaTime, UnityEngine.Space.World);
								angularVelocity = (angularVelocity > 0.01f) ? angularVelocity * damping : 0;
							}
							break;
						case TranslationTypeEnum.Scale:
							TargetObject.transform.localScale += vDrag;
							break;
					}
				}
			}

			// Not selected stop dragging
			if (!bSelected)
				bDragging = false;
		}

		protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			handOrigin = InputSources.Instance.hands.GetWorldPosition(InputSourceHands.FirstHandIndex);
			bSelected = true;
		}

		protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			bSelected = false;
		}

		protected override void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			bSelected = false;
		}
	}
}
