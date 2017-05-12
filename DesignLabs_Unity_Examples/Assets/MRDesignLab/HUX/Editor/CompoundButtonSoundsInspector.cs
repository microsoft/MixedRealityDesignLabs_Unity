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

            HUXEditorUtils.DrawProfileInspector(soundButton.SoundProfile, soundButton);
        }
    }
}
