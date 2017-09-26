//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using HUX.Interaction;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUX
{
    /// <summary>
    /// A custom string tag that can be used with the FilterTags class to enabled/disable
    /// raycasting on an object for the Focus Manager.
    /// Add a custom tag or select a pre-existing tag from the dropdown. The Focus Manger
    /// will now show your new tag allowing you to Enable/Disable raycasting on any filter with that tag.
    /// </summary>
    [System.Serializable]
    public class FilterTag
    {
        public const string DefaultTag = "Default";
        public string Tag = DefaultTag;
    }

    /// <summary>
    /// This class keeps a list of all valid string tags. It is used by the Focus Manger.
    /// In editor - In searchs the scene and prefab assets for all FilterTags and displays
    /// them as a Bit Mask.
    /// </summary>
    [System.Serializable]
    public class FilterTags
    {
        public List<string> ValidTags = new List<string>();

        public void AddTag(string tag)
        {
            if (!ValidTags.Contains(tag))
            {
                ValidTags.Add(tag);
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(FilterTags))]
    public class FilterTagsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> validTags = new List<string>();
            List<string> layerTags = new List<string>();
            layerTags.Add(FilterTag.DefaultTag);

            // Find all focus layers in scene
            InteractibleObject[] interactibleObjects = FilterTagDrawer.GetAllInteractibleObjects();

            // Get all layer tags from interactible objects
            for (int i = 0; i < interactibleObjects.Length; ++i)
            {
                InteractibleObject interactibleObject = interactibleObjects[i];
                if (interactibleObject != null && interactibleObject.FilterTag != null)
                {
                    string tag = interactibleObject.FilterTag.Tag;

                    if (!string.IsNullOrEmpty(tag) && !layerTags.Contains(tag))
                    {
                        layerTags.Add(tag);
                    }


                    // Get all layer tags from other components on interactible objects
                    Component[] accompanyingComponents = interactibleObject.GetComponents<Component>();
                    for (int j = 0; j < accompanyingComponents.Length; j++)
                    {
                        if (accompanyingComponents[j] == interactibleObject)
                            continue;

                        foreach (FieldInfo field in accompanyingComponents[j].GetType().GetFields())
                        {
                            if (field.IsPublic && field.FieldType == typeof(FilterTag))
                            {
                                FilterTag fieldFilterTag = (FilterTag)field.GetValue(accompanyingComponents[j]);
                                if (fieldFilterTag != null && !string.IsNullOrEmpty(fieldFilterTag.Tag))
                                {
                                    layerTags.Add(fieldFilterTag.Tag);
                                }
                            }
                        }
                    }
                }
            }

            // Sort alphabetically
            layerTags.Sort();

            EditorGUI.BeginProperty(position, label, property);
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Get the valid tags property
            SerializedProperty currentValidTagsProperty = property.FindPropertyRelative("ValidTags");

            // Add the current tags to the list
            for (int i = 0; i < currentValidTagsProperty.arraySize; ++i)
            {
                validTags.Add(currentValidTagsProperty.GetArrayElementAtIndex(i).stringValue);
            }

            // Get the current selected mask from the tags
            int mask = 0;
            for (int i = 0; i < layerTags.Count; ++i)
            {
                if (validTags.Contains(layerTags[i]))
                {
                    mask |= 1 << i;
                }
            }

            // Draw the mask feild
            Rect rect = new Rect(position.x, position.y, 150, 18);
            mask = EditorGUI.MaskField(rect, mask, layerTags.ToArray());

            // Get the new valid tags - the mask has changed
            validTags.Clear();
            for (int i = 0; i < layerTags.Count; ++i)
            {
                if ((mask & 1 << i) != 0)
                {
                    validTags.Add(layerTags[i]);
                }
            }

            // Set the tags
            currentValidTagsProperty.ClearArray();
            currentValidTagsProperty.arraySize = validTags.Count;

            for (int i = 0; i < currentValidTagsProperty.arraySize; ++i)
            {
                currentValidTagsProperty.GetArrayElementAtIndex(i).stringValue = validTags[i];
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }

    [CustomPropertyDrawer(typeof(FilterTag))]
    public class FilterTagDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40.0f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> layerTags = new List<string>();
            layerTags.Add(FilterTag.DefaultTag);

            // Find all focus layers in scene
            InteractibleObject[] interactibleObjects = GetAllInteractibleObjects();

            // Get all layer tags
            for (int i = 0; i < interactibleObjects.Length; ++i)
            {
                InteractibleObject interactibleObject = interactibleObjects[i];
                if (interactibleObject != null && interactibleObject.FilterTag != null)
                {
                    string tag = interactibleObject.FilterTag.Tag;

                    if (!string.IsNullOrEmpty(tag) && !layerTags.Contains(tag))
                    {
                        layerTags.Add(tag);
                    }

                    // Get all layer tags from other components on interactible objects
                    Component[] accompanyingComponents = interactibleObject.GetComponents<Component>();
                    for (int j = 0; j < accompanyingComponents.Length; j++)
                    {
                        if (accompanyingComponents[j] == interactibleObject)
                            continue;

                        foreach (FieldInfo field in accompanyingComponents[j].GetType().GetFields())
                        {
                            if (field.IsPublic && field.FieldType == typeof(FilterTag))
                            {
                                FilterTag fieldFilterTag = (FilterTag)field.GetValue(accompanyingComponents[j]);
                                if (fieldFilterTag != null && !string.IsNullOrEmpty(fieldFilterTag.Tag))
                                {
                                    layerTags.Add(fieldFilterTag.Tag);
                                }
                            }
                        }
                    }
                }
            }

            // Sort alphabetically
            layerTags.Sort();

            EditorGUI.BeginProperty(position, label, property);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Current layer tag
            SerializedProperty currentLayerProperty = property.FindPropertyRelative("Tag");
            int tagIndex = layerTags.IndexOf(currentLayerProperty.stringValue);

            // Draw popup with all current tags in scene
            Rect rect = new Rect(position.x, position.y, 150, 18);
            tagIndex = EditorGUI.Popup(rect, tagIndex, layerTags.ToArray());

            // Set tag value
            if (tagIndex >= 0 && tagIndex < layerTags.Count)
            {
                currentLayerProperty.stringValue = layerTags.ElementAt(tagIndex);
            }
            else
            {
                currentLayerProperty.stringValue = FilterTag.DefaultTag;
            }

            // Draw tag
            rect = new Rect(position.x, position.y + 20, 150, 17);
            currentLayerProperty.stringValue = EditorGUI.TextField(rect, currentLayerProperty.stringValue);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public static InteractibleObject[] GetAllInteractibleObjects()
        {
            List<InteractibleObject> results = new List<InteractibleObject>();

            // Find in scene
            object[] interactibleSceneObjects = GameObject.FindObjectsOfType(typeof(InteractibleObject));
            for (int i = 0; i < interactibleSceneObjects.Length; ++i)
            {
                results.Add((InteractibleObject)interactibleSceneObjects[i]);
            }

            // Find in assets
            string[] allPrefabPaths = GetAllPrefabPaths();
            for (int i = 0; i < allPrefabPaths.Length; ++i)
            {
                GameObject gameObject = AssetDatabase.LoadMainAssetAtPath(allPrefabPaths[i]) as GameObject;
                if (gameObject != null)
                {
                    InteractibleObject[] interactiblePrefabObjects = gameObject.GetComponents<InteractibleObject>();

                    if (interactiblePrefabObjects != null)
                    {
                        for (int j = 0; j < interactiblePrefabObjects.Length; ++j)
                        {
                            results.Add(interactiblePrefabObjects[j]);
                        }
                    }
                }
            }

            return results.ToArray();
        }

        private static string[] GetAllPrefabPaths()
        {
            List<string> results = new List<string>();
            string[] assetPaths = AssetDatabase.GetAllAssetPaths();

            foreach (string path in assetPaths)
            {
                if (path.Contains(".prefab")) { results.Add(path); }
            }
            return results.ToArray();
        }
    }
#endif
}