//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.XR.WSA.Input;
using HUX.Utility;
using HUX.Focus;

namespace HUX.Interaction
{
    public class InteractionManager : Singleton<InteractionManager>
    {
        public struct InteractionEventArgs
        {
            /// <summary>
            /// The focuser that triggered this event.
            /// </summary>
            public readonly AFocuser Focuser;

            public readonly Vector3 Position;
            public readonly bool IsPosRelative;
            public readonly Ray GazeRay;

            public InteractionEventArgs(AFocuser focuser, Vector3 pos, bool isRelative, Ray gazeRay)
            {
                this.Focuser = focuser;
                this.Position = pos;
                this.IsPosRelative = isRelative;
                this.GazeRay = gazeRay;
            }
        }

        /// <summary>
        /// Currently active gesture recognizer.
        /// </summary>
        public GestureRecognizer ActiveRecognizer { get; private set; }

		public UnityEngine.XR.WSA.Input.InteractionManager ActiveInteractionManager { get; private set; }

        /// <summary>
        /// is the user currently navigating.
        /// </summary>
        public bool IsNavigating { get; private set; }

        /// <summary>
        /// Is the user currently manipulating.
        /// </summary>
        public bool IsManipulating { get; private set; }

        /// <summary>
        /// The current manipulation position.
        /// </summary>
        [Obsolete]
        public Vector3 ManipulationPosition { get; private set; }

        /// <summary>
        /// Which gestures the interaction manager's active recognizer will capture
        /// </summary>
        public UnityEngine.XR.WSA.Input.GestureSettings RecognizableGesures = UnityEngine.XR.WSA.Input.GestureSettings.Tap | UnityEngine.XR.WSA.Input.GestureSettings.DoubleTap | UnityEngine.XR.WSA.Input.GestureSettings.Hold | UnityEngine.XR.WSA.Input.GestureSettings.NavigationX | UnityEngine.XR.WSA.Input.GestureSettings.NavigationY;

        /// <summary>
        /// Events that trigger manipulation or navigation is done on the specified object.
        /// </summary>
        public static event Action<GameObject, InteractionEventArgs> OnNavigationStarted;
        public static event Action<GameObject, InteractionEventArgs> OnNavigationUpdated;
        public static event Action<GameObject, InteractionEventArgs> OnNavigationCompleted;
        public static event Action<GameObject, InteractionEventArgs> OnNavigationCanceled;

        public static event Action<GameObject, InteractionEventArgs> OnTapped;
        public static event Action<GameObject, InteractionEventArgs> OnDoubleTapped;

        public static event Action<GameObject, InteractionEventArgs> OnHoldStarted;
        public static event Action<GameObject, InteractionEventArgs> OnHoldCompleted;
        public static event Action<GameObject, InteractionEventArgs> OnHoldCanceled;

 
        public static event Action<GameObject, InteractionEventArgs> OnManipulationStarted;
        public static event Action<GameObject, InteractionEventArgs> OnManipulationUpdated;
        public static event Action<GameObject, InteractionEventArgs> OnManipulationCompleted;
        public static event Action<GameObject, InteractionEventArgs> OnManipulationCanceled;

		public static event Action<GameObject, InteractionEventArgs> OnPressed;
		public static event Action<GameObject, InteractionEventArgs> OnReleased;

		/// <summary>
		/// If true, lock focus on the interactible object when an interaction begins,
		/// so that when looking away from it, the interaction events continue to fire
		/// </summary>
		public bool bLockFocus;

		/// <summary>
		/// The max duration of time that can pass after a tap to consider the next tap
		/// part of the multi-tap (as in, double tap etc)
		/// </summary>
		public float MultiTapWindow = 0.75f;

		/// <summary>
		/// The time that the last hold was released
		/// </summary>
		float lastHoldReleaseTime;

