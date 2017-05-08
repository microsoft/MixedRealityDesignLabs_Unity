//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_WSA
using UnityEngine.VR.WSA.Input;
#endif

public class InputSourceHands : InputSourceBase, ITargetingInputSource
{
	public event Action<InputSourceBase, bool> OnSelectChanged = delegate { };
	public event Action<InputSourceBase, bool> OnMenuChanged = delegate { };

    public event Action<CurrentHandState> OnFingerPressed = delegate { };
    public event Action<CurrentHandState> OnFingerReleased = delegate { };
    public event Action<CurrentHandState> OnHandEntered = delegate { };
    public event Action<CurrentHandState> OnHandLeft = delegate { };
    public event Action<CurrentHandState> OnHandMoved = delegate { };


    public int NumHandsVisible { get; private set; }
    public int NumFingersPressed { get; private set; }
    public int PrevFingersPressed { get; private set; }

    public const int FirstHandIndex = 0;
    public const int SecondHandIndex = 1;

    bool prevDragging;

    public float BloomGesture_FingerHoldTime = 2f;
    public float FingerHoldTime;

    public ButtonControlState menuGesture = new ButtonControlState();

    // Activation
    public float HandMinVisibleTime = 0.5f;
    public float HandMinReadyTime = 0.25f;
    public float HandReadyDot = 0.7f;
    float m_HandVisibleTime;
    float handReadyTime;

    // Special logic for a hand vector while dragging
    Vector3 dragLocalHandStart;
    Vector3 dragStartHeadPosition;
    Quaternion dragStartHeadRotation = Quaternion.identity;
    public Quaternion adjustedHandTargetRot = Quaternion.identity;

    // Depth gesture
    float dragStartDistance;
    float dragDistanceDeltaVel;
    float dragDistanceLast;
    Vector3 prevHandPosition;

    // Drag control
    public Vector2ControlState dragControl = new Vector2ControlState(false);

	private bool m_Select;
	private bool m_Menu;
    private Vector3 m_LastHandPosition = Vector3.zero;

    private Transform HeadTransform
    {
        get
        {
            return Camera.main.transform;
        }
    }

    // --------------------------------------------------------------------------------

    #region Targeting source interface

    /// <summary>
    /// The item name to use for this debug item.
    /// </summary>
    bool ITargetingInputSource.ShouldActivate()
    {
        return IsDoingHoldGesture();
    }

    Vector3 ITargetingInputSource.GetTargetOrigin()
    {
        return HeadTransform.position;
    }

    Quaternion ITargetingInputSource.GetTargetRotation()
    {
        if (IsManipulating())
        {
            return adjustedHandTargetRot;
        }
        return HeadTransform.rotation;
    }

    bool ITargetingInputSource.IsSelectPressed()
    {
        return NumFingersPressed > 0;
    }

    bool ITargetingInputSource.IsMenuPressed()
    {
        return menuGesture.PressedOnce;
    }

    void ITargetingInputSource.OnActivate(bool activated)
    {
        m_HandVisibleTime = 0;
        handReadyTime = 0;
        FingerHoldTime = 0;
    }

    bool ITargetingInputSource.IsReady()
    {
        return m_HandVisibleTime >= HandMinVisibleTime;
    }

	bool ITargetingInputSource.IsTargetingActive()
	{
		return IsDoingHoldGesture();
	}

	#endregion

	// --------------------------------------------------------------------------------

	public bool IsDoingHoldGesture()
    {
        bool fingerPressed = NumFingersPressed > 0;

        // Consider the hand holding when visible and finger pressed.
        // Ignore the hand ready timeout, usually used to prevent erroneous 
        return (IsHandVisible() && fingerPressed);
    }

    public bool IsHandVisible()
    {
        return m_HandVisibleTime >= HandMinVisibleTime;
    }

    public bool IsAnyHandVisible()
    {
        return NumHandsVisible > 0;
    }

	public bool IsPrimaryHand(CurrentHandState state)
	{
		return trackedHands.Count > 0 && trackedHands[0] == state;
	}

	// Position of hand at index
	public Vector3 GetWorldPosition(int idx)
    {
        if (trackedHands.Count > idx)
        {
            m_LastHandPosition = trackedHands[idx].Position;
        }
        return m_LastHandPosition;
    }

	public CurrentHandState GetHandState(int idx)
	{
		CurrentHandState handState = null;
		if (trackedHands.Count > idx)
		{
			handState = trackedHands[idx];
		}
		return handState;
	}


