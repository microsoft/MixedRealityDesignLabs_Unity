//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
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

            HUXEditorUtils.BeginSectionBox("Icon settings");
            iconButton.IconRenderer = HUXEditorUtils.DropDownComponentField<MeshRenderer>("Icon renderer", iconButton.IconRenderer, iconButton.transform);

            if (iconButton.IconRenderer == null)
            {
                HUXEditorUtils.ErrorMessage("You must specify an icon renderer", null);
                HUXEditorUtils.EndSectionBox();
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            if (iconButton.IconProfile.IconMaterial == null)
            {
                HUXEditorUtils.ErrorMessage("You must specify an icon material in the profile", null);
                HUXEditorUtils.EndSectionBox();
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            if (iconButton.IconProfile.IconMesh == null)
            {
                HUXEditorUtils.ErrorMessage("You must specify an icon mesh in the profile", null);
                HUXEditorUtils.EndSectionBox();
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            // Icon profiles provide their own fields for the icon name
            iconButton.Alpha = EditorGUILayout.Slider("Icon transparency", iconButton.Alpha, 0f, 1f);

            iconButton.IconName = iconButton.IconProfile.DrawIconSelectField(iconButton.IconName);

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.DrawProfileInspector(iconButton.IconProfile, iconButton);

            // Check to see if the icon is valid - if it's not, show the placeholder
            /*Texture2D icon = iconButton.IconRenderer.sharedMaterial.mainTexture as Texture2D;
            if (icon == null || icon == iconButton.IconProfile._IconNotFound_)
            {
                HUXEditorUtils.WarningMessage(
                    "Texture '" + iconButton.IconName + "' was not found in the selected profile. A substitute will be displayed until an icon file has been added to the profile.",
                    "Click to open profile",
                    ClickToOpen);
            }*/

            HUXEditorUtils.SaveChanges(iconButton, iconButton.IconProfile);
        }

        void ClickToOpen()
        {
            CompoundButtonIcon iconButton = (CompoundButtonIcon)target;
            UnityEditor.Selection.activeObject = iconButton.IconProfile;
        }
    }
}
