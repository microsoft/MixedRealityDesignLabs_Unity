//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Focus;
using HUX.Interaction;
using System.Collections;
using UnityEngine;

namespace HUX.Receivers
{
    /// <summary>
    /// This is an abstract class that does things based on a button it 
    /// is linked to for events.
    /// </summary>
    public class SelectHoldReceiver : InteractionReceiver
    {
        #region public members
        [Tooltip("Hold time required to trigger")]
        public float HoldTime = 2.0f;

        [Tooltip("Boolean for requiring gaze to maintain interaction")]
        public bool RequireGaze = true;
        #endregion

        #region private members
        private bool bSelecting;
        private bool bGazing;
        #endregion

        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            bSelecting = true;
            if (bGazing || !RequireGaze)
            {
                StartCoroutine("SelectHoldCheck");
            }
        }

        protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            bSelecting = false;
            StopCoroutine("SelectHoldCheck");
        }

        protected override void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            bSelecting = false;
            StopCoroutine("SelectHoldCheck");
        }

        protected void FocusEnter(FocusArgs args)
        {
            bGazing = true;
            if (bSelecting && args.CurNumFocusers == 1)
            {
                StartCoroutine("SelectHoldCheck");
            }
        }

        protected void FocusExit(FocusArgs args)
        {
            if (args.CurNumFocusers == 0)
            {
                bGazing = false;
                if (RequireGaze)
                {
                    StopCoroutine("SelectHoldCheck");
                }
            }
        }

        public IEnumerator SelectHoldCheck()
        {
            yield return new WaitForSeconds(HoldTime);
            OnSelectHold();
        }

        protected virtual void OnSelectHold() { }
    }
}
