// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Linq;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public class InputSourceMotionController : InputSourceSixDOFBase
{
    [Tooltip("Handedness for motion controller")] 
    public InteractionSourceHandedness Handedness = InteractionSourceHandedness.Right;
    public GameObject ControllerPrefab;

    public class ControllerState
    {
        public InteractionSourceHandedness Handedness; 
        public Vector3 PointerPosition = Vector3.zero; 
        public Quaternion PointerRotation = Quaternion.identity; 
        public Vector3 GripPosition = Vector3.zero; 
        public Quaternion GripRotation = Quaternion.identity; 
        public bool Grasped; 
        public bool MenuPressed; 
        public bool SelectPressed; 
        public float SelectPressedAmount; 
        public bool ThumbstickPressed; 
        public Vector2 ThumbstickPosition = Vector2.zero; 
        public bool TouchpadPressed; 
        public bool TouchpadTouched; 
        public Vector2 TouchpadPosition = Vector2.zero;
        public Vector3 LinearVelocity = Vector3.zero;
        public Vector3 AngularVelocity = Vector3.zero;
    }

    public ControllerState CurrentState { get { return m_currentState; } } 
    public uint ID { get { return m_controllerID; } } 

    private ControllerState m_currentState = new ControllerState(); 
    private uint m_controllerID; 
    private bool m_controllerPresent;
    private GameObject m_controller;

    public void OnEnable()
    {
#if UNITY_WSA
        InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
#endif
        if (ControllerPrefab != null)
        {
            m_controller = Instantiate(ControllerPrefab, transform);
            m_controller.transform.localPosition = Vector3.zero;
        }
    }

    public void OnDisable()
    {
#if UNITY_WSA
        InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
        InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;
        InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
#endif
        if(m_controller != null)
        {
            Destroy(m_controller);
        }
    }

    private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
    {
        if (IsSourceValid(obj.state.source) && m_controllerID == obj.state.source.id)
        {
            UpdateState(obj.state);
        }
    }

    private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
    {
        if (IsSourceValid(obj.state.source) && m_controllerID == obj.state.source.id)
        {
            m_controllerPresent = false;
        }
    }

    private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
    {
        if (IsSourceValid(obj.state.source))
        {
            if (!m_controllerPresent)
            {
                m_controllerID = obj.state.source.id;
                UpdateState(obj.state);
                m_controllerPresent = true;
            }
            else 
            {
                Debug.LogWarning("[MotionControllerInput] Two same handed controllers present!");
            }
        }
    }

    public override Vector3 position
    {
        get
        {
            return m_currentState.PointerPosition;
        }
    }

    public override Quaternion rotation
    {
        get
        {
            return m_currentState.PointerRotation;
        }
    }



    private bool IsSourceValid(UnityEngine.XR.WSA.Input.InteractionSource source)
    { 
        return source.kind == UnityEngine.XR.WSA.Input.InteractionSourceKind.Controller && source.handedness == Handedness;  
    } 

    /// <summary> 
    /// Process the updated pose data 
    /// </summary> 
    /// <param name="sourcePose"></param> 
    private void UpdatePose(InteractionSourcePose sourcePose)
    { 
        sourcePose.TryGetPosition(out m_currentState.PointerPosition, InteractionSourceNode.Pointer); 
        sourcePose.TryGetRotation(out m_currentState.PointerRotation, InteractionSourceNode.Pointer); 
        sourcePose.TryGetPosition(out m_currentState.GripPosition, InteractionSourceNode.Grip); 
        sourcePose.TryGetRotation(out m_currentState.GripRotation, InteractionSourceNode.Grip);

        sourcePose.TryGetAngularVelocity(out m_currentState.AngularVelocity);
        sourcePose.TryGetVelocity(out m_currentState.LinearVelocity);

        if(m_controller != null)
        {
            m_controller.transform.position = m_currentState.PointerPosition;
            m_controller.transform.rotation = m_currentState.PointerRotation;
        }
    } 

    /// <summary> 
    /// Update the state info from an interaction source state 
    /// </summary> 
    private void UpdateState(UnityEngine.XR.WSA.Input.InteractionSourceState sourceState)
    {
        UpdatePose(sourceState.sourcePose); 

        m_currentState.Grasped = sourceState.grasped; 
        m_currentState.MenuPressed = sourceState.menuPressed; 
        m_currentState.SelectPressed = sourceState.selectPressed; 
        m_currentState.SelectPressedAmount = sourceState.selectPressedAmount; 
        m_currentState.ThumbstickPressed = sourceState.thumbstickPressed; 
        m_currentState.ThumbstickPosition = sourceState.thumbstickPosition; 
        m_currentState.TouchpadPressed = sourceState.touchpadPressed; 
        m_currentState.TouchpadTouched = sourceState.touchpadTouched; 
        m_currentState.TouchpadPosition = sourceState.touchpadPosition; 
    } 
 
    public override bool buttonSelectPressed
    {
        get { return m_currentState.SelectPressed; }
    }

    public float selectPressedAmount
    {
        get { return m_currentState.SelectPressedAmount; }
    }

    public override bool buttonMenuPressed
    {
        get { return m_currentState.MenuPressed; }
    }

    public bool buttonGripPressed
    {
        get { return m_currentState.Grasped; }
    }

    public Vector2 padTouchPosition
    {
        get { return m_currentState.TouchpadPosition; }
    }

    public bool padTouched
    {
        get { return m_currentState.TouchpadTouched; }
    }

    public bool padPressed
    {
        get { return m_currentState.TouchpadPressed; }
    }

    public Vector2 thumbstickPosition
    {
        get { return m_currentState.ThumbstickPosition; }
    }

    public bool thumbstickPressed
    {
        get { return m_currentState.ThumbstickPressed; }
    }
}
