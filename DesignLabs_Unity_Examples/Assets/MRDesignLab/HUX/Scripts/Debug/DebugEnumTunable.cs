//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
public class DebugEnumTunable : DebugTunable
{
	/// <summary>
	/// The minimum enum value.
	/// </summary>
	public int MinEnumVal = -1;

	/// <summary>
	/// The maximum enum value.
	/// </summary>
	public int MaxEnumVal = -1;

	/// <summary>
	/// Enum tunable constructor.
	/// </summary>
	/// <param name="itemName">The display name for this debug item. (Including group name.)</param>
	/// <param name="minEnumVal">The minimum value for the enum value.</param>
	/// <param name="maxEnumVal">The maximum value for the enum value.</param>
	/// <remarks>If both minEnumValue and maxEnumValue are -1 then the min and max will be calculated from the enum type automatically.</remarks>
	public DebugEnumTunable(string itemName = null, int minEnumVal = -1, int maxEnumVal = -1) : base(itemName)
	{
		MinEnumVal = minEnumVal;
		MaxEnumVal = maxEnumVal;
	}
}
