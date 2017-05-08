//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using HUX.Speech;

/// <summary>
/// A class to send a target object a message when a voice command is recieved.
/// </summary>
public class SpeechReciever : MonoBehaviour 
{

	/// <summary>
	/// Internal protected member for our default gizmo icon
	/// </summary>
	protected string _gizmoIconDefault = "HUX/hux_speech_receiver_icon.png";

	/// <summary>
	/// Internal protected member for our gizmo selected icon
	/// </summary>
	protected string _gizmoIconSelected = "HUX/hux_speech_receiver_icon.png";

	/// <summary>
	/// Protected string for the current active gizmo icon
	/// </summary>
	protected string _gizmoIcon;

	[System.Serializable]
    public class SpeechData : IEquatable<SpeechData>
    {
        public string m_SpeechCommand;

        public GameObject m_Target;
        public string m_Message;

        public KeyCode m_DebugKeyCode;

        public KeywordManager.KeywordRecognizedDelegate m_Callback;

        public bool Equals(SpeechData other)
        {
            return m_SpeechCommand.Equals(other.m_SpeechCommand) &&
                   m_Target.Equals(other.m_Target) &&
                   m_Message.Equals(other.m_Message);
        }
    }

    /// <summary>Confidence expected by speech input for any commands setup.</summary>
    public KeywordConfidenceLevel m_RawConfidence = KeywordConfidenceLevel.Medium;

    /// <summary>
    /// The list of commands to add
    /// </summary>
    [SerializeField]
    private SpeechData[] m_Commands = new SpeechData[0];

    /// <summary>Extra commands that can be added at runtime to be tracked.</summary>
    [NonSerialized]
    private List<SpeechData> m_ExtraCommands = new List<SpeechData>();

    /// <summary>Tracks if commands have already been added to the speech input system.</summary>
    private bool m_CommandsAdded = false;

    /// <summary>
    /// Component OnEnable
    /// Currently tracked voice command are enabled.
    /// </summary>
    public void OnEnable()
    {
        StartCoroutine(WaitForKeywordManager());
    }

    private IEnumerator WaitForKeywordManager()
    {
        while (!KeywordManager.Instance)
        {
            yield return 0;
        }

        m_CommandsAdded = true;
        for (int index = 0; index < m_Commands.Length; index++)
        {
            SpeechData command = m_Commands[index];
            command.m_Callback = delegate (KeywordRecognizedEventArgs args)
            {
                command.m_Target.SendMessage(command.m_Message, null, SendMessageOptions.DontRequireReceiver);
            };
            KeywordManager.Instance.AddKeyword(m_Commands[index].m_SpeechCommand, command.m_Callback, m_RawConfidence);
        }

        for (int index = 0; index < m_ExtraCommands.Count; index++)
        {
            SpeechData command = m_ExtraCommands[index];
            command.m_Callback = delegate (KeywordRecognizedEventArgs args)
            {
                command.m_Target.SendMessage(command.m_Message, null, SendMessageOptions.DontRequireReceiver);
            };
            KeywordManager.Instance.AddKeyword(command.m_SpeechCommand, command.m_Callback, m_RawConfidence);
        }
    }

    /// <summary>
    /// Compontent OnDisable
    /// Currently tracked vocie commands are disabled.
    /// </summary>
    public void OnDisable()
    {
        m_CommandsAdded = false;
        for (int index = 0; index < m_Commands.Length; index++)
        {
            KeywordManager.Instance.RemoveKeyword(m_Commands[index].m_SpeechCommand, m_Commands[index].m_Callback);
        }

        for (int index = 0; index < m_ExtraCommands.Count; index++)
        {
            KeywordManager.Instance.RemoveKeyword(m_ExtraCommands[index].m_SpeechCommand, m_ExtraCommands[index].m_Callback);
        }
    }


