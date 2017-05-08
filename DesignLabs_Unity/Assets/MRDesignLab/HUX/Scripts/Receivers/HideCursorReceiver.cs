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
    /// Example receiver that scales based on gazing at an object
    /// </summary>
    public class HideCursorReceiver : InteractionReceiver
    {
        protected void FocusEnter(FocusArgs args)
        {
            args.Focuser.SetCursorActive(false);
        }

        protected void FocusExit(FocusArgs args)
        {
            args.Focuser.SetCursorActive(true);
        }
    }
}