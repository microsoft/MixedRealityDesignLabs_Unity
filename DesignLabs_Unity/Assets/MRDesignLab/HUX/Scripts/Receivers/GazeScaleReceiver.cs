//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using HUX.Focus;

namespace HUX.Receivers
{
    /// <summary>
    /// Example receiver that scales based on gazing at an object
    /// </summary>
    public class GazeScaleReceiver : InteractionReceiver
    {
        [Tooltip("Target object to receive interactions for.")]
        public GameObject TargetObject;

        [Tooltip("When gazing scale up to the Target Scale")]
        public Vector3 TargetScale = new Vector3(1.0f, 1.0f, 1.0f);

        [Tooltip("Time it takes to scale to Target Scale")]
        public float ScaleTime = 3.0f;

        private Vector3 InitialScale;
        private bool bScaling;

        public void Awake()
        {
            InitialScale = TargetObject.transform.localScale;
        }

        protected void FocusEnter(FocusArgs args)
        {
            bScaling = true;
        }

        protected void FocusExit(FocusArgs args)
        {
            if (args.CurNumFocusers == 0)
            {
                bScaling = false;
            }
        }

        public void Update()
        {
            if (bScaling)
            {
                TargetObject.transform.localScale = Vector3.Slerp(TargetObject.transform.localScale, TargetScale, Time.smoothDeltaTime / ScaleTime);
            }
            else if (TargetObject.transform.localScale != InitialScale)
            {
                TargetObject.transform.localScale = Vector3.Slerp(TargetObject.transform.localScale, InitialScale, Time.smoothDeltaTime / ScaleTime);
            }
        }
    }
}