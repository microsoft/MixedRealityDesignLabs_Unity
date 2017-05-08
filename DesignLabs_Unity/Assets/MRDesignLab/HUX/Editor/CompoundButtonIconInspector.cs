//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(CompoundButtonIcon))]
    public class CompoundButtonIconInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButtonIcon iconButton = (CompoundButtonIcon)target;

            iconButton.DisableIcon = EditorGUILayout.Toggle("Disable icon", iconButton.DisableIcon);
            if (iconButton.DisableIcon)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            iconButton.IconProfile = HUXEditorUtils.DrawProfileField<ButtonIconProfile>(iconButton.IconProfile);

            if (iconButton.IconProfile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            iconButton.IconRenderer = HUXEditorUtils.DropDownComponentField<MeshRenderer>("Icon renderer", iconButton.IconRenderer, iconButton.transform);
            //iconButton.IconRenderer = (MeshRenderer)EditorGUILayout.ObjectField("Icon renderer", iconButton.IconRenderer, typeof(MeshRenderer), true);

            if (iconButton.IconRenderer == null)
            {
                GUI.color = HUXEditorUtils.ErrorColor;
                EditorGUILayout.LabelField("You must specify an icon renderer");
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            iconButton.IconMaterial = (Material)EditorGUILayout.ObjectField("Icon material", iconButton.IconMaterial, typeof(Material), true);

            if (iconButton.IconMaterial == null)
            {
                iconButton.IconMaterial = iconButton.IconRenderer.sharedMaterial;
                if (iconButton.IconMaterial == null)
                {
                    GUI.color = HUXEditorUtils.ErrorColor;
                    EditorGUILayout.LabelField("You must specify an icon material");
                    HUXEditorUtils.SaveChanges(target);
                    return;
                }
            }

            int selectedIconIndex = 0;
            List<string> iconKeys = iconButton.IconProfile.GetIconKeys();
            iconKeys.Insert(0, "(None)");
            for (int i = 0; i < iconKeys.Count; i++)
            {
                if (iconButton.IconName == iconKeys[i])
                {
                    selectedIconIndex = i;
                    break;
                }
            }
            int newIconIndex = EditorGUILayout.Popup("Icon", selectedIconIndex, iconKeys.ToArray());
            // This will automatically set the icon in the editor view
            iconButton.IconName = (newIconIndex == 0 ? string.Empty : iconKeys[newIconIndex]);

            iconButton.Alpha = EditorGUILayout.Slider("Icon transparency", iconButton.Alpha, 0f, 1f);

            // Check to see if the icon is valid - if it's not, show the placeholder
            Texture2D icon = null;
            if (!iconButton.IconProfile.GetIcon (iconButton.IconName, out icon, false))
            {
                HUXEditorUtils.WarningMessage(
                    "Texture '" + iconButton.IconName + "' was not found in the selected profile. A substitute will be displayed until an icon file has been added to the profile.",
                    "Click to open profile",
                    ClickToOpen);
            }

            HUXEditorUtils.SaveChanges(target);
        }

        void ClickToOpen()
        {
            CompoundButtonIcon iconButton = (CompoundButtonIcon)target;
            UnityEditor.Selection.activeObject = iconButton.IconProfile;
        }
    }
}
