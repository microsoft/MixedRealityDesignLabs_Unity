//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System;

namespace HUX.Dialogs.Debug
{
	public class DebugVector3Item : DebugMenuItem
	{
		#region Editor Variables
		/// <summary>
		/// The X display item.
		/// </summary>
		public Text m_XVal;

		/// <summary>
		/// The Y display item.
		/// </summary>
		public Text m_YVal;

		/// <summary>
		/// The Z display item.
		/// </summary>
		public Text m_ZVal;
		#endregion


		#region Privation Variables
		/// <summary>
		/// The current value of the debug item.
		/// </summary>
		private Vector3 m_CurrentVal = Vector3.zero;

		/// <summary>
		/// The amount to change X,Y,Z by when incrementing and decrementing.
		/// </summary>
		private Vector3 m_Step = Vector3.one;

		/// <summary>
		/// The function to call when the value has been changed by the debug item.
		/// </summary>
		private Action<Vector3> m_Callback;

		/// <summary>
		/// The function to get the current value.
		/// </summary>
		private Func<Vector3> m_ValueFunc;
		#endregion

		#region Monobehaviour Function
		/// <summary>
		/// Standard MonoBehaviour Update
		/// </summary>
		private void Update()
		{
			if (m_ValueFunc != null)
			{
				Vector3 oldValue = m_CurrentVal;
				m_CurrentVal = m_ValueFunc();
				if (oldValue != m_CurrentVal)
				{
					UpdateDisplay();
				}
			}
		}
		#endregion

		#region Public Functions
		/// <summary>
		/// The Vector3 setup item
		/// </summary>
		/// <param name="name">The display name of this item.</param>
		/// <param name="callback">The callback function to call when the value changes.</param>
		/// <param name="valFunc">The function to call to get the current value. Can be null.</param>
		/// <param name="initialVal">The initial value of this item</param>
		/// <param name="step">The amount of change when dec/inc is pressed.</param>
		public void Setup(string name, Action<Vector3> callback, Func<Vector3> valFunc, Vector3 initialVal, Vector3 step)
		{
			Title = name;
			m_Callback = callback;
			m_ValueFunc = valFunc;
			m_CurrentVal = initialVal;
			m_Step = step;

			if (m_ValueFunc != null)
			{
				m_CurrentVal = m_ValueFunc();
			}

			UpdateDisplay();
		}

		/// <summary>
		/// X Inc Callback.
		/// </summary>
		public void OnXInc()
		{
			m_CurrentVal.x += m_Step.x;
			UpdateDisplay();
			DoCallback();
		}

		/// <summary>
		/// X Dec Callback.
		/// </summary>
		public void OnXDec()
		{
			m_CurrentVal.x -= m_Step.x;
			UpdateDisplay();
			DoCallback();
		}

		/// <summary>
		/// Y Inc Callback.
		/// </summary>
		public void OnYInc()
		{
			m_CurrentVal.y += m_Step.y;
			UpdateDisplay();
			DoCallback();
		}

		/// <summary>
		/// Y Dec Callback.
		/// </summary>
		public void OnYDec()
		{
			m_CurrentVal.y -= m_Step.y;
			UpdateDisplay();
			DoCallback();
		}

		/// <summary>
		/// Z Inc Callback.
		/// </summary>
		public void OnZInc()
		{
			m_CurrentVal.z += m_Step.z;
			UpdateDisplay();
			DoCallback();
		}

		/// <summary>
		/// Z Dec Callback.
		/// </summary>
		public void OnZDec()
		{
			m_CurrentVal.z -= m_Step.z;
			UpdateDisplay();
			DoCallback();
		}
		#endregion

		#region Private Functions
		/// <summary>
		/// Updates the debug display.
		/// </summary>
		private void UpdateDisplay()
		{
			if (m_XVal != null)
			{
				m_XVal.text = m_CurrentVal.x.ToString();
			}

			if (m_YVal != null)
			{
				m_YVal.text = m_CurrentVal.y.ToString();
			}

			if (m_ZVal != null)
			{
				m_ZVal.text = m_CurrentVal.z.ToString();
			}
		}

		/// <summary>
		/// Calls the callback with the current value.
		/// </summary>
		private void DoCallback()
		{
			if (m_Callback != null)
			{
				m_Callback(m_CurrentVal);
			}
		}
		#endregion
	}
}
