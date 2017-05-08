//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class InputSourceKeyboard : InputSourceBase {


	public static bool GetKey(KeyCode keyCode)
	{
		return Input.GetKey(keyCode);
	}

	public static bool GetKeyDown(KeyCode keyCode)
	{
		return Input.GetKeyDown(keyCode);
	}

	public static bool GetKeyUp(KeyCode keyCode)
	{
		return Input.GetKeyUp(keyCode);
	}
}
