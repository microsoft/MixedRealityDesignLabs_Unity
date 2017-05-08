//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Utility
{
	public class StateComponent : MonoBehaviour
	{
		[Tooltip("Specify a state/name for the state component.  StateController will activate/deactivate if stateName begins with a state")]
		public string stateName;
	}
}
