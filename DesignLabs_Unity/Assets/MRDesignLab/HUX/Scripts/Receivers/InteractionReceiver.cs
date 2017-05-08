//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using HUX;
using HUX.Interaction;
using HUX.Focus;

namespace HUX.Receivers
{
	/// <summary>
	/// An interaction receiver is simply a component that attached to a list of interactible objects and does something
	/// based on events from those interactible objects.  This is the base abstact class to extend from.
	/// </summary>
	public abstract class InteractionReceiver : MonoBehaviour
	{
		#region Public Members
		/// <summary>
		/// List of linked interactible objects to receive events for
		/// </summary>
		[Tooltip("Target Interactible Object to receive events for")]
		public List<GameObject> Interactibles = new List<GameObject>();

		/// <summary>
		/// List of linked targets that the receiver affects
		/// </summary>
		[Tooltip("Targets for the receiver to ")]
		public List<GameObject> Targets = new List<GameObject>();

		/// <summary>
		/// Flag for locking focus while selected
		/// </summary>
		[Tooltip("If true, this object will remain the prime focus while select is held")]
		public bool bLockFocus;
		#endregion

		#region Private and Protected Members
		/// <summary>
		/// Internal protected member for our default gizmo icon
		/// </summary>
		protected string _gizmoIconDefault = "HUX/hux_receiver_icon.png";

		/// <summary>
		/// Internal protected member for our gizmo selected icon
		/// </summary>
		protected string _gizmoIconSelected = "HUX/hux_receiver_icon_selected.png";

		/// <summary>
		/// Protected string for the current active gizmo icon
		/// </summary>
		protected string _gizmoIcon;

		/// <summary>
		/// Protected focuser for the current selecting focuser
		/// </summary>
		protected AFocuser _selectingFocuser;
		#endregion

		/// <summary>
		/// On enable subscrible to all interaction events on elements in the interactibles list.
		/// </summary>
		public virtual void OnEnable()
		{
			FocusManager.OnFocusEnter += OnFocusEnter;
			FocusManager.OnFocusExit += OnFocusExit;

			InteractionManager.OnNavigationStarted += OnNavigationStartedInternal;
			InteractionManager.OnNavigationUpdated += OnNavigationUpdatedInternal;
			InteractionManager.OnNavigationCompleted += OnNavigationCompletedInternal;
			InteractionManager.OnNavigationCanceled += OnNavigationCanceledInternal;

			InteractionManager.OnTapped += OnTapInternal;
			InteractionManager.OnDoubleTapped += OnDoubleTapInternal;

			InteractionManager.OnHoldStarted += OnHoldStartedInternal;
			InteractionManager.OnHoldCompleted += OnHoldCompletedInternal;
			InteractionManager.OnHoldCanceled += OnHoldCanceledInternal;

			InteractionManager.OnManipulationStarted += OnManipulationStartedInternal;
			InteractionManager.OnManipulationUpdated += OnManipulationUpdatedInternal;
			InteractionManager.OnManipulationCompleted += OnManipulationCompletedInternal;
			InteractionManager.OnManipulationCanceled += OnManipulationCanceledInternal;
		}

		/// <summary>
		/// On disable remove all linked interacibles from the delegate functions
		/// </summary>
		public virtual void OnDisable()
		{
			FocusManager.OnFocusEnter -= OnFocusEnter;
			FocusManager.OnFocusExit -= OnFocusExit;

			InteractionManager.OnNavigationStarted -= OnNavigationStartedInternal;
			InteractionManager.OnNavigationUpdated -= OnNavigationUpdatedInternal;
			InteractionManager.OnNavigationCompleted -= OnNavigationCompletedInternal;
			InteractionManager.OnNavigationCanceled -= OnNavigationCanceledInternal;

			InteractionManager.OnTapped -= OnTapInternal;
			InteractionManager.OnDoubleTapped -= OnDoubleTapInternal;

			InteractionManager.OnHoldStarted -= OnHoldStartedInternal;
			InteractionManager.OnHoldCompleted -= OnHoldCompletedInternal;
			InteractionManager.OnHoldCanceled -= OnHoldCanceledInternal;

			InteractionManager.OnManipulationStarted -= OnManipulationStartedInternal;
			InteractionManager.OnManipulationUpdated -= OnManipulationUpdatedInternal;
			InteractionManager.OnManipulationCompleted -= OnManipulationCompletedInternal;
			InteractionManager.OnManipulationCanceled -= OnManipulationCanceledInternal;
		}

		/// <summary>
		/// Register an interactible with this receiver.
		/// </summary>
		/// <param name="interactible">takes a GameObject as the interactible to register.</param>
		public virtual void RegisterInteractible(GameObject interactible)
		{
			if (interactible == null || Interactibles.Contains(interactible))
				return;

			Interactibles.Add(interactible);
		}

#if UNITY_EDITOR
		/// <summary>
		/// When selected draw lines to all linked interactibles
		/// </summary>
		protected virtual void OnDrawGizmosSelected()
		{
			if (this.Interactibles.Count > 0)
			{
				GameObject[] bioList = this.Interactibles.ToArray();

				for (int i = 0; i < bioList.Length; i++)
				{
					if (bioList[i] != null)
					{
						Gizmos.color = Color.green;
						Gizmos.DrawLine(this.transform.position, bioList[i].transform.position);
					}
				}
			}

			if (this.Targets.Count > 0)
			{
				GameObject[] targetList = this.Targets.ToArray();

				for (int i = 0; i < targetList.Length; i++)
				{
					if (targetList[i] != null)
					{
						Gizmos.color = Color.red;
						Gizmos.DrawLine(this.transform.position, targetList[i].transform.position);
					}
				}
			}
		}

