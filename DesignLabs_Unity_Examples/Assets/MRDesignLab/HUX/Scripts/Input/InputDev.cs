//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Utility;

/// <summary>
/// InputDev is the dev or debug equivalent of InputShell.  This exists mainly to separate
/// the debug controls and keep things cleaner.
/// </summary>
public class InputDev : Singleton<InputDev>
{
	// Virtual camera controls
	public ButtonControlState VirtualCamMovementToggle = new ButtonControlState();
	public Vector2ControlState VirtualCamTranslate = new Vector2ControlState();
	public Vector2ControlState VirtualCamRotate = new Vector2ControlState();
	public Vector2ControlState VirtualCamVertAndRoll = new Vector2ControlState();

	void Start()
	{
		VirtualCamTranslate.GenerateButtonEvents = true;
		VirtualCamRotate.GenerateButtonEvents = true;
		VirtualCamVertAndRoll.GenerateButtonEvents = true;
	}
}
