//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class InputSourceUnityGamepad : InputSourceGamepadBase
{
	bool present;

	public override bool IsPresent()
	{
		return Input.GetJoystickNames().Length > 0;
	}

	public override void _Update()
	{
		base._Update();

        if (IsPresent())
        {
            aButtonState.ApplyState(Input.GetButton("360_AButton"));
            bButtonState.ApplyState(Input.GetButton("360_BButton"));
            xButtonState.ApplyState(Input.GetButton("360_XButton"));
            yButtonState.ApplyState(Input.GetButton("360_YButton"));
            startButtonState.ApplyState(Input.GetButton("360_StartButton"));

            leftJoyVector = new Vector2(Input.GetAxis("LeftStickH"), Input.GetAxis("LeftStickV"));
            rightJoyVector = new Vector2(Input.GetAxis("RightStickH"), Input.GetAxis("RightStickV"));
            trigVector = new Vector2(Input.GetAxis("360_LTrigger"), Input.GetAxis("360_RTrigger"));
            padVector = new Vector2(Input.GetAxis("360_HorizontalDPAD"), Input.GetAxis("360_VerticalDPAD"));
        }
	}
}
