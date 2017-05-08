//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using HUX.Utility;
using System;

namespace HUX.Dialogs.Debug
{
    public class DebugLogEntry : MonoBehaviour, IPoolable
    {
        /// <summary>
        /// The title for the log entry.
        /// </summary>
        [SerializeField]
        private Text m_Title;

        /// <summary>
        /// The log line for the log entry.
        /// </summary>
        [SerializeField]
        private Text m_Line;

        /// <summary>
        /// The statck trace line for the log entry.
        /// </summary>
        [SerializeField]
        private Text m_StackTrace;

        /// <summary>
        /// The log type for the log entry.
        /// </summary>
        [SerializeField]
        public LogType LogType { get; private set; }

        /// <summary>
        /// The color to make the title when the LogType is Log
        /// </summary>
        [SerializeField]
        private Color m_LogTitleColor = Color.black;

        /// <summary>
        /// The color to make the title when the LogType is Warning
        /// </summary>
        [SerializeField]
        private Color m_WarningTitleColor = Color.yellow;

        /// <summary>
        /// The color to make the title when the LogType is Error
        /// </summary>
        [SerializeField]
        private Color m_ErrorTitleColor = Color.red;

        /// <summary>
        /// The color to make the title when the LogType is Assert
        /// </summary>
        [SerializeField]
        private Color m_AssertTitleColor = Color.red;

        /// <summary>
        /// The color to make the title when the LogType is Exception
        /// </summary>
        [SerializeField]
        private Color m_ExceptionTitleColor = Color.red;


		public bool IsActive
		{
			get
			{
//				return this.gameObject.activeSelf;
                return DebugLog.IsLogEntryInUse(this);
			}
		}

		public void ReturnToPool()
		{
			this.gameObject.SetActive(false);
		}

		/// <summary>
		/// Set the log text.
		/// </summary>
		/// <param name="line">The line to display.</param>
		/// <param name="stackTrace">The stack trace to display.</param>
		/// <param name="type">The log type.</param>
		public void SetText(string line, string stackTrace, LogType type)
        {
            LogType = type;

            switch (type)
            {
                case LogType.Log:
                {
                    m_Title.color = m_LogTitleColor;
                    m_Title.text = "Log:";
                    break;
                }

                case LogType.Warning:
                {
                    m_Title.color = m_WarningTitleColor;
                    m_Title.text = "Warning:";
                    break;
                }

                case LogType.Error:
                {
                    m_Title.color = m_ErrorTitleColor;
                    m_Title.text = "Error:";
                    break;
                }

                case LogType.Assert:
                {
                    m_Title.color = m_AssertTitleColor;
                    m_Title.text = "Assert:";
                    break;
                }

                case LogType.Exception:
                {
                    m_Title.color = m_ExceptionTitleColor;
                    m_Title.text = "Exception:";
                    break;
                }
            }

            m_Line.text = line;

            if (m_StackTrace != null)
            {
                m_StackTrace.text = stackTrace;
            }
        }
    }
}
