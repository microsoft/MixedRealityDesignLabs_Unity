//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using HUX;

public class InputSourceSixDOFBase : InputSourceVirtual, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

	// SixDOF comes from InputSourceSixDOFRay
	public virtual Vector3 position
	{
		get
		{
			return Vector3.zero;
		}
	}

	public virtual Quaternion rotation
	{
		get
		{
			return Quaternion.identity;
		}
	}

	// Buttons come from InputSourceNetMouse
	public virtual bool buttonSelectPressed
	{
		get
		{
			return false;
		}
	}

	public virtual bool buttonMenuPressed
	{
		get
		{
			return false;
		}
	}

	public virtual bool buttonAltSelectPressed
	{
		get
		{
			return false;
		}
	}

	// Aim precision stuff
	public enum EPrecisionMode
	{
		None,
		Smoothed,
		Velocity,
		VelocityAlignedScale,
		Zone,       // Precision based on radius from 'zone', zone follows when real dir moves out of it
		MovingZone, // 'Zone' follows average of precise dir
		ViewCenterDist,
		AvgPosDist,
		GyroMouse
	}

	public EPrecisionMode PrecisionMode = EPrecisionMode.Zone;

	Quaternion preciseRotation = Quaternion.identity;
	Quaternion prevRot = Quaternion.identity;

	public float preciseScale = 0.2f;

	// State vars
	Vector3 curDir;
	Vector3 lastDir;
	Vector3 dirDelta;
	Vector3 preciseDir;

	// Precision mode: Velocity
	float averageVel;
	public float velAvgRate = 5f;
	public Vector2 velRange = new Vector2(0, 2f);

	// Precision mode: Zone
	public Vector2 angRange = new Vector2(8f, 16f);
	public Vector2 slerpRangeAng = new Vector2(0, 0.2f);

	// Precision mode: Moving zone
	public float avgPreciseDirRate = 1f;
	Vector3 avgPreciseDir;

	// Depth gesture
	bool prevDragging;
	float dragStartDistance;
	[DebugTunable]
	public float dragDistanceDeadzone = 0.1f;

	public Vector2ControlState dragControl = new Vector2ControlState(false);

	// Touch pad input on the remote mouse
	public Vector2ControlState touchState = new Vector2ControlState(5f * Vector2.one);

	public GameObject debugSphere;

	Vector3 prevPosition;

	private bool m_Select;
	private bool m_Menu;

	public void SetPreciseMode(EPrecisionMode mode)
	{
		PrecisionMode = mode;
	}

	public override void _Update()
	{
		// We can't trust button input from the remote mouse, since it generates a lot of click events when you're only trying to move it
		touchState.ApplyPos(false, InputSources.Instance.netMouse.mousePos);

		UpdateDrag();

		UpdatePrecision();

		bool prev = m_Select;
		m_Select = (this as ITargetingInputSource).IsSelectPressed();
		if (prev != m_Select)
		{
			OnSelectChanged(this, m_Select);
		}

		prev = m_Menu;
		m_Menu = (this as ITargetingInputSource).IsMenuPressed();
		if (prev != m_Menu)
		{
			OnMenuChanged(this, m_Menu);
		}

		base._Update();
	}

	float GetOriginDistance()
	{
		Vector3 headPos = Veil.Instance.HeadTransform.position;
		Vector3 headToHand = position - headPos;

        if (position.y < headPos.y)
		{
			headToHand.y = 0;
		}

        return headToHand.magnitude;
	}

	void UpdateDrag()
	{
		bool isDragging = buttonSelectPressed && IsManipulating();

		if (isDragging)
		{
			if (!prevDragging)
			{
				dragControl.ApplyPos(false, Vector2.zero);
			}
			else
			{
				Vector3 posDelta = position - prevPosition;
				float pdDot = Vector3.Dot(posDelta.normalized, rotation * Vector3.forward);
				float distDelta = pdDot * posDelta.magnitude;

				dragControl.ApplyDelta(true, new Vector2(0, distDelta));
			}
		}
		else
		{
			dragControl.ApplyPos(false, Vector2.zero);
		}

        // Debug sphere
		if (debugSphere != null)
		{
			debugSphere.transform.position = position;// + rotation * Vector3.forward;
		}

		prevPosition = position;
		prevDragging = isDragging;
	}

	void UpdatePrecision()
	{
		curDir = rotation * Vector3.forward;
		dirDelta = curDir - lastDir;
		preciseDir = preciseRotation * Vector3.forward;
		averageVel = Mathf.Lerp(averageVel, dirDelta.magnitude / Time.deltaTime, velAvgRate);

		if (PrecisionMode == EPrecisionMode.Smoothed)
		{
			preciseRotation = MathUtilities.DynamicExpDecay(preciseRotation, rotation, 3f);
		}
		else if (PrecisionMode == EPrecisionMode.Velocity)
		{
			// Determine aimScale by the current speed of real direction
			float velP = Mathf.InverseLerp(velRange.x, velRange.y, averageVel);
			float aimScale = Mathf.Lerp(preciseScale, 1f, velP);

			Quaternion startPrecise = preciseRotation;

			// Apply relative delta, scaled down by aimScale
			PreciseApplyRotDeltaScaled(aimScale);

			// Do some slerping to real dir when moving fast, since it's bad to 
			if (aimScale > 0.9f)
			{
				PreciseSlerpToCurrent(Mathf.Lerp(0, 0.3f, Mathf.InverseLerp(0.9f, 1f, aimScale)));
			}

			preciseRotation = MathUtilities.DynamicExpDecay(startPrecise, preciseRotation, 3f);
		}
		else if (PrecisionMode == EPrecisionMode.VelocityAlignedScale)
		{
			// Determine aimScale by the current speed of real direction
			float velP = Mathf.InverseLerp(velRange.x, velRange.y, averageVel);
			float aimScale = Mathf.Lerp(preciseScale, 1f, velP);

			float finalCatchupAmt = Mathf.InverseLerp(0.1f, 1f, velP);

			// Also apply a max angle limit
			float zoneScale = GetZoneScale(curDir, preciseDir, new Vector2(0, 1f));
			if (zoneScale > 0.9f)
			{
				float zoneAmt = Mathf.InverseLerp(0.9f, 1f, zoneScale);
				finalCatchupAmt = Mathf.Max(zoneAmt, finalCatchupAmt);
			}

			PreciseApplyRotDeltaCatchupScaled(aimScale, finalCatchupAmt);

			// Do some slerping to real dir when moving fast, since it's bad to get too far away
			if (aimScale > 0.9f)
			{
				PreciseSlerpToCurrent(Mathf.Lerp(0, 0.3f, Mathf.InverseLerp(0.9f, 1f, aimScale)));
			}
		}
		else if (PrecisionMode == EPrecisionMode.Zone)
		{
			float zoneScale = GetZoneScale(curDir, preciseDir, new Vector2(preciseScale, 1f));

			PreciseApplyRotDeltaCatchupScaled(zoneScale, Mathf.InverseLerp(0.8f, 1f, zoneScale));
		}
		else if (PrecisionMode == EPrecisionMode.MovingZone)
		{
			// Update avgPreciseDir
			avgPreciseDir = Vector3.Slerp(avgPreciseDir, preciseDir, Time.deltaTime * avgPreciseDirRate);

			float zoneScale = GetZoneScale(curDir, avgPreciseDir, new Vector2(preciseScale, 1f));

			PreciseApplyRotDeltaCatchupScaled(zoneScale, Mathf.InverseLerp(0.8f, 1f, zoneScale));
		}

		lastDir = curDir;
		prevRot = rotation;
	}

	float NormalizeDotRange(float d)
	{
		return Mathf.InverseLerp(-1f, 1f, d);
	}

	float GetZoneScale(Vector3 dir, Vector3 zoneDir, Vector2 outRange)
	{
		float ang = Vector3.Angle(dir, zoneDir);
		return Mathf.Lerp(outRange.x, outRange.y, Mathf.InverseLerp(angRange.x, angRange.y, ang));
	}

	// Dot the input delta with the direction away from the precise dir
	// I.e. are we moving away from the precise zone
	float GetDeltaPreciseAlignment()
	{
		Vector3 preciseDelta = curDir - preciseDir;
		return Vector3.Dot(dirDelta.normalized, preciseDelta.normalized);
	}

	void PreciseSlerpToCurrentAligned(float amount, float alignPower)
	{
		//float slerpAmount = GetPreciseZoneScale(curDir, slerpRangeAng);
		float dirDotDelta = GetDeltaPreciseAlignment();

		//Debug.Log("ddd: " + dirDotDelta + ", sa: " + slerpAmount + " ang: " + ang);

		dirDotDelta = Mathf.Abs(dirDotDelta);
		//if (dirDotDelta > 0)
		{
			PreciseSlerpToCurrent(amount * Mathf.Pow(dirDotDelta, alignPower));
		}
	}

	void PreciseSlerpToCurrent(float pct)
	{
		preciseRotation = Quaternion.Slerp(preciseRotation, rotation, pct);
	}

	void PreciseApplyRotDeltaCatchupScaled(float scale, float alignAmount)
	{
		// Compute alignment scale for 'catchup'
		float alignScale = Mathf.Lerp(0.2f, 1.8f, NormalizeDotRange(GetDeltaPreciseAlignment()));

		// Blend from input scale to alignScale
		float finalScale = Mathf.Lerp(scale, alignScale, alignAmount);

		PreciseApplyRotDeltaScaled(finalScale);

	}

	void PreciseApplyRotDeltaScaled(float scale)
	{
		Quaternion rotDelta = rotation * Quaternion.Inverse(prevRot);
		preciseRotation = Quaternion.Lerp(Quaternion.identity, rotDelta, scale) * preciseRotation;
	}

	// ITargetingInputSource
	bool ITargetingInputSource.ShouldActivate()
	{
		return buttonSelectPressed;
	}
	Vector3 ITargetingInputSource.GetTargetOrigin()
	{
		return position;
	}
	Quaternion ITargetingInputSource.GetTargetRotation()
	{
		return PrecisionMode == EPrecisionMode.None ? rotation : preciseRotation;
	}

	bool ITargetingInputSource.IsSelectPressed()
	{
		return buttonSelectPressed;
	}
    bool ITargetingInputSource.IsMenuPressed()
    {
        return buttonMenuPressed;
    }
    void ITargetingInputSource.OnActivate(bool activated)
	{

	}
	bool ITargetingInputSource.IsReady()
	{
		return true;
	}
	bool ITargetingInputSource.IsTargetingActive()
	{
		return true;
	}
}

