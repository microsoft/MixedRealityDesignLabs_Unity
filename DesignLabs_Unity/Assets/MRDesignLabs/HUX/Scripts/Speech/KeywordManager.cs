//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using HUX.Utility;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif

namespace HUX.Speech
{
    /// <summary>
    /// The singleton class for handling speech keywords.
    /// </summary>
    public class KeywordManager : Singleton<KeywordManager>
    {
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only field for displaying commands in inspector
        /// </summary>
        public Dictionary<string, List<string>> EditorCommandDescriptions
        {
            get
            {
                if (!Application.isEditor)
                    return null;

                Dictionary<string, List<string>> editorCommands = new Dictionary<string, List<string>>();

                foreach (KeyValuePair<string, KeywordCommandInfo> command in m_Commands)
                {
                    editorCommands.Add(command.Key, command.Value.EditorHandlerDescriptions);
                }
                return editorCommands;
            }
        }
        #endif

        /// <summary>
        /// The dictionary for tracking the commands and listeners.
        /// The key is the keyword used and the value is the object containing all the listeners and the KeywordRecognizer.
        /// </summary>
        private Dictionary<string, KeywordCommandInfo> m_Commands = new Dictionary<string, KeywordCommandInfo>();

        /// <summary>
        /// If true old keyword entries will be removed on the next update.
        /// </summary>
        private bool m_UpdateRemovals = false;

        public delegate void KeywordRecognizedDelegate(KeywordRecognizedEventArgs args);

        /// <summary>
        /// Monobehaviour Update
        /// </summary>
        protected void Update()
        {
            if (m_UpdateRemovals)
            {
                m_UpdateRemovals = false;
                foreach (KeyValuePair<string, KeywordCommandInfo> pair in m_Commands)
                {
                    pair.Value.UpdateRemovals();
                }
            }
        }

		/// <summary>
		/// Monobehaviour Destroy
		/// </summary>
		protected void OnDestroy()
		{
			foreach (KeyValuePair<string, KeywordCommandInfo> pair in m_Commands)
			{
				pair.Value.Destroy();
			}

			m_Commands.Clear();
		}

        /// <summary>
        /// Adds a callback to be called when any of the keywords get called in the list of keywords passed in.
        /// </summary>
        /// <param name="keywords">The list of keywords to track.</param>
        /// <param name="handler">The callback to call when any of the keywords are spoken.</param>
        /// <param name="level">The minimum level of confidence for any of the keywords spoken.</param>
        public void AddKeywords(IEnumerable<string> keywords, KeywordRecognizedDelegate handler, KeywordConfidenceLevel level = KeywordConfidenceLevel.Medium)
        {
            foreach (string keyword in keywords)
            {
                this.AddKeyword(keyword, handler, level);
            }
        }

        /// <summary>
        /// Adds a callback to be called if the keyword is spoken.  If a string with commas is passed in it will be split
        /// and each word seperated by a comma will be used as a keyword.
        /// </summary>
        /// <param name="wordOrPhrase">The keyword to track.</param>
        /// <param name="handler">The callback to call when the keyword is spoken.</param>
        /// <param name="level">The minimum level of confidence for any of the keywords spoken.</param>
        public void AddKeyword(string wordOrPhrase, KeywordRecognizedDelegate handler, KeywordConfidenceLevel level = KeywordConfidenceLevel.Medium)
        {
            string[] phrases = wordOrPhrase.ToLower().Split(',');

            foreach (string phrase in phrases)
            {
                string fixedPhrase = phrase.Trim();
                if (fixedPhrase.Length > 0)
                {
                    if (!m_Commands.ContainsKey(fixedPhrase))
                    {
                        m_Commands[fixedPhrase] = new KeywordCommandInfo(fixedPhrase);
                    }

                    m_Commands[fixedPhrase].AddListener(handler, level);
                }
            }
        }

        /// <summary>
        /// Removes all the keywords assosiated with the callback.
        /// </summary>
        /// <param name="keywords">The list of keywords to remove.</param>
        /// <param name="handler">The callback assosiated with them.</param>
        public void RemoveKeywords(IEnumerable<string> keywords, KeywordRecognizedDelegate handler)
        {
            foreach (string keyword in keywords)
            {
                this.RemoveKeyword(keyword, handler);
            }
        }

