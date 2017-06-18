using HUX.Buttons;
using HUX.Collections;
using System;
using System.Collections;
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
        private GameObject BackdropBody;

        protected override void OnDrawGizmos() {
            if (!Application.isPlaying) {
                messageText.text = WordWrap(messageText.text, MaxCharsPerLine);
                FinalizeLayout();
            }
        }

        protected override void FinalizeLayout() {
            // Reset scale and position of buttons / backdrop
            BackdropBody.transform.localScale = Vector3.one;
            buttonParent.localPosition = Vector3.zero;
            float backdropPadding = 0.015f;

            // Update the collection
            ObjectCollection collection = buttonParent.GetComponent<ObjectCollection>();
            collection.Rows = buttonParent.childCount;
            collection.UpdateCollection();

            // Get the render bounds for our buttons, message and backdrop
            Bounds messageBounds = messageText.GetComponent<Renderer>().bounds;
            Bounds backdropBounds = BackdropBody.GetComponent<Renderer>().bounds;
            Bounds collectionBounds = new Bounds();
            bool setFirstPoint = false;
            foreach (Transform child in buttonParent) {
                CompoundButton button = child.GetComponent<CompoundButton>();
                if (!setFirstPoint) {
                    setFirstPoint = true;
                    collectionBounds = button.MainRenderer.bounds;
                } else {
                    collectionBounds.Encapsulate(button.MainRenderer.bounds);
                }
            }
            // Move the button parent so that the first button appears below the message
            float buttonsOffset = messageBounds.min.y - collectionBounds.max.y;
            buttonParent.Translate(0f, buttonsOffset, 0f, Space.Self);
            // Scale the backdrop so that the bottom of the backdrop is below the last button
            float backdropBoundsSize = (backdropBounds.max.y - (collectionBounds.min.y + buttonsOffset)) + backdropPadding;
            float yScale = backdropBoundsSize / backdropBounds.size.y;
            BackdropBody.transform.localScale = new Vector3(1f, yScale, 1f);
        }

        protected override void SetTitleAnMessage() {
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