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
    /// Simple receiver for moving an object when something is selected.
    /// </summary>
    public class MoveReceiver : InteractionReceiver
    {
        [Tooltip("Target object to apply the translation on")]
        public GameObject TargetObject;

        [Tooltip("Relative vector to move to when interacting")]
        public Vector3 RelativeMove;

        [Tooltip("Time to move to relative location")]
        public float MoveTime;

        [Tooltip("When true object will return to base location after interaction")]
        public bool ReturnOnUnselect;

		// When selected move to offset
		protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{

		}

		protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{

		}

		protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{

		}

		protected override void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{

		}
	}
}