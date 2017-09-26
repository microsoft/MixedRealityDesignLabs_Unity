//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CompoundButtonSounds))]
    public class CompoundButtonSoundsInspector : Editor
    {
        SerializedProperty profileProp;

        void OnEnable()
        {
            profileProp = serializedObject.FindProperty("Profile");
        }

        public override void OnInspectorGUI()
        {
            CompoundButtonSounds soundButton = (CompoundButtonSounds)target;

            GUI.color = Color.white;
            profileProp.objectReferenceValue = HUXEditorUtils.DrawProfileField<ButtonSoundProfile>(profileProp.objectReferenceValue as ButtonSoundProfile);

            if (soundButton.Profile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.DrawProfileInspector(soundButton.Profile, soundButton);

            HUXEditorUtils.SaveChanges(target, soundButton.Profile);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
