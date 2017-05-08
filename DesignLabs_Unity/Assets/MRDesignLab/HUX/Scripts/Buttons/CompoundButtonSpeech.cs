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

        public void Start ()
        {

            if (KeywordSource == KeywordSourceEnum.None)
                return;

            string keyWord = string.Empty;

            switch (KeywordSource)
            {
                case KeywordSourceEnum.ButtonText:
                default:
                    CompoundButtonText text = GetComponent<CompoundButtonText>();
                    keyWord = text.Text;
                    break;

                case KeywordSourceEnum.LocalOverride:
                    keyWord = Keyword;
                    break;
            }

            KeywordManager.Instance.AddKeyword(keyWord, new KeywordManager.KeywordRecognizedDelegate(KeywordHandler), ConfidenceLevel);
        }

        public void KeywordHandler(KeywordRecognizedEventArgs args)
        {
            Debug.Log("Keyword handler called in " + name + " for keyword " + args.text + " with confidence level " + args.confidence);
            // Send a pressed message to the button through the InteractionManager
            // (This will ensure InteractionReceivers also receive the event)
            // TEMP use a send message event
            gameObject.SendMessageUpwards("OnTapped", SendMessageOptions.DontRequireReceiver);
        }
    }
}
