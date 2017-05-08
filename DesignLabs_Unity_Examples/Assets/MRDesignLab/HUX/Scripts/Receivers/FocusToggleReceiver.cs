//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using HUX.Interaction;
using HUX.Focus;

namespace HUX.Receivers
{
    /// <summary>
    /// Simple receiver class for toggling a game object active or inactive on focus.
    /// </summary>
    public class FocusToggleReceiver : InteractionReceiver
    {

        /// <summary>
        /// Toggle targets active when in focus
        /// </summary>
        protected override void OnFocusEnter(GameObject obj, FocusArgs args)
		{
            // First check that the obj is ours and we have targets
            if (IsInteractible(obj) && Targets.Count > 0)
            {
                foreach(GameObject target in Targets)
                {
                    target.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Toggle targets inactive when out of focus
        /// </summary>
        protected override void OnFocusExit(GameObject obj, FocusArgs args)
        {
            // First check that the obj is ours and we have targets
            if (IsInteractible(obj) && Targets.Count > 0 && args.CurNumFocusers == 0)
            {
                foreach (GameObject target in Targets)
                {
                    target.SetActive(false);
                }
            }
        }
    }
}