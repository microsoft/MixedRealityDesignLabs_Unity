//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(CompoundButtonToggle))]
    public class CompoundButtonToggleInspector : Editor
    {
        public override void OnInspectorGUI() {
            CompoundButtonToggle toggle = (CompoundButtonToggle)target;

            HUXEditorUtils.BeginSectionBox("Target");

            toggle.Target = HUXEditorUtils.DropDownComponentField<MonoBehaviour>("Component", toggle.Target, toggle.transform, true);
            FieldInfo fieldInfo = null;
            Type profileType = null;
            if (toggle.Target == null) {
                HUXEditorUtils.ErrorMessage("Target must be set.");
                HUXEditorUtils.EndSectionBox();
                HUXEditorUtils.SaveChanges(target);
                return;
            } else {

                fieldInfo = toggle.Target.GetType().GetField("Profile");

                if (fieldInfo == null) {
                    HUXEditorUtils.ErrorMessage("Target component has no 'Profile' field - are you use this class inherits from ProfileButtonBase?");
                    HUXEditorUtils.EndSectionBox();
                    HUXEditorUtils.SaveChanges(target);
                    return;
                }

                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.fontSize = 18;
                labelStyle.fontStyle = FontStyle.Bold;

                profileType = fieldInfo.FieldType;
                EditorGUILayout.LabelField("Type: " + toggle.Target.GetType().Name + " / " + fieldInfo.FieldType.Name, labelStyle, GUILayout.MinHeight(24));

            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Toggling");
            HUXEditorUtils.DrawSubtleMiniLabel("Select on/off profiles of the type " + profileType.Name);
            if (toggle.OnProfile == null) {
                toggle.OnProfile = (ButtonProfile)fieldInfo.GetValue(toggle.Target);
            }
            if (toggle.OffProfile == null) {
                toggle.OffProfile = toggle.OnProfile;
            }
            ButtonProfile onProfile = (ButtonProfile)EditorGUILayout.ObjectField("On Profile", toggle.OnProfile, typeof(ButtonProfile), false);
            ButtonProfile offProfile = (ButtonProfile)EditorGUILayout.ObjectField("Off Profile", toggle.OffProfile, typeof(ButtonProfile), false);
            if (onProfile.GetType () == profileType) {
                toggle.OnProfile = onProfile;
            }
            if (offProfile.GetType() == profileType) {
                toggle.OffProfile = offProfile;
            }

            if (toggle.OnProfile.GetType() != profileType) {
                HUXEditorUtils.ErrorMessage("On profile object does not match type " + profileType.Name);
            }
            if (toggle.OffProfile.GetType() != profileType) {
                HUXEditorUtils.ErrorMessage("Off profile object does not match type " + profileType.Name);
            }

            if (onProfile == offProfile) {
                HUXEditorUtils.WarningMessage("Profiles are the same - toggle will have no effect");
            }

            toggle.Behavior = (CompoundButtonToggle.ToggleBehaviorEnum) EditorGUILayout.EnumPopup("Toggle behavior", toggle.Behavior);
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("State");
            if (!Application.isPlaying) {
                toggle.State = EditorGUILayout.Toggle(toggle.State);
            } else {
                EditorGUILayout.Toggle(toggle.State);
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target);
        }
    }
}