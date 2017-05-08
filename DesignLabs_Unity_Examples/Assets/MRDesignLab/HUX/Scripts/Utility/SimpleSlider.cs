//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;
using HUX.Focus;

namespace HUX
{
    /// <summary>
    /// Attach to an object to have the object slide based on cursor offset.
    /// </summary>
    public class SimpleSlider : MonoBehaviour
    {
        [Tooltip("Target for the slider to move")]
        public GameObject SliderTarget;

        [Tooltip("Base object interactable that the slider is attached to")]
        public InteractibleObject Interactible;

        public enum AxisType
        {
            xAxis,
            yAxis
        }

        public enum TrackingType
        {
            Cursor,
            Head,
            Hand
        }

        [Tooltip("Which relative axis to slide on")]
        public AxisType Axis = AxisType.yAxis;

        [Tooltip("What transform to track for the slider")]
        public TrackingType TrackTarget = TrackingType.Cursor;

        [Tooltip("Relative offset of slider target and base object")]
        public Vector3 RelativeOffset;

        [Tooltip("Time to lerp to target location")]
        public float LerpTime = 0.2f;

        [Tooltip("Clamp the offset at this value on either side")]
        public float ClampOffset = 1.0f;

        private bool useGaze = false;
        private bool isGazing = false;

        private void Start()
        {
            FocusManager.OnFocusEnter += OnFocusEnter;
            FocusManager.OnFocusExit += OnFocusExit;
        }

        private void OnDestroy()
        {
            FocusManager.OnFocusEnter -= OnFocusEnter;
            FocusManager.OnFocusExit -= OnFocusExit;
        }

        public void OnEnable()
        {
            if (Interactible != null)
            {
                useGaze = true;
            }
        }

        private void FixedUpdate()
        {
            Vector3 targetPos = FocusManager.Instance.GazeFocuser.Cursor.transform.position;

            switch (TrackTarget)
            {
                case TrackingType.Hand:
                {
                    targetPos = Veil.Instance.HandPosition;
                    break;
                }

                case TrackingType.Head:
                {
                    targetPos = Veil.Instance.HeadTransform.position;
                    break;
                }
            }

            RelativeOffset = this.transform.InverseTransformPoint(targetPos);

            if (useGaze && !isGazing)
            {
                return;
            }

            if (SliderTarget != null)
            {
                Vector3 locPos = Vector3.zero;
                switch (Axis)
                {
                    case AxisType.xAxis:
                    {
                        locPos = SliderTarget.transform.localPosition;
                        locPos.x = Mathf.Lerp(locPos.x, RelativeOffset.x, Time.fixedDeltaTime / LerpTime);
                        locPos.x = Mathf.Clamp(locPos.x, -ClampOffset, ClampOffset);
                        SliderTarget.transform.localPosition = locPos;
                        break;
                    }


                    case AxisType.yAxis:
                    {
                        locPos = SliderTarget.transform.localPosition;
                        locPos.y = Mathf.Lerp(locPos.y, RelativeOffset.y, Time.fixedDeltaTime / LerpTime);
                        locPos.y = Mathf.Clamp(locPos.y, -ClampOffset, ClampOffset);
                        SliderTarget.transform.localPosition = locPos;
                        break;
                    }
                }
            }
        }


        private void OnFocusEnter(GameObject obj, FocusArgs args)
        {
            if (obj == Interactible.gameObject)
            {
                isGazing = true;
            }
        }

        private void OnFocusExit(GameObject obj, FocusArgs args)
        {
            if (obj == Interactible.gameObject && args.CurNumFocusers == 0)
            {
                isGazing = false;
            }
        }
    }
}
