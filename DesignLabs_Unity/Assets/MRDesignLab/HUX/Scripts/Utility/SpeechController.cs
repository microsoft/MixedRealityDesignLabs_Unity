//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;
using System.Collections.Generic;
using HUX.Utility;
using HUX.Speech;

public class SpeechController : Singleton<SpeechController>
{
    [Header("Commands")]
    public KeywordConfidenceLevel m_RawConfidence = KeywordConfidenceLevel.Medium;
    private List<SpeechCommand> m_SpeechCommands = new List<SpeechCommand>();

    [Header("Debug Text")]
    public float m_TextDisplayTime = 2.0f;
    private float m_CurrentTextDisplayTime = 0.0f;
    public TextMesh m_TextMesh;

    private void Update()
    {
        if (m_TextMesh != null && Time.realtimeSinceStartup - m_CurrentTextDisplayTime > m_TextDisplayTime)
        {
            m_TextMesh.gameObject.SetActive(false);
        }
    }

    public void AddSpeechCommand(SpeechCommand speechCommand)
    {
        if (speechCommand != null)
        {
            m_SpeechCommands.Add(speechCommand);
            KeywordManager.Instance.AddKeyword(speechCommand.KeyWord, speechCommand.OnWord, m_RawConfidence);
        }
    }

    public void RemoveSpeechCommand(SpeechCommand speechCommand)
    {
        if (speechCommand != null)
        {
            if (m_SpeechCommands.Contains(speechCommand))
            {
                m_SpeechCommands.Remove(speechCommand);
            }

            KeywordManager.Instance.RemoveKeyword(speechCommand.KeyWord, speechCommand.OnWord);
        }
    }

    public void SetText(string text)
    {
        if (m_TextMesh != null)
        {
            m_TextMesh.text = text;
            m_TextMesh.gameObject.SetActive(true);

            m_CurrentTextDisplayTime = Time.realtimeSinceStartup;
        }
    }
}

public class SpeechCommand : MonoBehaviour
{
    [SerializeField]
    private string m_KeyWord;
    [SerializeField]
    private KeyCode m_DebugKey;

    public string KeyWord
    {
        get { return m_KeyWord; }
        set { m_KeyWord = value; }
    }

    protected virtual void Start()
    {
        SpeechController.Instance.AddSpeechCommand(this);
    }

    protected virtual void OnDestroy()
    {
        SpeechController.Instance.RemoveSpeechCommand(this);
    }
    
    protected virtual void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(m_DebugKey))
        {
            OnKeyword();
        }
#endif
    }
    
    public void OnWord(KeywordRecognizedEventArgs args)
    {
        if (string.Equals(args.text ,KeyWord, StringComparison.OrdinalIgnoreCase))
        {
            OnKeyword();
        }
    }

    protected virtual void OnKeyword() { }

    protected void SetText(string text)
    {
        SpeechController.Instance.SetText(text);
    }
}