		// This should be Start instead of Awake right?
		protected void Start()
        {
            ActiveRecognizer = new GestureRecognizer();

            ActiveRecognizer.SetRecognizableGestures(RecognizableGesures);

            ActiveRecognizer.Tapped += TappedCallback;

            ActiveRecognizer.NavigationStarted += NavigationStartedCallback;
            ActiveRecognizer.NavigationUpdated += NavigationUpdatedCallback;
            ActiveRecognizer.NavigationCompleted += NavigationCompletedCallback;
            ActiveRecognizer.NavigationCanceled += NavigationCanceledCallback;

            ActiveRecognizer.HoldStarted += HoldStartedCallback;
            ActiveRecognizer.HoldCompleted += HoldCompletedCallback;
            ActiveRecognizer.HoldCanceled += HoldCanceledCallback;

            ActiveRecognizer.ManipulationStarted += ManipulationStartedCallback;
            ActiveRecognizer.ManipulationUpdated += ManipulationUpdatedCallback;
            ActiveRecognizer.ManipulationCompleted += ManipulationCompletedCallback;
            ActiveRecognizer.ManipulationCanceled += ManipulationCanceledCallback;

            ActiveRecognizer.StartCapturingGestures();
            SetupEvents(true);

            UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourcePressed += InteractionManager_SourcePressedCallback;
            UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleased += InteractionManager_SourceReleasedCallback;

			bLockFocus = true;
		}

		private void InteractionManager_SourcePressedCallback(InteractionSourcePressedEventArgs args)
		{
			AFocuser focuser = GetFocuserForSource(args.state.source.kind);
			OnPressedEvent(focuser);
		}

		private void InteractionManager_SourceReleasedCallback(InteractionSourceReleasedEventArgs args)
		{
			AFocuser focuser = GetFocuserForSource(args.state.source.kind);
			OnReleasedEvent(focuser);
		}

		void Update()
		{
			// Simulate manipulation events when on a platform that doesn't have GestureRecognizer.
			// TODO: Simulate navigation events?
			// Only do manipulation event simulation in editor, and should we do it on PC?
			if (Application.platform == RuntimePlatform.WindowsEditor /*|| Application.platform == RuntimePlatform.WindowsPlayer*/)
			{
				AFocuser focuser = GetFocuserForSource(InteractionSourceKind.Hand);

				// Use the head's position + forward, since the focuser ray might point to the locked focus
				Vector3 newPos = Veil.Instance.HeadTransform.position + Veil.Instance.HeadTransform.forward;

				if (InputShell.Instance.SelectButton.PressedOnce)
				{
					ManipulationStartedEvent(focuser, newPos, focuser.FocusRay);
				}
				else if (InputShell.Instance.SelectButton.pressed)
				{
					ManipulationUpdatedEvent(focuser, newPos, focuser.FocusRay);
				}
				else if (InputShell.Instance.SelectButton.ReleasedOnce)
				{
					ManipulationCompletedEvent(focuser, newPos, focuser.FocusRay);
				}
			}
		}

		void SetupEvents(bool add)
        {
            if (add)
            {
                InputShell.Instance.OnTargetSourceSelectChanged += OnShellSelectEvent;
                InputShell.Instance.ZoomVector.OnMove += OnShellZoomEvent;
                InputShell.Instance.ZoomVector.buttonState.OnChanged += OnShellZoomEvent;
                InputShell.Instance.ScrollVector.OnMove += OnShellScrollEvent;
                InputShell.Instance.ScrollVector.buttonState.OnChanged += OnShellScrollEvent;
            }
            else
            {
                InputShell.Instance.OnTargetSourceSelectChanged -= OnShellSelectEvent;
                InputShell.Instance.ZoomVector.OnMove -= OnShellZoomEvent;
                InputShell.Instance.ZoomVector.buttonState.OnChanged -= OnShellZoomEvent;
                InputShell.Instance.ScrollVector.OnMove -= OnShellScrollEvent;
                InputShell.Instance.ScrollVector.buttonState.OnChanged -= OnShellScrollEvent;
            }
        }

