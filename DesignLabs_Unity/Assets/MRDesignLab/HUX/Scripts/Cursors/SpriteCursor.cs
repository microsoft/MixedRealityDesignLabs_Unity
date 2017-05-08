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
    /// Implementation of a 2D sprite based cursor
    /// </summary>
    public class SpriteCursor : Cursor 
	{
        protected SpriteRenderer spriteR;

        [Serializable]
        public class CursorStateData
        {
            public Sprite CursorSprite;
            public Color CursorColor;
        }

        [Tooltip("State Data for each of the cursor states")]
        public CursorStateData[] CursorStates = new CursorStateData[6];

        public override void Start()
        {
            spriteR = gameObject.GetComponent<SpriteRenderer> ();
            base.Start();
        }

        // For the General Cursor we want to simply override these states
        public override void OnStateChange(CursorState state)
        {
            CursorStateData csd = CursorStates [(int)state];
            spriteR.sprite = csd.CursorSprite;
            spriteR.color = csd.CursorColor;

            base.OnStateChange(state);
        }
    }
}
