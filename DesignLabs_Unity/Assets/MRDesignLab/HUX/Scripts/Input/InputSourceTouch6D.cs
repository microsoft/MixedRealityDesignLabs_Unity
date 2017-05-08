//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX;

public class InputSourceTouch6D : InputSourceSixDOFBase
{
	// SixDOF comes from InputSourceSixDOFRay
	public override Vector3 position
	{
		get
		{
			return InputSources.Instance.netMouse.Connected() ? InputSources.Instance.sixDOFRay.controllerGOoffset.transform.position : InputSources.Instance.sixDOFRay.position;
		}
	}

	public override Quaternion rotation
	{
		get
		{
			return InputSources.Instance.netMouse.Connected() ? InputSources.Instance.sixDOFRay.controllerGOoffset.transform.rotation : InputSources.Instance.sixDOFRay.rotation;
		}
	}

	// Buttons come from InputSourceNetMouse
	public override bool buttonSelectPressed
	{
		get
		{
			return InputSources.Instance.netMouse.Connected() ? (InputSources.Instance.netMouse.buttonSelectPressed || InputSources.Instance.netMouse.buttonLeftPressed) : (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.LeftArrow));
		}
	}

	public override bool buttonMenuPressed
	{
		get
		{
			return InputSources.Instance.netMouse.Connected() ? InputSources.Instance.netMouse.buttonDownArrowPressed : Input.GetKey(KeyCode.DownArrow);
		}
	}

	public override bool buttonAltSelectPressed
	{
		get
		{
			return InputSources.Instance.netMouse.Connected() ? InputSources.Instance.netMouse.buttonUpArrowPressed : false;
		}
	}
}

