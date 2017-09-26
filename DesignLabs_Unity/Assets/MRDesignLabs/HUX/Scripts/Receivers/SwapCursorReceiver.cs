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
    /// Example receiver that swaps a cursor out based on the target interaction
    /// </summary>
    public class SwapCursorReceiver : InteractionReceiver
    {
        public enum InteractionTypeEnum
        {
            OnGaze,
            OnSelect
        };

        public InteractionTypeEnum InteractionType = InteractionTypeEnum.OnGaze;
        public HUX.Cursors.Cursor NewCursorPrefab;
        public bool RevertOnExit = false;

        private FocusManager m_fManager;
        private HUX.Cursors.Cursor m_baseCursor;

        public void Start()
        {
            m_fManager = FocusManager.Instance;
            m_baseCursor = m_fManager.GazeFocuser.CursorPrefab;
        }

        protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (InteractionType == InteractionTypeEnum.OnSelect)
            {
                eventArgs.Focuser.SetCursor(NewCursorPrefab);
            }
        }

        protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (InteractionType == InteractionTypeEnum.OnSelect && RevertOnExit && m_baseCursor != null)
            {
                eventArgs.Focuser.SetCursor(m_baseCursor);
            }
        }

        protected void FocusEnter(FocusArgs args)
        {
            if (InteractionType == InteractionTypeEnum.OnGaze)
            {
                args.Focuser.SetCursor(NewCursorPrefab);
            }
        }

        protected void FocusExit(FocusArgs args)
        {
            if (InteractionType == InteractionTypeEnum.OnGaze && RevertOnExit && m_baseCursor != null)
            {
                args.Focuser.SetCursor(m_baseCursor);
            }
        }
    }
}