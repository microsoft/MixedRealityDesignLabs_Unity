//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class DebugButtonAttribute : DebugTunable
{
	/// <summary>
	/// The button name for the debug button.
	/// </summary>
	public string ButtonName = "Ok";

	/// <summary>
	/// 
	/// </summary>
	/// <param name="itemName"></param>
	/// <param name="buttonName"></param>
	public DebugButtonAttribute(string itemName = null, string buttonName = "Ok") : base(itemName)
	{
		ButtonName = buttonName;
	}
}
