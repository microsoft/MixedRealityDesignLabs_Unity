//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX;
using System;

public class InputSourceEditor : InputSourceVirtual, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

	// Mouse inputs
	[HideInInspector]
	public InputSourceMouse mouseSource;

	// Depth gesture
	bool wasDragButtonPressed;
	float dragStartPosY;
	//public float DragDistanceDelta;

	// Activation
	public float MouseMinTravel = 10f;
	private const float m_MouseTravel = 0.0f;

	public Vector2ControlState dragControl = new Vector2ControlState(new Vector2(1f, 1f / 250f), false);
	ButtonControlState readyButton = new ButtonControlState();

	bool bReady;

	private bool m_SelectDown;
	private bool m_MenuDown;

	void Start()
	{
		if (IsEnabled)
		{
			mouseSource = InputSources.Instance.mouse;
		}
		else
		{
			mouseSource = gameObject.AddComponent<InputSourceMouseDisabled>();
		}
	}

	public override void _Update()
	{
		UpdateDrag();

		readyButton.ApplyState(mouseSource.ButtonMiddlePressed);

		if (readyButton.PressedOnce)
		{
			bReady = !bReady;
		}

		bool prev = m_SelectDown;
		m_SelectDown = (this as ITargetingInputSource).IsSelectPressed();
		if (prev != m_SelectDown)
		{
			OnSelectChanged(this, m_SelectDown);
		}

		prev = m_MenuDown;
		m_MenuDown = (this as ITargetingInputSource).IsMenuPressed();
		if (prev != m_MenuDown)
		{
			OnMenuChanged(this, m_MenuDown);
		}

		base._Update();
	}

	void UpdateDrag()
	{
		bool dragButtonPressed = Input.GetKey(KeyCode.P);
		float dragPos = mouseSource.MousePos.y;
		if (dragButtonPressed)
		{
			if (!wasDragButtonPressed)
			{
				dragStartPosY = dragPos;
				//DragDistanceDelta = 0;
				dragControl.ApplyPos(true, Vector2.zero);
			}
			else
			{
				// Also update drag distance delta
				dragControl.ApplyPos(true, new Vector2(0, dragPos - dragStartPosY));
			}
		}
		else
		{
			dragControl.ApplyPos(false, Vector2.zero);
		}

		wasDragButtonPressed = dragButtonPressed;
	}

	// ITargetingInputSource
	bool ITargetingInputSource.ShouldActivate()
	{
		return m_MouseTravel >= MouseMinTravel || mouseSource.ButtonLeftPressed;
	}
	Vector3 ITargetingInputSource.GetTargetOrigin()
	{
		return Veil.Instance.HeadTransform.position;
	}
	Quaternion ITargetingInputSource.GetTargetRotation()
	{
		return Veil.Instance.HeadTransform.rotation;
	}
	bool ITargetingInputSource.IsSelectPressed()
	{
		return InputSources.Instance.mouse.ButtonLeftPressed;
	}
	bool ITargetingInputSource.IsMenuPressed()
	{
		return Input.GetKey(KeyCode.BackQuote);
	}
	void ITargetingInputSource.OnActivate(bool activated)
	{
	}
	bool ITargetingInputSource.IsReady()
	{
		//return HUX.Receivers.InputHandReceiver.HandVisible;
		return bReady;
	}

	bool ITargetingInputSource.IsTargetingActive()
	{
		return bReady;
	}
}
