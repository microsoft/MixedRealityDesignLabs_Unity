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
    //public class HandZoomWidget : CursorWidget
    //{
    //    public enum InteractionStateEnum
    //    {
    //        None = -1,
    //        Idle,
    //        Zooming
    //    }

    //    [Serializable]
    //    public class StateDatum
    //    {
    //        public StateDatum(InteractionStateEnum state) { this.InteractionState = state; this.Name = state.ToString(); }

    //        public string Name;
    //        public bool BaseCursorVisible = true;
    //        public InteractionStateEnum InteractionState;
    //        public List<GameObject> VisibleObjects = new List<GameObject>();
    //    }

    //    public StateDatum[] InteractionStates = new StateDatum[] { new StateDatum((InteractionStateEnum)0), new StateDatum((InteractionStateEnum)1) };
    //    public float OffsetScalingFactor = 1f;
    //    public GameObject CenterAffordance;
    //    public bool RequireHold = false;

    //    public SpriteRenderer Magnifier;
    //    public Sprite ZoomInSprite;
    //    public Sprite ZoomOutSprite;
    //    public Sprite ZoomNoneSprite;
        
    //    public InteractionStateEnum CurrentInteractionState = InteractionStateEnum.None;

    //    private InteractionStateEnum _activeState = InteractionStateEnum.None;
    //    private ScrollZoomInteractible _scrollInteractible;

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        if (_scrollInteractible != null)
    //        {
    //            CurrentInteractionState = InteractionStateEnum.Idle;
    //            Sprite newSprite = ZoomNoneSprite;

    //            if (_scrollInteractible.IsZooming)
    //            {
    //                CurrentInteractionState = InteractionStateEnum.Zooming;
    //                if (ZoomInSprite != null && ZoomOutSprite != null && ZoomNoneSprite != null)
    //                {
    //                    newSprite = _scrollInteractible.ZoomVal == 0 ? ZoomNoneSprite : _scrollInteractible.ZoomVal < 0 ? ZoomInSprite : ZoomOutSprite;
    //                }
    //            }

    //            if (CenterAffordance != null)
    //            {
    //                if (CenterAffordance.transform.localPosition != Vector3.zero)
    //                {
    //                    CenterAffordance.transform.localPosition = Vector3.zero;
    //                }
    //            }

    //            if (Magnifier != null)
    //            {
    //                if (Magnifier.sprite != newSprite)
    //                    Magnifier.sprite = newSprite;
    //            }

    //        }

    //        if (_activeState != CurrentInteractionState)
    //        {
    //            UpdateActiveState(CurrentInteractionState);
    //            _activeState = CurrentInteractionState;
    //        }
    //    }


    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            _scrollInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            if (_scrollInteractible != null
    //                && _scrollInteractible.Zoomable
    //                && _scrollInteractible.ZoomControlType == ScrollZoomInteractible.ZoomControlTypeEnum.SingleHand
    //                && (this.RequireHold ? Veil.Instance.IsFingerPressed() : true))
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if (_scrollInteractible != null)
    //                _scrollInteractible = null;
    //        }

    //        return false;
    //    }

    //    public void UpdateActiveState(InteractionStateEnum newState)
    //    {
    //        for (int i = 0; i < transform.childCount; i++)
    //        {
    //            GameObject go = transform.GetChild(i).gameObject;
    //            go.SetActive(InteractionStates[(int)newState].VisibleObjects.Contains(go));
    //        }
    //    }
    //}
}