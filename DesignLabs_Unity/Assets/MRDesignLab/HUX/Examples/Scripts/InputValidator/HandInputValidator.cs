//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;

namespace HUX.InputValidator
{
    public class HandInputValidator : AInputValidator
    {
        public Color HighlightColour;
        public Color BaseColour;
        public TextItem Hand1PositionLabel;
        public TextItem Hand2PositionLabel;
        public TextItem HandMovingLabel;
        private Coroutine m_HandMoveCoroutine;

        protected override void AddInputListeners()
        {
            InputSources.Instance.hands.OnFingerPressed += OnFingerPressed;
            InputSources.Instance.hands.OnFingerReleased += OnFingerReleased;
            InputSources.Instance.hands.OnHandEntered += OnHandEntered;
            InputSources.Instance.hands.OnHandLeft += OnHandLeft;
            InputSources.Instance.hands.OnHandMoved += OnHandMoved;
            InputSources.Instance.hands.OnMenuChanged += OnMenuChanged;
            InputSources.Instance.hands.OnSelectChanged += OnSelectChanged;

            HandMovingLabel.SetText("Not Moving");
        }

        private void Update()
        {
            var hand1WorldPosition = InputSources.Instance.hands.GetWorldPosition(0);
            if (hand1WorldPosition == Vector3.forward)
            {
                Hand1PositionLabel.SetText("Not Present");
            }
            else
            {
                Hand1PositionLabel.SetText(string.Format("POS: {0}", hand1WorldPosition));
            }

            var hand2WorldPosition = InputSources.Instance.hands.GetWorldPosition(1);
            if (hand2WorldPosition == Vector3.forward)
            {
                Hand2PositionLabel.SetText("Not Present");
            }
            else
            {
                Hand2PositionLabel.SetText(string.Format("POS: {0}", hand2WorldPosition));
            }
        }

        private void OnFingerPressed(InputSourceHands.CurrentHandState handState)
        {
            AddMessage(string.Format("Finger Pressed {0}", handState.HandId));
        }

        private void OnFingerReleased(InputSourceHands.CurrentHandState handState)
        {
            AddMessage(string.Format("Finger Released {0}", handState.HandId));
        }

        private void OnHandEntered(InputSourceHands.CurrentHandState handState)
        {
            AddMessage(string.Format("Hand Entered {0}", handState.HandId));
        }

        private void OnHandLeft(InputSourceHands.CurrentHandState handState)
        {
            AddMessage(string.Format("Hand Left {0}", handState.HandId));
        }

        private void OnHandMoved(InputSourceHands.CurrentHandState handState)
        {
            if (m_HandMoveCoroutine != null)
            {
                StopCoroutine(m_HandMoveCoroutine);
            }
            m_HandMoveCoroutine = StartCoroutine(WaitAndExecute(delegate
            {
                SetHandMovingLabelText("Hand Moving");
            }, 0.2f));
        }

        private IEnumerator WaitAndExecute(Action action, float waitTime)
        {
            action();
            yield return new WaitForSeconds(waitTime);
            SetHandMovingLabelText("Not Moving");
            m_HandMoveCoroutine = null;
        }

        private void SetHandMovingLabelText(string message)
        {
            HandMovingLabel.SetText(message);
            HandMovingLabel.Highlight();
        }

        private void OnMenuChanged(InputSourceBase sourceBase, bool arg2)
        {
            AddMessage("Menu Activated");
        }

        private void OnSelectChanged(InputSourceBase sourceBase, bool arg2)
        {
            AddMessage("Select Activated");
        }

        protected override void RemoveInputListeners()
        {
            InputSources.Instance.hands.OnFingerPressed -= OnFingerPressed;
            InputSources.Instance.hands.OnFingerReleased -= OnFingerReleased;
            InputSources.Instance.hands.OnHandEntered -= OnHandEntered;
            InputSources.Instance.hands.OnHandLeft -= OnHandLeft;
            InputSources.Instance.hands.OnHandMoved -= OnHandMoved;
            InputSources.Instance.hands.OnMenuChanged -= OnMenuChanged;
            InputSources.Instance.hands.OnSelectChanged -= OnSelectChanged;
        }
    }
}
