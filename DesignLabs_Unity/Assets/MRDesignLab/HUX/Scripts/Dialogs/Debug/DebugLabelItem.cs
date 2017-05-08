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
    public class DebugLabelItem : DebugMenuItem
    {
        /// <summary>
        /// The text that sits on the button.
        /// </summary>
        [SerializeField]
        private Text m_LabelText;

        /// <summary>
        /// The function to call when the button is pressed.
        /// </summary>
        private Func<string> m_StringFunc;

		/// <summary>
		/// Setup method.
		/// </summary>
		/// <param name="name">The name of this debug item.</param>
		/// <param name="labelText">The text to put on the label.</param>
		/// <param name="callback">The function to call for update label text.</param>
		public void Setup(string name, string labelText, Func<string> stringFunc)
        {
            Title = name;
            m_StringFunc = stringFunc;

            if (labelText != null)
            {
                m_LabelText.text = labelText;
            }
        }

        /// <summary>
        /// Unity Update function.
        /// </summary>
        private void Update()
        {
            if (m_StringFunc != null )
            {
                string val = m_StringFunc();

                if (val != m_LabelText.text)
                    m_LabelText.text = val;
            }
        }
    }
}
