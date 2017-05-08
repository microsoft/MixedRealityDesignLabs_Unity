//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(InteractionManager))]
    public class InteractionManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            InteractionManager interactionManager = (InteractionManager)target;

            interactionManager.RecognizableGesures = (UnityEngine.VR.WSA.Input.GestureSettings)HUXEditorUtils.EnumCheckboxField<UnityEngine.VR.WSA.Input.GestureSettings>(
                "Recognizable gestures",
                interactionManager.RecognizableGesures,
                "Default",
                UnityEngine.VR.WSA.Input.GestureSettings.Tap |
                UnityEngine.VR.WSA.Input.GestureSettings.DoubleTap |
                UnityEngine.VR.WSA.Input.GestureSettings.Hold | 
                UnityEngine.VR.WSA.Input.GestureSettings.NavigationX | 
                UnityEngine.VR.WSA.Input.GestureSettings.NavigationY);

            HUXEditorUtils.SaveChanges(target);
        }
    }
}