//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.Reflection;

namespace HUX.Utility
{
	[CustomPropertyDrawer(typeof(EventSender.EventEntry))]
	public class EventSenderEditor : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return property.isExpanded ? 100 : 15;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			SerializedProperty speechCommandProp = property.FindPropertyRelative("eventName");
			SerializedProperty targetProp = property.FindPropertyRelative("targetObject");
			SerializedProperty messageProp = property.FindPropertyRelative("funtionToCall");
			SerializedProperty userDataProp = property.FindPropertyRelative("userData");

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

				string[] eventOptions = GetEventOptions(targetProp.objectReferenceValue as GameObject);
				int eventIndex = System.Array.IndexOf(eventOptions, messageProp.stringValue);

				EditorGUI.BeginDisabledGroup(targetProp.objectReferenceValue == null);
				Rect methodArea = new Rect(position.x + 15, targetArea.y + 17, 85, 17);
				EditorGUI.LabelField(methodArea, "Event:");
				methodArea.x += methodArea.width;
				methodArea.width = 200;

				eventIndex = EditorGUI.Popup(methodArea, eventIndex, eventOptions);

				if (eventIndex >= 0)
				{
					messageProp.stringValue = eventOptions[eventIndex];
				}
				else
				{
					messageProp.stringValue = string.Empty;
				}

				EditorGUI.EndDisabledGroup();

				Rect userDataArea = new Rect(position.x + 15, methodArea.y + 17, 85, 17);
				EditorGUI.LabelField(userDataArea, "User Data:");
				userDataArea.x += userDataArea.width;
				userDataArea.width = 200;
				userDataProp.stringValue = EditorGUI.TextField(userDataArea, userDataProp.stringValue);
			}

			EditorGUI.EndProperty();
		}

		private string[] GetEventOptions(GameObject targetObj)
		{
			List<string> eventNames = new List<string>();

			if (targetObj)
			{
				Component[] components = targetObj.GetComponents(typeof(Component));
				for (int i = 0; i < components.Length; ++i)
				{
					Attribute[] attribs = Attribute.GetCustomAttributes(components[i].GetType());
					for (int j = 0; j < attribs.Length; ++j)
					{
						SendsEvent sendsEventAttrib = attribs[j] as SendsEvent;
						if (sendsEventAttrib != null)
						{
                            eventNames.Add(sendsEventAttrib.EventToSend);
						}
					}
				}
			}

			return eventNames.ToArray();
        }
	}
}
