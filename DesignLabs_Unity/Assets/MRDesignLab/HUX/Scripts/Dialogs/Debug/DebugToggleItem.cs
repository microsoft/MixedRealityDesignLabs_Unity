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
    /// A boolean toggle item for the debug menu.
    /// </summary>
    public class DebugToggleItem : DebugMenuItem
    {
        /// <summary>
        /// The UI toggle to use.
        /// </summary>
        [SerializeField]
        private Toggle m_Toggle;

        /// <summary>
        /// The function to call when the value changes.
        /// </summary>
        private Action<bool> m_Callback;

		/// <summary>
		/// The function to call to get the current value.
		/// </summary>
		private Func<bool> m_ValFunc;

		/// <summary>
		/// Setup function.
		/// </summary>
		/// <param name="name">The display name.</param>
		/// <param name="callback">The function to call when the value changes.</param>
		/// <param name="valuefunc">The function to call to get the current value.</param>
		/// <param name="initialValue">The initial value.</param>
		public void Setup(string name, Action<bool> callback, Func<bool> valueFunc, bool initialValue)
		{
			Title = name;
			m_ValFunc = valueFunc;
			if (m_ValFunc != null)
			{
				initialValue = m_ValFunc();
			}
			m_Toggle.isOn = initialValue;
			m_Callback = callback;
		}

		/// <summary>
		/// Public callback function for the toggle button.
		/// </summary>
		public void Toggle()
        {
            if (m_Callback != null)
            {
                m_Callback(m_Toggle.isOn);
            }
        }

		/// <summary>
		/// Unity Update function.
		/// </summary>
		private void Update()
		{
			if (m_ValFunc != null)
			{
				m_Toggle.isOn = m_ValFunc();
			}
		}
    }
}
