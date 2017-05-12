//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomPropertyDrawer(typeof(SpeechReciever.SpeechData))]
    public class SpeechDataEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded ? 100 : 15;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty speechCommandProp = property.FindPropertyRelative("m_SpeechCommand");
            SerializedProperty targetProp = property.FindPropertyRelative("m_Target");
            SerializedProperty messageProp = property.FindPropertyRelative("m_Message");
            SerializedProperty debugKeyProp = property.FindPropertyRelative("m_DebugKeyCode");

            Rect foldoutRect = position;
            foldoutRect.height = 17;
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, "Command: " + speechCommandProp.stringValue);
            if (property.isExpanded)
            {
                Rect speechArea = new Rect(position.x + 15, position.y + 17, 85, 17);
                EditorGUI.LabelField(speechArea, "Command:");
                speechArea.x += speechArea.width;
                speechArea.width = 200;
                speechCommandProp.stringValue = EditorGUI.TextField(speechArea, speechCommandProp.stringValue);

                Rect targetArea = new Rect(position.x + 15, speechArea.y + 17, 85, 17);
                EditorGUI.LabelField(targetArea, "Target:");
                targetArea.x += targetArea.width;
                targetArea.width = 200;
                targetProp.objectReferenceValue = EditorGUI.ObjectField(targetArea, targetProp.objectReferenceValue, typeof(GameObject), true);

                string[] methodOptions = HUXEditorUtils.getMethodOptions(targetProp.objectReferenceValue as GameObject);
                int methodIndex = System.Array.IndexOf(methodOptions, messageProp.stringValue);


                EditorGUI.BeginDisabledGroup(targetProp.objectReferenceValue == null);
                Rect methodArea = new Rect(position.x + 15, targetArea.y + 17, 85, 17);
                EditorGUI.LabelField(methodArea, "Message:");
                methodArea.x += methodArea.width;
                methodArea.width = 200;

                methodIndex = EditorGUI.Popup(methodArea, methodIndex, methodOptions);

                if (methodIndex >= 0)
                {
                    messageProp.stringValue = methodOptions[methodIndex];
                }
                else
                {
                    messageProp.stringValue = string.Empty;
                }

                EditorGUI.EndDisabledGroup();

                Rect debugArea = new Rect(position.x + 15, methodArea.y + 17, 85, 17);
                EditorGUI.LabelField(debugArea, "Debug key Code:");
                debugArea.x += debugArea.width;
                debugArea.width = 200;
                debugKeyProp.enumValueIndex = EditorGUI.Popup(debugArea, debugKeyProp.enumValueIndex, debugKeyProp.enumDisplayNames);
            }

            EditorGUI.EndProperty();
        }
    }

}