//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Interaction;

namespace HUX.Cursors
{
    //public class InteractionMenuWidget : CursorWidget
    //{
    //    [Header("Components")]
    //    public GameObject RadialObject;
    //    public GameObject ProgressObject;

    //    [Header("Radial Sprites")]
    //    public Sprite RadialScrollSprite;
    //    public Sprite RadialZoomSprite;
    //    public Sprite RadialMenuSprite;
    //    public GameObject ContextMenuPlaceholder;

    //    private ControlState currentControlState = ControlState.None;
    //    private ControlState nextControlState = ControlState.None;

    //    private SpriteRenderer radialSpriteRenderer;
    //    private Animator progressAnimator;

    //    private GameObject _lastTarget;

    //    private bool open = false;

    //    public enum ControlState
    //    {
    //        None = 0,
    //        Scroll = 1,
    //        Zoom = 2,
    //        Menu = 3
    //    }

    //    // Use this for initialization
    //    public override void Start()
    //    {
    //        this.progressAnimator = this.ProgressObject.GetComponent<Animator>();
    //        this.radialSpriteRenderer = this.RadialObject.GetComponent<SpriteRenderer>();

    //        SetControlState(ControlState.None);

    //        base.Start();
    //    }

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        if (this._curTarget != null && this._curTarget != this._lastTarget)
    //        {
    //            this._lastTarget = this._curTarget;
    //        }

    //        if (this.nextControlState != this.currentControlState)
    //        {
    //            this.UpdateRadialState(this.nextControlState);
    //        }

    //        bool menuShouldBeOpen = this.ShouldBeActive();

    //        if (!this.open && menuShouldBeOpen)
    //        {
    //            this.OpenMenu();
    //        }
    //        else if (this.open && !menuShouldBeOpen)
    //        {
    //            this.CloseMenu();
    //        }
    //    }

    //    public void SetControlState(ControlState nextState)
    //    {
    //        Debug.Log("Scroll state changed: " + nextState.ToString());
    //        this.nextControlState = nextState;

    //        ScrollZoomInteractible szInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //        if (szInteractible.DisableOnActionEnd)
    //            szInteractible.DisableOnActionEnd = false;

    //        if (szInteractible != null)
    //        {
    //            if (this.nextControlState == ControlState.Scroll && !szInteractible.Scrollable)
    //            {
    //                szInteractible.Scrollable = true;

    //                if (szInteractible.Zoomable)
    //                {
    //                    szInteractible.Zoomable = false;
    //                }

    //                this.ShowContextMenuPlaceholder(false);
    //                this.ShowContextMenu(false);
    //            }
    //            else if (this.nextControlState == ControlState.Zoom && !szInteractible.Zoomable)
    //            {
    //                szInteractible.Zoomable = true;

    //                if (szInteractible.Scrollable)
    //                {
    //                    szInteractible.Scrollable = false;
    //                }

    //                this.ShowContextMenuPlaceholder(false);
    //                this.ShowContextMenu(false);
    //            }
    //            else if (this.nextControlState == ControlState.Menu)
    //            {
    //                if (szInteractible.Scrollable)
    //                {
    //                    szInteractible.Scrollable = false;
    //                }
    //                if (szInteractible.Zoomable)
    //                {
    //                    szInteractible.Zoomable = false;
    //                }

    //                this.ShowContextMenuPlaceholder(true);
    //                this.ShowContextMenu(false);
    //            }
    //            else if (this.nextControlState == ControlState.None)
    //            {
    //                if (szInteractible.Scrollable)
    //                {
    //                    szInteractible.Scrollable = false;
    //                }
    //                if (szInteractible.Zoomable)
    //                {
    //                    szInteractible.Zoomable = false;
    //                }

    //                this.ShowContextMenuPlaceholder(false);
    //                this.ShowContextMenu(false);
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("ScrollZoomInteractible couldn't be found");
    //        }
    //    }

    //    public void OpenMenu()
    //    {
    //        Debug.Log("Interaction menu opened");
    //        this.open = true;
    //        this.ProgressObject.SetActive(true);
    //        this.progressAnimator.SetBool("ProgressOn", true);
    //    }

    //    public void CloseMenu()
    //    {
    //        Debug.Log("Interaction menu closed");

    //        this.open = false;
    //        this.progressAnimator.SetBool("ProgressOn", false);

    //        GameObject target = this._curTarget ?? this._lastTarget;

    //        if (target == null)
    //        {
    //            return;
    //        }

    //        ScrollZoomInteractible szInteractible = target.GetComponent<ScrollZoomInteractible>();

    //        if (szInteractible != null)
    //        {
    //            if (!szInteractible.DisableOnActionEnd)
    //                szInteractible.DisableOnActionEnd = true;

    //            this.ShowContextMenuPlaceholder(false);

    //            if (this.currentControlState == ControlState.Menu)
    //            {
    //                this.ShowContextMenu(true);
    //            }
    //        }
    //    }

    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            InteractionMenu i = _curTarget.GetComponent<InteractionMenu>();
    //            ScrollZoomInteractible szInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            return i != null &&
    //                Veil.Instance.IsFingerPressed() &&
    //                !szInteractible.IsScrolling &&
    //                !szInteractible.IsZooming;
    //        }

    //        return base.ShouldBeActive();
    //    }

    //    public void MenuComplete()
    //    {
    //        this.ProgressObject.SetActive(false);
    //        this.ShowContextMenu(true);
    //    }

    //    private void UpdateRadialState(ControlState state)
    //    {
    //        this.currentControlState = state;

    //        this.radialSpriteRenderer.sprite = this.MapStateToSprite(state);
    //    }

    //    private Sprite MapStateToSprite(ControlState state)
    //    {
    //        Sprite sprite = null;

    //        switch (state)
    //        {
    //            case ControlState.Scroll:
    //                sprite = this.RadialScrollSprite;
    //                break;
    //            case ControlState.Zoom:
    //                sprite = this.RadialZoomSprite;
    //                break;
    //            case ControlState.Menu:
    //                sprite = this.RadialMenuSprite;
    //                break;
    //        }

    //        return sprite;
    //    }

    //    private void ShowContextMenu(bool show)
    //    {
    //        GameObject target = this._curTarget ?? this._lastTarget;
    //        if (target != null)
    //        {
    //            ContextCursorMenu context = target.GetComponent<ContextCursorMenu>();
    //            if (context != null)
    //            {
    //                context.Visible = show;
    //            }
    //            else
    //            {
    //                Debug.LogError("Couldn't find context menu to change context menu visibility.");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Couldn't find target to change context menu visibility.");
    //        }
    //    }

    //    private void ShowContextMenuPlaceholder(bool show)
    //    {
    //        if (this.ContextMenuPlaceholder != null)
    //        {
    //            this.ContextMenuPlaceholder.SetActive(show);
    //        }
    //    }
    //}
}