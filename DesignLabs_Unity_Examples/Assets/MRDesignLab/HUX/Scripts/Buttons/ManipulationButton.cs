//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;

namespace HUX.Buttons
{
    /// <summary>
    /// Manipulation button when selected creates a bounding box and allows for manipulation
    /// </summary>
    public class ManipulationButton : Button
    {
        /// <summary>
        /// Prefab for the bounding box on maniplation
        /// </summary>
        [SerializeField]
        private GameObject BoundingBoxPrefab;

        /// <summary>
        /// Prefab for the app bar for manipulation
        /// </summary>
        [SerializeField]
        private GameObject AppBarPrefab;

        private BoundingBoxGizmo m_boundingBox;
        private AppBar m_appBar;

        /// <summary>
        /// On awake spawn the Bounding Box and App Bar
        /// </summary>
        public void Awake()
        {

        }

        /// <summary>
        /// On state change swap out the active mesh based on the state
        /// </summary>
        public override void OnStateChange(ButtonStateEnum newState)
        {
            base.OnStateChange(newState);
        }
    }
}