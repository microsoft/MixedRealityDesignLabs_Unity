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
    public class HandScrollWidget : CursorWidget
    {
        public enum InteractionStateEnum
        {
            None = -1,
            Idle,
            Scrolling
        }

        [Serializable]
        public class StateDatum
        {
            public StateDatum(InteractionStateEnum state) { this.InteractionState = state; this.Name = state.ToString(); }

            public string Name;
            public bool BaseCursorVisible = true;
            public InteractionStateEnum InteractionState;
            public List<GameObject> VisibleObjects = new List<GameObject>();
        }

        //public StateDatum[] InteractionStates = new StateDatum[] { new StateDatum((InteractionStateEnum)0), new StateDatum((InteractionStateEnum)1) };

        //public float OffsetScalingFactor = 1f;
        //public GameObject CenterAffordance;
        //public bool RequireHold = false;
        //public ScrollTextureInteractible.ScrollControlTypeEnum ScrollType = ScrollTextureInteractible.ScrollControlTypeEnum.JoystickInverted;
        
        //public InteractionStateEnum CurrentInteractionState = InteractionStateEnum.None;

        //private InteractionStateEnum _activeState = InteractionStateEnum.None;
        //private ScrollTextureInteractible _scrollInteractible;

        //// Update is called once per frame
        //void Update()
        //{
        //    if (_scrollInteractible != null)
        //    {
        //        CurrentInteractionState = InteractionStateEnum.Idle;

        //        if (_scrollInteractible.IsScrolling)
        //        {
        //            CurrentInteractionState = InteractionStateEnum.Scrolling;
        //        }

        //        if (CenterAffordance != null)
        //        {
        //            if (CurrentInteractionState == InteractionStateEnum.Scrolling)
        //            {
        //                Vector3 scrollOffset = _scrollInteractible.GetCursorOffset();
        //                CenterAffordance.transform.localPosition = scrollOffset * OffsetScalingFactor;
        //            }
        //            else if (CenterAffordance.transform.localPosition != Vector3.zero)
        //            {
        //                CenterAffordance.transform.localPosition = Vector3.zero;
        //            }
        //        }
        //    }

        //    if (_activeState != CurrentInteractionState)
        //    {
        //        UpdateActiveState(CurrentInteractionState);
        //        _activeState = CurrentInteractionState;
        //    }
        //}

        //public override bool ShouldBeActive()
        //{
        //    if (_curTarget != null)
        //    {
        //        _scrollInteractible = _curTarget.GetComponent<ScrollTextureInteractible>();
        //        if (_scrollInteractible != null
        //            && _scrollInteractible.Scrollable
        //            && _scrollInteractible.ScrollControlType == this.ScrollType
        //            && (this.RequireHold ? Veil.Instance.IsFingerPressed() : true))
        //        {
        //            return true;
        //        }
        //    }
        //    else
        //    {
        //        if (_scrollInteractible != null)
        //            _scrollInteractible = null;
        //    }

        //    return false;
        //}

        //public void UpdateActiveState(InteractionStateEnum newState)
        //{
        //    for (int i = 0; i < transform.childCount; i++)
        //    {
        //        GameObject go = transform.GetChild(i).gameObject;
        //        go.SetActive(InteractionStates[(int)newState].VisibleObjects.Contains(go));
        //    }
        //}
    }
}
