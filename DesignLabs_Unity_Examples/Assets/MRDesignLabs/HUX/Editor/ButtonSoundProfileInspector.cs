//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(ButtonSoundProfile))]
    public class ButtonSoundProfileInspector : ProfileInspector
    {
        public override void OnInspectorGUI()
        {
            ButtonSoundProfile soundProfile = (ButtonSoundProfile)target;
            //CompoundButtonSounds soundButton = (CompoundButtonSounds)targetComponent;

            HUXEditorUtils.BeginProfileBox();

            DrawClipEditor(ref soundProfile.ButtonPressed, ref soundProfile.ButtonPressedVolume, "Button Pressed");
            DrawClipEditor(ref soundProfile.ButtonTargeted, ref soundProfile.ButtonTargetedVolume, "Button Targeted");
            DrawClipEditor(ref soundProfile.ButtonHeld, ref soundProfile.ButtonHeldVolume, "Button Held");
            DrawClipEditor(ref soundProfile.ButtonReleased, ref soundProfile.ButtonReleasedVolume, "Button Released");
            DrawClipEditor(ref soundProfile.ButtonCancelled, ref soundProfile.ButtonCancelledVolume, "Button Cancelled");
            DrawClipEditor(ref soundProfile.ButtonObservation, ref soundProfile.ButtonObservationVolume, "Button Observation");
            DrawClipEditor(ref soundProfile.ButtonObservationTargeted, ref soundProfile.ButtonObservationTargetedVolume, "Button Observation Targeted");

            HUXEditorUtils.EndProfileBox();

            HUXEditorUtils.SaveChanges(this);
        }

        protected void DrawClipEditor(ref AudioClip clip, ref float volume, string label)
        {
            HUXEditorUtils.BeginSubSectionBox(label);
            EditorGUILayout.BeginHorizontal();
            clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(UnityEngine.AudioClip), true);
            volume = EditorGUILayout.Slider(volume, 0f, 1f);
            EditorGUILayout.EndHorizontal();
            HUXEditorUtils.EndSubSectionBox();
        }
    }
}