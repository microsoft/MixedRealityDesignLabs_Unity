//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;

namespace HUX.Cursors
{
    /// <summary>
    /// Implementation of a 3D mesh object based cursor
    /// </summary>
    public class MeshCursor : Cursor 
	{
        private MeshFilter meshF;

		//rotation, transform, scale are all driven by FoundationDriver
        [Serializable]
        public class CursorStateData
        {
            public string Title;
            public Mesh CursorMesh;
            public Color CursorColor;
        }

        [Tooltip("State Data for each of the cursor states")]
        public CursorStateData[] CursorStates = new CursorStateData[6];

        public override void Start()
        {
            meshF = gameObject.GetComponent<MeshFilter>();
            base.Start();
        }

        // For the General Cursor we want to simply override these states
        public override void OnStateChange(CursorState state)
        {
            CursorStateData csd = CursorStates [(int)state];
            meshF.mesh = csd.CursorMesh;
        }
	}
}
