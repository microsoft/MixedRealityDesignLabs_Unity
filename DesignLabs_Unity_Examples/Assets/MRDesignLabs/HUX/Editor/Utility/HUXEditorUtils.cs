//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    public class HUXEditorUtils
    {
        public readonly static Color DefaultColor = new Color(1f, 1f, 1f);
        public readonly static Color DisabledColor = new Color(0.6f, 0.6f, 0.6f);
        public readonly static Color BorderedColor = new Color(0.8f, 0.8f, 0.8f);
        public readonly static Color WarningColor = new Color(1f, 0.85f, 0.6f);
        public readonly static Color ErrorColor = new Color(1f, 0.55f, 0.5f);
        public readonly static Color SuccessColor = new Color(0.8f, 1f, 0.75f);
        public readonly static Color ObjectColor = new Color(0.85f, 0.9f, 1f);
        public readonly static Color HelpBoxColor = new Color(0.22f, 0.23f, 0.24f, 0.45f);
        public readonly static Color SectionColor = new Color(0.42f, 0.43f, 0.47f, 0.25f);
        public readonly static Color DarkColor = new Color(0.1f, 0.1f, 0.1f);
        public readonly static Color ObjectColorEmpty = new Color(0.75f, 0.8f, 0.9f);

        /// <summary>
        /// Draws a field for scriptable object profiles
        /// If base class T is abstract, includes a button for creating a profile of each type that inherits from base class T
        /// Otherwise just includes button for creating a profile of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static T DrawProfileField<T>(T profile) where T : ButtonProfile
        {
            Color prevColor = GUI.color;
            GUI.color = Color.Lerp(Color.white, Color.gray, 0.5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.Lerp(Color.white, Color.gray, 0.25f);
            EditorGUILayout.LabelField("Select a " + typeof(T).Name + " or create a new profile", EditorStyles.miniBoldLabel);
            T newProfile = profile;
            EditorGUILayout.BeginHorizontal();
            newProfile = (T)EditorGUILayout.ObjectField(profile, typeof(T), false);
            // is this an abstract class? 
            if (typeof(T).IsAbstract)
            {
                EditorGUILayout.BeginVertical();
                List<Type> types = GetDerivedTypes(typeof(T), Assembly.GetAssembly(typeof(T)));

                foreach (Type profileType in types)
                {
                    if (GUILayout.Button("Create " + profileType.Name))
                    {
                        profile = CreateProfile<T>(profileType);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                if (GUILayout.Button("Create Profile"))
                {
                    profile = CreateProfile<T>();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (profile == null)
            {
                ErrorMessage("You must choose a button profile.", null);
            }

            GUI.color = prevColor;
            return newProfile;
        }

        public static T CreateProfile<T>(Type profileType) where T : ButtonProfile
        {
            T asset = (T)ScriptableObject.CreateInstance(profileType);
            if (asset != null)
            {
                AssetDatabase.CreateAsset(asset, "Assets/New" + profileType.Name + ".asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("Couldn't create profile of type " + profileType.Name);
            }
            return asset;
        }

        public static T CreateProfile<T>() where T : ButtonProfile
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, "Assets/New" + typeof(T).Name + ".asset");
            AssetDatabase.SaveAssets();
            return asset;
        }

        public static void DrawFilterTagField(SerializedObject serializedObject, string propertyName)
        {
            SerializedProperty p = serializedObject.FindProperty(propertyName);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(p);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        public static void DrawProfileInspector(ButtonProfile profile, Component targetComponent)
        {
            ProfileInspector profileEditor = (ProfileInspector)Editor.CreateEditor(profile);
            profileEditor.targetComponent = targetComponent;

            profileEditor.OnInspectorGUI();
        }

        public static T DropDownComponentField<T>(string label, T obj, Transform transform, bool showComponentName = false) where T : UnityEngine.Component
        {
            T[] optionObjects = transform.GetComponentsInChildren<T>(true);
            int selectedIndex = 0;
            string[] options = new string[optionObjects.Length + 1];
            options[0] = "(None)";
            for (int i = 0; i < optionObjects.Length; i++)
            {
                if (showComponentName)
                {
                    options[i + 1] = optionObjects[i].GetType().Name + " (" + optionObjects[i].name + ")";
                }
                else
                {
                    options[i + 1] = optionObjects[i].name;
                }
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
            }
            else
            {
                obj = optionObjects[newIndex - 1];
            }

            //draw the object field so people can click it
            obj = (T)EditorGUILayout.ObjectField(obj, typeof(T), true);
            EditorGUILayout.EndHorizontal();

            return obj;
        }


        /// <summary>
        /// Draws enum values as a set of toggle fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="enumObj"></param>
        /// <returns></returns>
        public static int EnumCheckboxField<T>(string label, T enumObj) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum.");
            }
            return EnumCheckboxField<T>(label, enumObj, string.Empty, (T)Activator.CreateInstance(typeof(T)));
        }

        public static T SceneObjectField<T>(string label, T sceneObject) where T : Component {

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(label)) {
                sceneObject = (T)EditorGUILayout.ObjectField(sceneObject, typeof(T), true);
            } else {
                sceneObject = (T)EditorGUILayout.ObjectField(label, sceneObject, typeof(T), true);
            }
            if (sceneObject != null && sceneObject.gameObject.scene.name == null) {
                // Don't allow objects that aren't in the scene!
                sceneObject = null;
            }

            T[] objectsInScene = GameObject.FindObjectsOfType<T>();
            int selectedIndex = 0;
            string[] displayedOptions = new string[objectsInScene.Length + 1];
            displayedOptions[0] = "(None)";
            for (int i = 0; i < objectsInScene.Length; i++) {
                displayedOptions[i + 1] = objectsInScene[i].name;
                if (objectsInScene[i] == sceneObject) {
                    selectedIndex = i + 1;
                }
            }
            selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
            if (selectedIndex == 0) {
                sceneObject = null;
            } else {
                sceneObject = objectsInScene[selectedIndex - 1];
            }
            EditorGUILayout.EndHorizontal();
            return sceneObject;
        }

        /// <summary>
        /// Draws enum values as a set of toggle fields
        /// Also draws a button the user can click to set to a 'default' value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="label"></param>
        /// <param name="enumObj"></param>
        /// <param name="defaultName"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static int EnumCheckboxField<T>(string label, T enumObj, string defaultName, T defaultVal, bool ignoreNone = true, bool ignoreAll = true) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum.");
            }

            // Convert enum value to an int64 so we can treat it as a flag set
            int enumFlags = Convert.ToInt32(enumObj);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!string.IsNullOrEmpty(label)) {
                EditorGUILayout.LabelField(label, EditorStyles.miniLabel);
                DrawDivider();
            }

            System.Array enumVals = Enum.GetValues(typeof(T));
            int lastvalue = Convert.ToInt32((T)enumVals.GetValue(enumVals.GetLength(0) - 1));

            foreach (T enumVal in enumVals)
            {
                int flagVal = Convert.ToInt32(enumVal);
                if (ignoreNone && flagVal == 0 && enumVal.ToString().ToLower() == "none")
                {
                    continue;
                }
                if (ignoreAll && flagVal == lastvalue && enumVal.ToString().ToLower() == "all")
                {
                    continue;
                }
                bool selected = (flagVal & enumFlags) != 0;
                selected = EditorGUILayout.Toggle(enumVal.ToString(), selected);
                // If it's selected add it to the enumObj, otherwise remove it
                if (selected)
                {
                    enumFlags |= flagVal;
                }
                else
                {
                    enumFlags &= ~flagVal;
                }
            }
            if (!string.IsNullOrEmpty(defaultName))
            {
                if (GUILayout.Button(defaultName, EditorStyles.miniButton))
                {
                    enumFlags = Convert.ToInt32(defaultVal);
                }
            }
            EditorGUILayout.EndVertical();

            return enumFlags;
        }

        public static int EnumCheckboxField<T>(string label, T enumObj, string defaultName, T defaultVal, T valOnZero, bool ignoreNone = true, bool ignoreAll = true) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enum.");
            }

            // Convert enum value to an int64 so we can treat it as a flag set
            int enumFlags = Convert.ToInt32(enumObj);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.miniLabel);
            DrawDivider();
            System.Array enumVals = Enum.GetValues(typeof(T));
            int lastvalue = Convert.ToInt32((T)enumVals.GetValue(enumVals.GetLength(0) - 1));

            foreach (T enumVal in enumVals)
            {
                int flagVal = Convert.ToInt32(enumVal);
                if (ignoreNone && flagVal == 0 && enumVal.ToString().ToLower() == "none")
                {
                    continue;
                }
                if (ignoreAll && flagVal == lastvalue && enumVal.ToString().ToLower() == "all")
                {
                    continue;
                }
                bool selected = (flagVal & enumFlags) != 0;
                selected = EditorGUILayout.Toggle(enumVal.ToString(), selected);
                // If it's selected add it to the enumObj, otherwise remove it
                if (selected)
                {
                    enumFlags |= flagVal;
                }
                else
                {
                    enumFlags &= ~flagVal;
                }
            }
            if (!string.IsNullOrEmpty(defaultName))
            {
                if (GUILayout.Button(defaultName, EditorStyles.miniButton))
                {
                    enumFlags = Convert.ToInt32(defaultVal);
                }
            }
            EditorGUILayout.EndVertical();

            if (enumFlags == 0)
            {
                enumFlags = Convert.ToInt32(valOnZero);
            }
            return enumFlags;
        }

        public static string MaterialPropertyName(string property, Material mat, ShaderUtil.ShaderPropertyType type, bool allowNone = true, string defaultProperty = "_Color", string labelName = null)
        {
            Color tColor = GUI.color;
            // Create a list of available color and value properties
            List<string> props = new List<string>();

            int selectedPropIndex = 0;

            if (allowNone) {
                props.Add("(None)");
            }

            if (mat != null)
            {
                int propertyCount = ShaderUtil.GetPropertyCount(mat.shader);
                string propName = string.Empty;
                for (int i = 0; i < propertyCount; i++)
                {
                    if (ShaderUtil.GetPropertyType(mat.shader, i) == type)
                    {
                        propName = ShaderUtil.GetPropertyName(mat.shader, i);
                        if (propName == property)
                        {
                            // We've found our current property
                            selectedPropIndex = props.Count;
                        }
                        props.Add(propName);
                    }
                }

                GUI.color = string.IsNullOrEmpty(property) ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor;
                if (string.IsNullOrEmpty (labelName))
                {
                    labelName = type.ToString();
                }
                int newPropIndex = EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                if (allowNone) {
                    property = (newPropIndex > 0 ? props[newPropIndex] : string.Empty);
                } else {
                    if (props.Count > 0) {
                        property = props[newPropIndex];
                    } else {
                        property = defaultProperty;
                    }
                }
                GUI.color = HUXEditorUtils.DefaultColor;
                return property;
            }
            else
            {
                WarningMessage("Can't get material " + type.ToString() + " properties because material is null.");
                GUI.color = HUXEditorUtils.DefaultColor;
                return string.Empty;
            }
        }

        public static void Header (string header) {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 18;
            EditorGUILayout.LabelField(header, headerStyle, GUILayout.MinHeight(24));
        }

        public static void WarningMessage(string warning, string buttonMessage = null, Action buttonAction = null)
        {
            Color tColor = GUI.color;
            HUXEditorUtils.BeginSectionBox("Warning", HUXEditorUtils.WarningColor);
            EditorGUILayout.LabelField(warning, EditorStyles.wordWrappedLabel);
            if (!string.IsNullOrEmpty(buttonMessage) && buttonAction != null)
            {
                if (GUILayout.Button(buttonMessage))
                {
                    buttonAction.Invoke();
                }
            }
            HUXEditorUtils.EndSectionBox();
            GUI.color = tColor;
        }

        public static void ErrorMessage(string error, Action action = null, string fixMessage = null)
        {
            Color tColor = GUI.color;
            HUXEditorUtils.BeginSectionBox("Error", HUXEditorUtils.ErrorColor);
            EditorGUILayout.LabelField(error, EditorStyles.wordWrappedLabel);
            if (action != null && GUILayout.Button((fixMessage != null) ? fixMessage : "Fix now"))
            {
                action.Invoke();
            }
            HUXEditorUtils.EndSectionBox();
            GUI.color = tColor;
        }

        public static void BeginProfileBox()
        {
            GUI.color = HUXEditorUtils.WarningColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawSubtleMiniLabel("Profile" + ":");
            DrawSubtleMiniLabel("(Warning: this section edits the button profile. These changes will affect all buttons that use this profile.)");
        }

        public static void EndProfileBox()
        {
            EndSectionBox();
        }

        public static void BeginSectionBox(string label)
        {
            GUI.color = DefaultColor;
            /*GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.normal.background = SectionBackground;*/
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawSubtleMiniLabel(label + ":");
        }

        public static void HelpBox(bool show, string text) {

            if (show) {
                GUI.color = ObjectColor;
                GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
                helpBoxStyle.wordWrap = true;
                helpBoxStyle.fontSize = 9;
                helpBoxStyle.normal.background = HelpBoxBackground;
                EditorGUILayout.LabelField(text, helpBoxStyle);
            }
            GUI.color = DefaultColor;
        }

        public static bool BeginSectionBox(string label, ref bool foldout) {
            GUI.color = DefaultColor;
            /*GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.normal.background = SectionBackground;*/
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUI.color = Color.Lerp(DefaultColor, Color.grey, 0.5f); ;
            //GUI.contentColor = DarkColor;
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Normal;
            foldoutStyle.fontSize = 9;
            foldoutStyle.fontStyle = FontStyle.Normal;

            foldout = EditorGUILayout.Foldout(foldout, label + (foldout ? ":" : ""), true, foldoutStyle);

            GUI.color = DefaultColor;
            //GUI.contentColor = Color.white;

            return foldout;
        }

        public static void BeginSectionBox(string label, Color color)
        {
            GUI.color = color;
            /*GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.normal.background = SectionBackground;*/
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            /*GUIStyle foldoutStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
            foldoutStyle.fontStyle = FontStyle.Normal;
            foldoutStyle.fontSize = 12;
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.LabelField(label + ":", foldoutStyle);*/

            DrawSubtleMiniLabel(label + ":");
        }

        public static void EndSectionBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static void BeginSubSectionBox(string label, Color sectionColor)
        {
            GUI.color = sectionColor;
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.normal.background = SectionBackground;
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField(label + ":", EditorStyles.boldLabel);
        }

        public static void BeginSubSectionBox(string label)
        {
            GUI.color = DefaultColor;
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.normal.background = SectionBackground;
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField(label + ":", EditorStyles.boldLabel);
        }

        public static void EndSubSectionBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static void DrawSubtleMiniLabel(string label)
        {
            Color tColor = GUI.color;
            GUI.color = Color.Lerp(tColor, Color.grey, 0.5f);
            EditorGUILayout.LabelField(label, EditorStyles.wordWrappedMiniLabel);
            GUI.color = tColor;
        }

        public static void DrawDivider()
        {
            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = 2;
            GUILayout.Box("", styleHR);
        }

        public static void SaveChanges(UnityEngine.Object target)
        {
            if (Application.isPlaying)
                return;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        public static void SaveChanges(UnityEngine.Object target1, UnityEngine.Object target2)
        {
            if (Application.isPlaying)
                return;

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target1);
                EditorUtility.SetDirty(target2);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            }
        }

        public static string[] getMethodOptions(GameObject comp, List<System.Type> ignoreTypes = null)
        {
            List<string> methods = new List<string>();

            if (comp != null)
            {
                Component[] allComponents = comp.GetComponents<Component>();
                List<System.Type> doneTypes = new List<System.Type>();

                for (int index = 0; index < allComponents.Length; index++)
                {
                    System.Type compType = allComponents[index].GetType();
                    if (!doneTypes.Contains(compType) && (ignoreTypes == null || !ignoreTypes.Contains(compType)))
                    {
                        MethodInfo[] allMemebers = compType.GetMethods();
                        for (int memberIndex = 0; memberIndex < allMemebers.Length; memberIndex++)
                        {
                            if (allMemebers[memberIndex].IsPublic
                                && allMemebers[memberIndex].GetParameters().Length == 0
                                && !methods.Contains(allMemebers[memberIndex].Name)
                                && allMemebers[memberIndex].ReturnType == typeof(void))
                            {
                                methods.Add(allMemebers[memberIndex].Name);
                            }
                        }

                        doneTypes.Add(compType);
                    }
                }
            }

            return methods.ToArray();
        }

        /// <summary>
        /// Adds a prefab to the scene.
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="ignoreAlreadyInScene">If false the prefab will not be added if it exists in the hierarchy.</param>
        /// <returns>A refernce to the newly created prefab instance or one that exists in the scene if ignoreAlreadyInScene is false.</returns>
        public static GameObject AddToScene(string prefabPath, bool ignoreAlreadyInScene = true)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            GameObject instance = null;
            if (prefab != null)
            {
                instance = FindFirstPrefabInstance(prefab);

                if (instance == null || ignoreAlreadyInScene)
                {
                    instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                }
                else
                {
                    Debug.LogWarning("Instance already exits in the scene: " + prefabPath);
                }
            }
            else
            {
                Debug.LogError("Could not load prefab: " + prefabPath);
            }

            return instance;
        }

        /// <summary>
        /// Finds the first instance of a preface in the Hierarchy.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>First instance of the prefab or null if one is not found.</returns>
        public static GameObject FindFirstPrefabInstance(GameObject prefab)
        {
            GameObject result = null;
            GameObject[] allObjects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (GameObject obj in allObjects)
            {
                PrefabType type = PrefabUtility.GetPrefabType(obj);
                if (type == PrefabType.PrefabInstance)
                {
                    UnityEngine.Object GO_prefab = PrefabUtility.GetPrefabParent(obj);
                    if (prefab == GO_prefab)
                    {
                        result = obj;
                        break;
                    }
                }
            }
            return result;
        }

        public static void CorrectAmbientLightingInScene()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = 1.0f;

            // Normalize and set ambient light to default.
            Vector4 c = new Vector4(51.0f, 51.0f, 51.0f, 255.0f);
            c.Normalize();
            RenderSettings.ambientLight = new Color(c.x, c.y, c.z, c.w);


            RenderSettings.reflectionBounces = 1;
            RenderSettings.reflectionIntensity = 1.0f;

            RenderSettings.skybox = null;
            RenderSettings.fog = false;
        }

        private static List<Type> GetDerivedTypes(Type baseType, Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            List<Type> derivedTypes = new List<Type>();

            for (int i = 0, count = types.Length; i < count; i++)
            {
                Type type = types[i];
                if (IsSubclassOf(type, baseType))
                {
                    derivedTypes.Add(type);
                }
            }

            return derivedTypes;
        }

        private static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        private static Texture2D SectionBackground {
            get {
                if (sectionBackground == null) {
                    sectionBackground = new Texture2D(2, 2);
                    var pix = new Color[2 * 2];
                    for (int i = 0; i < pix.Length; i++) {
                        pix[i] = SectionColor;
                    }
                    sectionBackground.SetPixels(pix);
                    sectionBackground.Apply();
                }
                return sectionBackground;
            }
        }

        private static Texture2D HelpBoxBackground {
            get {
                if (helpBoxBackground == null) {
                    helpBoxBackground = new Texture2D(2, 2);
                    var pix = new Color[2 * 2];
                    for (int i = 0; i < pix.Length; i++) {
                        pix[i] = HelpBoxColor;
                    }
                    helpBoxBackground.SetPixels(pix);
                    helpBoxBackground.Apply();
                }
                return helpBoxBackground;
            }
        }

        private static Texture2D helpBoxBackground = null;
        private static Texture2D sectionBackground = null;
    }
}