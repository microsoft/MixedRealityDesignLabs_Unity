//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEditor;
using System.Collections;

public class HUXInputEditing
{
	public enum AxisType
	{
		KeyOrMouseButton = 0,
		MouseMovement = 1,
		JoystickAxis = 2
	};

	public enum AxisNum
	{
		XAxis = 0,
		YAxis,
		JoystickAndScrollWheel,
		Joystick4,
		Joystick5,
		Joystick6,
		Joystick7,
		Joystick8,
		Joystick9,
		Joystick10,
		Joystick11,
		Joystick12,
		Joystick13,
		Joystick14,
		Joystick15,
		Joystick16,
		Joystick17,
		Joystick18,
		Joystick19,
		Joystick20
	}

	public enum JoyNum
	{
		MotionsFromAllJoysticks,
		Joystick1,
		Joystick2,
		Joystick3,
		Joystick4,
		Joystick5,
		Joystick6,
		Joystick7,
		Joystick8,
		Joystick9,
		Joystick10,
		Joystick11,
	}

	public class InputControl
	{
		public string Name;
		public string DescriptiveName;
		public string DescriptiveNegativeName;
		public string NegativeButton;
		public string PositiveButton;
		public string AltNegativeButton;
		public string AltPositiveButton;

		public float Gravity;
		public float Dead;
		public float Sensitivity;

		public bool Snap = false;
		public bool Invert = false;

		public AxisType Type = AxisType.JoystickAxis;

		public AxisNum AxisNum = AxisNum.XAxis;
		public JoyNum JoyNum = JoyNum.MotionsFromAllJoysticks;

		public InputControl()
		{

		}

		public InputControl(string name, string descName, string descNegName,
			string negButton, string posButton, string altNegButton,
			string altPosButton, float grav, float dead,
			float sensitivity, bool snap, bool invert,
			AxisType type, AxisNum axisNum, JoyNum joyNum)
		{
			Name = name;
			DescriptiveName = descName;
			DescriptiveNegativeName = descNegName;
			NegativeButton = negButton;
			PositiveButton = posButton;
			AltNegativeButton = altNegButton;
			AltPositiveButton = altPosButton;

			Gravity = grav;
			Dead = dead;
			Sensitivity = sensitivity;

			Snap = snap;
			Invert = invert;

			Type = type;

			AxisNum = axisNum;
			JoyNum = joyNum;
        }
	}


	/// <summary>
	/// Adds an input control to the InputManger.  Proccess is as described as http://www.plyoung.com/blog/manipulating-input-manager-in-script.html
	/// </summary>
	public static void AddInputControl(InputControl newControl)
	{
		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesListProperty = serializedObject.FindProperty("m_Axes");

		SerializedProperty axisProp = null;

		for (int index = 0; index < axesListProperty.arraySize; index++)
		{
			SerializedProperty possibleProp = axesListProperty.GetArrayElementAtIndex(index);
			SerializedProperty axisName = GetChildProperty(possibleProp, "m_Name");
			if (axisName.stringValue == newControl.Name)
			{
				axisProp = possibleProp;
				break;
			}
		}


		if (axisProp == null)
		{
			axesListProperty.arraySize++;
			serializedObject.ApplyModifiedProperties();
			axisProp = axesListProperty.GetArrayElementAtIndex(axesListProperty.arraySize - 1);
		}

		GetChildProperty(axisProp, "m_Name").stringValue = newControl.Name;
		GetChildProperty(axisProp, "descriptiveName").stringValue = newControl.DescriptiveName;
		GetChildProperty(axisProp, "descriptiveNegativeName").stringValue = newControl.DescriptiveNegativeName;
		GetChildProperty(axisProp, "negativeButton").stringValue = newControl.NegativeButton;
		GetChildProperty(axisProp, "positiveButton").stringValue = newControl.PositiveButton;
		GetChildProperty(axisProp, "altNegativeButton").stringValue = newControl.AltNegativeButton;
		GetChildProperty(axisProp, "altPositiveButton").stringValue = newControl.AltPositiveButton;
		GetChildProperty(axisProp, "gravity").floatValue = newControl.Gravity;
		GetChildProperty(axisProp, "dead").floatValue = newControl.Dead;
		GetChildProperty(axisProp, "sensitivity").floatValue = newControl.Sensitivity;
		GetChildProperty(axisProp, "snap").boolValue = newControl.Snap;
		GetChildProperty(axisProp, "invert").boolValue = newControl.Invert;
		GetChildProperty(axisProp, "type").intValue = (int)newControl.Type;
		GetChildProperty(axisProp, "axis").intValue = (int)newControl.AxisNum;
		GetChildProperty(axisProp, "joyNum").intValue = (int)newControl.JoyNum;

		serializedObject.ApplyModifiedProperties();
	}

	/// <summary>
	/// Removes a control from the InputManager.
	/// </summary>
	/// <param name="controlName"></param>
	/// <param name="occurance"> 0 or less will remove all occurances of the control, while 1 will remove only the first, 2 will remove only the second etc,</param>
	/// <returns>true if a control was removed, false otherwise</returns>
	public static bool RemoveInputControl(string controlName, int occurance = 0)
	{
		bool removedControl = false;

		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesListProperty = serializedObject.FindProperty("m_Axes");

		int occuranceCount = 0;

		for (int index = 0; index < axesListProperty.arraySize; index++)
		{
			SerializedProperty possibleProp = axesListProperty.GetArrayElementAtIndex(index);
			SerializedProperty axisName = GetChildProperty(possibleProp, "m_Name");
			if (axisName.stringValue == controlName)
			{
				occuranceCount++;
				if (occurance <= 0 || occuranceCount == occurance)
				{
					axesListProperty.DeleteArrayElementAtIndex(index);
					//Go back an index so that we don't skip anyd
					index--;
					removedControl = true;
                }
			}
		}

		serializedObject.ApplyModifiedProperties();
		return removedControl;
	}

	private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
	{
		SerializedProperty child = parent.Copy();
		child.Next(true);
		do
		{
			if (child.name == name)
				return child;
		}
		while (child.Next(false));
		return null;
	}
}
