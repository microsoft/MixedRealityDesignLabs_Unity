//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
#if UNITY_WINRT && !UNITY_EDITOR
#define USE_WINRT
#endif

using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;

/// <summary>
/// InputSources holds all the input sources so they can be accessed from anywhere.
/// The input source fields here are automatically assigned to an instance of that type found in the scene.
/// InputSources is instantiated as a component of InputShellMap, and is not derived from MonoBehavior
///
/// See InputSourceBase.cs for steps on adding a new input source.
/// </summary>
[System.Serializable]
public class InputSources
{
	public static InputSources Instance;

	// Non-Virtual sources
	public InputSourceHands hands;
	public InputSourceMouse mouse;
	public InputSourceSixDOFRay sixDOFRay;
	public InputSourceNetMouse netMouse;
	public InputSourceNetGamepad netGamepad;
	public InputSourceHidGamepad hidGamepad;
	public InputSourceUnityGamepad unityGamepad;
	public InputSourceKeyboard keyboard;
	
    // Virtual sources
	public InputSourceTouch6D touch6D;
	public InputSourceWorldCursorMouse worldCursorMouse;
	public InputSourceWorldCursorGamepad worldCursorGamepad;
	
    //public InputSourceWorldCursorHands worldCursorHands;
	public InputSourceGamepad gamepad;
	public InputSourceGamepadCardinal gamepadCardinal;
	public InputSourceEditor editor;

	/// <summary>
	/// Input sources are automatically sorted into an array for updating
	/// </summary>
	public InputSourceBase[] sources = new InputSourceBase[0];

	/// <summary>
	/// Initialization.  Gets all input sources that exist in the scene, then sorts virtual sources
	/// to the end.  Finally, the input source fields in InputSources are automatically assigned using reflection.
	/// </summary>
	public void Init(GameObject owner)
	{
		Instance = this;

		// Currently looks for all input sources in scene
		sources = GameObject.FindObjectsOfType<InputSourceBase>();

		// Sort so that virtual sources are after physical
		for (int i = 0; i < sources.Length; ++i)
		{
			// If we find a virtual source, look for non-virtual sources
			if (sources[i].IsVirtual)
			{
				int h = sources.Length - 1;
				for (; h > i; --h)
				{
					// Found a non-virtual source, swap them
					if (!sources[h].IsVirtual)
					{
						InputSourceBase temp = sources[i];
						sources[i] = sources[h];
						sources[h] = temp;
						break;
					}
				}

				// There were no non-virtual sources after this one!
				if (h <= i)
				{
					break;
				}
			}
		}

		List<InputSourceBase> addedSources = new List<InputSourceBase>();

		// Auto-assign all InputSourceBase fields to the corresponding instance
#if USE_WINRT
		foreach (FieldInfo field in GetType().GetTypeInfo().DeclaredFields)
#else
		FieldInfo[] myFields = GetType().GetFields();
		foreach (FieldInfo field in myFields)
#endif
		{
			// Found an InputSourceBase field
#if USE_WINRT
			if (field.FieldType.GetTypeInfo().IsSubclassOf(typeof(InputSourceBase)))
#else
			if (field.FieldType.IsSubclassOf(typeof(InputSourceBase)))
#endif
			{
				// Con: Must create in editor, Pro: can configure properties
				// Find the matching component
				bool didSet = false;
				foreach (InputSourceBase isb in sources)
				{
					if (isb.GetType() == field.FieldType)
					{
						field.SetValue(this, isb);
						didSet = true;
						break;
					}
				}

				if (!didSet)
				{
					//Debug.LogError("Didn't find InputSourceBase for field " + field.Name);
					// Pro: Don't need to create in editor, Con: can't configure

					// Create the component if not found.  It will just have default values.
					Component c = owner.AddComponent(field.FieldType);
					field.SetValue(this, c);
					addedSources.Add(c as InputSourceBase);
				}
			}
			else if (field.FieldType.IsArray

#if USE_WINRT
				&& field.FieldType.GetElementType().GetTypeInfo().IsSubclassOf(typeof(InputSourceBase))
#else
				&& field.FieldType.GetElementType().IsSubclassOf(typeof(InputSourceBase))
#endif
		)
			{
				Type elementType = field.FieldType.GetElementType();
				List<InputSourceBase> typeSources = new List<InputSourceBase>();
				foreach (InputSourceBase isb in sources)
				{
					if (isb.GetType() == elementType)
					{
						typeSources.Add(isb);
					}
				}

				object[] arr = (object[])Array.CreateInstance(elementType, typeSources.Count);

				for (int index = 0; index < arr.Length; index++)
				{
					arr[index] = typeSources[index];
				}

				field.SetValue(this, arr);
			}
		}

		// Add the added soruces back into the source array
		InputSourceBase[] newSourceArray = new InputSourceBase[addedSources.Count + sources.Length];
		sources.CopyTo(newSourceArray, 0);
		addedSources.CopyTo(newSourceArray, sources.Length);
		sources = newSourceArray;

		ConfigureActiveSources();
	}

	/// <summary>
	/// Does platform or configuration specific setup work such as activating or deactivating relevant input sources
	/// </summary>
	void ConfigureActiveSources()
	{
		// Disable inputs that conflict with editor mode, vice versa
#if UNITY_EDITOR
		worldCursorMouse.IsEnabled = false;
#else
		editor.IsEnabled = false;	
#endif
	}

	/// <summary>
	/// Update all enabled input sources
	/// </summary>
	public void Update()
	{
		foreach (InputSourceBase isb in sources)
		{
			if (isb.IsEnabled)
			{
				isb._Update();
			}
		}
	}

	/// <summary>
	/// Templated find to get an input source from the array.  Not terribly useful
	/// </summary>
	public T FindSource<T>() where T : InputSourceBase
	{
		foreach (InputSourceBase isb in sources)
		{
			if (isb is T)
			{
				return isb as T;
			}
		}
		return null;
	}
}