        /// <summary>
        /// Removed the callback assosited with the keyword passed in.  If a string with commas is passed in it will be split
        /// and each word seperated by a comma will be used as a keyword.
        /// </summary>
        /// <param name="wordOrPhrase">The keyword to remove.</param>
        /// <param name="handler">The callback to remove.</param>
        public void RemoveKeyword(string wordOrPhrase, KeywordRecognizedDelegate handler)
        {
            string[] phrases = wordOrPhrase.ToLower().Split(',');

            foreach (string phrase in phrases)
            {
                string fixedPhrase = phrase.Trim();
                if (this.m_Commands.ContainsKey(fixedPhrase))
                {
                    m_Commands[fixedPhrase].RemoveListener(handler);
                }
            }

            m_UpdateRemovals = true;
        }

        /// <summary>
        /// Private class for handling the keyword recognizer for a keyword.
        /// </summary>
        private class KeywordCommandInfo
        {
            #if UNITY_EDITOR
            /// <summary>
            /// Editor-only field for revealing info about handlers
            /// </summary>
            public List<string> EditorHandlerDescriptions
            {
                get
                {
                    if (!Application.isEditor)
                        return null;

                    List<string> handlerDescriptions = new List<string>();
                    foreach (CommandInfo command in m_Handlers)
                    {
                        // Get the name of the target if it's a game object
                        Component targetComponent = command.m_Handler.Target as Component;
                        if (targetComponent != null)
                        {
                            handlerDescriptions.Add(
                                "GameObject: " + targetComponent.gameObject.name
                                + "\nMethod: " + targetComponent.GetType().Name + "." + command.m_Handler.Method.Name);
                        }
                        else
                        {
                            handlerDescriptions.Add(
                                "Method: " + command.m_Handler.Target.GetType().Name + "." + command.m_Handler.Method.Name);
                        }
                    }

                    return handlerDescriptions;
                }
            }
            #endif

            /// <summary>
            /// A listener entry for the keyword assosiated with the keyword.
            /// </summary>
            private class CommandInfo
            {
                /// <summary>
                /// The handler to call.
                /// </summary>
                public KeywordRecognizedDelegate m_Handler;

                /// <summary>
                /// The minimum confidence that must be met for this callback to be used.
                /// </summary>
                public KeywordConfidenceLevel m_ConfidenceLevel;

                /// <summary>
                /// If true this entry is flagged for removal and should not be used.
                /// </summary>
                public bool m_ToRemove;
            }

            /// <summary>
            /// The list of handlers assosiated with this keyword.
            /// </summary>
            private List<CommandInfo> m_Handlers = new List<CommandInfo>();

#if UNITY_WSA || UNITY_STANDALONE_WIN
            /// <summary>
            /// The recognizer that will inform us when this keyword is recognized.
            /// </summary>
            private KeywordRecognizer m_Recognizer;
#endif

            /// <summary>
            /// If true then there are CommandInfos to remove at the next chance.
            /// </summary>
            private bool m_CleanUpRemovals = false;

            /// <summary>
            /// True if this keyword is currently being listened for.
            /// </summary>
            private bool m_IsEnabled;
            public bool Enabled
            {
                get { return m_IsEnabled; }
                set { m_IsEnabled = value; }
            }

            /// <summary>
            /// Public constructor
            /// </summary>
            /// <param name="keyword">The keyword that should be listened for.</param>
            public KeywordCommandInfo(string keyword)
            {
                if (string.IsNullOrEmpty (keyword)) {
                    Debug.LogError("Attempted to register null keyword in keyword manager.");
                    return;
                }

                try {
#if UNITY_WSA || UNITY_STANDALONE_WIN
                    m_Recognizer = new KeywordRecognizer(new string[] { keyword });
                    m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;

                    if (!m_Recognizer.IsRunning) {
                        m_Recognizer.Start();
                    }
#endif
                    Enabled = true;
                } catch (System.Exception e) {
                    Debug.Log("Unable to register keyword. Speech recognition may not be supported on this machine. Full exception: " + e.ToString());
                    Enabled = false;
                }
            }

