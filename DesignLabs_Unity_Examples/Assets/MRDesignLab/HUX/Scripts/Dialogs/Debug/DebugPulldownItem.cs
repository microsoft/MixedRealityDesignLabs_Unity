//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HUX.Dialogs.Debug
{
	/// <summary>
	/// A type for displaying an enum in.
	/// </summary>
	public class DebugPulldownItem : DebugMenuItem
	{
		/// <summary>
		/// The UI element to display the current enum value in.
		/// </summary>
		[SerializeField]
		private Text m_ValueDisplay;

		/// <summary>
		/// The type of enum we are displaying.
		/// </summary>
		private Type m_EnumType;

		/// <summary>
		/// The callback to call when the enum value changes.
		/// </summary>
		private Action<int> m_Callback;

		/// <summary>
		/// The callback to call when to update the enum value.
		/// </summary>
		private Func<int> m_ValueFunc;

		/// <summary>
		/// The minimum enum value.
		/// </summary>
		private int m_Min;

		/// <summary>
		/// The maximum enum value.
		/// </summary>
		private int m_Max;

		/// <summary>
		/// The current enum value.
		/// </summary>
		private int m_Value;

		/// <summary>
		/// Setup function.
		/// </summary>
		/// <param name="name">The name of this item.</param>
		/// <param name="enumType">The type of the enum.</param>
		/// <param name="callback">The function to call when the value changes.</param>
		/// <param name="valueFunc">The function to call to get the updated value.</param>
		/// <param name="value">The initial value.</param>
		/// <param name="min">The minimum enum value.</param>
		/// <param name="max">The maximum enum value.</param>
		public void Setup(string name, Type enumType, Action<int> callback, Func<int> valueFunc, int value, int min, int max)
		{
			Title = name;
			m_EnumType = enumType;
			
			m_Min = min;
			m_Max = max;

			// We need to set the callback after setting m_Min and m_Max.  *Important* bug fix here.
			m_Callback = callback;
			m_ValueFunc = valueFunc;

			if (m_ValueFunc != null)
			{
				value = m_ValueFunc();
			}

			this.SetValue(value, false);
		}

		/// <summary>
		/// Called when the increment button is pressed.
		/// </summary>
		public void OnInc()
		{
			int newValue = m_Value + 1;
			if (newValue > m_Max)
			{
				newValue = m_Min;
			}

			this.SetValue(newValue, true);
		}

		/// <summary>
		/// Called when the decrement button is pressed.
		/// </summary>
		public void OnDec()
		{
			int newValue = m_Value - 1;
			if (newValue < m_Min)
			{
				newValue = m_Max;
			}

			this.SetValue(newValue, true);
		}

		/// <summary>
		/// Called to chance the enum value and update the display.
		/// </summary>
		/// <param name="newValue">The new value to set.</param>
		/// <param name="callCallback">If true we call the callback.</param>
		protected void SetValue(int newValue, bool callCallback)
		{
			m_Value = newValue;

			if (m_ValueDisplay != null)
			{
				m_ValueDisplay.text = Enum.GetName(m_EnumType, m_Value);
			}

			if (m_Callback != null && callCallback)
			{
				m_Callback(m_Value);
			}
		}

		/// <summary>
		/// Windows update Function.
		/// </summary>
		private void Update()
		{
			if (m_ValueFunc != null)
			{
				SetValue(m_ValueFunc(), false);
			}
		}
	}
}
