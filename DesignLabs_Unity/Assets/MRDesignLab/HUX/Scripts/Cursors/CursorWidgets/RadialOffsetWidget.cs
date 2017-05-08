//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HUX.Interaction;
using HUX.Utility;

namespace HUX.Cursors
{
    //public class RadialOffsetWidget : CursorWidget
    //{
    //    [Serializable]
    //    public class Piece
    //    {
    //        public enum DynamicAdjustmentType
    //        {
    //            None,
    //            Offset,
    //            Scale
    //        }

    //        public string Name;
    //        public GameObject DisplayObject;
    //        public GameObject DisplayInstance { get; set; }
    //        public DynamicAdjustmentType DynamicType;
    //        public float ScalingFactor = 1;
    //        public float EuclideanMagnitudeMax = (float)Math.Sqrt(3);
    //        public TransformCache _transformCache;

    //        public void Update(Vector3 inputControl)
    //        {
    //            if (DynamicType != DynamicAdjustmentType.None) {
    //                Vector3 myAmount = inputControl * ScalingFactor;
    //                if (myAmount.magnitude > EuclideanMagnitudeMax)
    //                {
    //                    myAmount *= (EuclideanMagnitudeMax / myAmount.magnitude);
    //                }

    //                switch (DynamicType)
    //                {
    //                    case DynamicAdjustmentType.Offset:
    //                        DisplayInstance.transform.localPosition = myAmount + _transformCache.position;
    //                        break;
    //                    case DynamicAdjustmentType.Scale:
    //                        inputControl = new Vector3(1.0f / inputControl.x, 1.0f / inputControl.y, 1.0f / inputControl.z);
    //                        DisplayInstance.transform.localScale = Vector3.Scale(_transformCache.scale, inputControl); ;
    //                        break;
    //                }
    //            }
    //        }

    //        public void CacheTransforms()
    //        {
    //            _transformCache.cache(DisplayInstance.transform);
    //        }
    //    }

    //    public List<Piece> Pieces = new List<Piece>();
    //    public Vector3 InputControl;

    //    public override bool ShouldBeActive()
    //    {
    //        if (_curTarget != null)
    //        {
    //            ScrollZoomInteractible i = _curTarget.GetComponent<ScrollZoomInteractible>();
    //            if (i != null 
    //                && i.ScrollControlType == ScrollZoomInteractible.ScrollControlTypeEnum.JoystickInverted
    //                && Veil.Instance.IsFingerPressed())
    //            {
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public void Update()
    //    {
    //        if (_curTarget != null)
    //        {
    //            ScrollZoomInteractible i = _curTarget.GetComponent<ScrollZoomInteractible>();

    //            foreach (Piece piece in Pieces)
    //            {
    //                Vector3 inputControl = Vector3.one;
    //                switch (piece.DynamicType)
    //                {
    //                    case RadialOffsetWidget.Piece.DynamicAdjustmentType.Offset:
    //                        inputControl = i.GetCursorOffset();
    //                        break;
    //                    case RadialOffsetWidget.Piece.DynamicAdjustmentType.Scale:
    //                        if (i.IsZooming)
    //                        {
    //                            inputControl = new Vector3(i.ZoomRate, i.ZoomRate, i.ZoomRate);
    //                        }
    //                        break;
    //                }

    //                InputControl = inputControl;
    //                piece.Update(inputControl);
    //            }
    //        }
    //    }

    //    public override void Start()
    //    {
    //        base.Start();
    //        InitDisplayables();
    //    }

    //    private void InitDisplayables()
    //    {
    //        foreach (Piece piece in Pieces)
    //        {
    //            if (piece.DisplayObject != null)
    //            {
    //                if (StaticExtensions.IsPrefab(piece.DisplayObject))
    //                {
    //                    piece.DisplayInstance = Instantiate(piece.DisplayObject);
    //                    piece.CacheTransforms();
    //                    //set it as a child
    //                    piece.DisplayInstance.transform.parent = transform;
    //                    piece.DisplayInstance.transform.localPosition = piece._transformCache.position;
    //                    piece.DisplayInstance.transform.localRotation = piece._transformCache.rotation;
    //                    piece.DisplayInstance.transform.localScale = piece._transformCache.scale;
    //                }
    //                else
    //                {
    //                    piece.DisplayInstance = piece.DisplayObject;
    //                    piece.CacheTransforms();
    //                }
    //            }
    //        }
    //    }
    //}
}
