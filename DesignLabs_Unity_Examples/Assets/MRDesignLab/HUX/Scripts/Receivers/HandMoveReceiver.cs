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
    /// Simple receiver for updated objects based on hand displacement after selecting.
    /// </summary>
    public class HandMoveReceiver : InteractionReceiver
    {
        #region public vars
        [Tooltip("Current Hand offset")]
        public Vector3 HandOffset = Vector3.zero;

        [Tooltip("Current Hand velocity")]
        public Vector3 HandVelocity = Vector3.zero;

        [Tooltip("Bool for whether or not the current object is selected")]
        public bool Selected;

        [Tooltip("Boolean for whether or not gaze is required")]
        public bool GazeRequired;

        [Tooltip("Target object to receive interactions for.")]
        public bool Togglable;
        #endregion

        #region private members
        private Vector3 handStartPos;
        private Vector3 handLastPos;
        private bool Held;
        private float timeRatio;
        #endregion

        /// <summary>
        /// Initialize the Select events.
        /// </summary>
        public override void OnEnable()
        {
            timeRatio = 1.0f / Time.fixedDeltaTime;
            base.OnEnable();
        }

        // Start returning offset data on select
		protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
            handStartPos = Veil.Instance.HandPosition;
            if (Togglable)
            {
                Held = true;
            }

            Selected = true;
        }

		protected override void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			DoHoldFinished();

		}

		protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			DoHoldFinished();
        }

		private void DoHoldFinished()
		{
			if (Togglable)
			{
				if (Held)
				{
					Held = false;
				}
				else
				{
					Selected = false;
				}
			}
			else
			{
				Selected = false;
			}

			HandOffset = Vector3.zero;
		}

        protected void FocusExit()
        {
            if (GazeRequired)
			{
				OnHoldCompleted(gameObject, new InteractionManager.InteractionEventArgs());
            }
		}

        public virtual void FixedUpdate()
        {
            if (Selected)
            {
                handLastPos = HandOffset;

                HandOffset = Veil.Instance.HandPosition - handStartPos;
                HandOffset = Veil.Instance.HeadTransform.InverseTransformDirection(HandOffset);

                HandVelocity = HandOffset - handLastPos;
                HandVelocity.Scale(new Vector3(timeRatio, timeRatio, timeRatio));
            }
        }
    }
}