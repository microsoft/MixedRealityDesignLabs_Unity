//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Focus;
using HUX.Interaction;
using System;
using UnityEngine;

namespace HUX.Buttons
{
    /// <summary>
    /// Base class for buttons.
    /// </summary>
    public abstract class Button : InteractibleObject
    {
        #region Public Members

        public enum ButtonStateEnum
        {
            /// <summary>
            /// Looking at and Pressed
            /// </summary>
            Pressed,
            /// <summary>
            /// Looking at and finger up
            /// </summary>
            Targeted,
            /// <summary>
            /// Not looking at it and finger is up
            /// </summary>
            Interactive,
            /// <summary>
            /// Looking at button finger down
            /// </summary> 
            ObservationTargeted,
            /// <summary>
            /// Not looking at it and finger down
            /// </summary>
            Observation,
            /// <summary>
            /// Button in a disabled state
            /// </summary>
            Disabled,
        }

        /// <summary>
        /// Current Button State
        /// </summary>
        [Header("Button")]
        [Tooltip("Current State of the Button")]
        public ButtonStateEnum ButtonState = ButtonStateEnum.Observation;

        /// <summary>
        /// If true the interactible will unselect when you look off of the object
        /// </summary>
        [Tooltip("If RequireGaze then looking away will unselect object")]
        public bool RequireGaze = true;

        /// <summary>
        /// Event to receive button state change
        /// </summary>
        public event Action<ButtonStateEnum> StateChange;

        /// <summary>
        /// Event fired when tap interaction received.
        /// </summary>
        public event Action<GameObject> OnButtonPressed;

        /// <summary>
        /// Event fired when hold interaction initiated.
        /// </summary>
        public event Action<GameObject> OnButtonHeld;

        /// <summary>
        /// Event fired when hold interaction released.
        /// </summary>
        public event Action<GameObject> OnButtonReleased;

        /// <summary>
        /// Event fired when hold interaction cancelled.
        /// </summary>
        public event Action<GameObject> OnButtonCancelled;

        #endregion

        #region Private and Protected Members
        /// <summary>
        /// Internal protected member for our default gizmo icon
        /// </summary>
        protected string _gizmoIconDefault = "HUX/hux_button_icon.png";

        /// <summary>
        /// Internal protected member for our gizmo selected icon
        /// </summary>
        protected string _gizmoIconSelected = "HUX/hux_button_icon_selected.png";

        /// <summary>
        /// Protected string for the current active gizmo icon
        /// </summary>
        protected string _gizmoIcon;

        /// <summary>
        /// Last state of hands being visible
        /// </summary>
        private bool m_bLastHandVisible = false;

        /// <summary>
        /// Check for disabled state or disabled behavior
        /// </summary>
        private bool m_disabled { get { return ButtonState == ButtonStateEnum.Disabled || !enabled; } }
        
        #endregion


        /// <summary>
        /// InteractionManager SendMessage("Tapped") receiver.
        /// </summary>
        /// <param name="args"></param>
        public virtual void Pressed(InteractionManager.InteractionEventArgs args)
        {
            if (enabled)
            {
                DoButtonPressed();

				// Set state to Pressed
				ButtonStateEnum newState = ButtonStateEnum.Pressed;
				this.OnStateChange(newState);
			}
        }

		public virtual void Released(InteractionManager.InteractionEventArgs args)
		{
			if (enabled)
			{
				DoButtonReleased();

				// Unset state from pressed.
				ButtonStateEnum newState = ButtonStateEnum.Targeted;
				this.OnStateChange(newState);
			}
		}