		/// <summary>
		/// On Draw Gizmo show the receiver icon
		/// </summary>
		protected virtual void OnDrawGizmos()
		{
			_gizmoIcon = UnityEditor.Selection.activeGameObject == this.gameObject ? _gizmoIconSelected : _gizmoIconDefault;
			Gizmos.DrawIcon(this.transform.position, _gizmoIcon, false);
		}
#endif

		/// <summary>
		/// Function to remove an interactible from the linked list.
		/// </summary>
		/// <param name="interactible"></param>
		public virtual void RemoveInteractible(GameObject interactible)
		{
			if (interactible != null && Interactibles.Contains(interactible))
			{
				Interactibles.Remove(interactible);
			}
		}

		/// <summary>
		/// Clear the interactibles list and unregister them
		/// </summary>
		public virtual void ClearInteractibles()
		{
			GameObject[] _intList = Interactibles.ToArray();

			for (int i = 0; i < _intList.Length; i++)
			{
				this.RemoveInteractible(_intList[i]);
			}
		}

		/// <summary>
		/// Is the game object interactible in our list of interactibles
		/// </summary>
		/// <param name="interactible"></param>
		/// <returns></returns>
		protected bool IsInteractible(GameObject interactible)
		{
			return (Interactibles != null && Interactibles.Contains(interactible));
		}

		/// <summary>
		/// Check if the object is in our list - if yes 
		/// </summary>
		/// <param name="sendEvent"></param>
		/// <param name="obj"></param>
		/// <param name="eventArgs"></param>
		private void CheckAndSendEvent(System.Action<GameObject, InteractionManager.InteractionEventArgs> sendEvent, GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			if (IsInteractible(obj))
			{
				sendEvent(obj, eventArgs);
			}
		}

		private void CheckLockFocus(AFocuser focuser)
		{
			if (bLockFocus)
			{
				//LockFocus(focuser);
			}
		}

		private void LockFocus(AFocuser focuser)
		{
			if (focuser != null)
			{
				ReleaseFocus();
				_selectingFocuser = focuser;
				_selectingFocuser.LockFocus();
			}
		}

		private void ReleaseFocus()
		{
			if (_selectingFocuser != null)
			{
				_selectingFocuser.ReleaseFocus();
				_selectingFocuser = null;
			}
		}

#region Focus Events

        protected virtual void OnFocusEnter(GameObject obj, FocusArgs args) { }
        protected virtual void OnFocusExit(GameObject obj, FocusArgs args) { }

#endregion

#region Manipulation & Navigation Events
        /// <summary>
        /// Events recieved from Interaction Manager when any object gets an event
        /// </summary>
        /// <param name="obj"> The object that recieved the event. </param>
        /// <param name="eventArgs"> The event arguments sent to that object. </param>
        protected virtual void OnNavigationStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnNavigationUpdated(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnNavigationCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnNavigationCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }

        protected virtual void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnDoubleTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }

        protected virtual void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }

        protected virtual void OnManipulationStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnManipulationUpdated(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnManipulationCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
        protected virtual void OnManipulationCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { }
#endregion

#region Manipulation & Navigation Internal Events
        /// <summary>
        /// The internal events. This recieves the event from Interaction Manager,
        /// checks to see if the object is in the list of interactibles - if yes sends the correct event.
        /// </summary>
        /// <param name="obj"> The object that recieved the event. </param>
        /// <param name="eventArgs"> The event arguments sent to that object. </param>
        private void OnNavigationStartedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckLockFocus(eventArgs.Focuser); CheckAndSendEvent(OnNavigationStarted, obj, eventArgs); }
        private void OnNavigationUpdatedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckAndSendEvent(OnNavigationUpdated, obj, eventArgs); }
        private void OnNavigationCompletedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnNavigationCompleted, obj, eventArgs); }
        private void OnNavigationCanceledInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnNavigationCanceled, obj, eventArgs); }

        private void OnTapInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckAndSendEvent(OnTapped, obj, eventArgs); }
        private void OnDoubleTapInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckAndSendEvent(OnDoubleTapped, obj, eventArgs); }

        private void OnHoldStartedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckLockFocus(eventArgs.Focuser); CheckAndSendEvent(OnHoldStarted, obj, eventArgs); }
        private void OnHoldCompletedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnHoldCompleted, obj, eventArgs); }
        private void OnHoldCanceledInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnHoldCanceled, obj, eventArgs); }

        private void OnManipulationStartedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckLockFocus(eventArgs.Focuser); CheckAndSendEvent(OnManipulationStarted, obj, eventArgs); }
        private void OnManipulationUpdatedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { CheckAndSendEvent(OnManipulationUpdated, obj, eventArgs); }
        private void OnManipulationCompletedInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnManipulationCompleted, obj, eventArgs); }
        private void OnManipulationCanceledInternal(GameObject obj, InteractionManager.InteractionEventArgs eventArgs) { ReleaseFocus(); CheckAndSendEvent(OnManipulationCanceled, obj, eventArgs); }
#endregion

    }
}
