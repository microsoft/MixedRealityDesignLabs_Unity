//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Speech;

namespace HUX.Interaction
{
    public class ScrollTextureInteractible : InteractibleObject
    {
   
        public GameObject TargetObject;

//        public enum ScrollControlTypeEnum
//        {
//            None,
//            Sticky,
//            Joystick,
//            JoystickInverted,
//            Head,
//            Hybrid
//        };

//        [Header("Scroll Values")]
//        public ScrollControlTypeEnum ScrollControlType = ScrollControlTypeEnum.Joystick;
//        public float ScrollSpeed = 1.00f;
//        public float HeadDeadZone = 1.0f;
//        public float HeadMaxAngle = 15.0f;
//        public float HeadTimer = 1.5f;

//        public string PageUpCommand = "page up";
//        public string PageDownCommand = "page down";
//        public TextMesh DebugText;

//        public float HandRange = 0.3f;
//        public enum ScrollDirectionEnum
//        {
//            X,
//            Y,
//            Both
//        }
//        public ScrollDirectionEnum ScrollDirection = ScrollDirectionEnum.Both;

//        public Vector2 MatScale { get { return _targetMat.GetTextureScale("_MainTex"); } }
//        public Vector2 MatOffset { get { return _targetMat.GetTextureOffset("_MainTex"); } }

//        [Header("General")]
//        public int MaterialIdx = 0;
//        public bool DisableOnActionEnd = false;
//        public bool EnableLimits = true;
//        public bool Initialized = false;
//        public float ScrollTime = 1.5f;
//        public AnimationCurve TimeAccelCurve = new AnimationCurve();

//        /// <summary>
//        /// If true the interactible will receive scroll events
//        /// </summary>
//        [Header("Scroll and Zoom Interaction")]
//        [Tooltip("Is this a scrollable interactible")]
//        public bool Scrollable = false;

//        #region private var
//        private float _xOffset;
//        private float _yOffset;
//        private float _zOffset;

//        private float _xLimit;
//        private float _yLimit;
//        private Vector2 _uvCoord;

//        private Material _targetMat;
//        private Vector2 _textOffset;
//        private Vector2 _textScale;
//        private Vector2 _lastOffset;
//        private Vector2 _moveOffset;

//        private FocusManager _FocusManager;
//        private Cursors.ScrollCursor _cursor;

//        private Vector2 _scrollRate = Vector2.zero;
//        private Vector2 _lastScrollRate = Vector2.zero;

//        private Vector3 _handStartPos = Vector3.zero;
//        private float _headStartDist = 0f;

//        private Vector3 _headStartForward = Vector3.forward;

//        private bool _relativeScroll = false;

//        private bool _bTransiting = false;
//        private Vector2 _transTarget;

//        private Vector3 _handVelocity = Vector3.zero;
//        private Vector3 _lastHandPos = Vector3.zero;

//        private Vector2 _cursorOffset = Vector2.zero;
//        private Vector2 _relMin;
//        private Vector2 _relMax;
//        private Vector2 _scrollVal;
//        private Vector2 _zoomCoord;

//        private float _hybridScrollStart;
//        private Vector2 _hybridBaseScrollRate;
//        private bool _scrollEdge = false;
//        private bool _hybridToggled = false;
//        private bool _updateCursor = false;

//        private float _fixedTimeDelta;
//        private Vector2 _dimensions;
//        protected bool _bScrolling;

//        public bool IsScrolling { get { return _bScrolling; } }

//        #endregion private var

//        protected void Start()
//        {
//            _fixedTimeDelta = Time.fixedDeltaTime;

//            _targetMat = TargetObject != null ? TargetObject.GetComponent<Renderer>().materials[MaterialIdx] : this.GetComponent<Renderer>().materials[MaterialIdx];
//            _textOffset = _targetMat.GetTextureOffset("_MainTex");
//            _lastOffset = _textOffset;

//            _textScale = _targetMat.GetTextureScale("_MainTex");

//            _xLimit = 1 - _textScale.x;
//            _yLimit = 1 - _textScale.y;

//            _FocusManager = FocusManager.Instance;
//            _cursor = _FocusManager.Cursor.GetComponent<Cursors.ScrollCursor>();

//            Initialized = true;

//            _dimensions = GetDimensions();
//        }

//        protected void OnEnable()
//        {
//            // Speech command input
//            KeywordManager.Instance.AddKeyword(PageUpCommand, OnWord, ConfidenceLevel.Low);
//            KeywordManager.Instance.AddKeyword(PageDownCommand, OnWord, ConfidenceLevel.Low);
//        }

//        protected void OnDisable()
//        {
//            KeywordManager.Instance.RemoveKeyword(PageUpCommand, OnWord);
//            KeywordManager.Instance.RemoveKeyword(PageDownCommand, OnWord);
//        }

//        public Vector3 GetCursorOffset()
//        {
//            if (ScrollControlType == ScrollControlTypeEnum.Head)
//            {
//                return _cursorOffset;
//            }
//            else
//            {
//                Vector3 offset = _scrollVal;
//                offset.x = -offset.x;
//                return offset;
//            }
//        }

//        public void SetMaterial(Material newMat)
//        {
//            TargetObject = TargetObject == null ? this.gameObject : TargetObject;

//            if (newMat != null)
//            {
//                 TargetObject.GetComponent<Renderer>().materials[MaterialIdx] = newMat;
//                _targetMat = TargetObject.GetComponent<Renderer>().materials[MaterialIdx];

//                _textOffset = _targetMat.GetTextureOffset("_MainTex");
//                _lastOffset = _textOffset;

//                _textScale = _targetMat.GetTextureScale("_MainTex");

//                _xLimit = 1 - _textScale.x;
//                _yLimit = 1 - _textScale.y;
//            }
//        }


//        public void FixedUpdate()
//        {
//            _relativeScroll = ScrollControlType == ScrollControlTypeEnum.Sticky ||
//                (ScrollControlType == ScrollControlTypeEnum.Hybrid);

//            if (_updateCursor)
//            {
//                _updateCursor = false;

//                UpdateCursorType();

//                if (_bScrolling && _relativeScroll)
//                {
//                    GetRelativeLimits(_FocusManager.Cursor.transform.position);
//                }
//            }

//            if (_cursor == null)
//                _cursor = _FocusManager.Cursor.GetComponent<Cursors.ScrollCursor>();

//            if (this.Scrollable)
//            {
//                UpdateScroll();
//            }
//            else
//            {
//                if (_bScrolling)
//                {
//                    _bScrolling = false;
//                    UpdateCursorType();
//                }
//            }

//            if (DisableOnActionEnd)
//            {
//                if(!_bScrolling && Scrollable)
//                {
//                    Scrollable = false;
//                }
//            }
//        }


//        #region Scrolling
//        private void UpdateScroll()
//        {
//            if (_bScrolling)
//            {
//                // Here's where we handle the scroll rates for head offset
//                if (ScrollControlType == ScrollControlTypeEnum.Head)
//                {
//                    // Lets get the angles as 
//                    Vector3 headRef = Veil.Instance.HeadTransform.InverseTransformDirection(_headStartForward);
//                    Quaternion headRot = Quaternion.FromToRotation(headRef, Vector3.forward);

//                    _scrollRate = Vector2.zero;
//                    Vector2 cursorOffset = Vector2.zero;
//                    if (Vector3.Angle(_headStartForward, Veil.Instance.HeadTransform.forward) > HeadDeadZone)
//                    {
//                        float xAngle = headRot.eulerAngles.y > 180f ? headRot.eulerAngles.y - 360f : headRot.eulerAngles.y;
//                        float yAngle = headRot.eulerAngles.x > 180f ? headRot.eulerAngles.x - 360f : headRot.eulerAngles.x;

//                        float valMod = Mathf.Clamp(Mathf.Abs(yAngle) / HeadMaxAngle, 0f, 1f);
//                        float scrollMod = TimeAccelCurve.Evaluate(valMod) * ScrollSpeed;

//                        _scrollRate = _fixedTimeDelta * new Vector2(Mathf.Clamp(xAngle, -HeadMaxAngle, HeadMaxAngle) * scrollMod, Mathf.Clamp(yAngle, -HeadMaxAngle, HeadMaxAngle) * -scrollMod);
//                        cursorOffset = valMod * new Vector2(0.0f, Mathf.Clamp(-yAngle / HeadMaxAngle, -1f, 1f));
//                    }

//                    if (this._cursor != null)
//                        this._cursor.SetScrollValue(cursorOffset);
//                }
//                else if (ScrollControlType != ScrollControlTypeEnum.Head && !_relativeScroll)
//                {
//                    if (this._cursor != null)
//                        this._cursor.SetScrollValue(_scrollVal);

//                }
//                else if (_relativeScroll && _firstHand.guidanceScore != -1)
//                {
//                    // Hand velocity is the averaged over the last 3 samples for distance traveled in meters in one second
//                    _handVelocity = (0.5f / _fixedTimeDelta) * (_handVelocity + (_firstHand.position - _lastHandPos));
//                    _lastHandPos = _firstHand.position;

//                    // Now update the scroll rates
//                    Vector3 handOffset = _handStartPos - _firstHand.position;

//                    float normScale = (1.0f / HandRange);
//                    _scrollRate = Vector2.Scale(new Vector2(handOffset.x, handOffset.y), new Vector2(normScale, normScale));

//                    // Check for hybrid at edge
//                    if (_scrollRate.x < _relMin.x || _scrollRate.y < _relMin.y ||
//                        _scrollRate.x > _relMax.x || _scrollRate.y > _relMax.y)
//                    {
//                        if (!_scrollEdge)
//                        {
//                            _hybridScrollStart = handOffset.magnitude - (_lastScrollRate.magnitude / (_fixedTimeDelta * 2.0f)); // * 0.75f;

//                            // Hand out of range
//                            _scrollEdge = true;
//                            _lastOffset = _textOffset;
//                        }

//                        _scrollRate.Normalize();

//                        if (ScrollControlType == ScrollControlTypeEnum.Sticky)
//                        {
//                            _lastOffset = _textOffset;
//                            _cursorOffset = Vector2.zero;
//                            OnReleaseStarted(souce);

//                            return;
//                        }
//                    }
//                    else
//                    {
//                        if (_scrollEdge)
//                        {
//                            _lastOffset = _textOffset - Vector2.Scale(_cursorOffset, _textScale);
//                            _scrollEdge = false;
//                        }

//                        _cursorOffset = new Vector2(_scrollRate.x, _scrollRate.y);
//                    }
//                }

//                switch (ScrollDirection)
//                {
//                    case ScrollDirectionEnum.Y:
//                        _textOffset.y = _relativeScroll && !_hybridToggled ? _lastOffset.y + (_scrollRate.y * _textScale.y) : Mathf.Lerp(_lastOffset.y, _lastOffset.y + _scrollRate.y, Time.fixedDeltaTime);
//                        break;
//                    case ScrollDirectionEnum.X:
//                        _textOffset.x = _relativeScroll && !_hybridToggled ? _lastOffset.x + (_scrollRate.x * _textScale.x) : Mathf.Lerp(_lastOffset.x, _lastOffset.x + _scrollRate.x, Time.fixedDeltaTime);
//                        break;
//                    case ScrollDirectionEnum.Both:
//                        _textOffset = _relativeScroll && !_hybridToggled ? _lastOffset + Vector2.Scale(_scrollRate, _textScale) : Vector2.Lerp(_lastOffset, _lastOffset + _scrollRate, Time.fixedDeltaTime);
//                        break;
//                }

//                bool _atLimit = false;

//                if (EnableLimits)
//                {
//                    _atLimit = _textOffset.y > _yLimit || _textOffset.x > _xLimit || _textOffset.x < 0 || _textOffset.y < 0;

//                    _textOffset.y = (_textOffset.y > _yLimit) ? _yLimit : _textOffset.y;
//                    _textOffset.y = _textOffset.y < 0 ? 0.0f : _textOffset.y;

//                    _textOffset.x = (_textOffset.x > _xLimit) ? _xLimit : _textOffset.x;
//                    _textOffset.x = _textOffset.x < 0 ? 0.0f : _textOffset.x;

//                }

//                if (_relativeScroll && this._cursor != null && !_scrollEdge && !_atLimit)
//                {
//                    this._cursor.UpdateOffset(_cursorOffset);
//                }

//                if (!_relativeScroll || _hybridToggled)
//                    _lastOffset = _textOffset;

//                // Update texture offset
//                _lastScrollRate = _textOffset - _targetMat.GetTextureOffset("_MainTex");
//                _targetMat.SetTextureOffset("_MainTex", _textOffset);
//            }

//            if (_bTransiting)
//            {
//                if (_textOffset != _lastOffset + _transTarget)
//                {
//                    if (EnableLimits)
//                    {
//                        Vector3 testCheck = _lastOffset + _transTarget;
//                        if (testCheck.y > _yLimit || testCheck.x > _xLimit || testCheck.x < 0 || testCheck.y < 0)
//                        {
//                            _transTarget = Vector2.zero;
//                            _bTransiting = false;
//                            _lastOffset = _textOffset;
//                            return;
//                        }
//                    }

//                    _textOffset += (_fixedTimeDelta * 5.0f) * _transTarget;

//                    if (Vector2.Distance(_textOffset, _lastOffset + _transTarget) < 0.01f)
//                    {
//                        _transTarget = Vector2.zero;
//                        _bTransiting = false;
//                        _lastOffset = _textOffset;
//                    }
//                }

//                _targetMat.SetTextureOffset("_MainTex", _textOffset);
//            }
//        }
//#endregion



//        public void OnWord(PhraseRecognizedEventArgs args)
//        {
//            if (gameObject == _FocusManager.PrimeFocus)
//            {
//                if (args.text == PageUpCommand)
//                {
//                    PageUp();
//                }
//                else if (args.text == PageDownCommand)
//                {
//                    PageDown();
//                }
//            }
//        }

//        public void PageUp()
//        {
//            _transTarget = _textOffset;
//            _transTarget.y = Mathf.Clamp(_textOffset.y - _textScale.y, 0.0f, 1.0f - _textScale.y);
//            _bTransiting = true;
//        }

//        public void PageDown()
//        {
//            _transTarget = _textOffset;
//            _transTarget.y = Mathf.Clamp(_textOffset.y + _textScale.y, 0.0f, 1.0f - _textScale.y);
//            _bTransiting = true;
//        }

//        public void UpdateOffset(float VerticalVal)
//        {
//            float YOffset = VerticalVal * 1.0f / _textScale.y;
//            YOffset = (YOffset > _yLimit) ? _yLimit : YOffset;
//            YOffset = YOffset < 0 ? 0.0f : YOffset;
//            _textOffset.y = YOffset;
//            _targetMat.SetTextureOffset("_MainTex", _textOffset);
//        }

//        // Get limits takes and returns the x and y limits as vec2
//        private void GetRelativeLimits(Vector3 target)
//        {
//            _uvCoord = _FocusManager.FocusHitInfo.textureCoord;

//            _relMin = new Vector2(-(_uvCoord.x * _dimensions.x), -(_uvCoord.y * _dimensions.y));
//            _relMax = new Vector2((1f - _uvCoord.x) * _dimensions.x, (1f - _uvCoord.y) * _dimensions.y);
//        }

//        // Get limits takes and returns the x and y limits as vec2
//        private Vector2 GetDimensions()
//        {
//            Vector2 dim = Vector2.zero;

//            MeshRenderer mr = this.gameObject.GetComponent<MeshRenderer>();
            
//            if (mr != null)
//            {
//                Vector3 relExtent = this.transform.rotation * mr.bounds.size;
//                // relExtent.Scale(this.transform.lossyScale);
//                dim = (Vector2)relExtent;
//            }

//            return dim;
//        }

//        public void UpdateCursorType()
//        {
//            if (_cursor != null)
//            {
//                Cursors.ScrollCursor.CursorTypeEnum cursorType = Cursors.ScrollCursor.CursorTypeEnum.Default;

//                if (_bScrolling)
//                {
//                    if (ScrollControlType == ScrollControlTypeEnum.Joystick)
//                    {
//                        cursorType = Cursors.ScrollCursor.CursorTypeEnum.Joystick;
//                    }
//                    else if (ScrollControlType == ScrollControlTypeEnum.JoystickInverted)
//                    {
//                        cursorType = Cursors.ScrollCursor.CursorTypeEnum.JoystickInverted;
//                    }
//                    else if (ScrollControlType == ScrollControlTypeEnum.Sticky ||
//                        (ScrollControlType == ScrollControlTypeEnum.Hybrid))
//                    {
//                        cursorType = Cursors.ScrollCursor.CursorTypeEnum.Sticky;
//                        _cursor.StickyLimits = (ScrollControlType == ScrollControlTypeEnum.Hybrid);
//                    }
//                    else if (ScrollControlType == ScrollControlTypeEnum.Head)
//                    {
//                        cursorType = Cursors.ScrollCursor.CursorTypeEnum.Look;
//                    }
//                }

//                this._cursor.UpdateOffset(Vector2.zero);
//                _cursor.SetCursorType(cursorType);
//            }
//        }

//        #region Gesture API Scroll Callbacks
//        public void OnNavigationStart(SourceState source, Vector3 navPos)
//        {
//            if (gameObject == _FocusManager.PrimeFocus)
//            {
//                if (Scrollable)
//                {
//                    if (ScrollControlType != ScrollControlTypeEnum.Head)
//                    {
//                        _bScrolling = true;
//                        _lastOffset = _textOffset;
//                        _handStartPos = _firstHand.position;
//                        _updateCursor = true;
//                        _hybridToggled = _scrollEdge = false;
//                    }
//                }
//            }
//        }

//        public void OnNavigationUpdate(SourceState source, Vector3 navPos, Ray ray)
//        {
//            if (gameObject == _FocusManager.PrimeFocus)
//            {
//                if (Scrollable)
//                {
//                    if (ScrollControlType != ScrollControlTypeEnum.Head && !_relativeScroll)
//                    {
//                        _scrollRate = ScrollControlType == ScrollControlTypeEnum.JoystickInverted ? new Vector2(navPos.x * ScrollSpeed, navPos.y * -ScrollSpeed) : new Vector2(navPos.x * ScrollSpeed, navPos.y * ScrollSpeed);
//                        _scrollVal = navPos;
//                    }
//                }
//            }
//        }

//        public void OnNavigationComplete(SourceState source, Vector3 navPos)
//        {
//            if (_bScrolling)
//            {
//                if (ScrollControlType != ScrollControlTypeEnum.Head)
//                {
//                    _bScrolling = false;
//                    _textOffset = _targetMat.GetTextureOffset("_MainTex");
//                    _lastOffset = _textOffset; // _targetMat.GetTextureOffset("_MainTex");

//                    if (ScrollControlType == ScrollControlTypeEnum.Sticky)
//                    {
//                        _transTarget = new Vector2(0.0f, 0.2f * navPos.y * _textScale.y);
//                        _bTransiting = true;
//                    }

//                    _scrollEdge = _hybridToggled = false;
//                   _updateCursor = true;
//                }
//            }
//        }
//        #endregion

//        public void OnSelectHeld(GameObject selectedObject)
//        {
//            if (gameObject == _FocusManager.PrimeFocus)
//            {
//                if (ScrollControlType == ScrollControlTypeEnum.Head)
//                {
//                    _bScrolling = true;
//                    _headStartForward = Veil.Instance.HeadTransform.forward;
//                    _updateCursor = true;
//                }
//            }
//        }

//        public void OnReleaseStarted(SourceState source)
//        {
//            if (_bScrolling)
//            {
//                if (ScrollControlType == ScrollControlTypeEnum.Head)
//                {
//                    _bScrolling = false;
//                    _updateCursor = true;
//                }
//                else
//                {
//                    this.OnNavigationComplete(source, _scrollRate);
//                }
//            }
//        }
    }
}