    public override void _Update()
    {
        PrevFingersPressed = NumFingersPressed;
        NumHandsVisible = trackedHands.Count;
        NumFingersPressed = 0;
        foreach (CurrentHandState hand in trackedHands)
        {
            if (hand.Pressed)
            {
                ++NumFingersPressed;
            }
        }

        // Update hand activation
        UpdateHandActivationState();

        // Update hand dragging, like a special hand based targeting vector while dragging and depth
        UpdateHandDrag();

		bool prev = m_Select;
		m_Select = (this as ITargetingInputSource).IsSelectPressed();
		if (prev != m_Select)
		{
			OnSelectChanged(this, m_Select);
		}

		prev = m_Menu;
		m_Menu = (this as ITargetingInputSource).IsMenuPressed();
		if (prev != m_Menu)
		{
			OnMenuChanged(this, m_Menu);
		}

        base._Update();
    }

    void UpdateHandActivationState()
    {
        // Update finger hold time and menu gesture detection
        if (IsDoingHoldGesture())
        {
            FingerHoldTime += Time.deltaTime;
            //bCompletedMenuGesture = (FingerHoldTime - Time.deltaTime < BloomGesture_FingerHoldTime && FingerHoldTime >= BloomGesture_FingerHoldTime);
        }
        else
        {
            FingerHoldTime = 0;
        }

        menuGesture.ApplyState(FingerHoldTime >= BloomGesture_FingerHoldTime && InputShellMap.Instance.CanCompleteHoldGesture());

        float startVisibleTime = m_HandVisibleTime;
        float startReadyTime = handReadyTime;

        // Update hand state timers
        if (NumHandsVisible > 0)
        {
            // Track the visibility time
            m_HandVisibleTime += Time.deltaTime;

            // Hand 'ready' time... raise in front of look direction
            Vector3 handPos = GetWorldPosition(0);
            if (Vector3.Dot((handPos - HeadTransform.position).normalized, HeadTransform.forward) >= HandReadyDot)
            {
                handReadyTime += Time.deltaTime;
            }
        }

        // If the timers didn't change, they just get reset
        if (m_HandVisibleTime == startVisibleTime)
        {
            m_HandVisibleTime = 0;
        }
        if (handReadyTime == startReadyTime)
        {
            handReadyTime = 0;
        }
    }


    void UpdateHandDrag()
    {
        bool isDragging = NumFingersPressed > 0 && IsManipulating();
        Vector3 handPos = GetWorldPosition(0);

        if (isDragging)
        {
            if (!prevDragging)
            {
                dragStartHeadPosition = HeadTransform.position;

                if (handPos.y < dragStartHeadPosition.y)
                {
                    dragStartHeadPosition.y = handPos.y;
                }

                dragStartHeadRotation = HeadTransform.rotation;

                dragLocalHandStart = Quaternion.Inverse(dragStartHeadRotation) * (handPos - dragStartHeadPosition);

                dragControl.ApplyPos(false, Vector2.zero);

                prevHandPosition = handPos;
                prevDragging = isDragging;
                UpdateHandDrag();
            }
            else
            {
                // Use the head position pivot, but at the starting height 
                Vector3 pivotPos = HeadTransform.position;
                pivotPos.y = dragStartHeadPosition.y;

                // Find where the hand has moved relative to the starting pose (of head and hand)
                Vector3 localOffsetFromStart = Quaternion.Inverse(dragStartHeadRotation) * (handPos - pivotPos);

                // Get the difference in rotation
                Quaternion handRotation = Quaternion.FromToRotation(dragLocalHandStart, localOffsetFromStart);

                // Apply to original head direction
                adjustedHandTargetRot = dragStartHeadRotation * handRotation;

                // Update drag distance
                Vector3 posDelta = handPos - prevHandPosition;
                float pdDot = Vector3.Dot(posDelta.normalized, HeadTransform.forward);
                float distDelta = pdDot * posDelta.magnitude;

                // Deadzone?
                //distDelta = Mathf.Sign(distDelta) * Mathf.Max(0, Mathf.Abs(distDelta) - dragDistanceDeadzone);

                // Update the drag control
                dragControl.ApplyDelta(true, new Vector2(0, distDelta));
            }
        }
        else
        {
            dragControl.ApplyPos(false, Vector2.zero);
        }

        prevHandPosition = handPos;
        prevDragging = isDragging;
    }

    // --------------------------------------------------------------------------------

    #region System-level hand input source

