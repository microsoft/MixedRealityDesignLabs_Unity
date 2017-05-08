//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Layer field for easy single layer selection in editor.
/// </summary>
[System.Serializable]
public struct LayerField
{
	/// <summary>
	/// The actual value of the layer.
	/// </summary>
	[SerializeField]
	private int m_Value;
	public int Value
	{
		get
		{
			return m_Value;
		}

		set
		{
			if (value >= 0 && value <= 31)
			{
				m_Value = value;
			}
			else
			{
				throw new System.Exception("Layer Field Value must be between 0 <-> 31");
			}
		}
	}

	/// <summary>
	/// The layer name associated with the layer.
	/// </summary>
	public string LayerName
	{
		get
		{
			return LayerMask.LayerToName(m_Value);
		}
	}

	/// <summary>
	/// The layer mask for this layer.
	/// </summary>
	public LayerMask Mask
	{
		get
		{
			LayerMask mask = new LayerMask();
			mask.value = 1 << m_Value;
			return mask;
		}
	}

	/// <summary>
	/// Create the layer field with the provided layer name.
	/// </summary>
	/// <param name="layerName"></param>
	public LayerField(string layerName)
	{
		m_Value = LayerMask.NameToLayer(layerName);
	}

	/// <summary>
	/// Create the layer field with the provided layer number.
	/// </summary>
	/// <param name="layerNum"></param>
	public LayerField(int layerNum)
	{
		if (layerNum >= 0 && layerNum <= 31)
		{
			m_Value = layerNum;
		}
		else
		{
			throw new System.Exception("Layer Field Value must be between 0 <-> 31");
		}
	}
}

#if UNITY_EDITOR
/// <summary>
/// The property drawer to make selecting the layer easy.
/// </summary>
[CustomPropertyDrawer(typeof(LayerField))]
public class LayerFieldDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		//position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		SerializedProperty prop = property.FindPropertyRelative("m_Value");

		prop.intValue = EditorGUI.LayerField(position, label, prop.intValue);

		EditorGUI.EndProperty();
	}
}
#endif
