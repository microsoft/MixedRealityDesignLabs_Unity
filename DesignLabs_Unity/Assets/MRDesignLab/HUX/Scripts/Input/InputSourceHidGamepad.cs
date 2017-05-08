//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HidControllerInput;

public class InputSourceHidGamepad : InputSourceGamepadBase
{
	bool present;
	void Awake()
	{
#if !UNITY_EDITOR && UNITY_WSA
		HidControllerInput.DeviceConnect.Init();
#endif
    }

    public override bool IsPresent()
	{
		return present;
	}

	public override void _Update()
	{
		base._Update();

		//Debug.Log("cs a: " + ControlsState.state.A);
		if (DeviceState.GetControllerCount() > 0)
		{
			DeviceState state = DeviceState.GetState(0);

			present = true;

			aButtonState.ApplyState(state.A);
			bButtonState.ApplyState(state.B);
			xButtonState.ApplyState(state.X);
			yButtonState.ApplyState(state.Y);
			startButtonState.ApplyState(state.start || state.logo);

			leftJoyVector = state.LeftStick;
			rightJoyVector = state.RightStick;
			trigVector = state.Triggers;
			padVector = state.DPad;
		}
		else
		{
			present = false;
		}

		// Invert
		leftJoyVector.y = -leftJoyVector.y;

		// Deadzone
		float deadzone = 0.15f;
		if (leftJoyVector.sqrMagnitude < deadzone * deadzone)
		{
			leftJoyVector = Vector2.zero;
		}
	}
}
