//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Cursors
{
    /// <summary>
    /// The scroll cursor is an animated cursor with added prefabs for scrolling info.
    /// </summary>
    public class ScrollCursor : Cursor
    {
        public enum CursorTypeEnum
        {
            Default,
            Joystick,
            JoystickInverted,
            Sticky,
            Look,
            Hybrid
        }

        public CursorTypeEnum CursorType = CursorTypeEnum.Default;

        public enum ScrollStateEnum
        {
            None,
            ScrollingUp,
            ScrollingDown,
            ScrollingLeft,
            ScrollingRight,
            ZoomIn,
            ZoomOut,
        }

        public ScrollStateEnum ScrollState = ScrollStateEnum.None;

        public GameObject BaseCursor;
        public GameObject JoystickCursor;
        public GameObject InvertedJoystickCursor;
        public GameObject StickyCursor;
        public GameObject LookCursor;

        public Sprite UpLimitSprite;
        public Sprite DownLimitSprite;

        public bool StickyLimits;

        private Sprite DefaultSprite;
        private Animator _cursorAnimator;

//        private Vector2 _minLimits;
//        private Vector2 _maxLimits;

        public override void Start()
        {
            if (BaseCursor != null)
                _cursorAnimator = BaseCursor.GetComponent<Animator>();

            SetCursorType(CursorType);

            if (StickyCursor != null)
                DefaultSprite = StickyCursor.GetComponent<SpriteRenderer>().sprite;

            base.Start();
        }

        public override void OnStateChange(CursorState state)
        {
            _cursorAnimator.SetInteger("CursorState", (int)state);
            base.OnStateChange(state);
        }

        public override void UpdateTransform()
        {
            if (CursorType == CursorTypeEnum.Default)
            {
                base.UpdateTransform();
            }
        }

        public override void SetVisible(bool visible)
        {
            Debug.Log("SetVisible:" + visible);
            BaseCursor.SetActive(visible);
            if (visible)
            {
                _cursorAnimator.SetInteger("CursorState", (int)currentState);
            }
            base.SetVisible(visible);
        }

        public void SetCursorType(CursorTypeEnum cursorType)
        {
            Debug.LogFormat("ScrollCursor - Cursor type changed: {0}", cursorType);
            System.Diagnostics.Debug.WriteLine(string.Format("ScrollCursor - Cursor type changed: {0}", cursorType));

            CursorType = cursorType;

            if (BaseCursor != null && BaseCursor.activeSelf != (CursorType == CursorTypeEnum.Default))
            {
                BaseCursor.SetActive(CursorType == CursorTypeEnum.Default);
            }

            if (JoystickCursor != null && JoystickCursor.activeSelf != (CursorType == CursorTypeEnum.Joystick))
            {
                JoystickCursor.SetActive(CursorType == CursorTypeEnum.Joystick);
            }

            if (InvertedJoystickCursor != null && InvertedJoystickCursor.activeSelf != (CursorType == CursorTypeEnum.JoystickInverted))
            {
                InvertedJoystickCursor.SetActive(CursorType == CursorTypeEnum.JoystickInverted);
            }

            if (StickyCursor != null && StickyCursor.activeSelf != (CursorType == CursorTypeEnum.Sticky))
            {
                StickyCursor.SetActive(CursorType == CursorTypeEnum.Sticky);
            }

            if (LookCursor != null && LookCursor.activeSelf != (CursorType == CursorTypeEnum.Look))
            {
                LookCursor.SetActive(CursorType == CursorTypeEnum.Look);
            }
        }

        public void SetScrollState(ScrollStateEnum state)
        {
            if (ScrollState != state)
            {
                ScrollState = state;
            }

            if (CursorType == CursorTypeEnum.Sticky)
            {
                switch (state)
                {
                    case ScrollStateEnum.None:
                        StickyCursor.GetComponent<SpriteRenderer>().sprite = DefaultSprite;
                        break;
                    case ScrollStateEnum.ScrollingUp:
                        StickyCursor.GetComponent<SpriteRenderer>().sprite = UpLimitSprite;
                        break;
                    case ScrollStateEnum.ScrollingDown:
                        StickyCursor.GetComponent<SpriteRenderer>().sprite = DownLimitSprite;
                        break;
                }
            }
        }

        public void SetScrollValue(Vector2 scrollVal)
        {
            if(this.CursorType == CursorTypeEnum.Joystick && this.JoystickCursor != null)
            {
              //  this.JoystickCursor.GetComponent<JoystickCursor>().UpdateScroll(scrollVal);
            }
            else if(this.CursorType == CursorTypeEnum.JoystickInverted && this.InvertedJoystickCursor != null)
            {
                scrollVal.x = -scrollVal.x;
             //   this.InvertedJoystickCursor.GetComponent<RingCursor>().UpdateCursor(scrollVal);
            }
            else if(this.CursorType == CursorTypeEnum.Look && this.LookCursor != null)
            {
              //  this.LookCursor.GetComponent<RingCursor>().UpdateCursor(scrollVal);
            }
        }


        public void UpdateOffset(Vector2 cursorOffset)
        {
            if(this.CursorType == CursorTypeEnum.Sticky)
            {
                Vector3 locPos = new Vector3(cursorOffset.x, -cursorOffset.y, 0f);
                locPos.Scale(InvertScale(this.transform.lossyScale));

                //Vector3 locPos = new Vector3(cursorOffset.x, cursorOffset.y , 0f);
                //locPos.Scale(InvertScale(StickyCursor.transform.lossyScale));
                StickyCursor.transform.localPosition = locPos;
            }
        }

        private Vector3 InvertScale(Vector3 sourceScale)
        {
            Vector3 invScale = new Vector3();

            invScale.x = 1.0f / sourceScale.x;
            invScale.y = 1.0f / sourceScale.y;
            invScale.z = 1.0f / sourceScale.z;

            return invScale;
        }

    }
}