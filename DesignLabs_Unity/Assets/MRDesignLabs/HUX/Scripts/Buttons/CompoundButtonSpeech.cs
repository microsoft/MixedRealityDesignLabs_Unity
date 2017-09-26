//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Speech;
using UnityEngine;

namespace HUX.Buttons
{
    /// <summary>
    /// This class will automatically link buttons to speech keywords
    /// (Currently disabled)
    /// </summary>
    [RequireComponent (typeof(CompoundButton))]
    public class CompoundButtonSpeech : MonoBehaviour
    {
        public enum KeywordSourceEnum
        {
            None,
            LocalOverride,
            ButtonText,
        }

        /// <summary>
        /// Source of the keyword to be used
        /// By default the text in a CompoundButtonText component will be used
        /// </summary>
        public KeywordSourceEnum KeywordSource = KeywordSourceEnum.ButtonText;

        /// <summary>
        /// Keyword used when KeywordSource is set to LocalOverride
        /// </summary>
        public string Keyword = string.Empty;

        /// <summary>
        /// The confidence level to use for this speech command
        /// </summary>
        public KeywordConfidenceLevel ConfidenceLevel = KeywordConfidenceLevel.Unknown;

        /// <summary>
        /// Variable to keep track of previous button text incase the button text changes after registration.
        /// </summary>
        private string prevButtonText;

        /// <summary>
        /// The final keyword that is registered with the Keyword Manager
        /// </summary>
        private string keyWord;
        
        public void Start ()
        {
            // Disable if no microphone devices are found
            if (Microphone.devices.Length == 0) {
                enabled = false;
                return;
            }

            if (KeywordSource == KeywordSourceEnum.None)
                return;

            keyWord = string.Empty;

            switch (KeywordSource)
            {
                case KeywordSourceEnum.ButtonText:
                default:
                    CompoundButtonText text = GetComponent<CompoundButtonText>();
                    keyWord = prevButtonText = text.Text;
                    break;

                case KeywordSourceEnum.LocalOverride:
                    keyWord = Keyword;
                    break;
            }

            KeywordManager.Instance.AddKeyword(keyWord, new KeywordManager.KeywordRecognizedDelegate(KeywordHandler), ConfidenceLevel);
        }

        public void Update()
        {
            // Check if Button text has changed. If so, remove previous keyword and add new button text
            if (KeywordSource == KeywordSourceEnum.ButtonText &&
                prevButtonText != null &&
                GetComponent<CompoundButtonText>().Text != prevButtonText)
            {
                KeywordManager.Instance.RemoveKeyword(prevButtonText, KeywordHandler);
                prevButtonText = GetComponent<CompoundButtonText>().Text;
                KeywordManager.Instance.AddKeyword(prevButtonText, new KeywordManager.KeywordRecognizedDelegate(KeywordHandler), ConfidenceLevel);
            }
        }

        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(this.keyWord))
                return;

            // Unregister callback and keyword when this script is destroyed
            KeywordManager.Instance.RemoveKeyword(this.keyWord, KeywordHandler);
        }
        
        public void KeywordHandler(KeywordRecognizedEventArgs args)
        {
            if (!gameObject.activeSelf || !enabled)
                return;

            Debug.Log("Keyword handler called in " + name + " for keyword " + args.text + " with confidence level " + args.confidence);
            // Send a pressed message to the button through the InteractionManager
            // (This will ensure InteractionReceivers also receive the event)

            gameObject.SendMessageUpwards("OnTapped", this.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
