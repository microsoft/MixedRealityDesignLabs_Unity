//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Interaction
{
//    public class ZoomTextureInteractible : BaseInteractibleObject
//    {
//        public GameObject ScrollTarget;

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

//        public enum ZoomControlTypeEnum
//        {
//            SingleHand,
//            TwoHandPinch,
//            TwoHandFree,
//            Head,
//            HandVertical
//        };

//        [Header("Zoom Values")]
//        public ZoomControlTypeEnum ZoomControlType = ZoomControlTypeEnum.SingleHand;
//        public float ZoomSpeed = 1.00f;
//        public float ZoomMotionRange = 1.00f;
//        public bool ContinuousZoom = false;
//        public string ZoomInCommand = "zoom in";
//        public string ZoomOutCommand = "zoom out";

//        public Vector2 MatScale { get { return _targetMat.GetTextureScale("_MainTex"); } }
//        public Vector2 MatOffset { get { return _targetMat.GetTextureOffset("_MainTex"); } }

//        [Header("General")]
//        public int MaterialIdx = 0;
//        public bool DisableOnActionEnd = false;
//        public bool EnableLimits = true;
//        public bool Initialized = false;
//        public float ScrollTime = 1.5f;
//        public AnimationCurve TimeAccelCurve = new AnimationCurve();

//        public float ZoomVal { get { return _zoomVal; } }
//        public float ZoomRate { get { return _zoomRate; } }

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

//        private HUXGazer _hGazer;
//        private Cursors.ScrollCursor _cursor;

//        private float _lastZoomTime;
//        private int _zoomLevel;

//        // private float _twoHandTime = 2.0f;
//        private float _towHandTimeElapsed;
//        private float _twoHandTimestamp = 0.0f;
//        private float _twoHandZ = 0.0f;
//        private bool _twoHands = false;
        
//        private Vector2 _scrollRate = Vector2.zero;
//        private Vector2 _lastScrollRate = Vector2.zero;

//        private Vector3 _handStartPos = Vector3.zero;
//        private float _headStartDist = 0f;

//        private Vector3 _headStartForward = Vector3.forward;

//        private bool _relativeScroll = false;

//        private float _zoomRate;
//        private float _zoomVal;
//        private float _zoomInitMagnitude;

//        private bool _bTransiting = false;
//        private Vector2 _transTarget;

//        private KeywordRecognizer _recognizer;

//        private Hand _firstHand;
//        private Hand _secondHand;

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

//        // Globals
//        private float _fixedTimeDelta;
//        private Vector2 _dimensions;

//        // Debug for hybrid offsets
//        private GameObject xMin;
//        private GameObject xMax;
//        private GameObject yMin;
//        private GameObject yMax;


//        #endregion private var

//        protected override void Start()
//        {
//            base.Start();

//            _fixedTimeDelta = Time.fixedDeltaTime;

//            _targetMat = ScrollTarget != null ? ScrollTarget.GetComponent<Renderer>().materials[MaterialIdx] : this.GetComponent<Renderer>().materials[MaterialIdx];
//            _textOffset = _targetMat.GetTextureOffset("_MainTex");
//            _lastOffset = _textOffset;

//            _textScale = _targetMat.GetTextureScale("_MainTex");

//            _xLimit = 1 - _textScale.x;
//            _yLimit = 1 - _textScale.y;

//            // Speech command input
//            _recognizer = new KeywordRecognizer();
//            _recognizer.KeywordRecognized += OnWord;
//            _recognizer.AddKeywords(new string[] { PageDownCommand, PageDownCommand, ZoomInCommand, ZoomOutCommand });
            
//            _hGazer = HUXGazer.Instance as HUXGazer;
//            _cursor = _hGazer.Cursor.GetComponent<Cursors.ScrollCursor>();

//            Initialized = true;

//            _dimensions = GetDimensions();
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
//            ScrollTarget = ScrollTarget == null ? this.gameObject : ScrollTarget;

//            if (newMat != null)
//            {
//                 ScrollTarget.GetComponent<Renderer>().materials[MaterialIdx] = newMat;
//                _targetMat = ScrollTarget.GetComponent<Renderer>().materials[MaterialIdx];

//                _textOffset = _targetMat.GetTextureOffset("_MainTex");
//                _lastOffset = _textOffset;

//                _textScale = _targetMat.GetTextureScale("_MainTex");

//                _xLimit = 1 - _textScale.x;
//                _yLimit = 1 - _textScale.y;
//            }
//        }


//        public void FixedUpdate()
//        {
//            _firstHand = InputHandReceiver.GetHand(InputController.HandID.First);
//            _secondHand = InputHandReceiver.GetHand(InputController.HandID.Second);

//            _twoHands = InputHandReceiver.HandState == InputController.HandStatesEnum.TwoHeld ||
//                 InputHandReceiver.HandState == InputController.HandStatesEnum.TwoVisibleOneHeld ||
//                 InputHandReceiver.HandState == InputController.HandStatesEnum.TwoVisible;

//            _relativeScroll = ScrollControlType == ScrollControlTypeEnum.Sticky ||
//                (ScrollControlType == ScrollControlTypeEnum.Hybrid);

//            if (_updateCursor)
//            {
//                _updateCursor = false;

//                UpdateCursorType();

//                if (_bScrolling && _relativeScroll)
//                {
//                    GetRelativeLimits(_hGazer.Cursor.transform.position);
//                }
//            }

//            if (_cursor == null)
//                _cursor = _hGazer.Cursor.GetComponent<Cursors.ScrollCursor>();

//            // If we're Zooming and scrolling kill the scrolling
//            if (_bZooming && _bScrolling)
//            {
//                this.OnScrollComplete(GestureSource.None, _scrollVal);
//            }

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

//            if (this.Zoomable)
//            {
//                UpdateZoom();
//            }
//            else
//            {
//                if (_bZooming)
//                {
//                    _bZooming = false;
//                    UpdateCursorType();
//                }
//            }

//            if (DisableOnActionEnd)
//            {
//                if(!_bScrolling && !_bZooming && (Zoomable || Scrollable))
//                {
//                    Zoomable = Scrollable = false;
//                }
//            }
//        }


//        #region Scrolling
//        private void UpdateScroll()
//        {
//            if (_bScrolling)
//            {
//                if (_twoHands)
//                {
//                    _bScrolling = false;
//                    return;
//                }

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
//                            OnReleaseStarted(new Hand());
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


//#region Zooming
//        private void UpdateZoom()
//        {
//            if (_bZooming)
//            {
//                Vector2 curScale = _targetMat.GetTextureScale("_MainTex");
//                Vector2 newScale = ContinuousZoom ? curScale : _textScale;

//                // Vector2 headOffset = Vector2.Lerp(_zoomCoord, _hGazer.GetRaycastHit().textureCoord, _fixedTimeDelta * 2.5f);

//                if (ZoomControlType == ZoomControlTypeEnum.TwoHandPinch || ZoomControlType == ZoomControlTypeEnum.TwoHandFree)
//                {
//                    if(_firstHand.guidanceScore == -1 || _secondHand.guidanceScore == -1 || ZoomControlType == ZoomControlTypeEnum.TwoHandPinch && InputHandReceiver.HandState != InputController.HandStatesEnum.TwoHeld ||
//                        ZoomControlType == ZoomControlTypeEnum.TwoHandFree && InputHandReceiver.HandState != InputController.HandStatesEnum.TwoVisibleOneHeld)
//                    {
//                        _twoHandTimestamp = 0.0f;
//                        ExitZoomState();
//                        return;
//                    }

//                    // Zoom rate -1 to 1
//                    _zoomVal = (_zoomInitMagnitude - (_firstHand.position - _secondHand.position).magnitude) / ZoomMotionRange;
//                }
//                else if (ZoomControlType == ZoomControlTypeEnum.Head) // Head Zooming: Get start pos and uze Z.
//                {
//                    float headDist = Vector3.Distance(Veil.Instance.HeadTransform.position, this.transform.position);
//                    float headVal = headDist - _headStartDist;

//                    _zoomVal = Mathf.Clamp(headVal / ZoomMotionRange, -1, 1);
//                }

//                if (ZoomControlType != ZoomControlTypeEnum.SingleHand && ZoomControlType != ZoomControlTypeEnum.HandVertical)
//                {
//                    // Apply the Curve
//                    _zoomVal = _zoomVal > 0 ? TimeAccelCurve.Evaluate(Mathf.Abs(_zoomVal)) : -TimeAccelCurve.Evaluate(Mathf.Abs(_zoomVal));
//                }

//                _zoomRate = _zoomVal * ZoomSpeed;
//                _zoomRate = Mathf.Clamp(_zoomRate, -0.9f, 1.0f);

//                // _zoomRate = _zoomRate * TimeAccelCurve.Evaluate(Mathf.Abs(_zoomRate));
//                _zoomRate = 1 + _zoomRate;

//                newScale.Scale(new Vector2(_zoomRate, _zoomRate));

//                // _zoomLevel 
//                if (EnableLimits)
//                {
//                    if (newScale.x > 1.0f )
//                        newScale = (1.0f / newScale.x) * newScale;

//                    if (newScale.y > 1.0f)
//                        newScale = (1.0f / newScale.y) * newScale;
//                }

//                _targetMat.SetTextureScale("_MainTex", newScale);

//                Vector2 scaleDiff = _textScale - newScale;

//                Vector2 newOffset = Vector2.Scale(scaleDiff, _zoomCoord);
//                newOffset += _textOffset;

//                //_xLimit = 1 - newOffset.x;
//                //_yLimit = 1 - newOffset.y;

//                //if (EnableLimits)
//                //{
//                //    newOffset.y = (newOffset.y > _yLimit) ? _yLimit : newOffset.y;
//                //    newOffset.y = newOffset.y < 0 ? 0.0f : newOffset.y;

//                //    newOffset.x = (newOffset.x > _xLimit) ? _xLimit : newOffset.x;
//                //    newOffset.x = newOffset.x < 0 ? 0.0f : newOffset.x;
//                //}

//                _targetMat.SetTextureOffset("_MainTex", newOffset);

//                if (DebugText != null)
//                    DebugText.text = "Additive Offest: " + scaleDiff.ToString();
//            }
//        }
//#endregion


//        public void OnWord(string text, ConfidenceLevel confidence)
//        {
//            if (confidence != ConfidenceLevel.Rejected)
//            {
//                if (this == Gazer.Instance.CurrentInteractibleObject)
//                {
//                    if (text == PageUpCommand)
//                    {
//                        PageUp();
//                    }
//                    else if (text == PageDownCommand)
//                    {
//                        PageDown();
//                    }
//                    else if (text == ZoomInCommand)
//                    {
//                        _textScale = _targetMat.GetTextureScale("_MainTex");
//                        _textScale.Scale(new Vector2(0.5f, 0.5f));
//                        _targetMat.SetTextureScale("_MainTex", _textScale);
//                    }
//                    else if (text == ZoomOutCommand)
//                    {
//                        _textScale = _targetMat.GetTextureScale("_MainTex");
//                        _textScale.Scale(new Vector2(2.0f, 2.0f));
//                        _targetMat.SetTextureScale("_MainTex", _textScale);
//                    }
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
//            _uvCoord = _hGazer.GetRaycastHit().textureCoord;

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
//                else if (_bZooming)
//                {

//                }

//                this._cursor.UpdateOffset(Vector2.zero);
//                _cursor.SetCursorType(cursorType);
//            }
//        }

//        #region Gesture API Scroll Callbacks
//        public override void OnScrollStart(GestureSource source, Vector2 scrollVelocity)
//        {
//            if (this == Gazer.Instance.CurrentInteractibleObject)
//            {
//                if (Scrollable && !_twoHands)
//                {
//                    if (ScrollControlType != ScrollControlTypeEnum.Head)// && !_bZooming)
//                    {
//                        _bScrolling = true;
//                        _lastOffset = _textOffset;
//                        _handStartPos = _firstHand.position;
//                        _updateCursor = true;
//                        _hybridToggled = _scrollEdge = false;
//                    }
//                }
//                else if(Zoomable && ZoomControlType == ZoomControlTypeEnum.HandVertical)
//                {
//                    _zoomCoord = _hGazer.GetRaycastHit().textureCoord;
//                    _bZooming = true;
//                }
//            }
//        }

//        public override void OnScrollUpdate(GestureSource source, Vector2 scrollVelocity)
//        {
//            if (this == Gazer.Instance.CurrentInteractibleObject)
//            {
//                if (Scrollable && !_twoHands)
//                {
//                    if (ScrollControlType != ScrollControlTypeEnum.Head && !_relativeScroll)
//                    {
//                        _scrollRate = ScrollControlType == ScrollControlTypeEnum.JoystickInverted ? new Vector2(scrollVelocity.x * ScrollSpeed, scrollVelocity.y * -ScrollSpeed) : new Vector2(scrollVelocity.x * ScrollSpeed, scrollVelocity.y * ScrollSpeed);
//                        _scrollVal = scrollVelocity;
//                    }
//                }
//                else if (Zoomable && ZoomControlType == ZoomControlTypeEnum.HandVertical)
//                {
//                    _zoomVal = -scrollVelocity.y;
//                }
//            }
//        }

//        public override void OnScrollComplete(GestureSource source, Vector2 scrollVelocity)
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
//                        _transTarget = new Vector2(0.0f, 0.2f * scrollVelocity.y * _textScale.y);
//                        _bTransiting = true;
//                    }

//                    _scrollEdge = _hybridToggled = false;
//                   _updateCursor = true;
//                }
//            }

//            if(_bZooming && ZoomControlType == ZoomControlTypeEnum.HandVertical)
//            {
//                ExitZoomState();
//            }
//        }
//        #endregion

//        #region Gesture API Zoom Callbacks
//        public override void OnZoomStart(GestureSource source, float zoomAmount)
//        {
//            if (this == Gazer.Instance.CurrentInteractibleObject && Zoomable)
//            {
//                if (ZoomControlType == ZoomControlTypeEnum.SingleHand)
//                {
//                    _zoomCoord = _hGazer.GetRaycastHit().textureCoord;
//                    _bZooming = true;
//                }
//            }
//        }

//        public override void OnZoomUpdate(GestureSource source, float zoomAmount)
//        {
//            if (this == Gazer.Instance.CurrentInteractibleObject && Zoomable)
//            {
//                if (ZoomControlType == ZoomControlTypeEnum.SingleHand)
//                {
//                    _zoomVal = -zoomAmount;
//                }
//            }
//        }

//        public override void OnZoomComplete(GestureSource source, float zoomAmount)
//        {
//            if (_bZooming)
//            {
//                if (ZoomControlType == ZoomControlTypeEnum.SingleHand)
//                {
//                    ExitZoomState();
//                }
//            }
//        }

//        private void ExitZoomState()
//        {
//            _textScale = _targetMat.GetTextureScale("_MainTex");
//            _textOffset = _targetMat.GetTextureOffset("_MainTex");

//            _xLimit = 1 - _textScale.x;
//            _yLimit = 1 - _textScale.y;

//            _bZooming = false;
//        }
//        #endregion

//        public override void OnSelectHeld(GameObject selectedObject)
//        {
//            if (this == Gazer.Instance.CurrentInteractibleObject)
//            {
//                if (ScrollControlType == ScrollControlTypeEnum.Head)
//                {
//                    _bScrolling = true;
//                    _headStartForward = Veil.Instance.HeadTransform.forward;
//                    _updateCursor = true;
//                }

//                if (ZoomControlType == ZoomControlTypeEnum.Head)
//                {
//                    _zoomCoord = _hGazer.GetRaycastHit().textureCoord;
//                    _bZooming = true;
//                    _headStartDist = Vector3.Distance(Veil.Instance.HeadTransform.position, this.transform.position);
//                }
//                else if (ZoomControlType == ZoomControlTypeEnum.TwoHandPinch || ZoomControlType == ZoomControlTypeEnum.TwoHandFree)
//                {
//                    if (_firstHand.guidanceScore != -1 && _secondHand.guidanceScore != -1)
//                    {
//                        if (_bScrolling)
//                        {
//                            _bScrolling = false;
//                            UpdateCursorType();
//                        }

//                        if ((ZoomControlType == ZoomControlTypeEnum.TwoHandPinch &&
//                            InputHandReceiver.HandState == InputController.HandStatesEnum.TwoHeld) ||
//                            (ZoomControlType == ZoomControlTypeEnum.TwoHandFree &&
//                            InputHandReceiver.HandState == InputController.HandStatesEnum.TwoVisibleOneHeld))
//                        {
//                            _zoomCoord = _hGazer.GetRaycastHit().textureCoord;
//                            _bZooming = true;
//                            _zoomInitMagnitude = (_firstHand.position - _secondHand.position).magnitude;
//                        }
//                    }
//                }
//            }
//            base.OnSelectHeld(selectedObject);
//        }

//        public override void OnReleaseStarted(Hand hand)
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
//                    this.OnScrollComplete(GestureSource.Hand, _scrollRate);
//                }
//            }

//            if (_bZooming)
//            {
//                if (ZoomControlType == ZoomControlTypeEnum.Head)
//                {
//                    _bZooming = false;
//                }
//                else
//                {
//                    this.OnZoomComplete(GestureSource.Hand, _zoomRate);
//                }
//            }

//            base.OnReleaseStarted(hand);
//        }
//    }
}
