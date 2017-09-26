//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HUX.Spatial;
using HUX.Speech;
using HUX.Utility;


namespace HUX.Dialogs.Debug
{
    /// <summary>
    /// A class for displaying debug logs on device.
    /// </summary>
    public class DebugLog : MonoBehaviour
    {
        /// <summary>
        /// The list of all active Debug logs to allow for easier adding of messages to all of them.
        /// </summary>
        protected static List<DebugLog> s_DebugOutputs = new List<DebugLog>();

        #region Static Methods
        /// <summary>
        /// Adds a message to all debug logs to be displayed.
        /// </summary>
        /// <param name="logString">The log to display.</param>
        /// <param name="type">The type of log.</param>
        public static void AddMessage(string logString, LogType type = LogType.Log)
        {
            for (int index = 0; index < s_DebugOutputs.Count; index++)
            {
                s_DebugOutputs[index].AddLog(logString, type);
            }
        }

        /// <summary>
        /// Adds a message to all debug logs to be displayed but waits until the next update.
        /// This is useful for calls that have been made from other threads.
        /// </summary>
        /// <param name="logString">The log to display.</param>
        /// <param name="type">The type of log.</param>
        public static void AddMessageDelayed(string logString, LogType type = LogType.Log)
        {
            for (int index = 0; index < s_DebugOutputs.Count; index++)
            {
                s_DebugOutputs[index].AddLogDelayed(logString, type);
            }
        }

