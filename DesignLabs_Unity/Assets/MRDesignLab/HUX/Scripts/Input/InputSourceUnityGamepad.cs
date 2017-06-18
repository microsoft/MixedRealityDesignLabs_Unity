//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class InputSourceUnityGamepad : InputSourceGamepadBase
{
    // Variations based on platform
    public static readonly string[] SupportedGamepadNames = {
        "controller (xbox 360 for windows)",
        "controller (xbox one for windows)",
        "xbox 360 controller for windows",
        "xbox one controller for windows",
        "xbox controller for windows",
    };

    public const string ButtonA = "Xbox_A";
	public const string ButtonB = "Xbox_B";
	public const string ButtonX = "Xbox_X";
	public const string ButtonY = "Xbox_Y";
	public const string ButtonStart = "Xbox_MenuButton";
	public const string AxisLeftStickH = "Xbox_LeftStick_X";
	public const string AxisLeftStickV = "Xbox_LeftStick_Y";
	public const string AxisRightStickH = "Xbox_RightStick_X";
	public const string AxisRightStickV = "Xbox_RightStick_Y";
	public const string AxisDpadH = "Xbox_Dpad_X";
	public const string AxisDpadV = "Xbox_Dpad_Y";
	public const string TriggerLeft = "Xbox_LeftTrigger";
	public const string TriggerRight = "Xbox_RightTrigger";
	public const string TriggerShared = "Xbox_Trigger_Shared";

	bool present;

	public override bool IsPresent()
	{
        foreach (string joystickName in Input.GetJoystickNames()) {
            foreach (string supportedGamepad in SupportedGamepadNames) {
                if (joystickName.ToLower() == supportedGamepad) {
                    return true;
                }
            }
        }
        return false;
	}

	public override void _Update()
	{
		base._Update();

		if (IsPresent())
		{
			aButtonState.ApplyState(Input.GetButton(ButtonA));
			bButtonState.ApplyState(Input.GetButton(ButtonB));
			xButtonState.ApplyState(Input.GetButton(ButtonX));
			yButtonState.ApplyState(Input.GetButton(ButtonY));
			startButtonState.ApplyState(Input.GetButton(ButtonStart));

			leftJoyVector = new Vector2(Input.GetAxis(AxisLeftStickH), Input.GetAxis(AxisLeftStickV));
			rightJoyVector = new Vector2(Input.GetAxis(AxisRightStickH), Input.GetAxis(AxisRightStickV));
			trigVector = new Vector2(Input.GetAxis(TriggerLeft), Input.GetAxis(TriggerRight));
			padVector = new Vector2(Input.GetAxis(AxisDpadH), Input.GetAxis(AxisDpadV));
		}
	}
}
