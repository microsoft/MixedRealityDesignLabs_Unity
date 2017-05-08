//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;

public class DebugVector3Tunable : DebugTunable
{
	/// <summary>
	/// The step amount for Vector3 fields.
	/// </summary>
	public Vector3 VecStep = Vector3.one;

	public DebugVector3Tunable(string itemName = null, float xStep = 1.0f, float yStep = 1.0f, float zStep = 1.0f) : base(itemName)
	{
		VecStep = new Vector3(xStep, yStep, zStep);
	}
}
