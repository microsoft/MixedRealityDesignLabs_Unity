//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;
using HUX.Focus;

public class ScrollableContent : MonoBehaviour, IHold
{
    public MaterialMapperBox boxMaterialMap;
    public ScrollMomentumPhysics scrollPhysics = new ScrollMomentumPhysics();

    public Vector3 ScrollInputScale = new Vector3(-1.0f, -1.0f, -1.0f);
    public float ScrollDampenScale = 5.0f;

    bool bDragging;

    float zoomAmount = 0;

    private AFocuser focuser = null;

    void Start()
    {
        scrollPhysics.UpdateSim();
        scrollPhysics.scrollPadSize = new Vector3(0.5f, 0.5f, 0.0f);

        if (boxMaterialMap == null)
        {
            boxMaterialMap = GetComponent<MaterialMapperBox>();
        }
    }

    Plane GetPlane()
    {
        return new Plane(boxMaterialMap.colliderBox.transform.forward, boxMaterialMap.colliderBox.transform.position);
    }

    public void OnHoldStarted(InteractionManager.InteractionEventArgs eventArgs)
    {
        focuser = eventArgs.Focuser;
		focuser.StartManipulation(boxMaterialMap.colliderBox.transform);
    }

    public void OnHoldCompleted(InteractionManager.InteractionEventArgs eventArgs)
    {
        focuser = null;
    }

    public void OnHoldCanceled(InteractionManager.InteractionEventArgs eventArgs)
    {
        focuser = null;
    }

    protected void OnZoom()
    {
        bool pressed = InputShell.Instance.ZoomVector.buttonState.pressed;
        float zoom = InputShell.Instance.ZoomVector.delta.y;

        if (pressed)
        {
            float zd = zoom * ScrollInputScale.z;
            Vector3 delta = new Vector3(0.0f, 0.0f, zd);
            if (delta != Vector3.zero)
            {
                scrollPhysics.ApplyScrollDelta(delta);
            }
            zoomAmount += zd;
        }
        else
        {
            zoomAmount = 0.0f;
        }
    }


    protected void OnScroll()
    {
        bool pressed = InputShell.Instance.ScrollVector.buttonState.pressed;
        Vector2 scroll = InputShell.Instance.ScrollVector.delta;

        if (pressed)
        {
            Vector2 dragAmount = Vector2.Scale(scroll, ScrollInputScale);
            scrollPhysics.ApplyScrollDelta((Vector3)(Vector2.Scale(dragAmount, new Vector2(1.0f, boxMaterialMap.GetContentAspect()))));
        }

        scrollPhysics.bDirectManipulating = pressed;
    }


    void Update()
    {
        if (focuser == null)
        {
            return;
        }

        // Set the scroll area size (i.e. the view-able window) so that bounds limiting can work.
        scrollPhysics.scrollScale = (Vector3)boxMaterialMap.GetContentScale() + new Vector3(0.0f, 0.0f, 1.0f);

        // Clamp the Z axis of the sim alone.
        scrollPhysics.UpdateSimZ();

        // Update the scroll area size again....
        scrollPhysics.scrollScale = (Vector3)boxMaterialMap.GetContentScale() + new Vector3(0.0f, 0.0f, 1.0f);

        // Apply zoom position (which can change the content position).
        Vector2 cursorPosInBounds = boxMaterialMap.WorldToBounds(focuser.Cursor.transform.position);
        scrollPhysics.ApplyScrollDelta(boxMaterialMap.SetContentZoom(scrollPhysics.scrollPos.z, cursorPosInBounds), true);

        // Update the scrolling sim, which will do velocity and clamping.
        scrollPhysics.UpdateSim();

        // Apply the scroll position to the material.
        boxMaterialMap.SetContentOffset(scrollPhysics.scrollPos);
    }
}
