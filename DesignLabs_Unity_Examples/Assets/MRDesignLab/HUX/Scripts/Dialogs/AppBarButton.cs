//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEngine;

namespace HUX.Interaction
{
    public class AppBarButton : MonoBehaviour
    {
        public const float ButtonWidth = 0.08f;
        public const float ButtonDepth = -0.0001f;
        const float MoveSpeed = 5f;

        public ButtonIconProfile CustomIconProfile;
        public AppBar ParentToolbar;
        public AppBar.ButtonTemplate Template;
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
                case AppBar.ToolbarStateEnum.Default:
                    // Show hide, adjust, remove buttons
                    // The rest are hidden
                    targetPosition = DefaultOffset;
                    switch (Template.Type)
                    {
                        case AppBar.ButtonTypeEnum.Hide:
                        case AppBar.ButtonTypeEnum.Remove:
                        case AppBar.ButtonTypeEnum.Adjust:
                        case AppBar.ButtonTypeEnum.Custom:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case AppBar.ToolbarStateEnum.Hidden:
                    // Show show button
                    // The rest are hidden
                    targetPosition = HiddenOffset;
                    switch (Template.Type)
                    {
                        case AppBar.ButtonTypeEnum.Show:
                            Show();
                            break;

                        default:
                            Hide();
                            break;
                    }
                    break;

                case AppBar.ToolbarStateEnum.Manipulation:
                    // Show done / remove buttons
                    // The rest are hidden
                    targetPosition = ManipulateOffset;
                    switch (Template.Type)
                    {
                        case AppBar.ButtonTypeEnum.Done:
                        case AppBar.ButtonTypeEnum.Remove:
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
            gameObject.name = Template.Name;
            cButton = GetComponent<CompoundButton>();
            cButton.MainRenderer.enabled = false;
            text = GetComponent<CompoundButtonText>();
            text.Text = Template.Text;
            icon = GetComponent<CompoundButtonIcon>();
            if (CustomIconProfile != null) {
                icon.IconProfile = CustomIconProfile;
                icon.IconName = string.Empty;
            }
            icon.IconName = Template.Icon;


            // Apply offset based on total number of buttons
            float xDefaultOffset = (ParentToolbar.NumDefaultButtons / 2) * ButtonWidth;
            float xManipulationOffset = (ParentToolbar.NumManipulationButtons / 2) * ButtonWidth;

            // For odd numbers of buttons, add an additional 1/2 button offset
            if (ParentToolbar.NumDefaultButtons > 1 && ParentToolbar.NumDefaultButtons % 2 == 0) {
                xDefaultOffset -= (ButtonWidth / 2);
            }
            if (ParentToolbar.NumManipulationButtons > 1 && ParentToolbar.NumManipulationButtons % 2 == 0) {
                xManipulationOffset -= (ButtonWidth / 2);
            }

            DefaultOffset = new Vector3(
                Template.DefaultPosition * ButtonWidth - xDefaultOffset,
                0f,
                Template.DefaultPosition * ButtonDepth);
            ManipulateOffset = new Vector3(
                Template.ManipulationPosition * ButtonWidth - xManipulationOffset,
                0f,
                Template.ManipulationPosition * ButtonDepth);
            HiddenOffset = Vector3.zero;
        }

        private Vector3 targetPosition;
        private CompoundButton cButton;
        private CompoundButtonText text;
        private CompoundButtonIcon icon;
    }
}