//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX;
using HUX.Receivers;
using System;
using UnityEngine;
using UnityEngine.VR;

/// <summary>
/// Class for manually controlling the camera when HuP isn't running. Attach to same GameObject that contains the Camera component.
/// </summary>
public class ManualCameraControl : MonoBehaviour
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle,
        Control,
        Shift,
        Focused,
        None
    }

    public float ExtraMouseSensitivityScale = 3.0f;
    public float DefaultMouseSensitivity = 0.1f;
    public MouseButton MouseLookButton = MouseButton.Control;
    public bool IsControllerLookInverted = true;
    public bool lockMovement = false;

    public bool ShouldUseInputGamepad = true;
    public float m_GamepadStickSensitivity = 3.0f;
    public float m_GamepadTriggerSensitivity = 1.5f;

    private bool isMouseJumping = false;

    private bool isGamepadLookEnabled = true;
    private bool isFlyKeypressEnabled = true;
    private Vector3 lastMousePosition = Vector3.zero;
    //private Vector3 lastTrackerToUnityTranslation = Vector3.zero;
    //private Quaternion lastTrackerToUnityRotation = Quaternion.identity;
    private bool appHasFocus = true;
    private bool wasLooking = false;

    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;

    private Veil m_Veil;

    public Vector3 Position
    {
        get
        {
            return m_Veil.transform.position;
        }
    }

    private void Awake()
    {
        // Workaround for Remote Desktop.  Without this, Game window mousing breaks in modes
        // that use Unity's Cursor.lockState feature.
        if (IsRunningUnderRemoteDesktop())
        {
            if (this.MouseLookButton >= MouseButton.Control && this.MouseLookButton <= MouseButton.Focused)
            {
                this.MouseLookButton = MouseButton.None;
                Debug.Log("Running under Remote Desktop, so changed MouseLook method to None");
            }
        }
    }

    private void Start()
    {
        m_Veil = Veil.Instance;

        m_InitialPosition = m_Veil.transform.position;
        m_InitialRotation = m_Veil.transform.rotation;

        // VR Mode
        if (UnityEngine.XR.XRSettings.loadedDeviceName == "Oculus" || UnityEngine.XR.XRSettings.loadedDeviceName == "PlayStationVR")
        {
            UnityEngine.XR.InputTracking.Recenter();
        }
    }

    private void Update()
    {
        // Undo the last tracker to Unity transforms applied
        //m_Veil.transform.Translate(-this.lastTrackerToUnityTranslation, Space.World);
        //m_Veil.transform.Rotate(-this.lastTrackerToUnityRotation.eulerAngles, Space.World);

        // Calculate and apply the camera control movement this frame
        Vector3 rotate = GetCameraControlRotation();
        Vector3 translate = GetCameraControlTranslation();

        m_Veil.transform.Rotate(rotate.x, 0.0f, 0.0f);
        m_Veil.transform.Rotate(0.0f, rotate.y, 0.0f, Space.World);
        //m_Veil.transform.Rotate(this.lastTrackerToUnityRotation.eulerAngles, Space.World);

        if (!lockMovement)
        {
            m_Veil.transform.Translate(translate, Space.World);
            //m_Veil.transform.Translate(this.lastTrackerToUnityTranslation, Space.World);
        }

        // Check if the gamepad is reseting position.
        if (this.ShouldUseInputGamepad && InputSources.Instance != null && InputSources.Instance.gamepad.startButtonPressed)
        {
			m_Veil.transform.position = m_InitialPosition;
			m_Veil.transform.rotation = m_InitialRotation;
		}
    }

    internal void SetPosition(Vector3 savedPlayerPosition)
    {
        m_Veil.transform.position = savedPlayerPosition;
    }

    private static float InputCurve(float x)
    {
        // smoothing input curve, converts from [-1,1] to [-2,2]
        return (Mathf.Sign(x) * (1.0f - Mathf.Cos(.5f * Mathf.PI * Mathf.Clamp(x, -1.0f, 1.0f))));
    }

    private float GetKeyDir(string neg, string pos)
    {
        return Input.GetKey(neg) ? -1.0f : Input.GetKey(pos) ? 1.0f : 0.0f;
    }

    private Vector3 GetCameraControlTranslation()
    {
        InputDev inputDev = InputDev.Instance;

        Vector3 deltaPosition = Vector3.zero;
        deltaPosition += GetKeyDir("left", "right") * Camera.main.transform.right;
        deltaPosition += GetKeyDir("down", "up") * Camera.main.transform.forward;

        // Support fly up/down keypresses if the current project maps it. This isn't a standard
        // Unity InputManager mapping, so it has to gracefully fail if unavailable.
        if (this.isFlyKeypressEnabled)
        {
            try
            {
                deltaPosition += InputCurve(Input.GetAxis("Fly")) * m_Veil.transform.up;
            }
            catch (System.Exception)
            {
                this.isFlyKeypressEnabled = false;
            }
        }

        // All input should come through inputDev, someday
        deltaPosition += InputCurve(inputDev.VirtualCamTranslate.delta.x) * this.transform.right;
        deltaPosition += InputCurve(inputDev.VirtualCamTranslate.delta.y) * this.transform.forward;
        deltaPosition += InputCurve(inputDev.VirtualCamVertAndRoll.delta.x) * this.transform.up;

        float accel = Input.GetKey(KeyCode.LeftShift) ? 1.0f : 0.1f;
        return accel * deltaPosition;
    }

    private Vector3 GetCameraControlRotation()
    {
        Vector3 rot = Vector3.zero;

        float inversionFactor = this.IsControllerLookInverted ? -1.0f : 1.0f;

        if (this.isGamepadLookEnabled)
        {
            try
            {
                // Get the axes information from the right stick of X360 controller
                rot.x += InputCurve(InputDev.Instance.VirtualCamRotate.delta.y) * inversionFactor;
                rot.y += InputCurve(InputDev.Instance.VirtualCamRotate.delta.x);
            }
            catch (Exception)
            {
                this.isGamepadLookEnabled = false;
            }
        }

        if (this.ShouldMouseLook)
        {
            if (!this.wasLooking)
            {
                OnStartMouseLook();
            }

            ManualCameraControl_MouseLookTick(ref rot);

            this.wasLooking = true;
        }
        else
        {
            if (this.wasLooking)
            {
                OnEndMouseLook();
            }

            this.wasLooking = false;
        }

        rot *= this.ExtraMouseSensitivityScale;

        return rot;
    }

    private void OnStartMouseLook()
    {
        if (this.MouseLookButton <= MouseButton.Middle)
        {
            // if mousebutton is either left, right or middle
            SetWantsMouseJumping(true);
        }
        else if (this.MouseLookButton <= MouseButton.Focused)
        {
            // if mousebutton is either control, shift or focused
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        // do nothing if (this.MouseLookButton == MouseButton.None)
    }

    private void OnEndMouseLook()
    {
        if (this.MouseLookButton <= MouseButton.Middle)
        {
            // if mousebutton is either left, right or middle
            SetWantsMouseJumping(false);
        }
        else if (this.MouseLookButton <= MouseButton.Focused)
        {
            // if mousebutton is either control, shift or focused
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }

        // do nothing if (this.MouseLookButton == MouseButton.None)
    }

    private void ManualCameraControl_MouseLookTick(ref Vector3 rot)
    {
        // Use frame-to-frame mouse delta in pixels to determine mouse rotation. The traditional
        // GetAxis("Mouse X") method doesn't work under Remote Desktop.
        Vector3 mousePositionDelta = Input.mousePosition - this.lastMousePosition;
        this.lastMousePosition = Input.mousePosition;

        if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
        {
            mousePositionDelta.x = Input.GetAxis("Mouse X");
            mousePositionDelta.y = Input.GetAxis("Mouse Y");
        }
        else
        {
            mousePositionDelta.x *= this.DefaultMouseSensitivity;
            mousePositionDelta.y *= this.DefaultMouseSensitivity;
        }

        rot.x += -InputCurve(mousePositionDelta.y);
        rot.y += InputCurve(mousePositionDelta.x);
    }


    private bool ShouldMouseLook
    {
        get
        {
#if !UNITY_METRO || UNITY_EDITOR
            // Only allow the mouse to control rotation when Unity has focus. This enables
            // the player to temporarily alt-tab away without having the player look around randomly
            // back in the Unity Game window.
            if (!this.appHasFocus)
            {
                return false;
            }
            else if (this.MouseLookButton == MouseButton.None)
            {
                return true;
            }
            else if (this.MouseLookButton <= MouseButton.Middle)
            {
                return Input.GetMouseButton((int)this.MouseLookButton);
            }
            else if (this.MouseLookButton == MouseButton.Control)
            {
                return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            }
            else if (this.MouseLookButton == MouseButton.Shift)
            {
                return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            }
            else if (this.MouseLookButton == MouseButton.Focused)
            {
                if (!this.wasLooking)
                {
                    // any kind of click will capture focus
                    return Input.GetMouseButtonDown((int)MouseButton.Left) || Input.GetMouseButtonDown((int)MouseButton.Right) || Input.GetMouseButtonDown((int)MouseButton.Middle);
                }
                else
                {
                    // pressing escape will stop capture
                    return !Input.GetKeyDown(KeyCode.Escape);
                }
            }
#endif

            return false;
        }
    }

    private void OnApplicationFocus(bool focusStatus)
    {
        this.appHasFocus = focusStatus;
    }


    private void SetWantsMouseJumping(bool wantsJumping)
    {
        if (wantsJumping != this.isMouseJumping)
        {
            this.isMouseJumping = wantsJumping;

            if (wantsJumping)
            {
                // unlock the cursor if it was locked
                UnityEngine.Cursor.lockState = CursorLockMode.None;

                // hide the cursor
                UnityEngine.Cursor.visible = false;

                this.lastMousePosition = Input.mousePosition;
            }
            else
            {
                // recenter the cursor (setting lockCursor has side-effects under the hood)
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.lockState = CursorLockMode.None;

                // show the cursor
                UnityEngine.Cursor.visible = true;
            }

#if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.SetWantsMouseJumping(wantsJumping ? 1 : 0);
#endif
        }
    }

#if UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern uint GetCurrentProcessId();

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool ProcessIdToSessionId(uint dwProcessId, out uint pSessionId);

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern uint WTSGetActiveConsoleSessionId();

    private bool IsRunningUnderRemoteDesktop()
    {
        uint processId = GetCurrentProcessId();
        uint sessionId;
        return ProcessIdToSessionId(processId, out sessionId) && (sessionId != WTSGetActiveConsoleSessionId());
    }
#else
    private bool IsRunningUnderRemoteDesktop()
    {
        return false;
    }
#endif

}
