//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Receivers;
using HUX.Focus;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUX.Interaction
{
    /// <summary>
    /// This is a simple interactible for toggling other interactible components.
    /// </summary>
    public class CarryInteractible : InteractibleObject
    {
        public enum AttachTargetEnum
        {
            Cursor,
            Hand
        }

        public enum AttachInteractionEnum
        {
            PressAndHold,
            Tapped,
            DoubleTapped
        }

        [Tooltip("Attachment target type")]
        public AttachTargetEnum m_AttachType = AttachTargetEnum.Cursor;

        [Tooltip("Interaction type to init and stop carry")]
        public AttachInteractionEnum m_InteractionType = AttachInteractionEnum.PressAndHold;

        [Tooltip("Time to lerp to cursor position")]
        public float m_PositionLerpTime = 0.2f;

        [Tooltip("Time to lerp to cursor rotation")]
        public float m_RotationLerpTime = 0.2f;

        [Tooltip("Create parent for all targets and reparent after")]
        public bool m_ParentObjects = false;

        private bool m_bAttached = false;

        private Vector3 m_HandLastPosition = Vector3.zero;
        private float m_HandDeadzone = 0.00001f;

		private AFocuser m_InteractingFocus = null;

        public void OnEnable()
        {
			if (m_AttachType == AttachTargetEnum.Hand)
			{
				InputSources.Instance.hands.OnHandMoved += OnHandMoved;
			}
        }

        public void OnDisable()
        {
			if (m_AttachType == AttachTargetEnum.Hand)
			{
				InputSources.Instance.hands.OnHandMoved -= OnHandMoved;
			}
        }

        public void OnHandMoved(InputSourceHands.CurrentHandState state)
        {
            if (Vector3.Distance(m_HandLastPosition, state.Position) >= m_HandDeadzone)
                m_HandLastPosition = state.Position;
        }

        /// <summary>
        /// On Hold start event for attaching objects if it's the correct option
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected void OnHoldStarted(InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.PressAndHold && m_InteractingFocus == null)
            {
                AttachObject(eventArgs.Focuser);
            }
        }

        /// <summary>
        /// On Completed hold 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected void OnHoldCompleted(InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.PressAndHold && m_bAttached && m_InteractingFocus == eventArgs.Focuser)
            {
                DetachObject();
            }
        }

        /// <summary>
        /// On hold cancel detach objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected void OnHoldCanceled(InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.PressAndHold && m_bAttached && m_InteractingFocus == eventArgs.Focuser)
            {
                DetachObject();
            }
        }

        /// <summary>
        /// On tapped if tapped interaction type then attach or detach
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected void OnTapped(InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.Tapped)
            {
                if (m_bAttached && m_InteractingFocus == eventArgs.Focuser)
                {
					m_InteractingFocus.ReleaseFocus();
                    DetachObject();
                }
				else if (m_InteractingFocus == null)
				{
					eventArgs.Focuser.LockFocus();
                    AttachObject(eventArgs.Focuser);
                }
            }
        }

        /// <summary>
        /// On double tapped if double tapped interaction type then attach or detach
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected void OnDoubleTapped(InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.DoubleTapped)
            {
                if (m_bAttached && m_InteractingFocus == eventArgs.Focuser)
                {
					m_InteractingFocus.ReleaseFocus();
                    DetachObject();
                }
                else if (m_InteractingFocus == null)
                {
					eventArgs.Focuser.LockFocus();
                    AttachObject(eventArgs.Focuser);
                }
            }
        }

        private void AttachObject(AFocuser focuser)
        {
			m_InteractingFocus = focuser;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            StartCoroutine("CarryObject");
            m_bAttached = true;
        }

        private void DetachObject()
        {
            StopCoroutine("CarryObject");
            gameObject.layer = LayerMask.NameToLayer("Default");
            m_bAttached = false;
			m_InteractingFocus = null;
		}

        // Start rotating target object.
        public IEnumerator CarryObject()
        {
            while (true)
            {
                Vector3 curPos = m_AttachType == AttachTargetEnum.Cursor ? m_InteractingFocus.Cursor.transform.position : m_HandLastPosition;
                Quaternion curRot = m_AttachType == AttachTargetEnum.Cursor ? m_InteractingFocus.Cursor.transform.rotation : Camera.main.transform.rotation;

                this.transform.position = Vector3.Lerp(this.transform.position, curPos, Time.deltaTime / m_PositionLerpTime);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, curRot, Time.deltaTime / m_RotationLerpTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}