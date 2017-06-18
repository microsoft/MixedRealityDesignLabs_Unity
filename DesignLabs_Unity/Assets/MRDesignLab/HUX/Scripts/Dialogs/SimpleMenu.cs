//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Receivers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Dialogs
{
    [Serializable]
    public abstract class SimpleMenuButton
    {
        public string Name;
        public int Index;
        public InteractionReceiver Target;

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name);
            }
        }
    }

    /// <summary>
    /// Base class for menu that automatically generates buttons from a template
    /// </summary>
    public abstract class SimpleMenu<T> : MonoBehaviour where T : SimpleMenuButton
    {
        /// <summary>
        /// How many buttons can be added to the menu
        /// </summary>
        public const int MaxButtons = 10;

        public GameObject ButtonPrefab;

        public TextMesh TitleText;

        public virtual T[] Buttons
        {
            get
            {
                return buttons;
            }
        }

        public virtual string Title {
            get {
                return TitleText.text;
            }
            set {
                TitleText.text = value;
            }
        }

        public bool DisplayTitle {
            get {
                return TitleText.gameObject.activeSelf;
            }
            set {
                TitleText.gameObject.SetActive(value);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Used by inspectors
        /// </summary>
        public virtual void EditorRefreshButtons()
        {

        }
#endif

        [SerializeField]
        protected Transform buttonParent;

        [SerializeField]
        protected T[] buttons = new T[MaxButtons];

        protected GameObject[] instantiatedButtons;
        protected GameObject instantiatedTitle;

        protected virtual void OnEnable()
        {
            GenerateButtons();
        }

        protected virtual GameObject CreateButton(T template)
        {
            GameObject newButton = GameObject.Instantiate(ButtonPrefab, buttonParent);
            newButton.name = template.Name;
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localRotation = Quaternion.identity;

            // Register the button with the interaction receiver, if it's set
            if (template.Target != null)
                template.Target.RegisterInteractible(newButton);

            return newButton;
        }

        protected virtual void GenerateButtons ()
        {
            if (instantiatedButtons != null)
                return;

            List<GameObject> instantiatedButtonsList = new List<GameObject>();
            int buttonIndex = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                if (!buttons[i].IsEmpty)
                {
                    buttons[i].Index = buttonIndex;
                    buttonIndex++;
                    instantiatedButtonsList.Add(CreateButton(buttons[i]));
                }
            }
            instantiatedButtons = instantiatedButtonsList.ToArray();
        }
    }
}