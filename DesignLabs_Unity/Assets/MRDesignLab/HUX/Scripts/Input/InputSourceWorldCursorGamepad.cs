//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class InputSourceWorldCursorGamepad : InputSourceWorldCursorBase
{
	// Input from gamepad
	public InputSourceNetGamepad gamepadSource;

	public override bool getSelectButtonPressed()
	{
		return gamepadSource.aButtonState.pressed;
	}

	public override bool getMenuButtonPressed()
	{
		return gamepadSource.startButtonState.pressed;
	}

	public override Vector2 getInputDelta()
	{
		return gamepadSource.leftJoyVector;
	}

	void Start()
	{
		gamepadSource = InputSources.Instance.netGamepad;
	}
}
