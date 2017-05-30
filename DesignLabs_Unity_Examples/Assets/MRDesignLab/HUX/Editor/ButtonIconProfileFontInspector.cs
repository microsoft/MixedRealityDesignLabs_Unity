//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(ButtonIconProfileFont))]
    public class ButtonIconProfileFontInspector : ProfileInspector
    {
        private GUIStyle testStringStyle;

        public override void OnInspectorGUI()
        {
            ButtonIconProfileFont iconProfile = (ButtonIconProfileFont)target;
            //CompoundButtonIcon iconButton = (CompoundButtonIcon)targetComponent;

            HUXEditorUtils.BeginProfileBox();

            HUXEditorUtils.BeginSectionBox(ButtonIconProfileFont.DefaultUnicodeFont + " font asset");
            EditorGUILayout.BeginHorizontal();
            Font font = (Font)EditorGUILayout.ObjectField(iconProfile.IconFont, typeof(Font), false);
            EditorGUILayout.EndHorizontal();
            if (font == null)
            {
                HUXEditorUtils.ErrorMessage(
                    "You must assign the '" + ButtonIconProfileFont.DefaultUnicodeFont + "' font for this profile to work. (If the font is installed, clicking 'Auto-assign' will find it for you.)",
                    SearchForFont,
                    "Auto-assign '" + ButtonIconProfileFont.DefaultUnicodeFont + "' font");
                if (GUILayout.Button("Download '" + ButtonIconProfileFont.DefaultUnicodeFont + "' font"))
                {
                    Application.OpenURL(StartupChecks.RequiredFontURL);
                }
            }
            else
            {
                bool correctFontName = false;
                foreach (string fontName in font.fontNames)
                {
                    if (fontName.Equals(ButtonIconProfileFont.DefaultUnicodeFont)) {
                        correctFontName = true;
                        break;
                    }
                }
                if (correctFontName)
                {
                    // We've got the right font, now check its properties
                    iconProfile.IconFont = font;
                    if (!font.dynamic)
                    {
                        HUXEditorUtils.ErrorMessage("Font character mode must be set to 'Dynamic'", SelectFontAsset, "Click to open font asset");
                    }
                } else
                {
                    // This is the wrong font, don't use it
                    iconProfile.IconFont = null;
                }
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Material & mesh properties");
            iconProfile._IconNotFound_ = (Texture2D)EditorGUILayout.ObjectField("Icon not found texture", iconProfile._IconNotFound_, typeof(Texture2D), false, GUILayout.MaxHeight (35f), GUILayout.MaxHeight (35f));
            iconProfile.IconMesh = (Mesh)EditorGUILayout.ObjectField("Icon mesh", iconProfile.IconMesh, typeof(Mesh), false);
            iconProfile.IconMaterial = (Material)EditorGUILayout.ObjectField("Icon material", iconProfile.IconMaterial, typeof(Material), false);
            iconProfile.AlphaColorProperty = HUXEditorUtils.MaterialPropertyName(iconProfile.AlphaColorProperty, iconProfile.IconMaterial, ShaderUtil.ShaderPropertyType.Color, false);
            iconProfile.AlphaTransitionSpeed = EditorGUILayout.Slider("Alpha Transition Speed", iconProfile.AlphaTransitionSpeed, 0f, 2f);
            iconProfile.FontScaleFactor = EditorGUILayout.IntField("Font scale factor", iconProfile.FontScaleFactor);
            iconProfile.RendererScale = EditorGUILayout.FloatField("Renderer scale", iconProfile.RendererScale);
            HUXEditorUtils.EndSectionBox();

            #region font selection

            //iconProfile.FontSize = EditorGUILayout.IntField("Size", Mathf.Clamp (iconProfile.FontSize, 10, 300));
            /*string[] osInstalledFontNames = Font.GetOSInstalledFontNames();
            int huxIconFontNameIndex = -1;
            for (int i = 0; i < osInstalledFontNames.Length; i++)
            {
                if (osInstalledFontNames[i] == ButtonIconProfileFont.DefaultUnicodeFont)
                {
                    huxIconFontNameIndex = i;
                }
            }
            
            if (huxIconFontNameIndex < 0)
            {
                HUXEditorUtils.ErrorMessage("Couldn't find font '" + ButtonIconProfileFont.DefaultUnicodeFont + "'", null);
            }
            else if (GUILayout.Button("Click to select '" + ButtonIconProfileFont.DefaultUnicodeFont + "'"))
            {
                iconProfile.OSFontName = osInstalledFontNames[huxIconFontNameIndex];
            }

            HUXEditorUtils.EndSectionBox();*/

            #endregion

            HUXEditorUtils.EndProfileBox();

            HUXEditorUtils.SaveChanges(this);
        }

        private void SelectFontAsset()
        {
            Selection.activeObject = ((ButtonIconProfileFont)target).IconFont;
        }

        private void SearchForFont()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:font", new string[] { "Assets" }))
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Font font = (Font)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Font));
                if (font != null)
                {
                    foreach (string fontName in font.fontNames)
                    {
                        if (fontName.Equals(HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont))
                        {
                            ((ButtonIconProfileFont)target).IconFont = font;
                            return;
                        }
                    }
                }
            }

            // If we've gotten this far we didn't find the font
            Debug.LogWarning("Couldn't find font in asset database.");
        }
    }
}