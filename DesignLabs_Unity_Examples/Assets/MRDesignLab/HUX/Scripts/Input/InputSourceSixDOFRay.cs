//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Receivers;
using HUX.Utility;

public class InputSourceSixDOFRay : InputSourceBase
{
	public Vector3 position;
	public Quaternion rotation;

	public GameObject controllerGO;
	public GameObject controllerGOoffset;

	public override void _Update()
	{
		if (controllerGO)
		{
			position = controllerGO.transform.position;
			rotation = controllerGO.transform.rotation;
		}

		base._Update();
	}
}
