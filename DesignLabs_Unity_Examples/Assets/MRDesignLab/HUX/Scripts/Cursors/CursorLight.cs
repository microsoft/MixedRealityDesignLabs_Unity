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
    /// This is the base abstract class for cursors and their defined states.
    /// </summary>
    public class CursorLight : Cursor 
	{
        private Light lightC;

		//rotation, transform, scale are all driven by FoundationDriver
        [Serializable]
        public class CursorStateData
        {
            public Color CursorColor;
        }

        [Tooltip("State Data for each of the cursor states")]
        public CursorStateData[] CursorStates = new CursorStateData[6];

        public override void Start()
        {
            lightC = gameObject.GetComponent<Light>();
            base.Start();
        }

        // For the General Cursor we want to simply override these states
        public override void OnStateChange(CursorState state)
        {
            CursorStateData csd = CursorStates [(int)state];
            lightC.color = csd.CursorColor;
        }
	}
}
