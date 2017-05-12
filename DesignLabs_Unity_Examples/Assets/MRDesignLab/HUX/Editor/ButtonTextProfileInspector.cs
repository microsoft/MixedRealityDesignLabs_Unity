//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(ButtonTextProfile))]
    public class ButtonTextProfileInspector : ProfileInspector
    {
        public override void OnInspectorGUI()
        {
            ButtonTextProfile textProfile = (ButtonTextProfile)target;
            CompoundButtonText textButton = (CompoundButtonText)targetComponent;

            HUXEditorUtils.BeginProfileBox();

            HUXEditorUtils.BeginSectionBox("Text properties");

            textProfile.Font = (Font)EditorGUILayout.ObjectField("Font", textProfile.Font, typeof(Font), false);
            textProfile.Alignment = (TextAlignment)EditorGUILayout.EnumPopup("Alignment", textProfile.Alignment);
            if (textButton == null || !textButton.OverrideAnchor)
            {
                textProfile.Anchor = (TextAnchor)EditorGUILayout.EnumPopup("Anchor", textProfile.Anchor);
            }
            if (textButton == null || !textButton.OverrideFontStyle)
            {
                textProfile.Style = (FontStyle)EditorGUILayout.EnumPopup("Style", textProfile.Style);
            }
            if (textButton == null || !textButton.OverrideSize)
            {
                textProfile.Size = EditorGUILayout.IntField("Size", textProfile.Size);
            }
            textProfile.Color = EditorGUILayout.ColorField(textProfile.Color);

            HUXEditorUtils.EndSectionBox();


            HUXEditorUtils.BeginSectionBox("Text Offset (based on anchor setting)");
            if (textButton == null || !textButton.OverrideOffset)
            {
                switch (textProfile.Anchor)
                {
                    case TextAnchor.LowerCenter:
                        textProfile.AnchorLowerCenterOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerCenterOffset);
                        break;

                    case TextAnchor.LowerLeft:
                        textProfile.AnchorLowerLeftOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerLeftOffset);
                        break;

                    case TextAnchor.LowerRight:
                        textProfile.AnchorLowerRightOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorLowerRightOffset);
                        break;

                    case TextAnchor.MiddleCenter:
                        textProfile.AnchorMiddleCenterOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleCenterOffset);
                        break;

                    case TextAnchor.MiddleLeft:
                        textProfile.AnchorMiddleLeftOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleLeftOffset);
                        break;

                    case TextAnchor.MiddleRight:
                        textProfile.AnchorMiddleRightOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorMiddleRightOffset);
                        break;

                    case TextAnchor.UpperCenter:
                        textProfile.AnchorUpperCenterOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperCenterOffset);
                        break;

                    case TextAnchor.UpperLeft:
                        textProfile.AnchorUpperLeftOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperLeftOffset);
                        break;

                    case TextAnchor.UpperRight:
                        textProfile.AnchorUpperRightOffset = EditorGUILayout.Vector3Field("Anchor (" + textProfile.Anchor.ToString() + ")", textProfile.AnchorUpperRightOffset);
                        break;
                }
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.EndProfileBox();

            HUXEditorUtils.SaveChanges(this);
        }
    }
}