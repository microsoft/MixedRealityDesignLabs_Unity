//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;

namespace HUX.InputValidator
{
    public class MouseInputValidator : AInputValidator
    {
        protected override void AddInputListeners()
        {
            InputSources.Instance.mouse.OnMousePressed += OnMousePressed;
            InputSources.Instance.mouse.OnMouseReleased += OnMouseReleased;
            InputSources.Instance.mouse.OnScrollWheelChanged += OnScrollWheelChanged;
        }

        private void OnScrollWheelChanged(float scrollWheelValue)
        {
            AddMessage(string.Format("Scroll Wheel Delta: {0}", scrollWheelValue.ToString("G4")));
        }

        private void OnMousePressed(MouseButton mouseButton)
        {
            AddMessage(string.Format("{0:G} Pressed", mouseButton));
        }

        private void OnMouseReleased(MouseButton mouseButton)
        {
            AddMessage(string.Format("{0:G} Released", mouseButton));
        }

        protected override void RemoveInputListeners()
        {
            InputSources.Instance.mouse.OnMousePressed -= OnMousePressed;
            InputSources.Instance.mouse.OnMouseReleased -= OnMouseReleased;
        }
    }
}
