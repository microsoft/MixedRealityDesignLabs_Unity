//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(ButtonIconProfileTexture))]
    public class ButtonIconProfileTextureInspector : ProfileInspector
    {
        static bool showTextures = false;
        static float textureSize = 50f;
        public override void OnInspectorGUI()
        {
            ButtonIconProfileTexture iconProfile = (ButtonIconProfileTexture)target;
            CompoundButtonIcon iconButton = (CompoundButtonIcon)targetComponent;

            HUXEditorUtils.BeginProfileBox();
            
            HUXEditorUtils.BeginSectionBox("Material & mesh properties");
            iconProfile._IconNotFound_ = (Texture2D)EditorGUILayout.ObjectField("Icon not found texture", iconProfile._IconNotFound_, typeof(Texture2D), false, GUILayout.MaxHeight(35f), GUILayout.MaxHeight(35f));
            iconProfile.IconMesh = (Mesh)EditorGUILayout.ObjectField("Icon mesh", iconProfile.IconMesh, typeof(Mesh), false);
            iconProfile.IconMaterial = (Material)EditorGUILayout.ObjectField("Icon material", iconProfile.IconMaterial, typeof(Material), false);
            iconProfile.AlphaColorProperty = HUXEditorUtils.MaterialPropertyName(iconProfile.AlphaColorProperty, iconProfile.IconMaterial, ShaderUtil.ShaderPropertyType.Color, false);
            iconProfile.AlphaTransitionSpeed = EditorGUILayout.Slider("Alpha Transition Speed", iconProfile.AlphaTransitionSpeed, 0f, 2f);
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Textures defined in profile");
            if (iconButton == null || showTextures)
            {
                var properties = iconProfile.GetType().GetFields();
                foreach (var property in properties)
                {
                    if (property.FieldType == typeof(Texture2D) && !property.Name.StartsWith("_"))
                    {
                        Texture2D iconVal = (Texture2D)property.GetValue(iconProfile);
                        iconVal = (Texture2D)EditorGUILayout.ObjectField(property.Name, iconVal, typeof(Texture2D), false, GUILayout.MaxHeight(textureSize));
                        property.SetValue(iconProfile, iconVal);
                    }
                }
                HUXEditorUtils.EndSectionBox();

                HUXEditorUtils.BeginSectionBox("Custom texture array");
                if (GUILayout.Button("Add custom icon"))
                {
                    System.Array.Resize<Texture2D>(ref iconProfile.CustomIcons, iconProfile.CustomIcons.Length + 1);
                }
                for (int i = 0; i < iconProfile.CustomIcons.Length; i++)
                {
                    Texture2D icon = iconProfile.CustomIcons[i];
                    icon = (Texture2D)EditorGUILayout.ObjectField(icon != null ? icon.name : "(Empty)", icon, typeof(Texture2D), false, GUILayout.MaxHeight(textureSize));
                    iconProfile.CustomIcons[i] = icon;
                }
                if (iconButton != null)
                {
                    if (GUILayout.Button("Hide icon textures"))
                    {
                        showTextures = false;
                    }
                }
            } else
            {
                if (GUILayout.Button ("Show icon textures"))
                {
                    showTextures = true;
                }
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.EndProfileBox();

            HUXEditorUtils.SaveChanges(this);
        }
    }
}