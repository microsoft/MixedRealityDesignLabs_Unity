//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;

namespace HUX.InputValidator
{
    public class GenericInputValidator : AInputValidator
    {
        public TextItem InputSourceText;
        Type previousInputSource;

        protected override void AddInputListeners()
        {
            Init();
            InputShell.Instance.SelectButton.OnChanged += OnSelectChanged;
            InputShell.Instance.MenuButton.OnChanged += OnMenuChanged;
            InputShell.Instance.ScrollVector.OnChanged += OnScrollChanged;
            InputShell.Instance.ZoomVector.OnChanged += OnZoomChanged;
            InputShell.Instance.CardinalVector.OnChanged += OnCardinalVectorChanged;
            
        }

        private void Init()
        {
            var inputSource = InputShellMap.Instance.inputSwitchLogic.CurrentTargetingSource;
            var type = inputSource.GetType();
            InputSourceText.SetText(string.Format("Source: {0}", type));
        }

        private void OnMenuChanged()
        {
            var inputSource = InputShellMap.Instance.inputSwitchLogic.CurrentTargetingSource;
            var message = inputSource.IsMenuPressed() ? "Pressed" : "Released";
            AddMessage(string.Format("Menu {0}", message));
        }

        private void OnSelectChanged()
        {
            var inputSource = InputShellMap.Instance.inputSwitchLogic.CurrentTargetingSource;
            var type = inputSource.GetType();
            var message = inputSource.IsSelectPressed() ? "Pressed" : "Released";
            AddMessage(string.Format("Select {0}", message));
            if (type != previousInputSource)
            {
                InputSourceText.SetText(string.Format("Source: {0}", type));
                InputSourceText.Highlight();
                previousInputSource = type;
            }
        }

        private void OnScrollChanged()
        {
            var position = InputShell.Instance.ScrollVector.pos;
            AddMessage(string.Format("Scroll x ={0}, y= {1}", position.x, position.y));
        }

        private void OnZoomChanged()
        {
            var position = InputShell.Instance.ZoomVector.pos;
            AddMessage(string.Format("Zoom x ={0}, y= {1}", position.x, position.y));
        }

        private void OnCardinalVectorChanged()
        {
            var position = InputShell.Instance.CardinalVector.pos;
            AddMessage(string.Format("Cardinal x ={0}, y= {1}", position.x, position.y));
        }

        protected override void RemoveInputListeners()
        {
            InputShell.Instance.SelectButton.OnChanged -= OnSelectChanged;
            InputShell.Instance.MenuButton.OnChanged -= OnMenuChanged;
            InputShell.Instance.ScrollVector.OnChanged -= OnScrollChanged;
            InputShell.Instance.ZoomVector.OnChanged -= OnZoomChanged;
            InputShell.Instance.CardinalVector.OnChanged -= OnCardinalVectorChanged;
        }
    }
}
