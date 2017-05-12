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
        HUXEditorUtils.WarningMessage("Fonts are missing from this project");

        EditorGUILayout.LabelField("In order for the icon components included in this package to function correctly, you will need to download and install the '" + HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont + "' icon font.", EditorStyles.wordWrappedLabel);
        EditorGUILayout.LabelField("After extracting the font import it as an asset and assign it in the 'DefaultButtonIconProfileFont' button profile.", EditorStyles.wordWrappedLabel);
        if (GUILayout.Button ("Click here to download font"))
        {
            Application.OpenURL(StartupChecks.RequiredFontURL);
        }
        if (GUILayout.Button ("Click here to view button profile"))
        {
            Object profile = AssetDatabase.LoadAssetAtPath(
                "Assets/MRDesignLab/HUX/Resources/Profiles/DefaultButtonIconProfileFont.asset",
                typeof(HUX.Buttons.ButtonIconProfileFont));
            Selection.activeObject = profile;
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
