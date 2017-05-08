//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

// Simply a base class for non-virtual gamepad input types (net vs hid.  Eventually also the Unity Input api)
public class InputSourceGamepadBase : InputSourceBase
{
	public Vector2 leftJoyVector;
	public Vector2 rightJoyVector;
	public Vector2 trigVector;
	public Vector2 padVector;

	public ButtonControlState startButtonState = new ButtonControlState();
	public ButtonControlState aButtonState = new ButtonControlState();
	public ButtonControlState bButtonState = new ButtonControlState();
	public ButtonControlState xButtonState = new ButtonControlState();
	public ButtonControlState yButtonState = new ButtonControlState();

	public virtual bool IsPresent()
	{
		return false;
	}
}