        /// <summary>
        /// Function to return if a DebugLogEntry is currently being used as a 
        /// log entry by any logs.  Used by the pool to determine if a entry can be used.
        /// (Inactive entries may merely be off because they are filtered for display.)
        /// </summary>
        /// <param name="entry">The entry to check for current use.</param>
        public static bool IsLogEntryInUse(DebugLogEntry entry)
        {
            foreach (DebugLog log in s_DebugOutputs)
            {
                if (log.m_LogEntries.Contains(entry))
                    return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// The prefab to use when creating new display entries.
        /// </summary>
        [SerializeField, Tooltip("The prefab to use when creating new display entries.")]
        private DebugLogEntry m_LogEntryPrefab;

        /// <summary>
        /// The prefab to sue when creating a new display entry with a stack trace.
        /// </summary>
        [SerializeField, Tooltip("The prefab to sue when creating a new display entry with a stack trace.")]
        private DebugLogEntry m_LogWithStackPrefab;

        /// <summary>
        /// The transform to store pooled log entry gameobjects.
        /// </summary>
        [SerializeField, Tooltip("The transform to store pooled log entry gameobjects.")]
        private Transform m_LogEntryPool;

        /// <summary>
        /// The Layout to add new display entries to.
        /// </summary>
        [SerializeField, Tooltip("The layout group to add new log display entires to.")]
        private LayoutGroup m_LogContent;

        /// <summary>
        /// The scroll rect to adjust when new entries are added.
        /// </summary>
        [SerializeField, Tooltip("The scroll rect to adjust when new entries are added.")]
        private ScrollRect m_ScrollRect;

        /// <summary>
        /// The game object to hide/show when the log is hidden and shown.
        /// </summary>
        [SerializeField, Tooltip("The GameObject to hide/show when the log is hidden and shown.")]
        private GameObject m_DebugLogObj;

        /// <summary>
        /// The SloverLink to turn off/on when the debug log is pinned/unpinned.
        /// </summary>
        [SerializeField, Tooltip("The SloverLink to turn off/on when the debug menu is pinned/unpinned.")]
        private SolverHandler m_SolverHandler;

        /// <summary>
        /// The pin button to update when the debug log is pinned/unpinned.
        /// </summary>
        [SerializeField, Tooltip("The pin button to update when the debug log is pinned/unpinned.")]
        private Toggle m_PinButton;

        [Header("Configuration")]
        /// <summary>
        /// If true adds options to show/hide and configure the debug output in the debug menu if it exists.
        /// </summary>
        [SerializeField, Tooltip("If true adds options to show/hide and configure the debug output in the debug menu if it exists.")]
        private bool m_AddDebugMenuOptions = true;

        /// <summary>
        /// The maximum number of lines to display in the log.  -1 is unlimited.
        /// </summary>
        [SerializeField, Tooltip("The maximum number of lines to display in the log.  -1 is unlimited.")]
        private int maxLines = -1;

        /// <summary>
        /// The types of log this debug log shows.
        /// </summary>
        [SerializeField]
        private List<LogType> logTypes = new List<LogType>();

        [Header("Voice Commands")]
        /// <summary>
        /// The Speech command spoken to show the debug log.
        /// </summary>
        public string OpenLogKeyword = "open debug log";

        /// <summary>
        /// The Speech command spoken to hide the debug log.
        /// </summary>
        public string CloseLogKeyword = "close debug log";

        /// <summary>
        /// The key pressed to show the debug log. (Only works in editor.)
        /// </summary>
        [Tooltip("The key pressed to show the debug log. (Only works in editor.)")]
        public KeyCode LogOpenKey = KeyCode.Home;

        /// <summary>
        /// The key pressed to hide the debug log. (Only works in editor.)
        /// </summary>
        [Tooltip("The key pressed to hide the debug log. (Only works in editor.)")]
        public KeyCode LogCloseKey = KeyCode.End;

        /// <summary>
        /// The controller pressed to toggle the debug log.
        /// </summary>
        [Tooltip("The controller label for toggling log")]
        public string LogToggleControllerMap = "Xbox_ViewButton";

        /// <summary>
        /// The Speech command spoken to scroll up the debug log.
        /// </summary>
        public string ScrollUpKeyword = "scroll up";

        /// <summary>
        /// The Speech command spoken to scroll down the debug log.
        /// </summary>
        public string ScrollDownKeyword = "scroll down";

        /// <summary>
        /// The Speech command spoken to scroll to the top of the debug log.
        /// </summary>
        public string GoToTopKeyword = "go to top";

        /// <summary>
        /// The Speech command spoken to scroll to the bottom of the debug log.
        /// </summary>
        public string GoToBottomKeyword = "go to bottom";

        /// <summary>
        /// The Speech command spoken to show messages.
        /// </summary>
        public string ShowMessagesKeyword = "show messages";

        /// <summary>
        /// The Speech command spoken to hide messages.
        /// </summary>
        public string HideMessagesKeyword = "hide messages";

        /// <summary>
        /// The Speech command spoken to toggle messages.
        /// </summary>
        public string ToggleMessagesKeyword = "toggle messages";

        /// <summary>
        /// The Speech command spoken to show warnings.
        /// </summary>
        public string ShowWarningsKeyword = "show warnings";

        /// <summary>
        /// The Speech command spoken to hide warnings.
        /// </summary>
        public string HideWarningsKeyword = "hide warnings";

        /// <summary>
        /// The Speech command spoken to toggle warnings.
        /// </summary>
        public string ToggleWarningsKeyword = "toggle warnings";

        /// <summary>
        /// The Speech command spoken to show errors.
        /// </summary>
        public string ShowErrorsKeyword = "show errors";

        /// <summary>
        /// The Speech command spoken to hide errors.
        /// </summary>
        public string HideErrorsKeyword = "hide errors";

        /// <summary>
        /// The Speech command spoken to toggle errors.
        /// </summary>
        public string ToggleErrorsKeyword = "toggle errors";

        /// <summary>
        /// The Speech command spoken to clear the log.
        /// </summary>
        public string ClearLogKeyword = "clear log";

        [Header("VR Tuneables")]

        /// <summary>
        /// The scale when displayed on Oculus
        /// </summary>
        [Tooltip("The scale when displayed on Oculus")]
        public float m_ScaleForHMD = 0.0015f;

        /// <summary>
        /// The max view degrees on Oculus
        /// </summary>
        [Tooltip("The max view degrees on Oculus")]
        public float m_MaxViewDegreesForHMD = 30.0f;


        /// <summary>
        /// The list of log entries currently in the debug log.
        /// </summary>
        private List<DebugLogEntry> m_LogEntries = new List<DebugLogEntry>();

        /// <summary>
        /// If true the log will scroll to the bottom in the next update.
        /// </summary>
        private bool m_GoToBottom;

        /// <summary>
        /// The logs to add on the next update.
        /// </summary>
        private List<KeyValuePair<string, LogType>> m_LogsToAdd = new List<KeyValuePair<string, LogType>>();

        private PrefabObjectPool<DebugLogEntry> m_LogPool;

        private PrefabObjectPool<DebugLogEntry> m_LogWithStackPool;

        /// <summary>
        /// If true the debug log is visible.
        /// </summary>
        public bool Visible
        {
            get { return m_DebugLogObj.activeInHierarchy; }
            set { m_DebugLogObj.SetActive(value); }
        }

        /// <summary>
        /// If true the LogType.Log is stored in this debug log.
        /// </summary>
        public bool ShowLogs
        {
            get { return logTypes.Contains(LogType.Log); }
            set { IncludeLogType(LogType.Log, value); }
        }

        /// <summary>
        /// If true the LogType.Warning is stored in this debug log.
        /// </summary>
        public bool ShowWarnings
        {
            get { return logTypes.Contains(LogType.Warning); }
            set { IncludeLogType(LogType.Warning, value); }
        }

        /// <summary>
        /// If true the LogTypes LogType.Error, LogType.Assert, LogType.Exception are stored in this debug log.
        /// </summary>
        public bool ShowErrors
        {
            get { return logTypes.Contains(LogType.Error); }
            set
            {
                IncludeLogType(LogType.Error, value);
                IncludeLogType(LogType.Assert, value);
                IncludeLogType(LogType.Exception, value);
            }
        }

        /// <summary>
        /// If true the LogType.Log is displayed in this debug log.
        /// </summary>
        private bool filterLogs = false;
        public bool FilterLogs
        {
            get { return filterLogs; }
            set
            {
                filterLogs = value;
                buttonFilterLogs.image.color = !filterLogs ? Color.white : Color.grey;
                FilterEntries(LogType.Log, !filterLogs);
            }
        }

        /// <summary>
        /// If true the LogType.Warning is displayed in this debug log.
        /// </summary>
        private bool filterWarnings = false;
        public bool FilterWarnings
        {
            get { return filterWarnings; }
            set
            {
                filterWarnings = value;
                buttonFilterWarnings.image.color = !filterWarnings ? Color.white : Color.grey;
                FilterEntries(LogType.Warning, !filterWarnings);
            }
        }

        /// <summary>
        /// If true the LogTypes LogType.Error, LogType.Assert, LogType.Exception are displayed in this debug log.
        /// </summary>
        private bool filterErrors = false;
        public bool FilterErrors
        {
            get { return filterErrors; }
            set
            {
                filterErrors = value;
                buttonFilterErrors.image.color = !FilterErrors ? Color.white : Color.grey;
                FilterEntries(LogType.Assert, !FilterErrors);
                FilterEntries(LogType.Error, !FilterErrors);
                FilterEntries(LogType.Exception, !FilterErrors);
            }
        }

        [Header("Filter Buttons")]
        public Button buttonFilterLogs;
        public Button buttonFilterWarnings;
        public Button buttonFilterErrors;

        #region Unity Methods.
        /// <summary>
        /// Unity standard Awake method
        /// </summary>
        private void Awake()
        {
            if (m_AddDebugMenuOptions && DebugMenu.Instance != null)
            {
                //add debug menu options
                DebugMenu.Instance.AddBoolItem("Debug Log\\Show Debug Output:", (bool newVal) => { Visible = newVal; }, null, Visible);
                DebugMenu.Instance.AddBoolItem("Debug Log\\Show Debug Logs:", (bool newVal) => { ShowLogs = newVal; }, null, ShowLogs);
                DebugMenu.Instance.AddBoolItem("Debug Log\\Show Debug Warnings:", (bool newVal) => { ShowWarnings = newVal; }, null, ShowWarnings);
                DebugMenu.Instance.AddBoolItem("Debug Log\\Show Debug Errors:", (bool newVal) => { ShowErrors = newVal; }, null, ShowErrors);

                DebugMenu.Instance.SetGroupPage("Debug Log", "Debug Log");
            }

            int initPoolMax = maxLines >= 0 ? maxLines : 10;

            m_LogPool = new PrefabObjectPool<DebugLogEntry>(m_LogEntryPrefab, m_LogEntryPool, initPoolMax);
            m_LogWithStackPool = new PrefabObjectPool<DebugLogEntry>(m_LogWithStackPrefab, m_LogEntryPool, initPoolMax);

            Application.logMessageReceived += HandleLog;
            s_DebugOutputs.Add(this);

            if (m_DebugLogObj && (UnityEngine.XR.XRSettings.loadedDeviceName == "Oculus" || UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR"))
            {
                // Scale up
                RectTransform rectTransform = m_DebugLogObj.GetComponent<RectTransform>();
                if (rectTransform)
                {
                    rectTransform.localScale = new Vector3(m_ScaleForHMD, m_ScaleForHMD, 1.0f);
                }

                SolverRectView solverRectView = m_DebugLogObj.GetComponent<SolverRectView>();
                if (solverRectView)
                {
                    solverRectView.MaxViewDegrees = new Vector2(m_MaxViewDegreesForHMD, m_MaxViewDegreesForHMD);
                }
            }
        }


        /// <summary>
        /// Unity standard Start method
        /// </summary>
        private void Start()
        {
            if (KeywordManager.Instance != null)
            {
                KeywordManager.Instance.AddKeyword(OpenLogKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(CloseLogKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ScrollUpKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ScrollDownKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(GoToTopKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(GoToBottomKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ShowMessagesKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(HideMessagesKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ToggleMessagesKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ShowWarningsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(HideWarningsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ToggleWarningsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ShowErrorsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(HideErrorsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ToggleErrorsKeyword, OnWord, KeywordConfidenceLevel.Low);
                KeywordManager.Instance.AddKeyword(ClearLogKeyword, OnWord, KeywordConfidenceLevel.Low);
            }

            buttonFilterLogs.transform.Find("Text").GetComponent<Text>().text = "0";
            buttonFilterWarnings.transform.Find("Text").GetComponent<Text>().text = "0";
            buttonFilterErrors.transform.Find("Text").GetComponent<Text>().text = "0";
        }

        /// <summary>
        /// Unity standard OnDestroy method
        /// </summary>
        private void OnDestroy()
        {
            s_DebugOutputs.Remove(this);
            if(KeywordManager.Instance != null)
            {
                KeywordManager.Instance.RemoveKeyword(OpenLogKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(CloseLogKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ScrollUpKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ScrollDownKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(GoToTopKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(GoToBottomKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ShowMessagesKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(HideMessagesKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ToggleMessagesKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ShowWarningsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(HideWarningsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ToggleWarningsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ShowErrorsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(HideErrorsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ToggleErrorsKeyword, OnWord);
                KeywordManager.Instance.RemoveKeyword(ClearLogKeyword, OnWord);
            }
        }

        /// <summary>
        /// Unity standard Update method
        /// </summary>
        private void Update()
        {
            if (m_LogsToAdd.Count > 0)
            {
                m_LogContent.enabled = false;

                while (m_LogsToAdd.Count > 0)
                {
                    KeyValuePair<string, LogType> log = m_LogsToAdd[0];
                    m_LogsToAdd.RemoveAt(0);

                    this.AddLog(log.Key, log.Value);
                }

                m_LogContent.enabled = true;
            }

            if (m_GoToBottom)
            {
                m_GoToBottom = false;
                m_ScrollRect.verticalNormalizedPosition = 0;
            }

#if !UNITY_METRO || UNITY_EDITOR
            if (Input.GetKeyDown(LogOpenKey))
            {
                Activate();
            }
            else if (Input.GetKeyDown(LogCloseKey))
            {
                Deactivate();
            }
#endif
            if (Input.GetButtonDown(LogToggleControllerMap))
            {
                if (m_DebugLogObj.activeSelf)
                {
                    Deactivate();
                }
                else
                {
                    Activate();
                }
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Makes the debug log stay in place when the user looks around.
        /// </summary>
        public void Pin()
        {
            if (m_PinButton != null)
            {
                m_PinButton.isOn = true;
            }
        }

        /// <summary>
        /// Makes the debug log follow the users gaze.
        /// </summary>
        public void UnPin()
        {
            if (m_PinButton != null)
            {
                m_PinButton.isOn = false;
            }
        }

        /// <summary>
        /// Public callback for the pins button toggle event.
        /// </summary>
        /// <param name="toggleBtn"></param>
        public void PinBtnPressed(Toggle toggleBtn)
        {
            m_SolverHandler.enabled = !toggleBtn.isOn;
        }

        /// <summary>
        /// Shows the debug log.
        /// </summary>
        public void Activate()
        {
            m_DebugLogObj.SetActive(true);
            UnPin();
        }

        /// <summary>
        /// Hides the debug log.
        /// </summary>
        public void Deactivate()
        {
            m_DebugLogObj.SetActive(false);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Helper method to make sure a type is added or removed from the log types.
        /// </summary>
        /// <param name="t">The log type to add or remove.</param>
        /// <param name="isIncluded">If true the log type will be added, if false the log type will be removed.</param>
        private void IncludeLogType(LogType t, bool isIncluded)
        {
            if (!logTypes.Contains(t) && isIncluded)
            {
                logTypes.Add(t);
            }
            else if (logTypes.Contains(t) && !isIncluded)
            {
                logTypes.Remove(t);
            }
        }

        /// <summary>
        /// Callback Method for handling standard Unity log messages.
        /// </summary>
        /// <param name="logString">The log string.</param>
        /// <param name="stackTrace">The stack associated with the log.</param>
        /// <param name="type">The type of the log.</param>
        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            //only add logs that are of the specified LogType
            bool typeFound = false;
            foreach (LogType t in logTypes)
            {
                if (t == type)
                {
                    typeFound = true;
                    break;
                }
            }
            if (!typeFound)
                return;

            //color text based on log type
            this.AddLog(logString, stackTrace, type);
        }

        /// <summary>
        /// Sets a log to be added at the next Update.
        /// </summary>
        /// <param name="logString">The log.</param>
        /// <param name="type">The type of the log.</param>
        private void AddLogDelayed(string logString, LogType type)
        {
            m_LogsToAdd.Add(new KeyValuePair<string, LogType>(logString, type));
        }

        /// <summary>
        /// Adds a log to the current display.
        /// </summary>
        /// <param name="logString">The log.</param>
        /// <param name="type">The type of the log.</param>
        private void AddLog(string logString, LogType type)
        {
            AddLog(logString, null, type);
        }

        private void AddLog(string logString, string stacktrace, LogType type)
        {
            if (this.maxLines == 0)
            {
                return;
            }

            m_GoToBottom = m_ScrollRect.verticalNormalizedPosition == 0;

            if (this.maxLines > 0 && m_LogEntries.Count + 1 > this.maxLines)
            {
                DebugLogEntry entry = m_LogEntries[0];
                m_LogEntries.RemoveAt(0);
                entry.ReturnToPool();
                entry.transform.SetParent(m_LogEntryPool);
            }

            DebugLogEntry newLog = null;

            if (string.IsNullOrEmpty(stacktrace))
            {
                if (m_LogPool.Count == 0)
                {
                    m_LogPool.IncreasePool(10);
                }
                newLog = m_LogPool.GetObject();
            }
            else
            {
                if (m_LogWithStackPool.Count == 0)
                {
                    m_LogWithStackPool.IncreasePool(10);
                }
                newLog = m_LogWithStackPool.GetObject();
            }

            newLog.gameObject.SetActive(true);
            newLog.transform.SetParent(m_LogContent.transform);
            newLog.transform.SetSiblingIndex(newLog.transform.parent.childCount - 1);
            newLog.SetText(logString, stacktrace, type);

            m_LogEntries.Add(newLog);

            if (type == LogType.Log)
                buttonFilterLogs.transform.Find("Text").GetComponent<Text>().text = m_LogEntries.FindAll(i => i.LogType == type).Count.ToString();
            else if (type == LogType.Warning)
                buttonFilterWarnings.transform.Find("Text").GetComponent<Text>().text = m_LogEntries.FindAll(i => i.LogType == type).Count.ToString();
            else if (type == LogType.Error)
                buttonFilterErrors.transform.Find("Text").GetComponent<Text>().text = m_LogEntries.FindAll(i => i.LogType == LogType.Assert || i.LogType == LogType.Error || i.LogType == LogType.Exception).Count.ToString();
        }

        /// <summary>
        /// The speech callback.
        /// </summary>
        /// <param name="args"></param>
        private void OnWord(KeywordRecognizedEventArgs args)
        {
            if (args.text == OpenLogKeyword)
            {
                Activate();
            }
            else if (args.text == CloseLogKeyword)
            {
                Deactivate();
            }
            else if (args.text == ScrollUpKeyword)
            {
                OnScrollAmount(5);
            }
            else if (args.text == ScrollDownKeyword)
            {
                OnScrollAmount(-5);
            }
            else if (args.text == GoToTopKeyword)
            {
                OnScrollPosition(1);
            }
            else if (args.text == GoToBottomKeyword)
            {
                OnScrollPosition(0);
            }
            else if (args.text == ShowMessagesKeyword)
            {
                FilterLogs = false;
            }
            else if (args.text == HideMessagesKeyword)
            {
                FilterLogs = true;
            }
            else if (args.text == ToggleMessagesKeyword)
            {
                OnClickFilterLogs();
            }
            else if (args.text == ShowWarningsKeyword)
            {
                FilterWarnings = false;
            }
            else if (args.text == HideWarningsKeyword)
            {
                FilterWarnings = true;
            }
            else if (args.text == ToggleWarningsKeyword)
            {
                OnClickFilterWarnings();
            }
            else if (args.text == ShowErrorsKeyword)
            {
                FilterErrors = false;
            }
            else if (args.text == HideErrorsKeyword)
            {
                FilterErrors = true;
            }
            else if (args.text == ToggleErrorsKeyword)
            {
                OnClickFilterErrors();
            }
            else if (args.text == ClearLogKeyword)
            {
                ClearLog();
            }
        }

        public void OnScrollAmount(float amount)
        {
            m_ScrollRect.verticalNormalizedPosition = Mathf.Clamp01(m_ScrollRect.verticalNormalizedPosition + ((1f / m_LogEntries.Count) * amount));
        }

        public void OnScrollPosition(float position)
        {
            m_ScrollRect.verticalNormalizedPosition = position;
        }

        public void OnClickFilterLogs()
        {
            FilterLogs = !FilterLogs;
        }

        public void OnClickFilterWarnings()
        {
            FilterWarnings = !FilterWarnings;
        }

        public void OnClickFilterErrors()
        {
            FilterErrors = !FilterErrors;
        }

        private void FilterEntries(LogType type, bool filter)
        {
            foreach (DebugLogEntry entry in m_LogEntries.FindAll(i => i.LogType == type))
                entry.gameObject.SetActive(filter);
        }

        public void ClearLog()
        {
            foreach (DebugLogEntry entry in m_LogEntries)
            {
                entry.transform.SetParent(m_LogEntryPool);
                entry.ReturnToPool();
            }
            m_LogEntries.Clear();

            buttonFilterLogs.transform.Find("Text").GetComponent<Text>().text = "0";
            buttonFilterWarnings.transform.Find("Text").GetComponent<Text>().text = "0";
            buttonFilterErrors.transform.Find("Text").GetComponent<Text>().text = "0";
        }

        #endregion

    }
}
