//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX;

public class InputSourceWorldCursorMouse : InputSourceWorldCursorBase
{
	// Input from mouse
	public InputSourceMouse mouseSource;
	public bool lockMouse = true;

	public override bool getSelectButtonPressed()
	{
		return mouseSource.ButtonLeftPressed;
	}

	public override bool getMenuButtonPressed()
	{
		return mouseSource.ButtonMiddlePressed;
	}

	public override Vector2 getInputDelta()
	{
		return mouseSource.MousePos - mouseSource.LastMousePos;
	}

	void Start()
	{
		mouseSource = InputSources.Instance.mouse;
	}

	public override void _Update()
	{
		if (!IsEnabled)
		{
			return;
		}

		if (lockMouse)
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		base._Update();
	}
}
