//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class StartupChecks
{

    public static bool FoundRequiredFonts = false;
    public static string EditorPrefsKey = "HUX_ShowStartupChecksWindow";
    public static string RequiredFontURL = @"http://download.microsoft.com/download/3/8/D/38D659E2-4B9C-413A-B2E7-1956181DC427/Hololens font.zip";
    static bool shownOnceThisSession = false;
    static float timeLaunched = 0f;
    static float launchDelay = 5f;
    static float checkInterval = 60f;
    static float lastCheck = 0f;
    static bool overridePrefs = false;

    static StartupChecks()
    {
        if (!overridePrefs)
        {
            if (!EditorPrefs.HasKey(EditorPrefsKey))
            {
                EditorPrefs.SetBool(EditorPrefsKey, true);
            }
            else if (!EditorPrefs.GetBool(EditorPrefsKey))
            {
                return;
            }
        }

        EditorApplication.update += Update;
        timeLaunched = Time.realtimeSinceStartup;
        lastCheck = -checkInterval;
    }

    static void Update()
    {
        if (shownOnceThisSession)
            return;

        if (Time.realtimeSinceStartup < timeLaunched + launchDelay)
            return;

        if (Time.realtimeSinceStartup < lastCheck + checkInterval)
            return;

        if (Application.isPlaying)
            return;

        lastCheck = Time.realtimeSinceStartup;
        FoundRequiredFonts = false;

        /*string[] osInstalledFonts = Font.GetOSInstalledFontNames();
        foreach (string fontName in osInstalledFonts)
        {
            if (fontName.ToLower().Equals (HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont.ToLower()))
            {
                FoundRequiredFonts = true;
                break;
            }
        }*/
        
        foreach (string guid in AssetDatabase.FindAssets("t:font", new string[] { "Assets" }))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            Font font = (Font)AssetDatabase.LoadAssetAtPath(assetPath, typeof (Font));
            if (font != null)
            {
                foreach (string fontName in font.fontNames)
                {
                    if (fontName.Equals (HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont))
                    {
                        FoundRequiredFonts = true;
                        break;
                    }
                }
            }

            if (FoundRequiredFonts)
                break;
        }

        if (!FoundRequiredFonts)
        {
            shownOnceThisSession = true;
            EditorWindow.GetWindow<StartupChecksWindow>(false, "Startup Check", true);
        }
    }
}
