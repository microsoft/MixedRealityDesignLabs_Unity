//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class StartupChecks
{

    public static bool FoundRequiredFonts = false;
    public static bool FoundRequiredAxis = false;
    public static HashSet<string> MissingAxis = new HashSet<string>();
    public static string EditorPrefsKey = "HUX_ShowStartupChecksWindow";
    public static string RequiredInputSettingsURL = @"https://github.com/Microsoft/MRDesignLabs_Unity/raw/master/DesignLabs_Unity/ProjectSettings/InputManager.asset";
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

    public static void ForceCheck()
    {
        DoCheck();
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

        DoCheck();

    }

    static void DoCheck()
    {
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
            Font font = (Font)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Font));
            if (font != null)
            {
                foreach (string fontName in font.fontNames)
                {
                    if (fontName.Equals(HUX.Buttons.ButtonIconProfileFont.DefaultUnicodeFont))
                    {
                        FoundRequiredFonts = true;
                        break;
                    }
                }
            }

            if (FoundRequiredFonts)
                break;
        }

        FoundRequiredAxis = false;

        MissingAxis = new HashSet<string> {
            InputSourceUnityGamepad.AxisDpadH,
            InputSourceUnityGamepad.AxisDpadV,
            InputSourceUnityGamepad.AxisLeftStickH,
            InputSourceUnityGamepad.AxisLeftStickV,
            InputSourceUnityGamepad.AxisRightStickH,
            InputSourceUnityGamepad.AxisRightStickV,
            InputSourceUnityGamepad.ButtonA,
            InputSourceUnityGamepad.ButtonB,
            InputSourceUnityGamepad.ButtonStart,
            InputSourceUnityGamepad.ButtonX,
            InputSourceUnityGamepad.ButtonY,
            InputSourceUnityGamepad.TriggerLeft,
            InputSourceUnityGamepad.TriggerRight,
            InputSourceUnityGamepad.TriggerShared,
        };

        var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        SerializedObject obj = new SerializedObject(inputManager);
        SerializedProperty axisArray = obj.FindProperty("m_Axes");

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            var axis = axisArray.GetArrayElementAtIndex(i);
            var name = axis.FindPropertyRelative("m_Name").stringValue;
            MissingAxis.Remove(name);
        }

        if (MissingAxis.Count == 0)
        {
            FoundRequiredAxis = true;
        }
        else
        {
            FoundRequiredAxis = false;
            foreach (string axisName in MissingAxis)
            {
                Debug.LogWarning("Didn't find axis " + axisName + " in Input Manager. Some HUX features will not work without these axis.");
            }
        }

        if (!FoundRequiredFonts || !FoundRequiredAxis)
        {
            shownOnceThisSession = true;
            EditorWindow window = EditorWindow.GetWindow<StartupChecksWindow>(false, "Startup Check", true);
            window.minSize = new Vector2(425, 450);
        }

    }
}
