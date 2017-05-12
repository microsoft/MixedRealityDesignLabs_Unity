//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEngine;

namespace HUX.Interaction
{
    public class ManipulationToolbarButton : MonoBehaviour
    {
        const float MoveSpeed = 5f;

        public ManipulationToolbar ParentToolbar;
        public ManipulationToolbar.ButtonTypeEnum Type = ManipulationToolbar.ButtonTypeEnum.Custom;
        public Vector3 DefaultOffset;
        public Vector3 HiddenOffset;
        public Vector3 ManipulateOffset;
        public FilterTag VisibleFilterTag;
        public FilterTag HiddenFilterTag;

        protected void OnEnable ()
        {
            // TODO move this into the tag manager
            VisibleFilterTag = new FilterTag();
            VisibleFilterTag.Tag = "Default";
            HiddenFilterTag = new FilterTag();
            HiddenFilterTag.Tag = "Hidden";
            RefreshType();
            Hide();
        }
        
        protected void Update()
        {
            switch (ParentToolbar.State)
            {
                case ManipulationToolbar.ToolbarStateEnum.Default:
                    // Show hide, adjust, remove buttons
                    // The rest are hidden
                    targetPosition = DefaultOffset;
                    switch (Type)
                    {
                        case ManipulationToolbar.ButtonTypeEnum.Hide:
                        case ManipulationToolbar.ButtonTypeEnum.Adjust:
                        case ManipulationToolbar.ButtonTypeEnum.Remove:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case ManipulationToolbar.ToolbarStateEnum.Hidden:
                    // Show show button
                    // The rest are hidden
                    targetPosition = HiddenOffset;
                    switch (Type)
                    {
                        case ManipulationToolbar.ButtonTypeEnum.Show:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case ManipulationToolbar.ToolbarStateEnum.Manipulation:
                    // Show done / remove buttons
                    // The rest are hidden
                    targetPosition = ManipulateOffset;
                    switch (Type)
                    {
                        case ManipulationToolbar.ButtonTypeEnum.Done:
                        case ManipulationToolbar.ButtonTypeEnum.Remove:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 0.5f);
        }

        private void Hide()
        {
            icon.Alpha = 0f;
            text.DisableText = true;
            cButton.enabled = false;
            cButton.FilterTag = HiddenFilterTag;
        }

        private void Show()
        {
            icon.Alpha = 1f;
            text.DisableText = false;
            cButton.enabled = true;
            cButton.FilterTag = VisibleFilterTag;
        }

        private void RefreshType()
        {
            // TODO move this into button templates

            string buttonName = string.Empty;
            string buttonText = string.Empty;
            string buttonIcon = string.Empty;
            
            switch (Type)
            {
                case ManipulationToolbar.ButtonTypeEnum.Adjust:
                    buttonName = "Adjust";
                    buttonText = "Adjust";
                    buttonIcon = "EBD2";
                    DefaultOffset = new Vector3(0.0f, 0f, -0.0001f);
                    ManipulateOffset = new Vector3(0.0f, 0f, -0.0001f);
                    break;

                case ManipulationToolbar.ButtonTypeEnum.Done:
                    buttonName = "Done";
                    buttonText = "Done";
                    buttonIcon = "E8FB";
                    DefaultOffset = new Vector3(-0.08f, 0f, -0.0002f);
                    ManipulateOffset = new Vector3(-0.04f, 0f, -0.0002f);
                    break;

                case ManipulationToolbar.ButtonTypeEnum.Hide:
                    buttonName = "Hide";
                    buttonText = "Hide Menu";
                    buttonIcon = "E76C";
                    DefaultOffset = new Vector3(-0.08f, 0f, -0.0003f);
                    ManipulateOffset = new Vector3(0.0f, 0f, -0.0003f);
                    break;

                case ManipulationToolbar.ButtonTypeEnum.Remove:
                    buttonName = "Remove";
                    buttonText = "Remove";
                    buttonIcon = "EC90";
                    DefaultOffset = new Vector3(0.08f, 0f, -0.0004f);
                    ManipulateOffset = new Vector3(0.04f, 0f, -0.0004f);
                    break;

                case ManipulationToolbar.ButtonTypeEnum.Show:
                    buttonName = "Show";
                    buttonText = "Show Menu";
                    buttonIcon = "E700";
                    DefaultOffset = new Vector3(-0.08f, 0f, -0.0005f);
                    ManipulateOffset = new Vector3(0.0f, 0f, -0.0005f);
                    break;

                case ManipulationToolbar.ButtonTypeEnum.Custom:
                default:
                    break;
            }

            gameObject.name = buttonName;
            cButton = GetComponent<CompoundButton>();
            cButton.MainRenderer.enabled = false;
            text = GetComponent<CompoundButtonText>();
            text.Text = buttonText;
            icon = GetComponent<CompoundButtonIcon>();
            icon.IconName = buttonIcon;

        }

        private Vector3 targetPosition;
        private CompoundButton cButton;
        private CompoundButtonText text;
        private CompoundButtonIcon icon;
    }
}