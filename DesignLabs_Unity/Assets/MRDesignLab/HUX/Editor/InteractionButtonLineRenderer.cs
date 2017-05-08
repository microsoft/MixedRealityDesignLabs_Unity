//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using HUX.Buttons;

/// <summary>
/// The Interaction Button Line Renderer
/// Draws a line between an InteractionReceiver and any buttons which are serialized on it.
/// </summary>
[CustomEditor(typeof(HUX.Receivers.InteractionReceiver), true)]
public class InteractionButtonLineRenderer : Editor
{
	#region Private Members

	/// <summary>
	/// Cache of button script serialized on the selected object.
	/// </summary>
	private List<Button> buttonsOnObject = new List<Button>(0);

	/// <summary>
	/// Serialized property iterator.
	/// </summary>
	private SerializedProperty serializedProperty = null;

	/// <summary>
	/// Cached transform of the selected object.
	/// </summary>
	private Transform interactionReceiverTransform = null;

	#endregion Private Members

	/// <summary>
	/// Finds and caches each of the Button scripts found on the selected object.
	/// </summary>
	private void OnEnable()
	{
		buttonsOnObject.Clear();

		serializedProperty = serializedObject.GetIterator();
		while (serializedProperty.Next(true))
		{
			if (serializedProperty.propertyType == SerializedPropertyType.ObjectReference)
			{
				// Try to cast the current object reference property to the Button type.
				Button button = (serializedProperty.objectReferenceValue as Button);

				// Only add to cache when cast is successful.
				if (button != null)
				{
					buttonsOnObject.Add(button);
				}
			}
		}
	}

	/// <summary>
	/// As the scene renders, loop through the cached button references and draw lines to them.
	/// </summary>
	public void OnSceneGUI()
	{
		interactionReceiverTransform = (serializedObject.targetObject as MonoBehaviour).transform;

		if (interactionReceiverTransform != null && buttonsOnObject != null)
		{
			foreach (Button button in buttonsOnObject)
			{
				Handles.DrawLine(interactionReceiverTransform.position, button.transform.position);
			}
		}
	}
}
