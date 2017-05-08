//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HUX.Utility;
using HUX.Interaction;
using HUX.Receivers;

namespace HUX
{
    /// <summary>
    /// Singleton Class for handling the audio events.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// Audio clip to play if a select event was done with no interactible as the target
        /// </summary>
        #region Global Audio
        [Tooltip("Audio clip for selecting over nothing")]
        public AudioClip SelectInvalidAudio;

        /// <summary>
        /// Audio Set is a template set of interaction sounds to play on an object of a specific type
        /// </summary>
        [System.Serializable]
        public class AudioSetDatum
        {
            /// <summary>
            /// Type of interactible to check for the audio
            /// </summary>
            public Type InteractibleType;
            /// <summary>
            /// Audio to play when hovering
            /// </summary>
            public AudioClip HoverAudio;
            /// <summary>
            /// Audio to play when hand is up
            /// </summary>
            public AudioClip IntentionAudio;
            /// <summary>
            /// Audio to play when the object is pressed
            /// </summary>
            public AudioClip PressAudio;
            /// <summary>
            /// Audio to play when object is released
            /// </summary>
            public AudioClip ReleaseAudio;
            /// <summary>
            /// Audio to play on observeration
            /// </summary>
            public AudioClip ObservationAudio;
        }

        public AudioSetDatum[] AudioSets = new AudioSetDatum[] { };

        [Tooltip("Set of audio clips for different interaction states of tile classes")]
        public AudioSetDatum TileAudio = new AudioSetDatum();

        [Tooltip("Set of audio clips for different interaction states of button classes")]
        public AudioSetDatum ButtonAudio = new AudioSetDatum();

        public bool Playing { get { return this.IsPlaying(); } }
        #endregion

        private GameObject speaker;
        private AudioSource speakerSource;

        private AudioSetDatum currentAudio;
        private bool bHandActive;
        private bool bQueueActive;
        private float timeStamp;

        private List<AudioClip> AudioQueue = new List<AudioClip>();

        /// <summary>
        /// Initialized the Audio Manager.
        /// </summary>
        void Start()
        {
            // Create an empty game object then add an audio source component
            speaker = new GameObject();
            speakerSource = speaker.AddComponent<AudioSource>();
            speakerSource.playOnAwake = false;
            speaker.transform.parent = this.gameObject.transform;
        }

        /// <summary>
        /// Awake function to set the current audio set and the instance
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            currentAudio = ButtonAudio;
        }

        public void Update()
        {
            var isAnyHandVisible = InputSources.Instance.hands.IsAnyHandVisible();
            if (bHandActive != isAnyHandVisible)
            {
                bHandActive = isAnyHandVisible;

                // Play the hover audio there is a current object
                if (bHandActive && currentAudio != null && currentAudio.HoverAudio != null)
                {
                    speakerSource.PlayOneShot(currentAudio.HoverAudio);
                }
            }
        }

        public void OnHoverStart()
        {
            // Play the hover audio if hand is active
            if (bHandActive && currentAudio.HoverAudio != null)
            {
                speakerSource.PlayOneShot(currentAudio.HoverAudio);
            }
        }

        public void OnHoverEnd(InteractibleObject interaction)
        {
            currentAudio = null;
        }

        public void OnSelectStart(InteractibleObject interaction)
        {
            if (currentAudio != null)
            {
                speakerSource.PlayOneShot(currentAudio.PressAudio);
            }
        }

        public void OnSelectEnd(InteractibleObject interaction)
        {
            if (currentAudio != null)
            {
                speakerSource.PlayOneShot(currentAudio.ReleaseAudio);
            }
        }

        public void PlayClip(AudioClip targetClip, bool interrupt = false)
        {
            if (targetClip != null)
            {
                if (interrupt)
                {
                    AudioQueue.Insert(0, targetClip);

                    if (bQueueActive)
                    {
                        StopCoroutine("StartQueue");
                        bQueueActive = false;
                    }
                }
                else
                {
                    AudioQueue.Add(targetClip);
                }

                if (!bQueueActive)
                    StartCoroutine("StartQueue");
            }
        }

        public void StopAudio(bool ClearQueue = false)
        {
            if (ClearQueue)
                AudioQueue.Clear();

            speakerSource.Stop();
        }

        private bool IsPlaying()
        {
            if (speakerSource.clip == null)
                return false;

            return bQueueActive ? true : Time.time - timeStamp < speakerSource.clip.length;
        }

        private IEnumerator StartQueue()
        {
            bQueueActive = true;

            while (AudioQueue.Count > 0)
            {
                speakerSource.clip = AudioQueue[0];
                speakerSource.Play();
                timeStamp = Time.time;

                while (Time.time - timeStamp < speakerSource.clip.length)
                {
                    yield return new WaitForEndOfFrame();
                }

                AudioQueue.RemoveAt(0);
            }

            bQueueActive = false;
            yield return null;
        }
    }
}
