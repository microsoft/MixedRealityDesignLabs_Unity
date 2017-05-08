//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Interaction;
using HUX.Receivers;

namespace HUX.Cursors
{
    /// <summary>
    /// The shell cursor is an animated cursor with advanced state information for GFOV.
    /// </summary>
    public class ShellCursor : AnimCursor
    {
        public GameObject BaseCursor;
        public GameObject ScrollCursor;

        [Header("GFOV Frame Data")]
        public int GFOVStartFrame;
        public int GFOVEndFrame;

        public int PressGFOVStartFrame;
        public int PressGFOVEndFrame;

        public int ScrollStart;
        public int ScrollEnd;

        public int ZoomStart;
        public int ZoomEnd;

        private bool _scrollActive = false;
        private bool _gfovActive = false;
        private bool _bSetStartFrame = false;

        public override void Start()
        {
            base.Start();
            if (BaseCursor != null)
                this.SetAnimState(BaseCursor.GetComponent<Animation>());
        }

        protected void OnEnable()
        {
            InteractionManager.OnNavigationStarted += OnNavigationStart;
            InteractionManager.OnNavigationUpdated += OnNavigationUpdate;
            InteractionManager.OnNavigationCompleted += OnNavigationComplete;
            InteractionManager.OnNavigationCanceled += OnNavigationCancel;
        }

        public IEnumerator UpdateGFOV()
        {
            while (InputSources.Instance != null && InputSources.Instance.hands.NumHandsVisible > 0 && !_scrollActive)
            {
				InputSourceHands.CurrentHandState curState = InputSources.Instance.hands.GetHandState(0);

                if (curState.SourceLossRisk > -1)
                {
                    if(curState.SourceLossRisk < 1)
                    {
                        if (this.currentState == CursorState.Interact || this.currentState == CursorState.Hover)
                        {
                            if (!_bSetStartFrame)
                            {
                                _animTime = (float)((GFOVEndFrame - GFOVStartFrame) * (1 - curState.SourceLossRisk) + GFOVStartFrame) / 30f; ;
                                _bSetStartFrame = true;
                            }
                            _animTargetTime = (float)((GFOVEndFrame - GFOVStartFrame) * (1 - curState.SourceLossRisk) + GFOVStartFrame) / 30f;
                        }
                        else if (this.currentState == CursorState.Select)
                        {
                            if (!_bSetStartFrame)
                            {
                                _bSetStartFrame = true;
                                _animTime = (float)((PressGFOVEndFrame - PressGFOVStartFrame) * (1 - curState.SourceLossRisk) + PressGFOVStartFrame) / 30f;
                            }
                            _animTargetTime = (float)((PressGFOVEndFrame - PressGFOVStartFrame) * (1 - curState.SourceLossRisk) + PressGFOVStartFrame) / 30f;
                        }

                        // Rotate the cursor locally based on the hand Guidance dot product
                        BaseCursor.transform.localRotation = Vector3.Dot(this.transform.right, curState.SourceLossMitigationDirection) > 0 ? Quaternion.AngleAxis(0f, Vector3.forward) : Quaternion.AngleAxis(180f, Vector3.forward);
                    }
                    else
                    {
                        _bSetStartFrame = false;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            _gfovActive = false;
            yield return null;
        }

        public override void SetLinearFrames(CursorState newState, CursorState priorState)
        {
            _bSetStartFrame = false;

            if (newState == CursorState.Observe)
            {
                // Reset on toggle back to observe
                if (BaseCursor != null && ScrollCursor != null)
                {

                    ScrollCursor.SetActive(false);
                    BaseCursor.SetActive(true);

                    this.SetAnimState(BaseCursor.GetComponent<Animation>());
                    _scrollActive = false;
                }
            }

            if (newState == CursorState.Interact || newState == CursorState.Hover || newState == CursorState.Select)
            {
                if (!_gfovActive)
                {
                    _gfovActive = true;
                    StartCoroutine("UpdateGFOV");
                } 
            }

            if (!_scrollActive)
            {
                base.SetLinearFrames(newState, priorState);
            }
        }

        public void OnNavigationStart(GameObject obj, InteractionManager.InteractionEventArgs args)
        {
            if (BaseCursor != null && ScrollCursor != null)
            {
                _scrollActive = true;

                BaseCursor.SetActive(false);
                ScrollCursor.SetActive(true);

                this.SetAnimState(ScrollCursor.GetComponent<Animation>());
                _animTime = ScrollStart / 30f;
            }
        }

        public void OnNavigationUpdate(GameObject obj, InteractionManager.InteractionEventArgs args)
        {
            _animTargetTime = (float)((ScrollEnd - ScrollStart) * Mathf.Clamp(args.Position.magnitude, 0f, 1f) + ScrollStart) / 30f;
            ScrollCursor.transform.localRotation = args.Position.y > 0 ? Quaternion.AngleAxis(0f, Vector3.forward) : Quaternion.AngleAxis(180f, Vector3.forward);

        }

        public void OnNavigationComplete(GameObject obj, InteractionManager.InteractionEventArgs args)
        {
            if (BaseCursor != null && ScrollCursor != null)
            {

                ScrollCursor.SetActive(false);
                BaseCursor.SetActive(true);

                this.SetAnimState(BaseCursor.GetComponent<Animation>());
                _scrollActive = false;
            }
        }

        public void OnNavigationCancel(GameObject obj, InteractionManager.InteractionEventArgs args)
        {
            if (BaseCursor != null && ScrollCursor != null)
            {

                ScrollCursor.SetActive(false);
                BaseCursor.SetActive(true);

                this.SetAnimState(BaseCursor.GetComponent<Animation>());
                _scrollActive = false;
            }
        }
    }
}
