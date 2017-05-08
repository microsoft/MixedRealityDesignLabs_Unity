//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HUX.Interaction;
using HUX.Utility;

namespace HUX.Cursors
{
    ///// <summary>
    ///// </summary>
    //public class InteractionStateWidget : CursorWidget
    //{
    //    public enum InteractionStateEnum
    //    {
    //        None = -1,
    //        Idle,
    //        Holding,
    //        Held,
    //        Scrolling,
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

    //    public StateDatum[] InteractionStates = new StateDatum[]{new StateDatum((InteractionStateEnum)0), new StateDatum((InteractionStateEnum)1), new StateDatum((InteractionStateEnum)2), new StateDatum((InteractionStateEnum)3), new StateDatum((InteractionStateEnum)4) };
    //    public float OffsetScalingFactor = 1f;
    //    public GameObject CenterAffordance;

    //    public SpriteRenderer Magnifier;
    //    public Sprite ZoomInSprite;
    //    public Sprite ZoomOutSprite;
    //    public Sprite ZoomNoneSprite;

    //    public ProgressAnim HoldProgressBar;
    //    public InteractionStateEnum CurrentInteractionState = InteractionStateEnum.None;

    //    private InteractionStateEnum _activeState = InteractionStateEnum.None;
    //    private ScrollZoomInteractible _scrollInteractible;
    //    private bool _hideProgress = false;

    //    // Update is called once per frame
    //    void Update()
    //    {
    //        if (_scrollInteractible != null)
    //        {
    //            CurrentInteractionState = InteractionStateEnum.Idle;
    //            Sprite newSprite = ZoomNoneSprite;

    //            if (_scrollInteractible.IsScrolling)
    //            {
    //                CurrentInteractionState = InteractionStateEnum.Scrolling;
    //                _hideProgress = true;
    //            }
    //            else if (_scrollInteractible.IsZooming)
    //            {
    //                CurrentInteractionState = InteractionStateEnum.Zooming;
    //                if (ZoomInSprite != null && ZoomOutSprite != null && ZoomNoneSprite != null)
    //                {
    //                    newSprite = _scrollInteractible.ZoomVal == 0 ? ZoomNoneSprite : _scrollInteractible.ZoomVal < 0 ? ZoomInSprite : ZoomOutSprite;
    //                }
    //                _hideProgress = true;
    //            }
    //            else if(_scrollInteractible.IsSelecting)
    //            {
    //                _hideProgress = false;

    //                if (HoldProgressBar != null)
    //                {
    //                    CurrentInteractionState = HoldProgressBar.State == 0 ? InteractionStateEnum.Holding : InteractionStateEnum.Held;
    //                }
    //            }

    //            if (HoldProgressBar != null)
    //            {
    //                if (_hideProgress)
    //                {
    //                    if (HoldProgressBar.State != 0)
    //                    {
    //                        HoldProgressBar.SetProgressState(0);
    //                    }

    //                    if (HoldProgressBar.gameObject.activeSelf)
    //                        HoldProgressBar.gameObject.SetActive(false);
    //                }
    //                else
    //                {
    //                    if (!HoldProgressBar.gameObject.activeSelf)
    //                        HoldProgressBar.gameObject.SetActive(true);
    //                }
    //            }

    //            if (CenterAffordance != null)
    //            {
    //                if(CurrentInteractionState == InteractionStateEnum.Scrolling)
    //                {
    //                    Vector3 scrollOffset = _scrollInteractible.GetCursorOffset();
    //                    CenterAffordance.transform.localPosition = scrollOffset * OffsetScalingFactor;
    //                }
    //                else if(CenterAffordance.transform.localPosition != Vector3.zero)
    //                {
    //                    CenterAffordance.transform.localPosition = Vector3.zero;
    //                }
    //            }

    //            if (Magnifier != null)
    //            {
    //                if (Magnifier.enabled == (CurrentInteractionState == InteractionStateEnum.Scrolling))
    //                    Magnifier.enabled = !(CurrentInteractionState == InteractionStateEnum.Scrolling);

    //                if ( Magnifier.sprite != newSprite)
    //                    Magnifier.sprite = newSprite;
    //            }

    //        }

    //        if (_activeState != CurrentInteractionState)
    //        {
    //            UpdateActiveState(CurrentInteractionState);
    //            _activeState = CurrentInteractionState;

    //            if (HoldProgressBar != null)
    //            {
    //                Animator anim = HoldProgressBar.gameObject.GetComponent<Animator>();
    //                if (anim != null ) 
    //                {
    //                    anim.SetBool("ProgressOn", CurrentInteractionState == InteractionStateEnum.Holding);
    //                }
    //            }
    //        }
    //    }


    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            _scrollInteractible = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            if (_scrollInteractible != null
    //                && _scrollInteractible.ScrollControlType == ScrollZoomInteractible.ScrollControlTypeEnum.JoystickInverted
    //                && Veil.Instance.IsFingerPressed())
    //            {
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            if(_scrollInteractible != null) 
    //                _scrollInteractible = null;
    //        }

    //        return false;
    //    }

    //    public void UpdateActiveState(InteractionStateEnum newState)
    //    {
    //        for(int i = 0; i < transform.childCount; i++)
    //        {
    //            GameObject go = transform.GetChild(i).gameObject;
    //            go.SetActive(InteractionStates[(int)newState].VisibleObjects.Contains(go));
    //        }
    //    }
    //}

}
