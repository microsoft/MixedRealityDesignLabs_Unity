//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX;
using System;
using UnityEngine;

public class InputSourceWorldCursorBase : InputSourceVirtual, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

	public WorldCursor worldCursor
	{
		get
		{
			return InputShell.Instance.worldCursor;
		}
	}

	public float Speed = 5f;
	public float AccelMax = 2.5f;
	public float AccelSmoothness = 10f;
	public Vector2 AccelRange = new Vector2(0.5f, 1f);

	float cursorAccelP = 0;

	// Activation
	public float InputMinAmount = 10f;
	protected float inputAmount;

	private bool m_Select;
	private bool m_Menu;

	public virtual bool getSelectButtonPressed()
	{
		return false;
	}

	public virtual bool getMenuButtonPressed()
	{
		return false;
	}

	public void OnMoveInput(Vector2 delta)
	{
		if (IsActiveTargetingSource())
		{
			// Apply acceleration
			float mouseMag = delta.magnitude;
			float curP = Mathf.InverseLerp(AccelRange.x, AccelRange.y, mouseMag);
			cursorAccelP = Mathf.Lerp(cursorAccelP, curP, AccelSmoothness * Time.deltaTime);
			float scale = Speed * (1f + cursorAccelP * AccelMax);
			delta *= scale;

            if (worldCursor != null)
            {
                // Move the world cursor
                worldCursor.ApplyMovementDelta(delta);
            }

			// Update focus targets of the focus manager and mixed world app manager to point at the world cursor
			//Vector3 newFocusRay = worldCursor.transform.position - Veil.Instance.HeadTransform.position;
			inputAmount = 0;
		}
		else
		{
			inputAmount += delta.magnitude;
		}
	}

	public virtual Vector2 getInputDelta()
	{
		return Vector2.zero;
	}

	public override void _Update()
	{
		if (!IsEnabled)
		{
			return;
		}

		// Read move delta from mouse device and apply it
		Vector2 delta = getInputDelta();
		if (delta != Vector2.zero)
		{
			OnMoveInput(delta);
		}

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

	// ITargetingInputSource
	bool ITargetingInputSource.ShouldActivate()
	{
		return inputAmount >= InputMinAmount || getSelectButtonPressed();
	}
	Vector3 ITargetingInputSource.GetTargetOrigin()
	{
		return Veil.Instance.HeadTransform.position;
	}
	Quaternion ITargetingInputSource.GetTargetRotation()
	{
        if (worldCursor == null)
            return Quaternion.identity;

		return Quaternion.LookRotation(worldCursor.transform.position - Veil.Instance.HeadTransform.position);
	}

	bool ITargetingInputSource.IsSelectPressed()
	{
		return getSelectButtonPressed();
	}

	bool ITargetingInputSource.IsMenuPressed()
	{
		return getMenuButtonPressed();
	}

	void ITargetingInputSource.OnActivate(bool activated)
	{
        if (worldCursor == null)
            return;

        worldCursor.SetCursorActive(activated);
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
