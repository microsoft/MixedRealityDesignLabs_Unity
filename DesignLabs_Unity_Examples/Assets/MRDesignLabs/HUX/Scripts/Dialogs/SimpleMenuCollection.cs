//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Collections;
using System;
using UnityEngine;

namespace HUX.Dialogs
{
    [Serializable]
    public class SimpleMenuCollectionButton : SimpleMenuButton
    {
        public string Text;
        public string Icon;
    }

    /// <summary>
    /// Class for setting up a quick-and-dirty menu using an ObjectCollection
    /// Useful for setting up program flow very quickly
    /// </summary>
    public class SimpleMenuCollection : SimpleMenu<SimpleMenuCollectionButton>
    {
        public ObjectCollection ParentCollection
        {
            get
            {
                return parentCollection;
            }
            set
            {
                parentCollection = value;
            }
        }

        [SerializeField]
        private ObjectCollection parentCollection;

#if UNITY_EDITOR
        public override void EditorRefreshButtons()
        {
            int buttonIndex = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == null)
                {
                    buttons[i] = new SimpleMenuCollectionButton();
                }
                if (!buttons[i].IsEmpty)
                {
                    buttons[i].Index = i;
                    buttonIndex++;
                }
            }
        }
#endif
        protected override void OnEnable()
        {
            // Set the button parent to the collection
            buttonParent = parentCollection.transform;

            // Set the collection type to tranfsorm order so DisplayOrder has an effect
            parentCollection.SortType = ObjectCollection.SortTypeEnum.TransformReversed;

            // Generate the buttons
            base.OnEnable();
        }

        protected override void GenerateButtons() {
            base.GenerateButtons();
            // If the title is active
            // Set the title to the first sibling so it's first
            // Otherwise set it as last so it doesn't affect placement
            if (DisplayTitle) {
                TitleText.transform.SetAsFirstSibling();
            } else {
                TitleText.transform.SetAsLastSibling();
            }
            // Set the number of rows to the number of buttons
            parentCollection.Rows = instantiatedButtons.Length + (DisplayTitle ? 1 : 0);
            // Sort the buttons
            parentCollection.UpdateCollection();
        }

        /// <summary>
        /// Disables button's main collider and sets text alpha to 1/2
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="enabled"></param>
        public void SetButtonEnabled (string buttonName, bool enabled) {
            for (int i = 0; i < instantiatedButtons.Length; i++) {
                if (instantiatedButtons[i].name.Equals(buttonName)) {
                    CompoundButton button = instantiatedButtons[i].GetComponent<CompoundButton>();
                    button.ButtonState = enabled ? Button.ButtonStateEnum.Interactive : Button.ButtonStateEnum.Disabled;
                    CompoundButtonText text = button.GetComponent<CompoundButtonText>();
                    text.Alpha = enabled ? 1f : 0.5f;
                    break;
                }
            }
        }

        protected override GameObject CreateButton(SimpleMenuCollectionButton template)
        {
            GameObject newButton = base.CreateButton(template);

            // Update the button's icon and text
            // If the button doesn't have these components, ignore these settings
            CompoundButtonIcon icon = newButton.GetComponent<CompoundButtonIcon>();
            if (icon != null)
                icon.IconName = template.Icon;

            CompoundButtonText text = newButton.GetComponent<CompoundButtonText>();
            if (text != null)
                text.Text = template.Text;

            // Set the display order of the button using the transform order
            newButton.transform.SetSiblingIndex(template.Index);

            return newButton;
        }
    }
}