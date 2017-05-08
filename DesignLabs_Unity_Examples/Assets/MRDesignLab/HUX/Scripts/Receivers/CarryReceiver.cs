//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using HUX;
using HUX.Interaction;
using HUX.Focus;

namespace HUX.Receivers
{
	/// <summary>
	/// Receiver that will start an object rotating or rotate by a set amount
	/// </summary>
	public class CarryReceiver : InteractionReceiver
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

        private GameObject m_parent;
        private bool m_bAttached = false;

        private Vector3 m_HandLastPosition = Vector3.zero;
        private float m_HandDeadzone = 0.00001f;

		private AFocuser m_InteractingFocus = null;

        public override void OnEnable()
        {
			if (m_AttachType == AttachTargetEnum.Hand)
			{
				InputSources.Instance.hands.OnHandMoved += OnHandMoved;
			}

            base.OnEnable();
        }

        public override void OnDisable()
        {
			if (m_AttachType == AttachTargetEnum.Hand)
			{
				InputSources.Instance.hands.OnHandMoved -= OnHandMoved;
			}

            base.OnDisable();
        }

        public void OnHandMoved(InputSourceHands.CurrentHandState state)
        {
			if (Vector3.Distance(m_HandLastPosition, state.Position) >= m_HandDeadzone)
			{
				m_HandLastPosition = state.Position;
			}
        }

        /// <summary>
        /// On Hold start event for attaching objects if it's the correct option
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
            if(m_InteractionType == AttachInteractionEnum.PressAndHold && m_InteractingFocus == null)
            {
                AttachObjects(eventArgs.Focuser);
            }
        }

        /// <summary>
        /// On Completed hold 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
            if (m_InteractionType == AttachInteractionEnum.PressAndHold && m_bAttached && m_InteractingFocus == eventArgs.Focuser)
            {
                DetachObjects();
            }
        }

        /// <summary>
        /// On hold cancel detach objects
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
            if (m_InteractionType == AttachInteractionEnum.PressAndHold && m_bAttached && m_InteractingFocus == eventArgs.Focuser)
            {
                DetachObjects();
            }
        }

        /// <summary>
        /// On tapped if tapped interaction type then attach or detach
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.Tapped)
            {
                if (m_bAttached && m_InteractingFocus == eventArgs.Focuser)
                {
					m_InteractingFocus.ReleaseFocus();
                    DetachObjects();
                }
                else if (m_InteractingFocus == null)
                {
					eventArgs.Focuser.LockFocus();
                    AttachObjects(eventArgs.Focuser);
                }
            }
        }

        /// <summary>
        /// On double tapped if double tapped interaction type then attach or detach
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnDoubleTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (m_InteractionType == AttachInteractionEnum.DoubleTapped)
            {
                if (m_bAttached && m_InteractingFocus == eventArgs.Focuser)
                {
					m_InteractingFocus.ReleaseFocus();
                    DetachObjects();
                }
                else if (m_InteractingFocus == null)
                {
					eventArgs.Focuser.LockFocus();
                    AttachObjects(eventArgs.Focuser);
                }
            }
        }

        private void AttachObjects(AFocuser focuser)
        {
			m_InteractingFocus = focuser;
			if (m_ParentObjects)
            {
                if (m_parent == null)
                    m_parent = new GameObject();

                foreach (GameObject target in Targets)
                {
                    target.layer = LayerMask.NameToLayer("Ignore Raycast");
                    target.transform.parent = m_parent.transform;
                }
            }
            else
            {
                foreach (GameObject target in Targets)
                {
                    target.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }

            StartCoroutine("CarryObjects");
            m_bAttached = true;
        }

        private void DetachObjects()
        {
            StopCoroutine("CarryObjects");

            if (m_ParentObjects)
            {

                foreach (GameObject target in Targets)
                {
                    target.layer = LayerMask.NameToLayer("Default");
                    target.transform.parent = null;
                }

                if (m_parent != null)
                    GameObject.Destroy(m_parent);
            }
            else
            {
                foreach (GameObject target in Targets)
                {
                    target.layer = LayerMask.NameToLayer("Default");
                }
            }

            m_bAttached = false;
			m_InteractingFocus = null;
        }

        // Start rotating target object.
        public IEnumerator CarryObjects()
		{
			while (true)
			{
				Vector3 curPos = m_AttachType == AttachTargetEnum.Cursor ? m_InteractingFocus.Cursor.transform.position : m_HandLastPosition;
				Quaternion curRot = m_AttachType == AttachTargetEnum.Cursor ? m_InteractingFocus.Cursor.transform.rotation : Camera.main.transform.rotation;

				if (m_ParentObjects)
				{
					if (m_parent != null)
					{
						m_parent.transform.position = Vector3.Lerp(m_parent.transform.position, curPos, Time.deltaTime / m_PositionLerpTime);
						m_parent.transform.rotation = Quaternion.Lerp(m_parent.transform.rotation, curRot, Time.deltaTime / m_RotationLerpTime);
					}
				}
				else
				{
					foreach (GameObject target in Targets)
					{
						target.transform.position = Vector3.Lerp(target.transform.position, curPos, Time.deltaTime / m_PositionLerpTime);
						target.transform.rotation = Quaternion.Lerp(target.transform.rotation, curRot, Time.deltaTime / m_RotationLerpTime);
					}
				}

				yield return new WaitForEndOfFrame();
			}
		}
	}
}
