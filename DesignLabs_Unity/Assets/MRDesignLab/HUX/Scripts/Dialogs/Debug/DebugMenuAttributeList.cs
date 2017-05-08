//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HUX.Dialogs.Debug;
using System;

public class DebugMenuAttributeList : MonoBehaviour {

	Dictionary<UnityEngine.Object, List<int>> m_urids = new Dictionary<UnityEngine.Object, List<int>>();

	// Use this for initialization
	void Start ()
	{
		List<DebugTunable> atts = AttributeCacheManager.Instance.GetAttributes<DebugTunable>();
		foreach (DebugTunable dt in atts)
		{
			dt.OnInstanceCreated += (att, inst) =>
			{
				//Debug.Log("OnInstanceCreated: " + inst);
				AddTunable(att as DebugTunable, inst);
			};

			dt.OnInstancesMaybeRemoved += (att) =>
			{
				//Debug.Log("OnInstancesMaybeRemoved:  Current count: " + m_urids.Count);
				List<UnityEngine.Object> objsToRemove = new List<UnityEngine.Object>();

				// Remove the items from the debug menu, and add to the list of items to remove from the map in the second loop (laame)
				foreach (KeyValuePair<UnityEngine.Object, List<int>> pair in m_urids)
				{
					// Look for objects which have been destroyed
					if (pair.Key == null)
					{
						// Remove all items for that instance
						foreach (int urid in pair.Value)
						{
							DebugMenu.Instance.RemoveItem(urid);
						}
						objsToRemove.Add(pair.Key);
					}
				}

				foreach (UnityEngine.Object obj in objsToRemove)
				{
					m_urids.Remove(obj);
				}

				//Debug.Log("New urids length: " + m_urids.Count);
			};
		}
	}

	public void AddTunable(DebugTunable attTunable, UnityEngine.Object inst)
	{
		if (attTunable == null)
		{
			Debug.Log("AddTunable:  null attTunable, what?");
			return;
		}

		int urid = -1;
		if (attTunable.IsField())
		{
			// Initial value
			object value = attTunable.fieldInfo.GetValue(inst);
			string debugName = string.IsNullOrEmpty(attTunable.ItemName) ? attTunable.fieldInfo.Name : attTunable.ItemName;

			// Add the item
			if (attTunable is DebugBoolTunable)
			{
				urid = DebugMenu.Instance.AddBoolItem(debugName, (bool newVal) => { attTunable.fieldInfo.SetValue(inst, newVal); }, () => { return (bool)attTunable.fieldInfo.GetValue(inst); }, (bool)value, this.gameObject);
			}
			else if (attTunable is DebugNumberTunable)
			{
				DebugNumberTunable numTuneable = attTunable as DebugNumberTunable;
				if (attTunable.fieldInfo.FieldType == typeof(float))
				{
					urid = DebugMenu.Instance.AddFloatItem(debugName, (float newVal) => { attTunable.fieldInfo.SetValue(inst, newVal); }, () => { return (float)attTunable.fieldInfo.GetValue(inst); }, (float)value, numTuneable.Fmin, numTuneable.Fmax, this.gameObject, (int)numTuneable.FStep);
				}
				else if (attTunable.fieldInfo.FieldType == typeof(int))
				{
					urid = DebugMenu.Instance.AddIntItem(debugName, (int newVal) => { attTunable.fieldInfo.SetValue(inst, newVal); }, () => { return (int)attTunable.fieldInfo.GetValue(inst); }, (int)value, (int)numTuneable.Fmin, (int)numTuneable.Fmax, this.gameObject, (int)Mathf.Max(1, (int)numTuneable.FStep));
				}
			}
			else if (attTunable is DebugVector3Tunable)
			{
				DebugVector3Tunable vec3Tuneable = attTunable as DebugVector3Tunable;
				urid = DebugMenu.Instance.AddVector3Item(debugName, (Vector3 newVal) => { attTunable.fieldInfo.SetValue(inst, newVal); }, () => { return (Vector3)attTunable.fieldInfo.GetValue(inst); }, (Vector3)value, vec3Tuneable.VecStep, this.gameObject);
			}
			else if (CustomAttributeBase.IsEnum(attTunable.fieldInfo.FieldType) && attTunable is DebugEnumTunable)
			{
				DebugEnumTunable enumTunable = attTunable as DebugEnumTunable;
				// Get range from enum values
				Type fieldType = attTunable.fieldInfo.FieldType;

				int minv = enumTunable.MinEnumVal;
				int maxv = enumTunable.MaxEnumVal;

				if (minv == -1 && maxv == -1)
				{
					Array arr = Enum.GetValues(fieldType);
					if (arr != null && arr.Length > 0)
					{
						minv = (int)arr.GetValue(0);
						maxv = (int)arr.GetValue(arr.Length - 1);
					}
				}

				urid = DebugMenu.Instance.AddEnumItem(debugName, attTunable.fieldInfo.FieldType, (int newVal) => { attTunable.fieldInfo.SetValue(inst, newVal); }, () => { return (int)attTunable.fieldInfo.GetValue(inst); }, (int)value, minv, maxv, this.gameObject);
			}
		}
		else if (attTunable.IsMethod())
		{
			string debugName = string.IsNullOrEmpty(attTunable.ItemName) ? attTunable.methodInfo.Name : attTunable.ItemName;

			if (attTunable is DebugButtonAttribute)
			{
				DebugButtonAttribute buttonTunable = attTunable as DebugButtonAttribute;

				urid = DebugMenu.Instance.AddButtonItem(debugName, buttonTunable.ButtonName, () => { buttonTunable.methodInfo.Invoke(inst, null); }, this.gameObject);
			}
		}

		// Add the item urid to the list of items associated with the object instance
		if (urid != -1)
		{
			if (!m_urids.ContainsKey(inst))
			{
				m_urids[inst] = new List<int>();
			}
			m_urids[inst].Add(urid);
		}
	}
}
