//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(CompoundButtonSounds))]
    public class CompoundButtonSoundsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButtonSounds soundButton = (CompoundButtonSounds)target;

            GUI.color = Color.white;
            soundButton.SoundProfile = HUXEditorUtils.DrawProfileField<ButtonSoundProfile>(soundButton.SoundProfile);

            if (soundButton.SoundProfile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.BeginSectionBox("States", HUXEditorUtils.WarningColor);
            HUXEditorUtils.DrawSubtleMiniLabel("(Warning: this section edits the button profile. These changes will affect all buttons that use this profile.)");

            DrawClipEditor(ref soundButton.SoundProfile.ButtonPressed, ref soundButton.SoundProfile.ButtonPressedVolume, "Button Pressed");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonTargeted, ref soundButton.SoundProfile.ButtonTargetedVolume, "Button Targeted");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonHeld, ref soundButton.SoundProfile.ButtonHeldVolume, "Button Held");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonReleased, ref soundButton.SoundProfile.ButtonReleasedVolume, "Button Released");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonCancelled, ref soundButton.SoundProfile.ButtonCancelledVolume, "Button Cancelled");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonCancelled, ref soundButton.SoundProfile.ButtonCancelledVolume, "Button Cancelled");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonObservation, ref soundButton.SoundProfile.ButtonObservationVolume, "Button Observation");
            DrawClipEditor(ref soundButton.SoundProfile.ButtonObservationTargeted, ref soundButton.SoundProfile.ButtonObservationTargetedVolume, "Button Observation Targeted");

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(soundButton, soundButton.SoundProfile);
        }

        protected void DrawClipEditor(ref AudioClip clip, ref float volume, string label) {
            HUXEditorUtils.BeginSubSectionBox(label);
            EditorGUILayout.BeginHorizontal();
            clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(UnityEngine.AudioClip), true);
            volume = EditorGUILayout.Slider(volume, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            HUXEditorUtils.EndSubSectionBox();
        }
    }
}
