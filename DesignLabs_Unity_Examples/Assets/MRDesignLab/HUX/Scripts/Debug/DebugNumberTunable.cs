//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class DebugNumberTunable : DebugTunable
{
	/// <summary>
	/// The minimum number for number fields.
	/// </summary>
	public float Fmin = 1.0f;

	/// <summary>
	/// The maximum number for number fields
	/// </summary>
	public float Fmax = 10.0f;

	/// <summary>
	/// The step amount for number fields.
	/// </summary>
	public float FStep = 0.1f;

	/// <summary>
	/// Number tuneable constructor.
	/// </summary>
	/// <param name="itemName">The name of the debug item. (Including group name.)</param>
	/// <param name="minRange">The minimum number this number item can be.</param>
	/// <param name="maxRange">The maximum number this number item can be.</param>
	/// <param name="step">The step to change the number by.</param>
	public DebugNumberTunable(string itemName = null, float minRange = 1.0f, float maxRange = 10.0f, float step = -1) : base(itemName)
	{
		Fmin = minRange;
		Fmax = maxRange;

		if (step == -1)
		{
			FStep = (Fmax - Fmin) / 20f;
		}
		else
		{
			FStep = step;
		}
	}

	public bool HasRange()
	{
		return Fmin != 0 || Fmax != 0;
	}

	/// <summary>
	/// Map a int range to the new range provided.
	/// </summary>
	/// <param name="value">The value to max.</param>
	/// <param name="out_min">The min of the new range.</param>
	/// <param name="out_max">The max of the new range.</param>
	/// <returns>The new value set between the range provided.</returns>
	/// <remarks>MapRange maps an input value between the tunable's [fmin,fmax] to output between [out_min,out_max]</remarks>
	public int MapRange(int value, float out_min, float out_max)
	{
		if (Fmin != 0 || Fmax != 0)
		{
			return (int)MapRange(value, Fmin, Fmax, out_min, out_max);
		}
		return value;
	}

	/// <summary>
	/// Map a float range to the new range provided.
	/// </summary>
	/// <param name="value">The value to max.</param>
	/// <param name="out_min">The min of the new range.</param>
	/// <param name="out_max">The max of the new range.</param>
	/// <returns>The new value set between the range provided.</returns>
	public float MapRange(float value, float out_min, float out_max)
	{
		if (Fmin != 0 || Fmax != 0)
		{
			return MapRange(value, Fmin, Fmax, out_min, out_max);
		}
		return value;
	}

	/// <summary>
	/// Maps a number from one range to another range.
	/// </summary>
	/// <param name="value">The value to map.</param>
	/// <param name="in_min">The minimum of the source range.</param>
	/// <param name="in_max">The maximum of the source range.</param>
	/// <param name="out_min">The minimum of the output range.</param>
	/// <param name="out_max">The maximum of the output range.</param>
	/// <returns>The new value set between the output range.</returns>
	/// <remarks>Generic function to map value from input range [in_min,in_max] to output range [out_min,out_max]</remarks>
	public static float MapRange(float value, float in_min, float in_max, float out_min, float out_max)
	{
		float p = (value - in_min) / (in_max - in_min);
		return (float)out_min + p * (float)(out_max - out_min);
	}
}