            public void Destroy()
            {
                Enabled = false;
#if UNITY_WSA || UNITY_STANDALONE_WIN
                m_Recognizer.OnPhraseRecognized -= OnPhraseRecognized;
                m_Recognizer.Dispose();
                m_Recognizer = null;
#endif
            }

            /// <summary>
            /// Adds a new listener entry.
            /// </summary>
            /// <param name="listener">The callback to call.</param>
            /// <param name="level">The minimum confidence for this callback to be called.</param>
            public void AddListener(KeywordRecognizedDelegate listener, KeywordConfidenceLevel level)
            {
                if (listener != null)
                {
                    CommandInfo info = new CommandInfo();
                    info.m_Handler = listener;
                    info.m_ConfidenceLevel = level;

                    m_Handlers.Add(info);

                    Enabled = true;
                }
            }

            /// <summary>
            /// Removes a listener from the tracking list.
            /// </summary>
            /// <param name="listener">The callback to remove.</param>
            public void RemoveListener(KeywordRecognizedDelegate listener)
            {
                if (listener != null)
                {
                    int activeHandlers = 0;
                    for (int index = 0; index < m_Handlers.Count; index++)
                    {
                        if (m_Handlers[index].m_Handler == listener)
                        {
                            m_Handlers[index].m_ToRemove = true;
                            m_CleanUpRemovals = true;
                        }
                        else if (!m_Handlers[index].m_ToRemove)
                        {
                            activeHandlers++;
                        }
                    }

                    if (activeHandlers == 0)
                    {
                        Enabled = false;
                    }
                }
            }

#if UNITY_WSA || UNITY_STANDALONE_WIN
            private KeywordConfidenceLevel WSAConfidenceToKeywordConfidence(ConfidenceLevel level)
            {
                KeywordConfidenceLevel newLevel = KeywordConfidenceLevel.Unknown;
                switch (level)
                {
                    case ConfidenceLevel.High:
                        {
                            newLevel = KeywordConfidenceLevel.High;
                            break;
                        }

                    case ConfidenceLevel.Medium:
                        {
                            newLevel = KeywordConfidenceLevel.Medium;
                            break;
                        }

                    case ConfidenceLevel.Low:
                        {
                            newLevel = KeywordConfidenceLevel.Low;
                            break;
                        }

                    case ConfidenceLevel.Rejected:
                        {
                            newLevel = KeywordConfidenceLevel.Rejected;
                            break;
                        }

                    default:
                        {
                            Debug.LogError("Unknown Confidecne Level: " + level.ToString());
                            break;
                        }
                }

                return newLevel;
            }

            /// <summary>
            /// Called when the keyword recognizer registers our keyword.
            /// </summary>
            /// <param name="args"></param>
            private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
            {
                if (Enabled)
                {
                    KeywordConfidenceLevel confidence = WSAConfidenceToKeywordConfidence(args.confidence);
                    for (int index = 0; index < m_Handlers.Count; index++)
                    {
                        if (!m_Handlers[index].m_ToRemove && confidence <= m_Handlers[index].m_ConfidenceLevel)
                        {
                            m_Handlers[index].m_Handler(new KeywordRecognizedEventArgs(args.text, confidence, args.phraseStartTime, args.phraseDuration));
                        }
                    }
                }
            }
#endif

			/// <summary>
			/// Removes any listeners flagged for removal.
			/// </summary>
			public void UpdateRemovals()
			{
				if (m_CleanUpRemovals)
				{
					List<CommandInfo> toRemove = new List<CommandInfo>();

					for (int index = 0; index < m_Handlers.Count; index++)
					{
						if (m_Handlers[index].m_ToRemove)
						{
							toRemove.Add(m_Handlers[index]);
						}
					}

					for (int index = 0; index < toRemove.Count; index++)
					{
						m_Handlers.Remove(toRemove[index]);
					}

					m_CleanUpRemovals = false;
				}
			}
        }
    }
}
