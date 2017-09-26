//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX;
using UnityEditor;
using UnityEngine;

public class StartupChecksWindow : EditorWindow {

    bool dontShowAgain = false;

    void OnGUI ()
    {
        if (StartupChecks.FoundRequiredFonts && StartupChecks.FoundRequiredAxis)
        {
            HUXEditorUtils.Header("Everything found!");
            return;
        }

        if (!StartupChecks.FoundRequiredFonts) {
            HUXEditorUtils.Header("Fonts Missing");
            HUXEditorUtils.WarningMessage("Fonts are missing from this project");

            EditorGUILayout.LabelField("In order for the icon components included in this package to function correctly, you will need to download and install the '" + HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont + "' icon font.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField("After extracting the font import it as an asset and assign it in the 'DefaultButtonIconProfileFont' button profile.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Click here to download font")) {
                Application.OpenURL(StartupChecks.RequiredFontURL);
            }
            if (GUILayout.Button("Click here to view button profile")) {
                Object profile = AssetDatabase.LoadAssetAtPath(
                    "Assets/MRDesignLab/HUX/Resources/Profiles/DefaultButtonIconProfileFont.asset",
                    typeof(HUX.Buttons.ButtonIconProfileFont));
                Selection.activeObject = profile;
            }
        }

        EditorGUILayout.LabelField(" ");

        if (!StartupChecks.FoundRequiredAxis) {
            HUXEditorUtils.Header("Input Axis Missing");
            HUXEditorUtils.WarningMessage("Input axis are missing from this project");
            EditorGUILayout.LabelField("In order for certain input scripts in this package to function correctly, you will need to set up the following axis in Unity's input manager:", EditorStyles.wordWrappedLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (string missingAxis in StartupChecks.MissingAxis) {
                EditorGUILayout.LabelField(missingAxis, EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("You can also replace your InputManager.asset file with the one provided in the DesignLabs_Unity repository.", EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Click here to download InputManager.asset file")) {
                Application.OpenURL(StartupChecks.RequiredInputSettingsURL);
            }
        }
        dontShowAgain = EditorGUILayout.Toggle("Don't show again", dontShowAgain);
        
    }

    void OnDestroy ()
    {
        if (dontShowAgain)
        {
            EditorPrefs.SetBool(StartupChecks.EditorPrefsKey, false);
        }
    }
}
