//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Utility;
using HUX;

/// <summary>
/// Base class for all input sources, i.e. hands, fawkes, mouse, etc
/// </summary>
public class InputSourceBase : MonoBehaviour
{
	/// <summary>
	/// An input source is virtual if it references another InputSource for its inputs.
	/// This means it should be updated after whichever it references.  Currently all
	/// virtual inputs are sorted after non-virtual ones, but a proper dependency sort
	/// could be implemented for virtual inputs which reference other virtual inputs.
	///
	/// Virtual input sources should inherit from InputSourceVirtual.
	/// 
	/// To implement a new input source, derive from InputSourceBase.  Derive from InputSourceVirtual
	/// if the source will use input from another input source.  Implement ITargetingInputSource
	/// if the source can be activated for targeting.  Then, add a member to InputSources of the type
	/// of the input source you just created, and create an instance of the component under the
	/// appropriate game object under InputMapping in the Veil prefab.  In InputShellMap, route
	/// input from your new source to the appropriate semantics in ApplyInputToShell()
	/// </summary>
	public virtual bool IsVirtual
	{
		get { return false; }
	}

	/// <summary>
	/// If false, this input source is not updated
	/// </summary>
	public bool IsEnabled = true;

	/// <summary>
	/// For ITargetingInputSources, when false, they will never activate, but are still updated
	/// </summary>
	public bool CanActivate = true;

	private bool m_IsManipulating = false;
	private Transform m_ManipulationTransform = null;

	public Transform ManipulationTransform
	{
		get
		{
			return m_ManipulationTransform;
		}
	}

	/// <summary>
	/// Helper to determine if this input is active
	/// </summary>
	public bool IsActiveTargetingSource()
	{
		return InputShellMap.Instance.inputSwitchLogic.IsTargetingSourceActive(this);
	}

	/// <summary>
	/// Helper to determine if something is being manipulated
	/// </summary>
	public bool IsManipulating()
	{
		return m_IsManipulating;
	}

	public void StartManipulating(Transform frame = null)
	{
		if (!m_IsManipulating)
		{
			m_IsManipulating = true;
			m_ManipulationTransform = frame;
		}
	}

	public void StopManipulating()
	{
		if (m_IsManipulating)
		{
			m_IsManipulating = false;
			m_ManipulationTransform = null;
		}
	}

	/// <summary>
	/// Intersect the targeting ray with a plane centered on the specified transform
	/// </summary>
	public bool GetTargetPoint(Transform frame, out Vector3 targetPoint)
	{
		Plane p = new Plane(frame.forward, frame.position);
		return GetTargetPoint(p, out targetPoint);
	}

	/// <summary>
	/// Returns the origin of the current targeting source
	/// </summary>
	private Vector3 GetOrigin()
	{
		if (this is ITargetingInputSource)
		{
			return (this as ITargetingInputSource).GetTargetOrigin();
		}
		return Veil.Instance.HeadTransform.position;
	}

	/// <summary>
	/// Returns the rotation of the current targeting source
	/// </summary>
	private Quaternion GetOrientation()
	{
		if (this is ITargetingInputSource)
		{
			return (this as ITargetingInputSource).GetTargetRotation();
		}
		return Veil.Instance.HeadTransform.rotation;
	}

	/// <summary>
	/// Intersects the targeting ray with the specified plane
	/// </summary>
	public bool GetTargetPoint(Plane plane, out Vector3 targetPoint)
	{
		Ray targetingRay = new Ray(GetOrigin(), GetOrientation() * Vector3.forward);

		float enter = -1f;
		if (plane.Raycast(targetingRay, out enter))
		{
			targetPoint = targetingRay.origin + targetingRay.direction * enter;
			return true;
		}
		targetPoint = Vector3.zero;
		return false;
	}

	/// <summary>
	/// If manipulation is happening, get the point where the cursor intersects the manipulation surface,
	/// which is a plane centered on the transform
	/// </summary>
	public Vector3 GetManipulationPoint()
	{
		Vector3 point = Vector3.zero;
		if (m_ManipulationTransform != null)
		{
			GetTargetPoint(m_ManipulationTransform, out point);
		}
		else
		{
			point = GetOrigin() + GetOrientation() * Vector3.forward;
		}
		return point;
	}

	/// <summary>
	/// Gets X,Y coords for the manipulation point, local to the manipulation surface
	/// </summary>
	public Vector2 GetManipulationPlaneProjection()
	{
		if (m_ManipulationTransform != null)
		{
			Vector3 delta = GetManipulationPoint() - m_ManipulationTransform.position;
			Vector2 coords = new Vector2(Vector3.Dot(delta, m_ManipulationTransform.right), Vector3.Dot(delta, m_ManipulationTransform.up));
			return Vector2.Scale(coords, new Vector2(1f / m_ManipulationTransform.lossyScale.x, 1f / m_ManipulationTransform.lossyScale.y));
		}
		return Vector2.zero;
	}

	/// <summary>
	/// Don't use Update, use _Update, which is called on all active sources by InputShellMap.
	/// Input sources are updated manually by InputShellMap, so they will have the latest data
	/// at the right time.
	/// </summary>
	public virtual void _Update() { }

	// ---------------------------------------------------------------------------------------------

	#region Network transport helpers

	/// <summary>
	/// Helpers for input sources which use a network transport
	/// </summary>
	public int readNormalizedFloat(out float val, byte[] bytes, int index)
	{
		int newIndex = index;
		int b1 = (int)bytes[newIndex++];
		int b2 = (int)bytes[newIndex++];
		val = (float)(b1 * 255 + b2);
		val /= 32512f;
		val -= 1f;
		return newIndex - index;
	}

	// Converts rotation to yaw/pitch mapping +/-180 degrees to (-1,1)
	// Most 
	public static Vector2 normalizedYawPitch(Quaternion rot)
	{
		float yaw = rot.eulerAngles.y;
		float pitch = rot.eulerAngles.x;

		if (yaw > 180f)
		{
			yaw -= 360f;
		}
		if (pitch > 180f)
		{
			pitch -= 360f;
		}
		return new Vector2(yaw / 180f, -pitch / 180f);
	}

	#endregion
}

/// <summary>
/// Base class for virutal input sources
/// </summary>
public class InputSourceVirtual : InputSourceBase
{
	public override bool IsVirtual
	{
		get { return true; }
	}
}


/// <summary>
/// ITargetingInputSource, interface for input sources which can be switched between for targeting.
/// </summary>
public interface ITargetingInputSource
{
	event System.Action<InputSourceBase, bool> OnSelectChanged;
	event System.Action<InputSourceBase, bool> OnMenuChanged;

	/// <summary>
	/// If true is returned, this input source will be activated as the active source
	/// </summary>
	bool ShouldActivate();

	/// <summary>
	/// Gets the targeting source's current origin
	/// </summary>
	Vector3 GetTargetOrigin();

	/// <summary>
	/// Gets the targeting source's current rotation
	/// </summary>
	Quaternion GetTargetRotation();

	/// <summary>
	/// Callback when activated or deactivated
	/// </summary>
	void OnActivate(bool activated);

	/// <summary>
	/// Returns true when the select input is activated
	/// </summary>
	bool IsSelectPressed();

	/// <summary>
	/// Returns true when the menu input is activated
	/// </summary>
	bool IsMenuPressed();

	/// <summary>
	/// Distinguishes between ready and non-ready state while active.  Used only by hands for cursor right now
	/// </summary>
	bool IsReady();

	bool IsTargetingActive();
}
