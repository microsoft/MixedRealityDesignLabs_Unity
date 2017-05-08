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
    //public class HandVerticalZoomWidget : CursorWidget
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
    //        public bool OverrideAnchorType = false;
    //        public AnchorTypeEnum AnchorType = AnchorTypeEnum.Cursor;
    //        public InteractionStateEnum InteractionState;
    //        public List<GameObject> VisibleObjects = new List<GameObject>();
    //    }

    //    public StateDatum[] InteractionStates = new StateDatum[] { new StateDatum((InteractionStateEnum)0), new StateDatum((InteractionStateEnum)1) };

    //    public float OffsetScalingFactor = 1f;
    //    public GameObject CenterAffordance;
    //    public bool RequireHold = false;
    //    public ScrollZoomInteractible.ZoomControlTypeEnum ZoomType = ScrollZoomInteractible.ZoomControlTypeEnum.HandVertical;

    //    public InteractionStateEnum CurrentInteractionState = InteractionStateEnum.None;

    //    private InteractionStateEnum _activeState = InteractionStateEnum.None;
    //    private ScrollZoomInteractible _scrollInteractible;
    //    private AnchorTypeEnum _initialAnchorType;

    //    void Update()
    //    {
    //        if (_scrollInteractible != null)
    //        {
    //            CurrentInteractionState = InteractionStateEnum.Idle;

    //            if (_scrollInteractible.IsZooming)
    //            {
    //                CurrentInteractionState = InteractionStateEnum.Zooming;
    //            }

    //            if (CenterAffordance != null)
    //            {
    //                if (CurrentInteractionState == InteractionStateEnum.Zooming)
    //                {
    //                    Vector3 zoomOffset = new Vector3(0, -_scrollInteractible.ZoomVal);

    //                    CenterAffordance.transform.localPosition = zoomOffset * OffsetScalingFactor;
    //                }
    //                else if (CenterAffordance.transform.localPosition != Vector3.zero)
    //                {
    //                    CenterAffordance.transform.localPosition = Vector3.zero;
    //                }
    //            }
    //        }

    //        if (_activeState != CurrentInteractionState)
    //        {
    //            UpdateActiveState(CurrentInteractionState);
    //            _activeState = CurrentInteractionState;
    //        }
    //    }

    //    public override void Start()
    //    {
    //        this._initialAnchorType = this.AnchorType;
    //        base.Start();
    //    }

    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            _scrollInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            if (_scrollInteractible != null
    //                && _scrollInteractible.Zoomable
    //                && _scrollInteractible.ZoomControlType == this.ZoomType
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
    //        StateDatum interactionState = InteractionStates[(int)newState];
    //        for (int i = 0; i < transform.childCount; i++)
    //        {
    //            GameObject go = transform.GetChild(i).gameObject;
    //            go.SetActive(interactionState.VisibleObjects.Contains(go));
    //        }

    //        AnchorTypeEnum anchorType = this._initialAnchorType;
    //        if (interactionState.OverrideAnchorType)
    //        {
    //            anchorType = interactionState.AnchorType;
    //        }
    //        if(this.AnchorType != anchorType)
    //        {
    //            this.SwitchAnchorType(anchorType);
    //        }
    //    }
    //}
}