        void OnShellSelectEvent(InputSourceBase inputSource, bool newState)
        {
            AFocuser focuser = InputSourceFocuser.GetFocuserForInputSource(inputSource);
            if (focuser != null)
            {
                if (newState)
                {
                    if (inputSource != InputSources.Instance.hands)
                    {
						OnPressedEvent(focuser);

						// Need to track hold time before firing this
						HoldStartedEvent(focuser, focuser.FocusRay);
					}
                }
                else
                {
                    if (inputSource != InputSources.Instance.hands)
                    {
						OnReleasedEvent(focuser);

						// Only fire this if HoldStarted was fired
						HoldCompletedEvent(focuser, focuser.FocusRay);

						// Send double tap (should be multi-tap someday)
						if (Time.time - lastHoldReleaseTime <= MultiTapWindow)
						{
							DoubleTappedEvent(focuser, focuser.FocusRay);
						}
						
						// Need to only fire this if hold wasn't started?
						TappedEvent(focuser, focuser.FocusRay);

						lastHoldReleaseTime = Time.time;
					}
                }
            }
        }

        void OnShellZoomEvent()
        {
            AFocuser focuser = InputShellMap.Instance.inputSwitchLogic.GetFocuserForCurrentTargetingSource();
            if (focuser != null && focuser.PrimeFocus != null)
            {
                focuser.PrimeFocus.SendMessage("OnZoom", null, SendMessageOptions.DontRequireReceiver);
            }
        }

        void OnShellScrollEvent()
        {
            AFocuser focuser = InputShellMap.Instance.inputSwitchLogic.GetFocuserForCurrentTargetingSource();
            if (focuser != null && focuser.PrimeFocus != null)
            {
                focuser.PrimeFocus.SendMessage("OnScroll", null, SendMessageOptions.DontRequireReceiver);
            }
        }

		void OnPressedEvent(AFocuser focuser)
		{
			GameObject focusObject = focuser.PrimeFocus;
			InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, focuser.FocusRay);

			if (focusObject != null)
			{
				focusObject.SendMessage("Pressed", eventArgs, SendMessageOptions.DontRequireReceiver);
				SendEvent(OnPressed, focusObject, eventArgs);
				CheckLockFocus(focuser);
			}
		}

		void OnReleasedEvent(AFocuser focuser)
		{			
			GameObject focusObject = focuser.PrimeFocus;
			InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, focuser.FocusRay);

