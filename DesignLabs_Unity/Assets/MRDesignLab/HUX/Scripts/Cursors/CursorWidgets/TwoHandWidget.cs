//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HUX.Interaction;

namespace HUX.Cursors
{
    //public class TwoHandWidget : RadialOffsetWidget
    //{

    //    public SpriteRenderer RightHandSprite;
    //    public Sprite RightHandOpenSprite;
    //    public Sprite RightHandClosedSprite;

    //    public SpriteRenderer LeftHandSprite;
    //    public Sprite LeftHandOpenSprite;
    //    public Sprite LeftHandClosedSprite;

    //    public SpriteRenderer RingSprite;
    //    private ScrollZoomInteractible _scrollInteractible;

    //    public SpriteRenderer Magnifier;
    //    public Sprite ZoomInSprite;
    //    public Sprite ZoomOutSprite;
    //    public Sprite ZoomNoneSprite;

    //    private float lastZoomVal = 0.0f;

    //    public void LateUpdate()
    //    {
    //        switch (InputHandReceiver.HandState)
    //        {
    //            case InputController.HandStatesEnum.TwoVisible:
    //                if (LeftHandSprite.sprite != LeftHandOpenSprite)
    //                    LeftHandSprite.sprite = LeftHandOpenSprite;
    //                if (RightHandSprite.sprite != RightHandOpenSprite)
    //                    RightHandSprite.sprite = RightHandOpenSprite;
    //                break;
    //            case InputController.HandStatesEnum.TwoVisibleOneHeld:
    //                if (LeftHandSprite.sprite != LeftHandClosedSprite)
    //                    LeftHandSprite.sprite = LeftHandClosedSprite;
    //                if (RightHandSprite.sprite != RightHandOpenSprite)
    //                    RightHandSprite.sprite = RightHandOpenSprite;
    //                break;
    //            case InputController.HandStatesEnum.TwoHeld:
    //                if (LeftHandSprite.sprite != LeftHandClosedSprite)
    //                    LeftHandSprite.sprite = LeftHandClosedSprite;
    //                if (RightHandSprite.sprite != RightHandClosedSprite)
    //                    RightHandSprite.sprite = RightHandClosedSprite;
    //                break;
    //        }

    //        if(this._scrollInteractible != null)
    //        {
    //            Sprite newSprite = this.ZoomNoneSprite;

    //            if (this._scrollInteractible.IsZooming)
    //            {

    //                if (this.ZoomInSprite != null && this.ZoomOutSprite != null && this.ZoomNoneSprite != null)
    //                {
    //                    newSprite = this._scrollInteractible.ZoomVal == this.lastZoomVal ? this.Magnifier.sprite : this._scrollInteractible.ZoomVal < this.lastZoomVal ? this.ZoomInSprite : this.ZoomOutSprite;
    //                }

    //                this.lastZoomVal = this._scrollInteractible.ZoomVal;
    //            }
    //            else
    //            {
    //                this.lastZoomVal = 0.0f;
    //            }

    //            if (this.Magnifier != null)
    //            {
    //                if (this.Magnifier.sprite != newSprite)
    //                    this.Magnifier.sprite = newSprite;
    //            }
    //        }
    //    }

    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            _scrollInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            bool _twoHand = (InputHandReceiver.HandState == InputController.HandStatesEnum.TwoVisible) ||
    //                (InputHandReceiver.HandState == InputController.HandStatesEnum.TwoVisibleOneHeld) ||
    //                (InputHandReceiver.HandState == InputController.HandStatesEnum.TwoHeld);

    //            if (_scrollInteractible != null && _scrollInteractible.Zoomable && _twoHand)
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if (_scrollInteractible != null)
    //            {
    //                _scrollInteractible = null;
    //                this.lastZoomVal = 0.0f;
    //            }
    //        }

    //        return false;
    //    }
    //}
}
