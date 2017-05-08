//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX;

public class InputSourceWorldCursorHands : InputSourceWorldCursorBase
{
	// Input from hands
	public InputSourceHands handsSource;

	Quaternion startRotationInv = Quaternion.identity;

	public Vector2ControlState dragControl = new Vector2ControlState(false);

	int prevHandsVisible;

	[DebugTunable]
	public float debugInputAmount;
	[DebugTunable]
	public float debugHandControlX;
	[DebugTunable]
	public float debugHandControlY;

	public override bool getSelectButtonPressed()
	{
		return handsSource.NumFingersPressed > 0;
	}

	public override bool getMenuButtonPressed()
	{
		return handsSource.menuGesture.pressed;
	}

	public override Vector2 getInputDelta()
	{
		return dragControl.delta;
	}

	void Start()
	{
		handsSource = InputSources.Instance.hands;
	}

	Vector3 GetHandDir()
	{
		return handsSource.GetWorldPosition(0) - Veil.Instance.HeadTransform.position;
	}

	Quaternion getLocalHandRotation()
	{
		Vector3 localHandDir = Quaternion.Inverse(Veil.Instance.HeadTransform.rotation) * GetHandDir();
		return Quaternion.LookRotation(localHandDir);
	}

	public override void _Update()
	{
		if (!IsEnabled)
		{
			return;
		}

		debugInputAmount = inputAmount;
		debugHandControlX = dragControl.pos.x;
		debugHandControlY = dragControl.pos.y;

		if (handsSource.NumHandsVisible > 0)
		{
			if (prevHandsVisible == 0)
			{
				startRotationInv = Quaternion.Inverse(getLocalHandRotation());
			}
			else
			{
				Quaternion dragOffsetRotation = startRotationInv * getLocalHandRotation();
				dragControl.ApplyPos(true, normalizedYawPitch(dragOffsetRotation));
			}
		}
		else
		{
			dragControl.ApplyPos(false, Vector2.zero);
		}

		prevHandsVisible = handsSource.NumHandsVisible;

		base._Update();
	}
}
