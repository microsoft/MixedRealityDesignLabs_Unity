//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HUX.Cursors
{
    public class GenericStateWidget : CursorWidget
    {
        public enum GranularGazeState
        {
            DontCare,
            True,
            False
        }

        public GranularGazeState OverInteractible = GranularGazeState.DontCare;
        public GranularGazeState HandVisible = GranularGazeState.DontCare;
        public GranularGazeState FingerPressed = GranularGazeState.DontCare;
        public GranularGazeState TwoHandsVisible = GranularGazeState.DontCare;
        public GranularGazeState TwoFingersPressed = GranularGazeState.DontCare;

        public override void OnStateChange(Cursor.CursorState ParentState)
        {
            base.OnStateChange(ParentState);
        }

        public override bool ShouldBeActive()
        {
            bool beActive = true;
            beActive &= (OverInteractible == GranularGazeState.DontCare || (OverInteractible == GranularGazeState.True) == (_curTarget != null));
            beActive &= (HandVisible == GranularGazeState.DontCare || (HandVisible == GranularGazeState.True) == (Veil.Instance.HandVisible));
            beActive &= (FingerPressed == GranularGazeState.DontCare || (FingerPressed == GranularGazeState.True) == (Veil.Instance.IsFingerPressed()));
            return beActive;
        }
    }
}
