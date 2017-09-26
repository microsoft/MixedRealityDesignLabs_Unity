//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using HUX.Utility;

namespace HUX.Interaction
{
    /// <summary>
    /// This is a Singleton class for handling the global bounding box and app bar used for manipulating objects
    /// This class can also be extended per project for additional functionality
    /// </summary>
    public class ManipulationManager : Singleton<ManipulationManager>
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

        private BoundingBoxManipulate m_boundingBox;
        private AppBar m_appBar;

        /// <summary>
        /// Returns the spawned and active bounding box
        /// </summary>
        public BoundingBoxManipulate ActiveBoundingBox { get { return m_boundingBox; } }

        /// <summary>
        /// Returns the current active app bar
        /// </summary>
        public AppBar ActiveAppBar { get { return m_appBar; } }

        /// <summary>
        /// On Start spawn in the active bounding box and app bar for manipulation
        /// </summary>
        protected void Start()
        {
            // Spawn in the bounding box and assign internally
            GameObject boundBoxGO = Instantiate(BoundingBoxPrefab) as GameObject;
            m_boundingBox = boundBoxGO.GetComponent<BoundingBoxManipulate>();

            // Spawn in the app bar and assign internally
            GameObject appBarGO = Instantiate(AppBarPrefab) as GameObject;
            m_appBar = appBarGO.GetComponent<AppBar>();
        }
    }
}
