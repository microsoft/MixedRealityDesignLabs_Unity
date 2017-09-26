//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX;
using HUX.Focus;

public class BeamController : MonoBehaviour
{
	LineRenderer lineRenderer;

	[HideInInspector]
	public bool beamActive;

	Vector3[] linePoints = new Vector3[2];

	public Color startColor = new Color(1f, 1f, 1f, 0.3f);
	public Color endColor = Color.blue;

	// Curve
	public int NumCurvePoints = 10;
	float CurveModeScale = 0;

	// Color fade
	public Vector2 ActionBrightnessRange = new Vector2(0, 1f);
	public float motionAmount;
	[DebugNumberTunable(null, 0, 50f)]
	public float motionDecay = 0.05f;
	[DebugNumberTunable(null, 0, 200f)]
	public float motionScale = 200f;
	[DebugNumberTunable(null, 0, 200f)]
	public float motionDirScale = 200f;
	public float colorFadeAmount = 0.5f;
	Vector3 avgBeamOrigin;
	Vector3 avgBeamDirection;
	Vector3 lastBeamOrigin;
	Vector3 lastBeamDirection;

	private AFocuser m_Focuser;

	void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
		SetBeamColor(0);
	}

	private void OnEnable()
	{
		AddCallback();
	}

	private void OnDisable()
	{
		//Make sure they have not already been destroyed.
		if (m_Focuser != null && m_Focuser.Cursor != null)
		{
			m_Focuser.Cursor.OnPostUpdateTransform -= PostCursorUpdateTransform;
		}
	}

	public void Init(AFocuser focuser)
	{
		m_Focuser = focuser;
		AddCallback();
	}

	private void AddCallback()
	{
		if (m_Focuser != null && m_Focuser.Cursor != null)
		{
			m_Focuser.Cursor.OnPostUpdateTransform += PostCursorUpdateTransform;
		}
	}


	Color colorWithAlpha(Color c, float a)
	{
		return new Color(c.r, c.g, c.b, a * c.a);
	}

	void PostCursorUpdateTransform()
	{
		if (beamActive && m_Focuser != null && m_Focuser.Cursor != null)
		{
			Vector3 beamOrigin = m_Focuser.TargetOrigin;
			Vector3 beamEnd = m_Focuser.Cursor.GetCursorOrWidgetPosition();
			SetBeamPoints(beamOrigin, m_Focuser.TargetDirection, beamEnd);
		}
	}

	public void SetBeamPoints(Vector3 origin, Vector3 direction, Vector3 target)
	{
		avgBeamOrigin = Vector3.Lerp(avgBeamOrigin, origin, 5f * Time.deltaTime);
		avgBeamDirection = Vector3.Lerp(avgBeamDirection, direction, 5f * Time.deltaTime);

		float finalCurveScale = /*SixDOFDemo.shared.CurveScale*/1f * CurveModeScale;
		if (finalCurveScale == 0)
		{
			SetLinePointsLength(2);

			linePoints[0] = origin;
			linePoints[1] = target;
		}
		else
		{
			SetLinePointsLength(NumCurvePoints);

			// Calculate start and end vectors
			Vector3 toTarget = target - origin;
			float dist = toTarget.magnitude;

			direction = Vector3.Lerp(toTarget.normalized, direction, finalCurveScale);

			Vector3 endDir = (origin + direction * dist) - target;
			float maxEndLen = Mathf.Min(dist, endDir.magnitude);
			endDir.Normalize();

			//dist *= finalCurveScale;

			// Generate curve points
			for (int i = 0; i < NumCurvePoints; ++i)
			{
				float p = i / (float)(NumCurvePoints - 1);

				Vector3 dirP = origin + direction * p * dist;
				Vector3 tarP = target + endDir * (1f - p) * maxEndLen;
				linePoints[i] = Vector3.Lerp(dirP, tarP, p);
			}
		}

		lineRenderer.SetPositions(linePoints);
	}

	void SetLinePointsLength(int len)
	{
		if (linePoints.Length != len)
		{
			linePoints = new Vector3[len];
            lineRenderer.positionCount = len;
		}
	}

	void Update()
	{
		if (beamActive)
		{
			// Update beam curve amount
			if (m_Focuser.IsManipulating)
			{
				if (m_Focuser.PrimeFocus != null && m_Focuser.PrimeFocus.GetComponent<ScrollableContent>() != null)
				{
					CurveModeScale = 0.25f;
				}
				else
				{
					CurveModeScale = 0.75f;
				}

				ActionBrightnessRange = new Vector2(0.5f, 1f);
			}
			else
			{
				float heldAmt = (m_Focuser.IsSelectPressed ? 0.25f : 0);
				CurveModeScale = 0;
				ActionBrightnessRange = new Vector2(heldAmt, 0.5f + heldAmt);
			}

			// Update motion amount 
			motionAmount += ((avgBeamOrigin - lastBeamOrigin).magnitude * motionScale + (avgBeamDirection - lastBeamDirection).magnitude * motionDirScale) * Time.deltaTime;
			motionAmount -= motionDecay * Time.deltaTime;
			motionAmount = Mathf.Clamp01(motionAmount);

			// Apply color
			SetBeamColor(Mathf.Lerp(ActionBrightnessRange.x, ActionBrightnessRange.y, motionAmount));

			lastBeamOrigin = avgBeamOrigin;
			lastBeamDirection = avgBeamDirection;
		}
		else
		{
			motionAmount = 0;
		}
	}

	public void SetBeamColor(float brightnessP)
	{
		float colorP = Mathf.Lerp(1f, brightnessP, colorFadeAmount);
        lineRenderer.startColor = colorWithAlpha(colorP * startColor, brightnessP);
        lineRenderer.endColor = colorWithAlpha(colorP * endColor, brightnessP);
	}

	public void SetBeamActive(bool active)
	{
		if (beamActive != active)
		{
			lineRenderer.enabled = active;

			beamActive = active;
		}
	}
}
