//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;

namespace HUX
{
    [CustomEditor(typeof(CompoundButtonSpeech))]
    public class CompoundButtonSpeechInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButtonSpeech speechButton = (CompoundButtonSpeech)target;

            HUXEditorUtils.BeginSectionBox("Keyword source");
            speechButton.KeywordSource = (CompoundButtonSpeech.KeywordSourceEnum) EditorGUILayout.EnumPopup(speechButton.KeywordSource);
            CompoundButtonText text = speechButton.GetComponent<CompoundButtonText>();
            switch (speechButton.KeywordSource)
            {
                case CompoundButtonSpeech.KeywordSourceEnum.ButtonText:
                default:
                    if (text == null)
                    {
                        HUXEditorUtils.ErrorMessage("No CompoundButtonText component found.", AddText);
                    } else if (string.IsNullOrEmpty (text.Text))
                    {
                        HUXEditorUtils.WarningMessage("No keyword found in button text.");
                    } else
                    {
                        EditorGUILayout.LabelField("Keyword: " + text.Text);
                    }
                    break;

                case CompoundButtonSpeech.KeywordSourceEnum.LocalOverride:
                    speechButton.Keyword = EditorGUILayout.TextField(speechButton.Keyword);
                    break;

                case CompoundButtonSpeech.KeywordSourceEnum.None:
                    HUXEditorUtils.DrawSubtleMiniLabel("(Speech control disabled)");
                    break;
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target);
        }

        private void AddText ()
        {
            CompoundButtonSpeech speechButton = (CompoundButtonSpeech)target;
            speechButton.gameObject.AddComponent<CompoundButtonText>();
        }
    }
}
