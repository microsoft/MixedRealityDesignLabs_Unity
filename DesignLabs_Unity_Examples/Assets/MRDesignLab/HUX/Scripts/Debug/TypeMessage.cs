//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using HUX.Utility;

[Serializable]
public class TypeMessage
{
	static Dictionary<string, Type> cachedEnumTypes = new Dictionary<string, Type>();

	public static void RegisterEnum(string typeKey, Type type)
	{
		if (!cachedEnumTypes.ContainsKey(typeKey))
		{
			cachedEnumTypes.Add(typeKey, type);
		}
	}

	[Serializable]
	public struct DataType
	{
		public const string BOOLEAN = "boolean";
		public const string RANGE = "range";
		public const string STRING = "string";
		public const string BANG = "bang";
		public const string ENUM = "enum";
		public const string COLOR = "rgba";

		public string type;

		public override string ToString()
		{
			return type;
		}

		public DataType(string value)
		{
			type = value;
		}

		public static implicit operator DataType(string value)
		{
			return new DataType(value);
		}

		public static bool operator ==(DataType a, string b)
		{
			return a.type == b;
		}

		public static bool operator !=(DataType a, string b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public string name;
	public DataType type = new DataType("");

	public static string TRUE = "true";
	public static string FALSE = "false";
	public static int RANGE_MIN = 0;
	public static int RANGE_MAX = 1023;

	// RemapRange maps an input value between [min,max] to output between [RANGE_MIN, RANGE_MAX]
	// This should be used for app->Spacebrew
	public static int MapRange(int value, int min, int max)
	{
		float p = (float)(value - min) / (float)(max - min);
		return (int)(RANGE_MIN + p * (float)(RANGE_MAX - RANGE_MIN));
	}

	public static float MapRange(float value, float min, float max)
	{
		float p = (value - min) / (max - min);
		return (float)RANGE_MIN + p * (float)(RANGE_MAX - RANGE_MIN);
	}

	public static string SystemTypeToType(Type type)
	{
		string ret = DataType.BOOLEAN;
		if (type == typeof(bool))
		{
			ret = DataType.BOOLEAN;
		}
		else if (type == typeof(string))
		{
			ret = DataType.STRING;
		}
		else if (type == typeof(int) || type == typeof(float))
		{
			ret = DataType.RANGE;
		}
		else if (type == typeof(void))
		{
			ret = DataType.BANG;
		}
		else if (type == typeof(Color))
		{
			ret = DataType.COLOR;
		}
		else if (CustomAttributeBase.IsEnum(type))
		{
			ret = DataType.ENUM;

			// Could make another map to search this way
			foreach (KeyValuePair<string, Type> i in cachedEnumTypes)
			{
				if (i.Value == type)
				{
					return i.Key;
				}
			}
		}
		else
		{
			ret = type.ToString();
		}
		return ret;
	}

	// Converts object to string!
	public string GetValueString(object value, DebugTunable tunable = null)
	{
		string dataType = "" + type;
		string ret = "";

		switch (dataType)
		{
			case DataType.COLOR:
				ret = ((Color)value).ToString();
				break;
			case DataType.BOOLEAN:
				ret = (bool)value ? TRUE : FALSE;
				break;
			case DataType.RANGE:
				DebugNumberTunable numTunable = tunable as DebugNumberTunable;

				if (value.GetType() == typeof(int))
				{
					if (numTunable != null)
					{
						ret = numTunable.MapRange((int)value, RANGE_MIN, RANGE_MAX).ToString();
					}
					else
					{
						ret = ((int)value).ToString();
					}
				}
				else if (value.GetType() == typeof(float))
				{
					if (numTunable != null)
					{
						ret = numTunable.MapRange((float)value, RANGE_MIN, RANGE_MAX).ToString();
					}
					else
					{
						ret = ((float)value).ToString();
					}
				}
				break;
			case DataType.STRING:
				ret = (string)value;
				break;
			default:
				{
					if (dataType.StartsWith("enum_"))
					{
						ret = value.ToString();
					}
					else
					{
						// Unsupported type?  Can still format to string
						ret = value.ToString();
					}
				}
				break;
		}

		return ret;
	}

	public static object ParseValue(string dataType, string value)
	{
		object ret = null;

		// Bang has no value
		if (dataType == DataType.BANG || value == null)
		{
			return null;
		}

		// Object types first
		switch (dataType)
		{
			case DataType.COLOR:
				ret = value.JSONToColor();
				break;
			default:
				break;
		}

		if (ret != null)
		{
			return ret;
		}

		// String types last
		string valueString = value;
		switch (dataType)
		{
			case DataType.BOOLEAN:
				{
					bool v = false;
					if (bool.TryParse(valueString, out v))
					{
						ret = v;
					}
				}
				break;
			case DataType.RANGE:
				{
					int v = 0;
					if (int.TryParse(valueString, out v))
					{
						ret = v;
					}
				}
				break;
			case DataType.STRING:
				ret = valueString;
				break;
			default:
				{
					if (dataType.StartsWith("enum_"))
					{
						Type enumType;
						if (cachedEnumTypes.TryGetValue(dataType, out enumType))
						{
							ret = Enum.Parse(enumType, valueString);
						}
						else
						{
							Debug.LogError("Enum type not cached");
						}
					}
					else
					{
						Debug.LogError("Unrecognized type!");
					}
				}
				break;
		}
		return ret;
	}
}
