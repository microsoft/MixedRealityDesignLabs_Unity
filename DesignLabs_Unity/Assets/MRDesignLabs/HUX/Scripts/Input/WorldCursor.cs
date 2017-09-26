//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX;
using HUX.Interaction;
using HUX.Focus;

/// <summary>
/// <see cref="MonoBehaviour"/>
/// 
/// </summary>
public class WorldCursor : MonoBehaviour
{
    public float SurfaceRayOffset = 0.1f;
    public float CursorSize = 0.02f;

    public bool Clamp = false;
    public float ClampScale = 1.3f;
    public float ScreenClampDotThresh = 0.71f;

    public Vector2 DistanceClamp = new Vector2(0.1f, 7.5f);

    public float DepthOffset = 0;

    public bool AccrueInputsForUpdate;

    bool cursorActive;
    public bool IsActive
    {
        get
        {
            return cursorActive;
        }
    }

    Vector2 accruedDelta;

    public Vector2 currentScreenPos;

    GameObject pointer;

    void Start()
    {
        pointer = transform.GetChild(0).gameObject;

        CenterCursor();
        UpdateCursorPlacement();
    }

    public void AddDepthDelta(float delta)
    {
        DepthOffset += delta;
        //transform.position = CastCursor(HUXGazer.Instance.GetRay());
    }

    public void SnapToGazeDir()
    {
        CastCursor(FocusManager.Instance.GazeFocuser.FocusRay);
    }

    public void ApplyMovementDelta(Vector2 mouseDelta, bool forceApply = false)
    {
        if (mouseDelta != Vector2.zero)
        {
            // If desired, just add up the delta inputs to apply once in update, in case there are many inputs each frame
            if (AccrueInputsForUpdate && !forceApply)
            {
                accruedDelta += mouseDelta;
                return;
            }

            //FocusManager focus = FocusManager.Instance;
            Transform focusTransform = Veil.Instance.HeadTransform;// focus.GazeTransform;

            // Get ray to current mouse pos
            Ray newRay = new Ray(focusTransform.transform.position, Vector3.forward);//focus.FocusRay;
            Vector3 newDirection = transform.position - newRay.origin;

            // Move mouse
            float vFov = 0.5f * Mathf.Sin(Mathf.Deg2Rad * Veil.Instance.DeviceFOV);
            float hFov = 0.5f * Mathf.Sin(Mathf.Deg2Rad * (Veil.Instance.DeviceFOV * Veil.c_horiz_ratio));
            float dist = newDirection.magnitude;

            newDirection += dist * (focusTransform.up * mouseDelta.y * vFov + focusTransform.right * mouseDelta.x * hFov);
            newDirection.Normalize();

            // Clamp direction to screen tho
            if (Clamp)
            {
                // Snap mouse to screen if looking far enough away from mouse
                float headMouseDot = Vector3.Dot(newDirection, focusTransform.forward);
                if (headMouseDot < Mathf.Max(0, ScreenClampDotThresh))
                {
                    newDirection = focusTransform.forward;
                }

                // Clamp mouse to view bounds (times a scale) so that we can't move the mouse forever away and have to bring it back
                float dot = Vector3.Dot(newDirection, focusTransform.up);
                float delta = Mathf.Abs(dot) - vFov * ClampScale;
                currentScreenPos.y = dot / vFov;
                if (delta > 0)
                {
                    newDirection -= Mathf.Sign(dot) * delta * focusTransform.up;
                }

                dot = Vector3.Dot(newDirection, focusTransform.right);
                delta = Mathf.Abs(dot) - hFov * ClampScale;
                currentScreenPos.x = dot / hFov;
                if (delta > 0)
                {
                    newDirection -= Mathf.Sign(dot) * delta * focusTransform.right;
                }

                newDirection.Normalize();
            }

            // Re-cast with new direction
            newRay.direction = newDirection;
            transform.position = RaycastPosition(newRay);

            UpdateCursorPlacement();
        }
    }

    public void CastCursor(Ray ray)
    {
        transform.position = RaycastPosition(ray);
        UpdateCursorPlacement();
    }

    void Update()
    {
        if (AccrueInputsForUpdate && accruedDelta != Vector2.zero)
        {
            ApplyMovementDelta(accruedDelta, true);
            accruedDelta = Vector2.zero;
        }
    }

    public void SetCursorActive(bool active)
    {
        if (active == cursorActive)
        {
            return;
        }
        //Debug.Log("SetMouseCursorActive: " + active);

        cursorActive = active;
        if (active)
        {
            pointer.SetActive(true);
            CenterCursor();
        }
        else
        {
            pointer.SetActive(false);
        }
    }

    public void SetCursorPosition(Vector3 worldPos)
    {
        transform.position = worldPos;

        UpdateCursorPlacement();
    }

    public void CenterCursor()
    {
        transform.position = FocusManager.Instance.GazeFocuser.TargetOrigin + FocusManager.Instance.GazeFocuser.TargetDirection * 2.5f;
        CastCursor(GetRayForScreenPos(Vector2.zero));
    }

    public Ray GetRayForScreenPos(Vector2 pos)
    {
        AFocuser focus = FocusManager.Instance.GazeFocuser;
        Ray newRay = focus.FocusRay;

        float vFov = 0.5f * Mathf.Sin(Mathf.Deg2Rad * Veil.Instance.DeviceFOV);
        float hFov = 0.5f * Mathf.Sin(Mathf.Deg2Rad * (Veil.Instance.DeviceFOV * Veil.c_horiz_ratio));

        newRay.direction += (focus.TargetOrientation * Vector3.up) * pos.y * vFov + (focus.TargetOrientation * Vector3.right) * pos.x * hFov;

        return newRay;
    }

    public Vector3 RaycastPosition(Ray newRay)
    {
        AFocuser focus = FocusManager.Instance.GazeFocuser;
        Vector3 newPos = Vector3.zero;
        RaycastHit newHit;

        if (Physics.Raycast(newRay, out newHit))
        {
            Vector3 backupDir = newHit.point - focus.TargetOrigin;
            newPos = newHit.point - backupDir * SurfaceRayOffset;
        }
        else
        {
            float currentDepth = Mathf.Clamp((transform.position - focus.TargetOrigin).magnitude, DistanceClamp.x, DistanceClamp.y);

            Debug.DrawLine(focus.TargetOrigin, newRay.origin);

            newPos = focus.TargetOrigin + newRay.direction * currentDepth;
        }

        //newPos += newRay.direction * DepthOffset;

        return newPos;
    }

    public void UpdateCursorPlacement()
    {
        Vector3 vec = transform.position - FocusManager.Instance.GazeFocuser.TargetOrigin;
        float dist = vec.magnitude;
        transform.localScale = Vector3.one * dist * CursorSize;
        if (vec.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(vec);
        }
    }
}
