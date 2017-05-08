//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using HUX.Interaction;

namespace HUX.Receivers
{
    /// <summary>
    /// Simple receiver class for toggling a game object active or inactive.
    /// </summary>
    public class ToggleActiveReceiver : InteractionReceiver
    {
		// When selected toggle color change
		protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs e)
		{
            Debug.Log("On tapped in interaction receiver");
            if (Targets.Count > 0)
            {
                foreach(GameObject target in Targets)
                {
                    target.SetActive(!target.activeSelf);
                }
            }
        }
    }
}