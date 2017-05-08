//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Receivers;
using HUX.Focus;

namespace HUX.Interaction
{
    /// <summary>
    /// Simplified Affordance Interactible for move/rotate/scale.
    /// </summary>
    public class AffordanceInteractible : MonoBehaviour, ToggleInteractible.IToggleable
    {

        #region public members
        public enum AffordTypeEnum
        {
            Move,
            Rotate,
            Scale
        }

        [Tooltip("Type of affordance to use for interaction")]
        public AffordTypeEnum AffordanceType;

        [Tooltip("Damping for displacement")]
        public float damping = 0.9f;

        [Tooltip("Speed when affordance is additive")]
        public float speed = 1.0f;

        [Tooltip("Sensitivity for hand displacement")]
        public float sensitivity = 7.0f;

        [Tooltip("Filter relative directions by setting to 0.0")]
        public Vector3 SpatialFilter = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("Target Object to affect")]
        public GameObject TargetObject;
        #endregion

        #region private members
        private Vector3 vDown;
        private Vector3 vDrag;

        private bool bDragging;
        private bool bSelected;

        private float angularVelocity;
        private Vector3 rotationAxis;

        private Vector3 handOrigin;
        private Vector3 handPos;
        #endregion

        void Start()
        {
            bDragging = false;
            angularVelocity = 0;
            rotationAxis = Vector3.zero;
        }

        void Update()
        {
            // on mouse down
            if (bSelected)
            {
                handPos = InputSources.Instance.hands.GetWorldPosition(InputSourceHands.FirstHandIndex);

                if (!bDragging)
                {
                    // extract vDown from the RaycastHit
                    vDown = handPos - handOrigin;

                    // start dragging
                    bDragging = true;
                }
                else
                {
                    // extract vDrag from the RaycastHit
                    vDrag = handPos - handOrigin;

                    // Rotate the hand offset based on the head facing.
                    Quaternion quatForward = FocusManager.Instance.GazeFocuser.TargetOrientation;
                    vDrag = quatForward * vDrag;

                    switch (AffordanceType)
                    {
                        case AffordTypeEnum.Move:
                            TargetObject.transform.position += vDrag;
                            break;
                        case AffordTypeEnum.Rotate:
                            // compute the rotation axis and angular velocity from vDown and vDrag
                            rotationAxis = Vector3.Cross(vDrag, vDown);
                            angularVelocity = Vector3.Angle(vDrag, vDown) * speed;

                            // apply the angular velocity
                            if (angularVelocity > 0)
                            {
                                TargetObject.transform.Rotate(rotationAxis, angularVelocity * Time.deltaTime, UnityEngine.Space.World);
                                angularVelocity = (angularVelocity > 0.01f) ? angularVelocity * damping : 0;
                            }
                            break;
                        case AffordTypeEnum.Scale:
                            TargetObject.transform.localScale += vDrag;
                            break;
                    }
                }
            }

            // Not selected stop dragging
            if (!bSelected)
                bDragging = false;
        }

		private void OnHoldStarted(InteractionManager.InteractionEventArgs e)
		{
			handOrigin = InputSources.Instance.hands.GetWorldPosition(InputSourceHands.FirstHandIndex);
			bSelected = true;
		}

		private void OnHoldCompleted(InteractionManager.InteractionEventArgs e)
		{
			bSelected = false;
		}

		private void OnHoldCanceled(InteractionManager.InteractionEventArgs e)
		{
			bSelected = false;
		}
	}
}
