//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace HUX.Dialogs
{
    /// <summary>
    /// Dialog that approximates the look of a Hololens shell dialog
    /// </summary>
    public class SimpleDialogShell : SimpleDialog
    {
        public int MaxCharsPerLine = 40;

        [SerializeField]
        private TextMesh titleText;

        [SerializeField]
        private TextMesh messageText;

        [SerializeField]
        private GameObject[] oneButtonSet;

        [SerializeField]
        private GameObject[] twoButtonSet;

        /*[SerializeField]
        private GameObject[] threeButtonSet;*/

        protected new void OnDrawGizmos()
        {
            messageText.text = WordWrap(messageText.text, MaxCharsPerLine);
        }

        protected override void FinalizeLayout() {
            // nothing to do here
            // in another dialog we might resize to accomodate text size, etc
        }

        protected override void GenerateButtons()
        {
            List<ButtonTypeEnum> buttonTypes = new List<ButtonTypeEnum>();
            foreach (ButtonTypeEnum buttonType in Enum.GetValues(typeof(ButtonTypeEnum)))
            {
                if (buttonType == ButtonTypeEnum.None)
                    continue;

                // If this button type flag is set
                if ((buttonType & result.Buttons) == buttonType)
                {
                    buttonTypes.Add(buttonType);
                }
            }

            GameObject[] buttonSet = null;
            switch (buttonTypes.Count)
            {
                case 1:
                    buttonSet = oneButtonSet;
                    break;

                case 2:
                    buttonSet = twoButtonSet;
                    break;

                /*case 3:
                    buttonSet = threeButtonSet;
                    break;*/

                default:
                    UnityEngine.Debug.LogError("This dialog only supports up to 2 buttons - you've tried to create " + buttonTypes.Count);
                    return;

            }

            for (int i = 0; i < buttonSet.Length; i++)
            {
                CompoundButtonText text = buttonSet[i].GetComponent<CompoundButtonText>();
                text.Text = buttonTypes[i].ToString();
                buttonSet[i].AddComponent<SimpleDialogButton>().Type = buttonTypes[i];
                RegisterInteractible(buttonSet[i].gameObject);
                buttonSet[i].gameObject.SetActive(true);
            }
        }

        protected override void SetTitleAndMessage() {
            titleText.text = Result.Title;
            messageText.text = WordWrap(Result.Message, MaxCharsPerLine);
        }

        public static string WordWrap(string text, int maxCharsPerLine) {
            int pos = 0;
            int next = 0;
            StringBuilder stringBuilder = new StringBuilder();
            
            if (maxCharsPerLine < 1)
                return text;
            
            for (pos = 0; pos < text.Length; pos = next) {
                int endOfLine = text.IndexOf(System.Environment.NewLine, pos);

                if (endOfLine == -1)
                    next = endOfLine = text.Length;
                else
                    next = endOfLine + System.Environment.NewLine.Length;
                
                if (endOfLine > pos) {
                    do {
                        int len = endOfLine - pos;

                        if (len > maxCharsPerLine)
                            len = BreakLine(text, pos, maxCharsPerLine);

                        stringBuilder.Append(text, pos, len);
                        stringBuilder.Append(System.Environment.NewLine);
                        
                        pos += len;

                        while (pos < endOfLine && Char.IsWhiteSpace(text[pos]))
                            pos++;

                    } while (endOfLine > pos);
                } else stringBuilder.Append(System.Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
        
        public static int BreakLine(string text, int pos, int max) {
            int i = max - 1;

            while (i >= 0 && !Char.IsWhiteSpace(text[pos + i])) {
                i--;
            }

            if (i < 0) {
                return max;
            }

            while (i >= 0 && Char.IsWhiteSpace(text[pos + i])) {
                i--;
            }

            return i + 1;
        }
    }
}