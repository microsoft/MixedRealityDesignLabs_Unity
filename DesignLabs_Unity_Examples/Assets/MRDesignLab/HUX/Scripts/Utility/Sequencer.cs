//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUX.Interaction;
using HUX.Dialogs;
using HUX.Speech;
using UnityEngine.SceneManagement;

namespace HUX.Utility
{
    /// <summary>
    /// Sequencer has an array of messages and prefabs that are triggered based on external events.
    /// </summary>
    public class Sequencer : MonoBehaviour
    {
        #region public vars

        [Tooltip("Target message box for displaying messages to")]
        public MessageBox MessageBoxPrefab;

        [Tooltip("Voice command to restart demo")]
        public string ResetVoiceCommand = "Restart Demo";

        [Tooltip("Voice command to restart demo")]
        public KeyCode ResetButton = KeyCode.R;

        [Tooltip("Voice command to repeat step")]
        public string RepeatStepCommand = "Repeat Step";

        [Tooltip("Voice command to repeat step")]
        public KeyCode RepeatStepButton = KeyCode.T;

        [Tooltip("Voice command to repeat step")]
        public KeyCode SkipStepButton = KeyCode.Space;

        [Tooltip("Delay from initial start to the demo starting")]
        public float StartDelay = 2.0f;

        [Tooltip("The demo element index to start at.")]
        public int StartIndex = 0;

        [System.Serializable]
        public class EventConditionDatum
        {
            [Tooltip("The event condition is dictated by the prefab no condition settings used")]
            public bool PrefabCondition = false;

            [Tooltip("Target interactible to check for interaction with")]
            public InteractibleObject targetObject;

            [Tooltip("Condition type for the interaction check")]
            public InteractionCondition.ConditionType targetCondition;

            [Tooltip("Time this needs to be valid")]
            public float duration;

            [Tooltip("Phrase for speech")]
            public string speechPhrase = "";
        }

        [System.Serializable]
        public class StepDatum
        {
            public string Title;
            public bool EnableStep = true;
            public bool InstructionStep = false;
            public MessageBox.MessageDatum Message;
            public string VoiceCommand;
            public bool WaitForEvent;
            public bool WaitForAudio;
            public EventConditionDatum EventCondition;
            public float AudioDelay = 1.0f;
            public AudioClip StepAudio;
            public GameObject StepPrefab;
            public bool PrefabPersists;
        }

        [Tooltip("Array of demo elements to play through")]
        public List<StepDatum> SequenceElements = new List<StepDatum>();

        [Tooltip("Level to load at then of sequence")]
        public string TransitionToLevel;

        [Tooltip("Confidence for Speech command recognition")]
        public KeywordConfidenceLevel ConfidenceThreshold = KeywordConfidenceLevel.Medium;

        #endregion public vars

        #region private vars
        private bool SequenceComplete = false;
        private bool WaitingForEvent = false;
        private bool WaitingForAudio = false;
        private int curIdx = 0;
        private GameObject spawnedPrefab;
        private ISequencerCondition curCondition;
        private int _repeatIdx = 0;
        private bool _audioQueued = false;
        private MessageBox msgBox;
        #endregion private vars

        public bool Complete
        {
            get { return SequenceComplete;  }
        }

        public void Start()
        {

            if (MessageBoxPrefab != null)
            {
                msgBox = GameObject.Instantiate(MessageBoxPrefab);
            }

            KeywordManager.Instance.AddKeyword(ResetVoiceCommand, OnKeyWord, ConfidenceThreshold);
            KeywordManager.Instance.AddKeyword(RepeatStepCommand, OnKeyWord, ConfidenceThreshold);

            for (int i = 0; i < SequenceElements.Count; i++)
            {
                if (SequenceElements[i].VoiceCommand != "")
                {
                    KeywordManager.Instance.AddKeyword(SequenceElements[i].VoiceCommand, OnKeyWord, ConfidenceThreshold);
                }
            }

            StartCoroutine("PlaySequence", this.StartIndex);
        }

