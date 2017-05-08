//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HUX
{
    public static class EditorTools
    {
        public static T DrawProfileField<T> (T profile) where T : UnityEngine.ScriptableObject
        {
            Color prevColor = GUI.color;
            GUI.color = Color.Lerp(Color.white, Color.gray, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.white;
            EditorGUILayout.LabelField("Select a " + typeof(T).Name + " or create a new profile", EditorStyles.miniBoldLabel);
            T newProfile = profile;
            EditorGUILayout.BeginHorizontal();
            newProfile = (T)EditorGUILayout.ObjectField(profile, typeof(T), false);
            if (GUILayout.Button("Create new profile"))
            {
                T asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, "Assets/New" + typeof(T).Name + ".asset");
                AssetDatabase.SaveAssets();
                newProfile = asset;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.color = prevColor;
            return newProfile;
        }

        public static T DropDownComponentField<T>(string label, T obj, Transform transform) where T : UnityEngine.Component
        {
            T[] optionObjects = transform.GetComponentsInChildren<T>(true);
            int selectedIndex = 0;
            string[] options = new string[optionObjects.Length + 1];
            options[0] = "(None)";
            for (int i = 0; i < optionObjects.Length; i++)
            {
                options[i + 1] = optionObjects[i].name;
                if (obj == optionObjects[i])
                {
                    selectedIndex = i + 1;
                }
            }

            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.Popup(label, selectedIndex, options);
            if (newIndex == 0)
            {
                // Zero means '(None)'
                obj = null;
            } else
            {
                obj = optionObjects[newIndex - 1];
            }

            //draw the object field so people can click it
            obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), true);
            EditorGUILayout.EndHorizontal();

            return obj;
        }

        public static void SaveChanges (UnityEngine.Object target)
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        public static void SaveChanges(UnityEngine.Object target1, UnityEngine.Object target2)
        {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target1);
                EditorUtility.SetDirty(target2);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }
    }
}