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
    /// A debug menu item that displays a button for the user to click.
    /// </summary>
    public class DebugButtonItem : DebugMenuItem
    {
        /// <summary>
        /// The text that sits on the button.
        /// </summary>
        [SerializeField]
        private Text m_BtnText;

        /// <summary>
        /// The function to call when the button is pressed.
        /// </summary>
        private Action m_Callback;

		/// <summary>
		/// The single argument function to call when the button is pressed.
		/// </summary>
		private Action<string> m_ParamCallback;

		/// <summary>
		/// The argument to pass when the function is called
		/// </summary>
		private string m_Argument;

		/// <summary>
		/// Setup method.
		/// </summary>
		/// <param name="name">The name of this debug item.</param>
		/// <param name="buttonName">The text to put on the button.</param>
		/// <param name="callback">The function to call when the button is pressed.</param>
		public void Setup(string name, string buttonName, Action callback)
        {
            Title = name;
            m_Callback = callback;

            if (m_BtnText != null)
            {
                m_BtnText.text = buttonName;
            }
        }

		/// <summary>
		/// Setup method.
		/// </summary>
		/// <param name="name">The name of this debug item.</param>
		/// <param name="buttonName">The text to put on the button.</param>
		/// <param name="callback">The function to call when the button is pressed.</param>
		/// <param name="argument">The argument to pass to the function that is called when the button is pressed.</param>
		public void Setup(string name, string buttonName, Action<string> callback, string argument)
		{
			m_ParamCallback = callback;
			m_Argument = argument;

			Setup(name, buttonName, null);
		}

		/// <summary>
		/// The public button callback.
		/// </summary>
		public void OnButtonTriggered()
        {
			if (m_Callback != null)
			{
				m_Callback();
			}
			else if (m_ParamCallback != null)
			{
				m_ParamCallback(m_Argument);
			}
        }
    }
}
