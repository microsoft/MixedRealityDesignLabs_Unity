//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

using HUX.Interaction;
using HUX.Focus;

namespace HUX.Utility
{
    /// <summary>
    /// Sequence condition that will trigger based off of an interaction with a target
    /// </summary>
    public class InteractionCondition : ISequencerCondition
    {
        public enum ConditionType
        {
            Tapped,
            HoldStarted,
            HoldReleased,
            Gazed,
            Ungazed,
            HandVisible,
            HandLost
        }

        [Tooltip("Target interactible to check for interaction with")]
        public InteractibleObject targetObject;

        [Tooltip("Condition type for the interaction check")]
        public ConditionType targetCondition;

        [Tooltip("Time this needs to be valid")]
        public float duration;

        private float TimePassed = 0.0f;
        private float TimeStamp = 0.0f;
        private bool _lastHandVisible;

        private bool _tapped = false;
        private bool _held = false;
        private bool _gazed = false;

        public void RegisterCondition()
        {
            Sequencer sequencer = GameObject.FindObjectOfType<Sequencer>();
            if(sequencer != null)
            {
                sequencer.RegisterCondition(this);

                // Register events
                InteractionManager.OnTapped += OnTapped;
                InteractionManager.OnHoldStarted += OnHoldStarted;
                InteractionManager.OnHoldCompleted += OnHoldCompleted;
                InteractionManager.OnHoldCanceled += OnHoldCanceled;

                FocusManager.OnFocusEnter += OnFocusEnter;
                FocusManager.OnFocusExit += OnFocusExit;
            }
            else
            {
                Debug.LogWarning("No sequencer Found!!!");
            }
        }

        public void UnregisterCondition()
        {
            // Unregister events
            InteractionManager.OnTapped -= OnTapped;
            InteractionManager.OnHoldStarted -= OnHoldStarted;
            InteractionManager.OnHoldCompleted -= OnHoldCompleted;
            InteractionManager.OnHoldCanceled -= OnHoldCanceled;

            FocusManager.OnFocusEnter -= OnFocusEnter;
            FocusManager.OnFocusExit -= OnFocusExit;
        }

        public bool CheckCondition()
        {
            TimeStamp = TimeStamp == 0.0f ? Time.time : TimeStamp;
            float _timeDelta = Time.time - TimeStamp;
            TimeStamp = Time.time;

            bool wasTapped = _tapped;
            _tapped = false;

            if (targetObject != null || targetCondition == ConditionType.HandVisible || targetCondition == ConditionType.HandLost)
            {
                switch (targetCondition)
                {
                    case ConditionType.Tapped:
               
                        if (duration > 0.0f)
                        {
                            TimePassed = wasTapped ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return wasTapped;
                        }
                    case ConditionType.HoldStarted:
                        if (duration > 0.0f)
                        {
                            TimePassed = _held ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return _held;
                        }
                    case ConditionType.HoldReleased:
                        if (duration > 0.0f)
                        {
                            TimePassed = !_held ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return !_held;
                        }
                    case ConditionType.Gazed:
                        if (duration > 0.0f)
                        {
                            TimePassed = _gazed ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return _gazed;
                        }
                    case ConditionType.Ungazed:
                        if (duration > 0.0f)
                        {
                            TimePassed = !_gazed ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return !_gazed;
                        }
                    case ConditionType.HandVisible:
                        if (duration > 0.0f)
                        {
                            TimePassed = Veil.Instance.HandVisible ? TimePassed += _timeDelta : 0.0f;
                            return duration < TimePassed;
                        }
                        else
                        {
                            return Veil.Instance.HandVisible;
                        }
                    case ConditionType.HandLost:
                        if (duration > 0.0f)
                        {
                            if (_lastHandVisible != Veil.Instance.HandVisible && _lastHandVisible)
                            {
                                TimePassed += 1.0f;
                            }

                            _lastHandVisible = Veil.Instance.HandVisible;

                            return duration <= TimePassed;
                        }
                        else
                        {
                            return !Veil.Instance.HandVisible;
                        }
                }
            }
            return false;
        }

        #region Event Callbacks

        private void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (obj == targetObject)
            {
                _tapped = true;
            }
        }

        private void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (obj == targetObject)
            {
                _held = true;
            }
        }

        private void OnHoldCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (obj == targetObject)
            {
                _held = false;
            }
        }

        private void OnHoldCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (obj == targetObject)
            {
                _held = false;
            }
        }

        private void OnFocusExit(GameObject obj, FocusArgs args)
        {
            if (obj == targetObject && args.CurNumFocusers == 0)
            {
                _gazed = false;
            }
        }

        private void OnFocusEnter(GameObject obj, FocusArgs args)
        {
            if (obj == targetObject)
            {
                _gazed = true;
            }
        }

        #endregion
    }
}

