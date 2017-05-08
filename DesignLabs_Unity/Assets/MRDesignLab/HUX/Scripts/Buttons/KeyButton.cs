//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine; 
using System.Collections;


namespace HUX.Buttons
{
    /// <summary>
    /// Key button for key collections (virtual keyboard/keypad)
    /// </summary>
    public class KeyButton : Button
    {
        #region public members
        /// <summary>
        /// Key text color when in an idle state
        /// </summary>
        [Tooltip("Text Color When Idle")]
        public Color IdleTxColor = Color.gray;

        /// <summary>
        /// Key text color when in pressed state
        /// </summary>
        [Tooltip("Text Color When Pressed")]
        public Color PressTxColor = Color.blue;

        /// <summary>
        /// Key background color when key is pressed
        /// </summary>
        [Tooltip("Background Color When Pressed")]
        public Color PressBgColor = Color.blue;

        /// <summary>
        /// Key background color when in idle
        /// </summary>
        [Tooltip("Background Color When Idle")]
        public Color IdleBgColor = Color.gray;

        /// <summary>
        /// Key background color when hovering
        /// </summary>
        [Tooltip("Background Color When Hovering")]
        public Color HoverBgColor = Color.grey;

        /// <summary>
        /// Audio clip to play on key press
        /// </summary>
        [Tooltip("Audio clip for key press")]
        public AudioClip KeySound;

        /// <summary>
        /// KeyType enum for being able to assign different key types
        /// </summary>
        public enum KeyTypeEnum
        {
            /// <summary>
            /// Key is a text entry key, character or text to add to text target
            /// </summary>
            Text,
            /// <summary>
            /// Key is backspace
            /// </summary>
            Backspace,
            /// <summary>
            /// Key is a submit key
            /// </summary>
            Submit,
            /// <summary>
            /// Key is a space entry
            /// </summary>
            Space,
            /// <summary>
            /// Key is a caps key
            /// </summary>
            Caps,
            /// <summary>
            /// Key represents symbols key on keyboard
            /// </summary>
            Symbols,
            /// <summary>
            /// Key representing reload key on keyboard.
            /// </summary>
            Reload,
            /// <summary>
            /// Key representing bookmark key on keyboard.
            /// </summary>
            Bookmark
        }

        /// <summary>
        /// KeyType enum for this key button instance
        /// </summary>
        [Tooltip("Type of key (used to filter on events)")]
        public KeyTypeEnum KeyType = KeyTypeEnum.Text;

        /// <summary>
        /// Background gameobject for this key button
        /// </summary>
        [Tooltip("Game object for key background")]
        public GameObject BackgroundObject;

        /// <summary>
        /// Textmesh target for this key button
        /// </summary>
        [Tooltip("Textmesh object for key text")]
        public TextMesh KeyText;

        /// <summary>
        /// Character representation for this key button.  The value is used
        /// to populate the target text.
        /// </summary>
        [Tooltip("Character for key button")]
        public string Character;
        #endregion

        #region private members
        /// <summary>
        /// The local renderer component for the background game object.
        /// </summary>
        private Renderer _bgRenderer;
        #endregion

        /// <summary>
        /// Start function for setting up the local variables and calling start in base.
        /// </summary>
        protected void Start()
        {
            // Grab the text mesh for the key in a child component
            KeyText = gameObject.GetComponentInChildren<TextMesh>();
            if(BackgroundObject != null)
                _bgRenderer = BackgroundObject.GetComponent<Renderer>();
        }

        /// <summary>
        /// Late update function for checking that the key text matches the chracter.
        /// </summary>
        private new void LateUpdate()
        {
            if (KeyText != null && KeyText.text != Character)
                KeyText.text = Character;
        }

        /// <summary>
        /// OnStateChange override for changing button state on the key button.
        /// </summary>
        public override void OnStateChange(ButtonStateEnum newState)
        {
            switch (newState)
            {
                case ButtonStateEnum.Observation:
                    if (_bgRenderer != null)
                    {
                        _bgRenderer.material.color = IdleBgColor;
                    }
                    KeyText.color = IdleTxColor;
                    break;
                case ButtonStateEnum.Targeted:
                    if (_bgRenderer != null)
                    {
                        _bgRenderer.material.color = IdleBgColor;
                    }
                    KeyText.color = IdleTxColor;
                    break;
                case ButtonStateEnum.Interactive:
                    if (_bgRenderer != null)
                    {
                        _bgRenderer.material.color = HoverBgColor;
                    }
                    KeyText.color = IdleTxColor;
                    break;
                case ButtonStateEnum.Pressed:
                    if (_bgRenderer != null)
                    {
                        _bgRenderer.material.color = PressBgColor;
                    }
                    KeyText.color = PressTxColor;

                    if (KeySound != null && AudioManager.Instance != null)
                        AudioManager.Instance.PlayClip(KeySound);

                    break;
            }
        }
    }
}