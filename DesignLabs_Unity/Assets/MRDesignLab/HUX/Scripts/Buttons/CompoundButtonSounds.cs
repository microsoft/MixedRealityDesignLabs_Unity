//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Buttons
{
    /// <summary>
    /// A convenient way to play sounds in response to button actions / states
    /// </summary>
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonSounds : MonoBehaviour
    {
        const float MinTimeBetweenSameClip = 0.1f;

        public ButtonSoundProfile SoundProfile;
        
        [SerializeField]
        private AudioSource audioSource;
        private static string lastClipName;
        private static float lastClipTime;

        void Start ()
        {
            Button button = GetComponent<Button>();
            button.OnButtonCancelled += OnButtonCancelled;
            button.OnButtonHeld += OnButtonHeld;
            button.OnButtonPressed += OnButtonPressed;
            button.OnButtonReleased += OnButtonReleased;
            button.StateChange += StateChange;

            audioSource = GetComponent<AudioSource>();
        }

        void StateChange(Button.ButtonStateEnum newState)
        {
            if (SoundProfile == null)
            {
                Debug.LogError("Sound profile was null in button " + name);
                return;
            }

            switch (newState)
            {
                case Button.ButtonStateEnum.Observation:
                    PlayClip(SoundProfile.ButtonObservation, SoundProfile.ButtonObservationVolume);
                    break;

                case Button.ButtonStateEnum.ObservationTargeted:
                    PlayClip(SoundProfile.ButtonObservationTargeted, SoundProfile.ButtonObservationTargetedVolume);
                    break;

                case Button.ButtonStateEnum.Targeted:
                    PlayClip(SoundProfile.ButtonTargeted, SoundProfile.ButtonTargetedVolume);
                    break;

                default:
                    break;
            }
        }

        void OnButtonCancelled(GameObject go)
        {
            PlayClip(SoundProfile.ButtonCancelled, SoundProfile.ButtonCancelledVolume);
        }

        void OnButtonHeld(GameObject go)
        {
            PlayClip(SoundProfile.ButtonHeld, SoundProfile.ButtonHeldVolume);
        }

        void OnButtonPressed(GameObject go)
        {
            PlayClip(SoundProfile.ButtonPressed, SoundProfile.ButtonPressedVolume);
        }

        void OnButtonReleased (GameObject go)
        {
            PlayClip(SoundProfile.ButtonReleased, SoundProfile.ButtonReleasedVolume);
        }

        void PlayClip (AudioClip clip, float volume)
        {
            if (clip != null)
            {
                // Don't play the clip if we're spamming it
                if (clip.name == lastClipName && Time.realtimeSinceStartup < MinTimeBetweenSameClip)
                    return;

                lastClipName = clip.name;
                lastClipTime = Time.realtimeSinceStartup;
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(clip, volume);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(clip, transform.position, volume);
                }
            }
        }
    }
}