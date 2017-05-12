//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(CompoundButtonText))]
    public class CompoundButtonTextInspector : Editor
    {
        public GUIStyle textAreaLabel;

        public override void OnInspectorGUI()
        {
            CompoundButtonText textButton = (CompoundButtonText)target;

            textAreaLabel = new GUIStyle(EditorStyles.textArea);
            textAreaLabel.fontSize = 32;

            GUI.color = Color.white;
            
            textButton.DisableText = EditorGUILayout.Toggle("Disable text", textButton.DisableText);
            if (textButton.DisableText)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            textButton.TextProfile = HUXEditorUtils.DrawProfileField<ButtonTextProfile>(textButton.TextProfile);
            
            if (textButton.TextProfile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            textButton.TextMesh = HUXEditorUtils.DropDownComponentField <TextMesh>("Text mesh", textButton.TextMesh, textButton.transform);

            //textButton.TextMesh = (TextMesh)EditorGUILayout.ObjectField("Text mesh", textButton.TextMesh, typeof(TextMesh), true);

            if (textButton.TextMesh == null)
            {
                GUI.color = HUXEditorUtils.ErrorColor;
                EditorGUILayout.LabelField("You must select a text mesh object.");
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.BeginSectionBox("Overrides");
            EditorGUILayout.BeginHorizontal();
            textButton.OverrideFontStyle = EditorGUILayout.Toggle("Font style", textButton.OverrideFontStyle);
            if (textButton.OverrideFontStyle)
            {
                textButton.Style = (FontStyle)EditorGUILayout.EnumPopup(textButton.Style);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            textButton.OverrideAnchor = EditorGUILayout.Toggle("Text anchor", textButton.OverrideAnchor);
            if (textButton.OverrideAnchor) {
                textButton.Anchor = (TextAnchor)EditorGUILayout.EnumPopup(textButton.Anchor);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            textButton.OverrideSize = EditorGUILayout.Toggle("Text size", textButton.OverrideSize);
            if (textButton.OverrideSize)
            {
                textButton.Size = EditorGUILayout.IntField(textButton.Size);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            textButton.OverrideOffset = EditorGUILayout.Toggle("Offset", textButton.OverrideOffset);
            if (textButton.OverrideOffset)
            {
                EditorGUILayout.LabelField("(You may now manually adjust the offset of the text)", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndHorizontal();

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Button text");

            textButton.Text = EditorGUILayout.TextArea(textButton.Text, textAreaLabel);

            HUXEditorUtils.EndSectionBox();
            //textButton.Alpha = EditorGUILayout.Slider("Text transparency", textButton.Alpha, 0f, 1f);

            HUXEditorUtils.DrawProfileInspector(textButton.TextProfile, textButton);

            HUXEditorUtils.SaveChanges(target);
        }
    }
}