    public class CurrentHandState
    {
        public uint HandId;

        public Vector3 Position;
        public Vector3 Velocity;

        public bool Pressed;
        public bool LastPressed;

        public double SourceLossRisk;
        public Vector3 SourceLossMitigationDirection;
    }

    private static List<CurrentHandState> trackedHands = new List<CurrentHandState>();

    // Subscribe to the hand events
    private void Awake()
    {
#if UNITY_WSA
        InteractionManager.SourceDetected += WSASourceEntered;
        InteractionManager.SourceLost += WSASourceLost;
        InteractionManager.SourceUpdated += WSASourceUpdate;

        InteractionManager.SourcePressed += WSAFingerPressed;
        InteractionManager.SourceReleased += WSAFingerReleased;
#endif
    }

    // Cleanup hand event subscriptions
    private void OnDestroy()
    {
#if UNITY_WSA
        InteractionManager.SourceDetected -= WSASourceEntered;
        InteractionManager.SourceLost -= WSASourceLost;
        InteractionManager.SourceUpdated -= WSASourceUpdate;

        InteractionManager.SourcePressed -= WSAFingerPressed;
        InteractionManager.SourceReleased -= WSAFingerReleased;
#endif
    }

#if UNITY_WSA
    public void WSASourceEntered(InteractionSourceState state)
    {
        // Track Hands
        if (state.source.kind == InteractionSourceKind.Hand)
        {
            CurrentHandState inputState = new CurrentHandState();

            state.properties.location.TryGetPosition(out inputState.Position);

            UpdateFromWSASource(inputState, state);
            SourceEntered(inputState);
        }
    }

    public void WSASourceUpdate(InteractionSourceState state)
    {
        if (state.source.kind == InteractionSourceKind.Hand)
        {
            Vector3 newPosition;
            if (state.properties.location.TryGetPosition(out newPosition))
            {
                CurrentHandState inputState = trackedHands.Find(CurrentInputState => CurrentInputState.HandId == state.source.id); // handID

                UpdateFromWSASource(inputState, state);
                SourceUpdate(inputState, newPosition);
            }
        }
    }

    public void WSASourceLost(InteractionSourceState state)
    {
        if (state.source.kind == InteractionSourceKind.Hand)
        {
            CurrentHandState inputState = trackedHands.Find(CurrentInputState => CurrentInputState.HandId == state.source.id); // handID

            UpdateFromWSASource(inputState, state);
            SourceLost(inputState);
        }
    }

    private void WSAFingerReleased(InteractionSourceState state)
    {
        CurrentHandState inputState = trackedHands.Find(CurrentInputState => CurrentInputState.HandId == state.source.id);
        if (inputState != null)
        {
            UpdateFromWSASource(inputState, state);
            OnFingerReleased(inputState);
        }
    }

    private void WSAFingerPressed(InteractionSourceState state)
    {
        CurrentHandState inputState = trackedHands.Find(CurrentInputState => CurrentInputState.HandId == state.source.id);
        if (inputState != null)
        {
            UpdateFromWSASource(inputState, state);
            OnFingerPressed(inputState);
        }
    }
#endif

    public void SourceEntered(CurrentHandState inputState)
    {
        trackedHands.Add(inputState);
        OnHandEntered(inputState);
    }

    public void SourceUpdate(CurrentHandState inputState, Vector3 newPosition)
    {
        inputState.Position = newPosition;
        OnHandMoved(inputState);
    }

    public void SourceLost(CurrentHandState inputState)
    {
        lock (trackedHands)
        {
            trackedHands.Remove(inputState);
        }

        OnHandLeft(inputState);

        if (trackedHands.Count == 0)
        {

        }
        else
        {
            trackedHands.Sort(
                delegate (CurrentHandState h1, CurrentHandState h2)
                {
                    return h1.HandId == h2.HandId ? 0 : h1.HandId < h2.HandId ? -1 : 1;
                }
            );
        }
    }

#if UNITY_WSA
    private void UpdateFromWSASource(CurrentHandState currentInputState, InteractionSourceState state)
    {
        currentInputState.HandId = state.source.id;
        currentInputState.LastPressed = currentInputState.Pressed;
        currentInputState.Pressed = state.pressed;
        state.properties.location.TryGetVelocity(out currentInputState.Velocity);

        currentInputState.SourceLossRisk = state.properties.sourceLossRisk;
        currentInputState.SourceLossMitigationDirection = state.properties.sourceLossMitigationDirection;
    }
#endif
#endregion
}