    private void Update()
    {
        for (int index = 0; index < m_Commands.Length; index++)
        {
            if (Input.GetKeyDown(m_Commands[index].m_DebugKeyCode))
            {
                m_Commands[index].m_Target.SendMessage(m_Commands[index].m_Message, null, SendMessageOptions.DontRequireReceiver);
            }
        }

        for (int index = 0; index < m_ExtraCommands.Count; index++)
        {
            if (Input.GetKeyDown(m_ExtraCommands[index].m_DebugKeyCode))
            {
                m_ExtraCommands[index].m_Target.SendMessage(m_ExtraCommands[index].m_Message, null, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    #region Public Methods

    /// <summary>
    /// Adds an extra command that sends message to the target object when speechCommand is said.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speechCommand"></param>
    /// <param name="message"></param>
    /// <param name="code">Key code to press to trigger the command while in the editor.</param>
    public void AddExtraCommand(GameObject target, string speechCommand, string message, KeyCode code = KeyCode.None)
    {
        SpeechData data = new SpeechData();
        data.m_Message = message;
        data.m_SpeechCommand = speechCommand;
        data.m_Target = target;
        data.m_DebugKeyCode = code;

        m_ExtraCommands.Add(data);

        if (m_CommandsAdded)
        {
            data.m_Callback = delegate (KeywordRecognizedEventArgs args)
            {
                data.m_Target.SendMessage(data.m_Message, null, SendMessageOptions.DontRequireReceiver);
            };
            KeywordManager.Instance.AddKeyword(data.m_SpeechCommand, data.m_Callback, m_RawConfidence);
        }
	}

	/// <summary>
	/// Removes all extra commands that has the target object, speechCommand and message.
	/// </summary>
	/// <param name="target"></param>
	/// <param name="speechCommand"></param>
	/// <param name="message"></param>
	public void RemoveExtraCommand(GameObject target, string speechCommand, string message)
    {
        SpeechData data = new SpeechData();
        data.m_Message = message;
        data.m_SpeechCommand = speechCommand;
        data.m_Target = target;

        List<SpeechData> commandsToRemove = new List<SpeechData>();
        for (int index = 0; index < m_ExtraCommands.Count; index++)
        {
            if (data.Equals(m_ExtraCommands[index]))
            {
                commandsToRemove.Add(m_ExtraCommands[index]);
                if (m_CommandsAdded)
                {
                    KeywordManager.Instance.RemoveKeyword(m_ExtraCommands[index].m_SpeechCommand, m_ExtraCommands[index].m_Callback);
                }
			}
		}

        for (int index = 0; index < commandsToRemove.Count; index++)
        {
            m_ExtraCommands.Remove(commandsToRemove[index]);
        }
    }

    /// <summary>
    /// Removes all the extra command track by this reciever that have the target object provided.
    /// </summary>
    /// <param name="target"></param>
    public void RemoveAllExtraCommands(GameObject target)
    {
        List<SpeechData> commandsToRemove = new List<SpeechData>();
        for (int index = 0; index < m_ExtraCommands.Count; index++)
        {
            if (m_ExtraCommands[index].m_Target == target)
            {
                commandsToRemove.Add(m_ExtraCommands[index]);
                if (m_CommandsAdded)
                {
                    KeywordManager.Instance.RemoveKeyword(m_ExtraCommands[index].m_SpeechCommand, m_ExtraCommands[index].m_Callback);
                }
            }
        }

        for (int index = 0; index < commandsToRemove.Count; index++)
        {
            m_ExtraCommands.Remove(commandsToRemove[index]);
        }
    }

    /// <summary>
    /// Removes all the extra commands tracked by this reciever.
    /// </summary>
    public void RemoveAllExtraCommands()
    {
        if (m_CommandsAdded)
        {
            for (int index = 0; index < m_ExtraCommands.Count; index++)
            {
                KeywordManager.Instance.RemoveKeyword(m_ExtraCommands[index].m_SpeechCommand, m_ExtraCommands[index].m_Callback);
            }
        }

        m_ExtraCommands.Clear();
    }

	#endregion

	#region EDITOR_FUNCTIONALITY
#if UNITY_EDITOR
	/// <summary>
	/// When selected draw lines to all linked gameobjects of speechcommands
	/// </summary>
	protected virtual void OnDrawGizmosSelected()
	{
		if (m_Commands.Length > 0)
		{
			for (int i = 0; i < m_Commands.Length; i++)
			{
				if (m_Commands[i].m_Target != null)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawLine(this.transform.position, m_Commands[i].m_Target.transform.position);
				}
			}
		}
	}

	/// <summary>
	/// On Draw Gizmo show the receiver icon
	/// </summary>
	protected virtual void OnDrawGizmos()
	{
		_gizmoIcon = UnityEditor.Selection.activeGameObject == this.gameObject ? _gizmoIconSelected : _gizmoIconDefault;
		Gizmos.DrawIcon(this.transform.position, _gizmoIcon, false);
	}
#endif
	#endregion
}
