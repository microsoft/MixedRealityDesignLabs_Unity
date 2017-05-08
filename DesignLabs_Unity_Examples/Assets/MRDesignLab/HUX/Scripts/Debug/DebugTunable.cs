//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Reflection;

/// <summary>
/// Base class for debug attributes.
/// </summary>
public class DebugTunable : CustomAttributeBase
{
	/// <summary>
	/// The item name to use for this debug item.
	/// </summary>
	public string ItemName = null;

	/// <summary>
	/// The type of the tuneable.
	/// </summary>
	public Type tunableType;

	/// <summary>
	/// Base constructor.
	/// </summary>
	/// <param name="itemName">The name to give this debug item (including group name).  If null the member name will be used.</param>
	public DebugTunable(string itemName = null) : base()
	{
		// DebugTunable instances are cached
		bCacheInstances = true;
		this.ItemName = itemName;
	}

	/// <summary>
	/// Override for set into to save some information.
	/// </summary>
	/// <param name="_memberInfo"></param>
	/// <param name="_methodInfo"></param>
	/// <param name="_fieldInfo"></param>
	public override void SetInfo(MemberInfo _memberInfo, MethodInfo _methodInfo, FieldInfo _fieldInfo)
	{
		base.SetInfo(_memberInfo, _methodInfo, _fieldInfo);

		if (fieldInfo != null)
		{
			tunableType = fieldInfo.FieldType;
		}
		else if (methodInfo != null)
		{
			if (methodInfo.GetParameters().Length == 1)
			{
				tunableType = methodInfo.GetParameters()[0].ParameterType;
			}			
		}
	}

	/// <summary>
	/// Registers this type and all the classes that inherit from it with the attribute manager.
	/// </summary>
	public new static void RegisterTypeWithManager()
	{
		AttributeCacheManager.Instance.RegisterCustomAttributeType<DebugTunable>();
	}
}
