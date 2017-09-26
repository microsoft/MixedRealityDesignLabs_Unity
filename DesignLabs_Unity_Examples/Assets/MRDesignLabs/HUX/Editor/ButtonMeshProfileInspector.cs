//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(ButtonMeshProfile))]
    public class ButtonMeshProfileInspector : ProfileInspector
    {
        public override void OnInspectorGUI()
        {
            ButtonMeshProfile meshProfile = (ButtonMeshProfile)target;
            CompoundButtonMesh meshButton = (CompoundButtonMesh)targetComponent;
            
            HUXEditorUtils.BeginProfileBox();

            // Draw an editor for each state datum
            meshProfile.SmoothStateChanges = EditorGUILayout.Toggle("Smooth state changes", meshProfile.SmoothStateChanges);
            if (meshProfile.SmoothStateChanges)
            {
                meshProfile.AnimationSpeed = EditorGUILayout.Slider("Animation speed", meshProfile.AnimationSpeed, 0.01f, 1f);
            }
            meshProfile.StickyPressedEvents = EditorGUILayout.Toggle("'Sticky' pressed events", meshProfile.StickyPressedEvents);
            if (meshProfile.StickyPressedEvents)
            {
                meshProfile.StickyPressedTime = EditorGUILayout.Slider("'Sticky' pressed event time", meshProfile.StickyPressedTime, 0.01f, 1f);
            }

            // Validate our button states - ensure there's one for each button state enum value
            Button.ButtonStateEnum[] buttonStates = (Button.ButtonStateEnum[])System.Enum.GetValues(typeof(Button.ButtonStateEnum));
            List<CompoundButtonMesh.MeshButtonDatum> missingStates = new List<CompoundButtonMesh.MeshButtonDatum>();
            foreach (Button.ButtonStateEnum buttonState in buttonStates)
            {
                bool foundState = false;
                foreach (CompoundButtonMesh.MeshButtonDatum datum in meshProfile.ButtonStates)
                {
                    if (datum.ActiveState == buttonState)
                    {
                        foundState = true;
                        break;
                    }
                }

                if (!foundState)
                {
                    CompoundButtonMesh.MeshButtonDatum missingState = new CompoundButtonMesh.MeshButtonDatum(buttonState);
                    missingState.Name = buttonState.ToString();
                    missingStates.Add(missingState);
                }
            }

            // If any were missing, add them to our button states
            // They may be out of order but we don't care
            if (missingStates.Count > 0)
            {
                missingStates.AddRange(meshProfile.ButtonStates);
                meshProfile.ButtonStates = missingStates.ToArray();
            }

            foreach (CompoundButtonMesh.MeshButtonDatum datum in meshProfile.ButtonStates)
            {
                HUXEditorUtils.BeginSubSectionBox(datum.ActiveState.ToString());
                //datum.Name = EditorGUILayout.TextField("Name", datum.Name);
                if (meshButton != null && meshButton.TargetTransform == null)
                {
                    HUXEditorUtils.DrawSubtleMiniLabel("(No target transform specified for scale / offset)");
                }
                else
                {
                    datum.Offset = EditorGUILayout.Vector3Field("Offset", datum.Offset);
                    datum.Scale = EditorGUILayout.Vector3Field("Scale", datum.Scale);

                    if (datum.Scale == Vector3.zero)
                    {
                        GUI.color = HUXEditorUtils.WarningColor;
                        if (GUILayout.Button("Warning: Button state scale is zero. Click here to fix.", EditorStyles.miniButton))
                        {
                            datum.Scale = Vector3.one;
                        }
                    }
                }

                GUI.color = HUXEditorUtils.DefaultColor;
                if (meshButton != null && meshButton.Renderer == null)
                {
                    HUXEditorUtils.DrawSubtleMiniLabel("(No target renderer specified for color / value material properties)");
                }
                else
                {
                    if (!string.IsNullOrEmpty(meshProfile.ColorPropertyName))
                    {
                        datum.StateColor = EditorGUILayout.ColorField(meshProfile.ColorPropertyName + " value", datum.StateColor);
                    }
                    if (!string.IsNullOrEmpty(meshProfile.ValuePropertyName))
                    {
                        datum.StateValue = EditorGUILayout.FloatField(meshProfile.ValuePropertyName + " value", datum.StateValue);
                    }

                }
                HUXEditorUtils.EndSubSectionBox();
            }

            HUXEditorUtils.EndProfileBox();

            HUXEditorUtils.SaveChanges(this);
        }
    }
}