        public IEnumerator PlaySequence(int idx = 0)
        {
            yield return new WaitForSeconds(StartDelay);

            for (int i = idx; i < SequenceElements.Count; i++)
            {
                if (!SequenceElements[i].EnableStep)
                    continue;

                curIdx = i;

                // If we have a spawned prefab then destroy it and set to null unless set to persist.
                if (spawnedPrefab != null && !SequenceElements[curIdx].PrefabPersists)
                {
                    GameObject.Destroy(spawnedPrefab);
                    spawnedPrefab = null;
                }

                if (SequenceElements[curIdx].StepAudio != null)
                {
                    _audioQueued = true;
                    StartCoroutine("PlayAudio");
                }

                msgBox.PlayMessage(SequenceElements[curIdx].Message);
                WaitingForEvent = SequenceElements[curIdx].WaitForEvent;
                WaitingForAudio = SequenceElements[curIdx].WaitForAudio;

                if (SequenceElements[curIdx].InstructionStep)
                    _repeatIdx = curIdx;

                if (WaitingForEvent && !SequenceElements[curIdx].EventCondition.PrefabCondition)
                {
                    InteractionCondition stepCondition = new InteractionCondition();

                    stepCondition.targetObject = SequenceElements[curIdx].EventCondition.targetObject;
                    stepCondition.targetCondition = SequenceElements[curIdx].EventCondition.targetCondition;
                    stepCondition.duration = SequenceElements[curIdx].EventCondition.duration;
                        
                    RegisterCondition(stepCondition);
                }

                if (SequenceElements[curIdx].StepPrefab != null)
                {
                    spawnedPrefab = GameObject.Instantiate(SequenceElements[curIdx].StepPrefab) as GameObject;
                }

                // Check for event
                while (msgBox.playingMessage || WaitingForEvent || WaitingForAudio)
                {
                    if (WaitingForEvent && curCondition != null && !msgBox.playingMessage)
                    {
                        WaitingForEvent = !curCondition.CheckCondition();
                        if (!WaitingForEvent)
                            curCondition = null;
                    }
                    WaitingForAudio = _audioQueued  || AudioManager.Instance.Playing;
                    yield return new WaitForEndOfFrame();
                }
            }

            if (TransitionToLevel != null && TransitionToLevel.Length > 0 && TransitionToLevel != SceneManager.GetActiveScene().name)
            {
                // Debug.Log("Current: " + Application.loadedLevelName + " Loading: " + TransitionToLevel);
                SceneManager.LoadScene(TransitionToLevel, LoadSceneMode.Single);
            }

            SequenceComplete = true;
            yield return null;
        }

        public void RepeatStep()
        {
            StopCoroutine("PlaySequence");
            AudioManager.Instance.StopAudio(true);
            StartCoroutine("PlaySequence", _repeatIdx);
        }

        public IEnumerator PlayAudio()
        {
            _audioQueued = true;
            yield return new WaitForSeconds(SequenceElements[curIdx].AudioDelay);
            AudioManager.Instance.PlayClip(SequenceElements[curIdx].StepAudio);
            _audioQueued = false;
        }

        public void FixedUpdate()
        {
            if (Input.GetKeyDown(SkipStepButton))
            {

            }

            if (Input.GetKeyDown(RepeatStepButton))
            {

            }

            if (Input.GetKeyDown(ResetButton))
            {

            }
        }

        public void RegisterCondition(ISequencerCondition newCondition)
        {
            curCondition = newCondition;
        }

        private void OnKeyWord(KeywordRecognizedEventArgs args)
        {
            if (SequenceElements[curIdx].VoiceCommand.Equals(args.text))
            {
                StartCoroutine("FadeAndContinue");
            }

            if (ResetVoiceCommand.Equals(args.text))
            {
                StopCoroutine("PlaySequence");

                if (spawnedPrefab != null)
                {
                    GameObject.Destroy(spawnedPrefab);
                    spawnedPrefab = null;
                }

                StartCoroutine("PlaySequence", 0);
            }

            if (RepeatStepCommand.Equals(args.text))
            {
                if (spawnedPrefab != null)
                {
                    GameObject.Destroy(spawnedPrefab);
                    spawnedPrefab = null;
                }

                RepeatStep();
            }
        }

        public void OnDestroy()
        {
            KeywordManager.Instance.RemoveKeyword(ResetVoiceCommand, OnKeyWord);
            KeywordManager.Instance.RemoveKeyword(RepeatStepCommand, OnKeyWord);

            for (int i = 0; i < SequenceElements.Count; i++)
            {
                if (SequenceElements[i].VoiceCommand != "")
                {
                    KeywordManager.Instance.RemoveKeyword(SequenceElements[i].VoiceCommand, OnKeyWord);
                }
            }

            if (msgBox != null )
            {
                GameObject.Destroy(msgBox.gameObject);
            }

            if (spawnedPrefab != null)
            {
                GameObject.Destroy(spawnedPrefab);
            }
        }

        private IEnumerator FadeAndContinue()
        {
            msgBox.StartCoroutine("FadeText", true);
            yield return new WaitForSeconds(msgBox.fadeTime);
            WaitingForEvent = false;
        }
    }
}
