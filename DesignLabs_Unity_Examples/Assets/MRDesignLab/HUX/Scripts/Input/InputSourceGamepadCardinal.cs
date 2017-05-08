//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX;
using System;
using HUX.Focus;

public class InputSourceGamepadCardinal : InputSourceVirtual, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

	public InputSourceGamepad gamepadSource;

	public GameObject currentTarget;

	public Action<bool> OnActivate = (b) => { };

	private bool m_Select;
	private bool m_Menu;

	void Start()
	{
		gamepadSource = InputSources.Instance.gamepad;

		gamepadSource.gamepadSource.bButtonState.OnChanged += DeactivateEvent;
	}

	void DeactivateEvent()
	{
		if (gamepadSource.gamepadSource.bButtonState.pressed)
		{
			DeactivateCardinalInput();
		}
	}

	public void DeactivateCardinalInput()
	{
		bool wasActive = IsActiveTargetingSource();
		SetTargetControl(null);

		if (wasActive)
		{
			// TODO: Maybe should have a stack?  Or is cardinal always going to defer to gamepad when cancelled.  Maybe.
			InputShellMap.Instance.inputSwitchLogic.ActivateTargetingSource(InputSources.Instance.gamepad);
		}
	}

	public void SetTargetControl(GameObject target)
	{
		if (currentTarget != target)
		{
			currentTarget = target;
			if (currentTarget == null)
			{
				DeactivateCardinalInput();
			}
		}
	}

	public override void _Update()
	{
		base._Update();

		if (IsActiveTargetingSource())
		{
			AFocuser focuser = InputShellMap.Instance.inputSwitchLogic.GetFocuserForCurrentTargetingSource();

			if (focuser != null && focuser.Cursor != null)
			{
				focuser.Cursor.gameObject.SetActive(currentTarget == null);
			}
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
	}

	// Targeting source interface
	bool ITargetingInputSource.ShouldActivate()
	{
		// Moving this logic to application level to request cardinal input..
		return false;// InputShell.Instance.CardinalVector.delta != Vector2.zero && Shell.Instance.IsPinsPanelActive();
	}

	Vector3 ITargetingInputSource.GetTargetOrigin()
	{
		return Veil.Instance.HeadTransform.position;
	}

	Quaternion ITargetingInputSource.GetTargetRotation()
	{
		if (currentTarget != null)
		{
			Vector3 targetPos = currentTarget.transform.position;
			Collider c = currentTarget.GetComponent<Collider>();
			if (c != null)
			{
				targetPos = c.bounds.center;
			}

			Vector3 targetDir = targetPos - Veil.Instance.HeadTransform.position;
			return Quaternion.LookRotation(targetDir);
		}
		return Veil.Instance.HeadTransform.rotation;
	}

	bool ITargetingInputSource.IsSelectPressed()
	{
		return currentTarget != null && gamepadSource.aButtonPressed;
	}

	bool ITargetingInputSource.IsMenuPressed()
	{
		return gamepadSource.startButtonPressed;
	}

	void ITargetingInputSource.OnActivate(bool activated)
	{
		OnActivate(activated);

		if (!activated)
		{
			AFocuser focuser = InputShellMap.Instance.inputSwitchLogic.GetFocuserForCurrentTargetingSource();

			if (focuser != null && focuser.Cursor != null)
			{
				focuser.Cursor.gameObject.SetActive(true);
			}
		}
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
