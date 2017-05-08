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
    /// Receiver that will start an object rotating or rotate by a set amount
    /// </summary>
    public class RotateReceiver : InteractionReceiver
    {
        [Tooltip("Vector for which relative axis to rotate on")]
        public Vector3 RotateAxis = Vector3.up;

        [Tooltip("Base sensitivity for rotation amount")]
        public float Sensitivity = 5.0f;

        [Tooltip("Curve applied to the rotation input")]
        public AnimationCurve SensitivityCurve;

        private Vector3 m_handOrigin;
        private Vector3 m_curHandPos;
        private float rate;
        private bool m_rotating;

        private AFocuser m_Focuser = null;

        protected override void OnNavigationStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_Focuser != null)
            {
                m_Focuser = eventArgs.Focuser;
                m_handOrigin = eventArgs.Position;
                m_Focuser.LockFocus();
                StartCoroutine("RotateObject");
            }
        }

        protected override void OnNavigationUpdated(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_Focuser == eventArgs.Focuser)
            {
                m_curHandPos = eventArgs.Position;
            }
        }

        protected override void OnNavigationCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_Focuser = eventArgs.Focuser)
            {
                StopCoroutine("RotateObject");
                m_Focuser.ReleaseFocus();
                m_Focuser = null;
            }
        }

        protected override void OnNavigationCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (eventArgs.Focuser == m_Focuser)
            {
                StopCoroutine("RotateObject");
                m_Focuser.ReleaseFocus();
                m_Focuser = null;
            }
        }

        // Start rotating target object.
        IEnumerator RotateObject()
        {
            while (true)
            {
                Vector3 handDirection = m_handOrigin - m_curHandPos;

                float rateMod = SensitivityCurve != null ? SensitivityCurve.Evaluate(Vector3.Distance(m_curHandPos, m_handOrigin)) : 1;

                rate = Vector3.Dot(handDirection, Veil.Instance.HeadTransform.right);
                rate *= (Sensitivity * rateMod);

                foreach(GameObject targetObject in Targets)
                {
                    targetObject.transform.Rotate(RotateAxis, rate * Time.deltaTime, UnityEngine.Space.World);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