			if (focusObject != null)
			{
				focusObject.SendMessage("Released", eventArgs, SendMessageOptions.DontRequireReceiver);
				SendEvent(OnReleased, focusObject, eventArgs);
				ReleaseFocus(focuser);
			}
		}

		private void CheckLockFocus(AFocuser focuser)
		{
			if (bLockFocus)
			{
				LockFocus(focuser);
			}
		}

		private void LockFocus(AFocuser focuser)
		{
			if (focuser != null)
			{
				ReleaseFocus(focuser);
				focuser.LockFocus();
			}
		}

		private void ReleaseFocus(AFocuser focuser)
		{
			if (focuser != null)
			{
				focuser.ReleaseFocus();
				focuser = null;
			}
		}

		void OnDestroy()
        {
            ActiveRecognizer.Tapped -= TappedCallback;

            ActiveRecognizer.NavigationStarted -= NavigationStartedCallback;
            ActiveRecognizer.NavigationUpdated -= NavigationUpdatedCallback;
            ActiveRecognizer.NavigationCompleted -= NavigationCompletedCallback;
            ActiveRecognizer.NavigationCanceled -= NavigationCanceledCallback;

            ActiveRecognizer.HoldStarted -= HoldStartedCallback;
            ActiveRecognizer.HoldCompleted -= HoldCompletedCallback;
            ActiveRecognizer.HoldCanceled -= HoldCanceledCallback;

            ActiveRecognizer.ManipulationStarted -= ManipulationStartedCallback;
            ActiveRecognizer.ManipulationUpdated -= ManipulationUpdatedCallback;
            ActiveRecognizer.ManipulationCompleted -= ManipulationCompletedCallback;
            ActiveRecognizer.ManipulationCanceled -= ManipulationCanceledCallback;

            ActiveRecognizer.CancelGestures();

            SetupEvents(false);
        }


		private AFocuser GetFocuserForSource(UnityEngine.XR.WSA.Input.InteractionSourceKind source)
		{
			AFocuser focuser = FocusManager.Instance != null ? FocusManager.Instance.GazeFocuser : null;
			switch (source)
			{
				case UnityEngine.XR.WSA.Input.InteractionSourceKind.Hand:
				{
					focuser = InputSourceFocuser.GetFocuserForInputSource(InputSources.Instance.hands);
					break;
				}

				case UnityEngine.XR.WSA.Input.InteractionSourceKind.Controller:
				{
					focuser = InputSourceFocuser.GetFocuserForInputSource(InputSources.Instance.sixDOFRay);
					break;
				}
			}
			return focuser;
		}

        #region Events

        /// <summary>
        /// Navigation Started event fires an message event upwards on the focus object for "NavigationStarted"
        /// and calls OnNavigationStarted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="relativePosition"></param>
        /// <param name="ray"></param>
        private void NavigationStartedEvent(AFocuser focuser, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = true;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, relativePosition, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("NavigationStarted", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnNavigationStarted, focusObject, eventArgs);
					CheckLockFocus(focuser);
				}

                if (focuser.UIInteractibleFocus != null)
                {
                    WorldGraphicsRaycaster.RayEventData eventData = focuser.GetPointerData() as WorldGraphicsRaycaster.RayEventData;

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.pointerPress = focuser.UIInteractibleFocus;
                    eventData.selectedObject = focuser.UIInteractibleFocus;

                    eventData.dragging = true;
                    eventData.eligibleForClick = false;
                    eventData.pointerDrag = focuser.UIInteractibleFocus;

                    Vector2 curPos = eventData.position;
                    eventData.InitialDragHandPos = eventData.pointerPressRaycast.worldPosition + (Camera.main.transform.forward * eventData.pointerPressRaycast.distance);
                    eventData.position = Camera.main.WorldToScreenPoint(eventData.InitialDragHandPos);

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.beginDragHandler);

                    eventData.position = curPos;
                    eventData.CurrentAddition = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Navigation Updated event fires an message event upwards on the focus object for "NavigationUpdated"
        /// and calls OnNavigationUpdated for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="relativePosition"></param>
        /// <param name="ray"></param>
        private void NavigationUpdatedEvent(AFocuser focuser, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = true;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, relativePosition, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("NavigationUpdated", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnNavigationUpdated, focusObject, eventArgs);
                }

                WorldGraphicsRaycaster.RayEventData pointerEventData = focuser.GetPointerData() as WorldGraphicsRaycaster.RayEventData;
                if (pointerEventData.pointerDrag != null)
                {
                    pointerEventData.delta = relativePosition;
                    Vector2 curPos = pointerEventData.position;

                    pointerEventData.CurrentAddition += relativePosition * pointerEventData.ScaleMultiplier;
                    Vector3 offsetpos = pointerEventData.InitialDragHandPos + pointerEventData.CurrentAddition;

                    pointerEventData.position = Camera.main.WorldToScreenPoint(offsetpos);
                    FocusManager.Instance.ExecuteUIFocusEvent(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.dragHandler);

                    pointerEventData.position = curPos;
                }
            }
        }

        /// <summary>
        /// Navigation Completed event fires an message event upwards on the focus object for "NavigationCompleted"
        /// and calls OnNavigationCompleted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="relativePosition"></param>
        /// <param name="ray"></param>
        private void NavigationCompletedEvent(AFocuser focuser, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;

                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, relativePosition, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("NavigationCompleted", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnNavigationCompleted, focusObject, eventArgs);
					ReleaseFocus(focuser);
                }

                if (focuser.UIInteractibleFocus != null)
                {
                    WorldGraphicsRaycaster.RayEventData pointerEventData = focuser.GetPointerData() as WorldGraphicsRaycaster.RayEventData;
                    if (pointerEventData.pointerDrag != null)
                    {
                        pointerEventData.delta = relativePosition;
                        Vector2 curPos = pointerEventData.position;

                        pointerEventData.CurrentAddition += relativePosition * pointerEventData.ScaleMultiplier;
                        Vector3 offsetpos = pointerEventData.InitialDragHandPos + pointerEventData.CurrentAddition;

                        pointerEventData.position = Camera.main.WorldToScreenPoint(offsetpos);

                        FocusManager.Instance.ExecuteUIFocusEvent(pointerEventData.pointerDrag, pointerEventData, ExecuteEvents.endDragHandler);
                        pointerEventData.position = curPos;
                        pointerEventData.pointerDrag = null;
                        pointerEventData.dragging = false;
                    }

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, pointerEventData, ExecuteEvents.pointerUpHandler);
                    pointerEventData.selectedObject = null;
                    pointerEventData.pointerPress = null;
                }
            }
        }

        /// <summary>
        /// Navigation Canceled event fires an message event upwards on the focus object for "NavigationCanceled"
        /// and calls OnNavigationCanceled for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="relativePosition"></param>
        /// <param name="ray"></param>
        private void NavigationCanceledEvent(AFocuser focuser, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, relativePosition, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("NavigationCanceled", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnNavigationCanceled, focusObject, eventArgs);
					ReleaseFocus(focuser);
				}

				if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData pointerEventData = focuser.GetPointerData();

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, focuser.GetPointerData(), ExecuteEvents.endDragHandler);
                    pointerEventData.pointerDrag = null;

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, focuser.GetPointerData(), ExecuteEvents.pointerUpHandler);
                    pointerEventData.selectedObject = null;
                }
            }
        }

        /// <summary>
        /// Tapped event fires an message event upwards on the focus object for "Tapped"
        /// and calls OnTapped for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="ray"></param>
        private void TappedEvent(AFocuser focuser, Ray ray)
        {
            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("Tapped", eventArgs, SendMessageOptions.DontRequireReceiver);
                }

                SendEvent(OnTapped, focusObject, eventArgs);

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    eventData.pointerPress = focuser.UIInteractibleFocus;
                    eventData.eligibleForClick = true;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.selectedObject = focuser.UIInteractibleFocus;

                    eventData.clickCount = 1;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerClickHandler);

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;
                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                }
            }
        }

        /// <summary>
        /// DoubleTapped event fires an message event upwards on the focus object for "DoubleTapped"
        /// and calls OnDoubleTapped for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="ray"></param>
        private void DoubleTappedEvent(AFocuser focuser, Ray ray)
        {
            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("DoubleTapped", eventArgs, SendMessageOptions.DontRequireReceiver);
                }

                SendEvent(OnDoubleTapped, focusObject, eventArgs);

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    eventData.pointerPress = focuser.UIInteractibleFocus;
                    eventData.eligibleForClick = true;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.selectedObject = focuser.UIInteractibleFocus;
                    eventData.clickCount = 1;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerClickHandler);

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;
                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.selectedObject = focuser.UIInteractibleFocus;


                    eventData.clickCount = 2;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerClickHandler);

                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;
                    eventData.eligibleForClick = false;
                    eventData.pointerPress = null;
                    eventData.clickCount = 0;
                }
            }
        }

        /// <summary>
        /// HoldStarted event fires an message event upwards on the focus object for "HoldStarted"
        /// and calls OnHoldStarted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="ray"></param>
        private void HoldStartedEvent(AFocuser focuser, Ray ray)
        {
            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("HoldStarted", eventArgs, SendMessageOptions.DontRequireReceiver);
					SendEvent(OnHoldStarted, focusObject, eventArgs);
					CheckLockFocus(focuser);
                }

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    eventData.eligibleForClick = true;
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.pointerPress = focuser.UIInteractibleFocus;
                    eventData.selectedObject = focuser.UIInteractibleFocus;
                }
            }
        }

        /// <summary>
        /// HoldCompleted event fires an message event upwards on the focus object for "HoldCompleted"
        /// and calls OnHoldCompleted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="ray"></param>
        private void HoldCompletedEvent(AFocuser focuser, Ray ray)
        {
            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("HoldCompleted", eventArgs, SendMessageOptions.DontRequireReceiver);
					SendEvent(OnHoldCompleted, focusObject, eventArgs);
					ReleaseFocus(focuser);
                }

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerClickHandler);
                    eventData.selectedObject = null;

                    eventData.pointerPress = null;
                    eventData.eligibleForClick = false;
                }
            }
        }

        /// <summary>
        /// HoldCanceled event fires an message event upwards on the focus object for "HoldCanceled"
        /// and calls OnHoldCanceled for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="ray"></param>
        private void HoldCanceledEvent(AFocuser focuser, Ray ray)
        {
            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, Vector3.zero, true, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessage("HoldCanceled", eventArgs, SendMessageOptions.DontRequireReceiver);
					SendEvent(OnHoldCanceled, focusObject, eventArgs);
					ReleaseFocus(focuser);
				}


                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;

                    eventData.pointerPress = null;
                    eventData.eligibleForClick = false;
                }
            }
        }

        /// <summary>
        /// ManipulationStarted event fires an message event upwards on the focus object for "ManipulationStarted"
        /// and calls OnManipulationStarted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="position"></param>
        /// <param name="ray"></param>
        private void ManipulationStartedEvent(AFocuser focuser, Vector3 position, Ray ray)
        {
            IsManipulating = true;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, position, false, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessageUpwards("ManipulationStarted", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnManipulationStarted, focusObject, eventArgs);
					CheckLockFocus(focuser);
                }

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerDownHandler);
                    eventData.selectedObject = focuser.UIInteractibleFocus;
                }
            }
        }

        /// <summary>
        /// ManipulationUpdated event fires an message event upwards on the focus object for "ManipulationUpdated"
        /// and calls OnManipulationUpdated for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="position"></param>
        /// <param name="ray"></param>
        private void ManipulationUpdatedEvent(AFocuser focuser, Vector3 position, Ray ray)
        {
            IsManipulating = true;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, position, false, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessageUpwards("ManipulationUpdated", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnManipulationUpdated, focusObject, eventArgs);
                }

                FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, focuser.GetPointerData(), ExecuteEvents.moveHandler);
            }
        }

        /// <summary>
        /// ManipulationCompleted event fires an message event upwards on the focus object for "ManipulationCompleted"
        /// and calls OnManipulationCompleted for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="position"></param>
        /// <param name="ray"></param>
        private void ManipulationCompletedEvent(AFocuser focuser, Vector3 position, Ray ray)
        {
            IsManipulating = false;

            if (focuser != null)
            {
                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, position, false, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessageUpwards("ManipulationCompleted", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnManipulationCompleted, focusObject, eventArgs);
					ReleaseFocus(focuser);
                }

                if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, eventData, ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;
                }
            }
        }

        /// <summary>
        /// ManipulationCanceled event fires an message event upwards on the focus object for "ManipulationCanceled"
        /// and calls OnManipulationCanceled for InteractibleObjects
        /// </summary>
        /// <param name="focuser"></param>
        /// <param name="position"></param>
        /// <param name="ray"></param>
        private void ManipulationCanceledEvent(AFocuser focuser, Vector3 position, Ray ray)
        {
            IsManipulating = false;

            if (focuser != null)
            {

                GameObject focusObject = focuser.PrimeFocus;
                InteractionEventArgs eventArgs = new InteractionEventArgs(focuser, position, false, ray);

                if (focusObject != null)
                {
                    focusObject.SendMessageUpwards("ManipulationCanceled", eventArgs, SendMessageOptions.DontRequireReceiver);
                    SendEvent(OnManipulationCanceled, focusObject, eventArgs);
					ReleaseFocus(focuser);
				}

				if (focuser.UIInteractibleFocus != null)
                {
                    PointerEventData eventData = focuser.GetPointerData();
                    FocusManager.Instance.ExecuteUIFocusEvent(focuser.UIInteractibleFocus, focuser.GetPointerData(), ExecuteEvents.pointerUpHandler);
                    eventData.selectedObject = null;
                }
            }
        }

        /// <summary>
        /// Send event internal function for call all registered send event delegates
        /// </summary>
        /// <param name="sendEvent"></param>
        /// <param name="gameObject"></param>
        /// <param name="eventArgs"></param>
        private void SendEvent(Action<GameObject, InteractionEventArgs> sendEvent, GameObject gameObject, InteractionEventArgs eventArgs)
        {
            if (sendEvent != null)
            {
                sendEvent(gameObject, eventArgs);
            }
        }

        #endregion

        #region Gesture Recognizer Callbacks
        private void NavigationStartedCallback(UnityEngine.XR.WSA.Input.NavigationStartedEventArgs args)
        {
            AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 relativePosition;
            Vector3 forward;
            
            if (args.sourcePose.TryGetForward(out forward)  && args.sourcePose.TryGetPosition(out relativePosition))
            {
                Ray ray = new Ray(relativePosition, forward);

                if (focuser != null)
                {
                    NavigationStartedEvent(focuser, relativePosition, ray);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
            }
        }

        private void NavigationUpdatedCallback(NavigationUpdatedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        NavigationUpdatedEvent(focuser, position, ray);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
            }
        }

        private void NavigationCompletedCallback(NavigationCompletedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        NavigationCompletedEvent(focuser, position, ray);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
            }
        }

        private void NavigationCanceledCallback(UnityEngine.XR.WSA.Input.NavigationCanceledEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        NavigationCanceledEvent(focuser, position, ray);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
            }

        }

        private void TappedCallback(UnityEngine.XR.WSA.Input.TappedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        if (args.tapCount >= 2)
                        {
                            DoubleTappedEvent(focuser, ray);
                        }
                        else
                        {
                            TappedEvent(focuser, ray);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }

        }

        private void HoldStartedCallback(UnityEngine.XR.WSA.Input.HoldStartedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        HoldStartedEvent(focuser, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void HoldCompletedCallback(UnityEngine.XR.WSA.Input.HoldCompletedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        HoldCompletedEvent(focuser, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void HoldCanceledCallback(UnityEngine.XR.WSA.Input.HoldCanceledEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        HoldCanceledEvent(focuser, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void ManipulationStartedCallback(UnityEngine.XR.WSA.Input.ManipulationStartedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        ManipulationStartedEvent(focuser, position, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void ManipulationUpdatedCallback(UnityEngine.XR.WSA.Input.ManipulationUpdatedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        ManipulationUpdatedEvent(focuser, position, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void ManipulationCompletedCallback(UnityEngine.XR.WSA.Input.ManipulationCompletedEventArgs args)
        {
			AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);
                    if (focuser != null)
                    {
                        ManipulationCompletedEvent(focuser, position, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        private void ManipulationCanceledCallback(UnityEngine.XR.WSA.Input.ManipulationCanceledEventArgs args)
        {
            AFocuser focuser = GetFocuserForSource(args.source.kind);
            Vector3 position;
            Vector3 forward;
            Ray ray;

            if (args.sourcePose.TryGetPosition(out position))
            {
                if (args.sourcePose.TryGetForward(out forward))
                {
                    ray = new Ray(position, forward);

                    if (focuser != null)
                    {
                        ManipulationCanceledEvent(focuser, position, ray);
                    }
                }
                else
                {
                    Debug.LogWarning("Failed to get pointer ray source ID: " + args.source.id);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get pointer position source ID: " + args.source.id);
            }
        }

        #endregion
    }
}