		/// <summary>
		/// InteractionManager SendMessage("HoldStarted") receiver.
		/// </summary>
		/// <param name="args"></param>
		public virtual void HoldStarted(InteractionManager.InteractionEventArgs args)
        {
            if (!m_disabled)
            {
                DoButtonPressed();

                // Set state to Pressed
                ButtonStateEnum newState = ButtonStateEnum.Pressed;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// InteractionManager SendMessage("HoldUpdated") receiver.
        /// </summary>
        /// <param name="args"></param>
        public virtual void HoldUpdated(InteractionManager.InteractionEventArgs args)
        {
            if (!m_disabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonHeld();
            }
        }

        /// <summary>
        /// InteractionManager SendMessage("HoldCompleted") receiver.
        /// </summary>
        /// <param name="args"></param>
        public virtual void HoldCompleted(InteractionManager.InteractionEventArgs args)
        {
            if (!m_disabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonReleased();

                // Unset state from pressed.
                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// InteractionManager SendMessage("HoldCanceled") receiver.
        /// </summary>
        /// <param name="args"></param>
        public virtual void HoldCanceled(InteractionManager.InteractionEventArgs args)
        {
            if (!m_disabled && ButtonState == ButtonStateEnum.Pressed)
            {
                DoButtonCancelled();
                // Unset state from pressed.

                ButtonStateEnum newState = ButtonStateEnum.Targeted;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// Called when button is pressed down.
        /// </summary>
        protected void DoButtonPressed()
        {
            if (OnButtonPressed != null)
            {
                OnButtonPressed(gameObject);

                ButtonStateEnum newState = ButtonStateEnum.Pressed;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// Called when button is released.
        /// </summary>
        protected void DoButtonReleased()
        {
            if (OnButtonReleased != null)
            {
                OnButtonReleased(gameObject);

                ButtonStateEnum newState = ButtonStateEnum.Observation;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// Called while button is pressed down.
        /// </summary>
        protected void DoButtonHeld()
        {
            if (OnButtonHeld != null)
            {
                OnButtonHeld(gameObject);
            }
        }

        /// <summary>
        /// Called when something interrupts the button pressed state.
        /// </summary>
        protected void DoButtonCancelled()
        {
            if (OnButtonCancelled != null)
            {
                OnButtonCancelled(gameObject);
            }
        }

        /// <summary>
        /// FocusManager SendMessage("FocusEnter") receiver.
        /// </summary>
        public void FocusEnter(FocusArgs args)
        {
            if (!m_disabled && args.CurNumFocusers == 1)
            {
                ButtonStateEnum newState = Veil.Instance.HandVisible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                this.OnStateChange(newState);
            }
        }

        /// <summary>
        /// FocusManager SendMessage("FocusExit") receiver.
        /// </summary>
        public void FocusExit(FocusArgs args)
        {
            if (!m_disabled && args.CurNumFocusers == 0)
            {
                if (ButtonState == ButtonStateEnum.Pressed)
                {
                    DoButtonCancelled();
                }

                ButtonStateEnum newState = Veil.Instance.HandVisible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;

                if (RequireGaze || ButtonState != ButtonStateEnum.Pressed)
                {
                    this.OnStateChange(newState);
                }
            }
        }

        /// <summary>
        /// Use LateUpdate to check for whether or not the hand is up
        /// </summary>
        public void LateUpdate()
        {
            var isAnyHandVisible = InputSources.Instance.hands.IsAnyHandVisible();
            if (!m_disabled && m_bLastHandVisible != isAnyHandVisible)
            {
                OnHandVisibleChange(isAnyHandVisible);
            }
        }

        /// <summary>
        /// Event to fire off when hand visibity changes
        /// </summary>
        /// <param name="visible"></param>
        public virtual void OnHandVisibleChange(bool visible)
        {
            m_bLastHandVisible = visible;

            ButtonStateEnum newState = ButtonState;

            switch (ButtonState)
            {
                case ButtonStateEnum.Interactive:
                {
                    newState = visible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
                    break;
                }

                case ButtonStateEnum.Targeted:
                {
                    newState = visible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                    break;
                }

                case ButtonStateEnum.Observation:
                {
                    newState = visible ? ButtonStateEnum.Interactive : ButtonStateEnum.Observation;
                    break;
                }

                case ButtonStateEnum.ObservationTargeted:
                {
                    newState = visible ? ButtonStateEnum.Targeted : ButtonStateEnum.ObservationTargeted;
                    break;
                }

            }

            OnStateChange(newState);
        }

        /// <summary>
        /// Ensures the button returns to a neutral state when disabled
        /// </summary>
        public virtual void OnDisable()
        {
            if (ButtonState != ButtonStateEnum.Disabled)
            {
                OnStateChange(ButtonStateEnum.Observation);
            }
        }

        /// <summary>
        /// Callback virtual function for when the button state changes
        /// </summary>
        /// <param name="newState">
        /// A <see cref="ButtonStateEnum"/> for the new button state.
        /// </param>
        public virtual void OnStateChange(ButtonStateEnum newState)
        {
            ButtonState = newState;

            // Send out the action/event for the statechange
            if (this.StateChange != null)
                this.StateChange(newState);
        }

#if UNITY_EDITOR
        /// <summary>
        /// On draw gizmo shows the icon for the object in the editor 
        /// </summary>
        private void OnDrawGizmos()
        {
            // Simple visualization if Gazer is none - we could be in a level without the gazer spawned yet, or in editor.
            Collider collider = this.GetCachedComponent<Collider>();
            if (collider != null)
            {
                _gizmoIcon = UnityEditor.Selection.activeGameObject == this.gameObject ? _gizmoIconSelected : _gizmoIconDefault;
                Gizmos.DrawIcon(this.transform.position, _gizmoIcon, false);
                Gizmos.DrawIcon(collider.bounds.center + (collider.bounds.size.y * Vector3.up), _gizmoIcon, false);
            }
        }
#endif
    }
}