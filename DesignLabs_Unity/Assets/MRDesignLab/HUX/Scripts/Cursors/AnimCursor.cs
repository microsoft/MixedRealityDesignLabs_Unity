//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

namespace HUX.Cursors
{
    /// <summary>
    /// Implementation of an animation based cursor
    /// </summary>
    public class AnimCursor : Cursor 
	{
        public enum AnimCursorTypeEnum
        {
            Animator,
            Linear,
            BlendShapes
        }

        public AnimCursorTypeEnum AnimCursorType = AnimCursorTypeEnum.Animator;

        [Serializable]
        public class AnimFrameData
        {
            public CursorState FromState;
            public CursorState ToState;
            public int StartFrame;
            public int EndFrame;
        }

        [Header("Frame Data")]
        public AnimFrameData[] AnimData;

        public bool UseLight = true;

        [SerializeField]
        private Light _cursorLight;
        private Animator _cursorAnimator;
        private Animation _cursorAnim;
        private AnimationState _animState;

        protected float _animTime;
        protected float _animTargetTime;

        protected bool _lockedVisibility = false;

        public override void Start()
        {
            switch (AnimCursorType)
            {
                case AnimCursorTypeEnum.Animator:
                    _cursorAnimator = gameObject.GetComponentInChildren<Animator>();
                    break;
                case AnimCursorTypeEnum.Linear:
                    _cursorAnim = gameObject.GetComponentInChildren<Animation>();
                    SetAnimState(_cursorAnim);
                    break;
                case AnimCursorTypeEnum.BlendShapes:
                    break;
            }

            base.Start();
        }

        protected void SetAnimState(Animation targetAnim)
        {
            _cursorAnim = targetAnim;
            _animState = _cursorAnim[_cursorAnim.clip.name];

            // Setup anim state attributes
            _animState.speed = 0.0f;
            _animState.weight = 1.0f;
            _animState.enabled = true;

        }

        // For the General Cursor we want to simply override these states
        public override void OnStateChange(CursorState state)
        {
            switch (AnimCursorType)
            {
                case AnimCursorTypeEnum.Animator:
                    _cursorAnimator.SetInteger("CursorState", (int)state);
                    break;
                case AnimCursorTypeEnum.Linear:
                    SetLinearFrames(state, currentState);
                    break;
                case AnimCursorTypeEnum.BlendShapes:
                    break;
            }

            base.OnStateChange(state);
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            _cursorLight.enabled = UseLight;

            if (AnimCursorType == AnimCursorTypeEnum.Linear)
            {
                if (_animTime != _animTargetTime)
                {
                    _animTime = _animTime < _animTargetTime ? _animTime + Time.deltaTime : _animTime - Time.deltaTime;
                    _animTime = Mathf.Abs(_animTargetTime - _animTime) < (1f / 30f) ? _animTargetTime : _animTime;
                    _animState.time = _animTime;
                }
            }
        }

        /// <summary>
        /// Set linear frams sets the start frame and end frame to scrub when not using an animator.
        /// </summary>
        /// <param name="newState"></param>
        /// <param name="priorState"></param>
        public virtual void SetLinearFrames(CursorState newState, CursorState priorState)
        {
            for(int i = 0; i < AnimData.Length; i++)
            {
                if (AnimData[i].FromState == priorState && AnimData[i].ToState == newState)
                {
                    _animTime = AnimData[i].StartFrame / 30f;
                    _animTargetTime = AnimData[i].EndFrame / 30f;
                }
            }
        }

        /// <summary>
        /// Check that the cursor is visible
        /// </summary>
        /// <returns></returns>
        public override bool IsVisible()
        {
            GameObject AnimGo = _cursorAnimator != null ? _cursorAnimator.gameObject : _cursorAnim != null ? _cursorAnim.gameObject : null;
            if (AnimGo != null)
            {
                return AnimGo.activeSelf;
            }

            return true;
        }

        /// <summary>
        /// Use this to hide the base cursor without desabling it
        /// </summary>
        /// <param name="visible"></param>
        public override void SetVisible(bool visible)
        {
            if (!_lockedVisibility)
            {
                GameObject AnimGo = _cursorAnimator != null ? _cursorAnimator.gameObject : _cursorAnim != null ? _cursorAnim.gameObject : null;
                if (AnimGo != null)
                {
                    AnimGo.SetActive(visible);
                }
            }

        }

        /// <summary>
        /// Lock the current visibility of the cursor.
        /// </summary>
        public void LockVisibility(bool locked)
        {
            _lockedVisibility = locked;
        }

    }
}
