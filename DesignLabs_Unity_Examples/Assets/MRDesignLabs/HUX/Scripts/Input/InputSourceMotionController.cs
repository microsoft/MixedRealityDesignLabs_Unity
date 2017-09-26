// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System.Linq;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public class InputSourceMotionController : InputSourceSixDOFBase
{
    public InputMotionControllerReceiver MotionControllerReceiver;

#if UNITY_WSA
    private Transform GetTransform(InteractionSourceHandedness hand)
    {
        ControllerInfo controllerInfo = null;
        if(MotionControllerReceiver != null)
        {
            if (hand == InteractionSourceHandedness.Left)
            {
                controllerInfo = MotionControllerReceiver.controllerDictionary.Where(x => x.Value.name.Equals("LeftController")).Select(x=>x.Value).FirstOrDefault();
            }
            else if (hand == InteractionSourceHandedness.Right)
            {
                controllerInfo = MotionControllerReceiver.controllerDictionary.Where(x => x.Value.name.Equals("RightController")).Select(x => x.Value).FirstOrDefault();
            }
        }

        Transform trans = null;
        if (controllerInfo != null)
            trans = controllerInfo.GetComponent<Transform>();
        return trans;
    }
#endif
    public override Vector3 position
    {
        get
        {
            float locationShift = 0f;
#if UNITY_WSA
            locationShift = (this.handedness == InteractionSourceHandedness.Left) ? -0.01f : 0.01f;
            Transform transform = GetTransform(this.handedness);
#endif
            return transform != null ? transform.position + new Vector3(locationShift, -0.02f, 0.0075f) : Vector3.one;
        }
    }

    public override Quaternion rotation
    {
        get
        {
#if UNITY_WSA
            Transform transform = GetTransform(this.handedness);
#endif
            return transform != null ? transform.rotation * Quaternion.Euler(45f, 0, 0) : Quaternion.identity;
        }
    }


    /// <summary>
    /// Returns a rotation between -180 and 180.
    /// </summary>
    public float Yaw
    {
        get
        {
            Quaternion rot = rotation;
            return Mathf.Asin((2.0f * rot.x * rot.y) + (2.0f * rot.z * rot.w)) * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Returns a rotation between -180 and 180.
    /// </summary>
    public float Roll
    {
        get
        {
            Quaternion rot = rotation;
            return Mathf.Atan2((2.0f * rot.y * rot.w) - (2.0f * rot.x * rot.z), 1 - (2.0f * rot.y * rot.y) - (2.0f * rot.z * rot.z)) * Mathf.Rad2Deg;
        }
    }

    /// <summary>
    /// Returns a rotation between -180 and 180.
    /// </summary>
    public float Pitch
    {
        get
        {
            Quaternion rot = rotation;
            return Mathf.Atan2((2.0f * rot.x * rot.w) - (2.0f * rot.y * rot.z), 1 - (2.0f * rot.x * rot.x) - (2.0f * rot.z * rot.z)) * Mathf.Rad2Deg;
        }
    }

    public void UpdateInput(InteractionSourceUpdatedEventArgs obj)
    {
        // Position
        Vector3 newPosition;
        if (obj.state.sourcePose.TryGetPosition(out newPosition, InteractionSourceNode.Grip))
        {
            m_Position = newPosition;
        }

        // Rotation
        Quaternion newRotation;
        if (obj.state.sourcePose.TryGetRotation(out newRotation, InteractionSourceNode.Grip))
        {
            m_Rotation = newRotation;
        }

        // Select/Trigger
        m_ButtonSelectPressed = obj.state.selectPressed;
        m_SelectPressedAmount = obj.state.selectPressedAmount;

        // Menu
        m_ButtonMenuPressed = obj.state.menuPressed;

        // Grip
        m_ButtonGripPressed = obj.state.grasped;

        // Touchpad
        m_PadTouchPosition = obj.state.touchpadPosition;
        m_PadTouched = obj.state.touchpadTouched;
        m_PadPressed = obj.state.touchpadPressed;

        // Thumbstick
        m_ThumbstickPosition = obj.state.thumbstickPosition;
        m_ThumbstickPressed = obj.state.thumbstickPressed;

        // Handedness
        handedness = obj.state.source.handedness;
        //m_Handedness = obj.state.source.handedness;
    }
    
    public override bool buttonSelectPressed
    {
        get { return m_ButtonSelectPressed; }
    }

    public float selectPressedAmount
    {
        get { return m_SelectPressedAmount; }
    }

    public override bool buttonMenuPressed
    {
        get { return m_ButtonMenuPressed; }
    }

    public bool buttonGripPressed
    {
        get { return m_ButtonGripPressed; }
    }

    public Vector2 padTouchPosition
    {
        get { return m_PadTouchPosition; }
    }

    public bool padTouched
    {
        get { return m_PadTouched; }
    }

    public bool padPressed
    {
        get { return m_PadPressed; }
    }

    public Vector2 thumbstickPosition
    {
        get { return m_ThumbstickPosition; }
    }

    public bool thumbstickPressed
    {
        get { return m_ThumbstickPressed; }
    }

    public InteractionSourceHandedness handedness = InteractionSourceHandedness.Unknown;

    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private bool m_ButtonSelectPressed = false;
    private float m_SelectPressedAmount = 0.0F;
    private bool m_ButtonMenuPressed = false;
    private bool m_ButtonGripPressed = false;
    private Vector2 m_PadTouchPosition = Vector2.zero;
    private bool m_PadTouched = false;
    private bool m_PadPressed = false;
    private Vector2 m_ThumbstickPosition = Vector2.zero;
    private bool m_ThumbstickPressed = false;
}
