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
    /// Example receiver for toggling the color of an object based on an interaction.
    /// </summary>
    public class ColorReceiver : InteractionReceiver
    {
        [Tooltip("Default color of the object")]
        public Color DefaultColor;

        [Tooltip("Target color to toggle when interacting with the object.")]
        public Color TargetColor;

        private bool _bToggled;

        /// <summary>
        /// On select started toggle the color of the object.
        /// </summary>
		protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            _bToggled = !_bToggled;
            Renderer targetRenderer;

            for (int i = 0; i < Targets.Count; i++)
            {
                targetRenderer = Targets[i].GetComponent<Renderer>();
                targetRenderer.material.color = _bToggled ? TargetColor : DefaultColor;
            }
        }
    }
}
