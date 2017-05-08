//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;

namespace HUX.Dialogs.Debug
{
    /// <summary>
    /// The base class for all items that can be added to the debug menu.
    /// </summary>
    public abstract class DebugMenuItem : MonoBehaviour
    {
        /// <summary>
        /// The UI element to display the item title in.
        /// </summary>
        [SerializeField]
        private Text m_Title;

        /// <summary>
        /// The items title.
        /// </summary>
        public string Title
        {
            get
            {
                if (m_Title != null)
                {
                    return m_Title.text;
                }

                return null;
            }

            set
            {
                if (m_Title != null)
                {
                    m_Title.text = value;
                }
            }
        }        
    }
}