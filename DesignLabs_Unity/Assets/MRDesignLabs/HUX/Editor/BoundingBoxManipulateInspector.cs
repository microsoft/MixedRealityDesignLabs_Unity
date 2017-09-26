//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Interaction;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(BoundingBoxManipulate))]
    public class BoundingBoxManipulateInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            BoundingBoxManipulate bbm = (BoundingBoxManipulate)target;
            
            GUI.color = HUXEditorUtils.DefaultColor;

            bbm.Target = (GameObject)EditorGUILayout.ObjectField("Target", bbm.Target, typeof (GameObject), true);
            bbm.ActiveHandle = (BoundingBoxHandle)EditorGUILayout.ObjectField("Active Handle", bbm.ActiveHandle, typeof(BoundingBoxHandle), true);
            bbm.PhysicsLayer = EditorGUILayout.IntSlider("Physics / Rendering Layer", bbm.PhysicsLayer, 0, 32);
            bbm.IgnoreLayer = EditorGUILayout.IntSlider("Ignore Mesh Renderers on this Layer", bbm.IgnoreLayer, 0, 32);
            bbm.DragMultiplier = EditorGUILayout.Slider("Drag input scale", bbm.DragMultiplier, 0.01f, 20f);
            bbm.RotateMultiplier = EditorGUILayout.Slider("Rotation input scale", bbm.RotateMultiplier, 0.01f, 20f);
            bbm.ScaleMultiplier = EditorGUILayout.Slider("Scale input scale", bbm.ScaleMultiplier, 0.01f, 20f);
            bbm.MinScalePercentage = EditorGUILayout.Slider("Minimum scale per operation", bbm.MinScalePercentage, 0.05f, 1f);
            bbm.PermittedOperations = (BoundingBoxManipulate.OperationEnum) HUXEditorUtils.EnumCheckboxField<BoundingBoxManipulate.OperationEnum>(
                "Permitted Operations",
                bbm.PermittedOperations,
                "Default",
                BoundingBoxManipulate.OperationEnum.ScaleUniform | BoundingBoxManipulate.OperationEnum.RotateY | BoundingBoxManipulate.OperationEnum.Drag,
                BoundingBoxManipulate.OperationEnum.Drag);

            if (!Application.isPlaying)
            {
                bbm.AcceptInput = EditorGUILayout.Toggle("Accept Input", bbm.AcceptInput);
                bbm.ManipulatingNow = EditorGUILayout.Toggle("Manipulating Now", bbm.ManipulatingNow);
            }

            HUXEditorUtils.SaveChanges(bbm);
        }
    }
}