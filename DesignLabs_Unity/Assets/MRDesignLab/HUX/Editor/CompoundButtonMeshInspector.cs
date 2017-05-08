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
    [CustomEditor(typeof(CompoundButtonMesh))]
    public class CompoundButtonMeshInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButtonMesh meshButton = (CompoundButtonMesh)target;

            GUI.color = HUXEditorUtils.DefaultColor;
            meshButton.MeshProfile = HUXEditorUtils.DrawProfileField<ButtonMeshProfile>(meshButton.MeshProfile);

            if (meshButton.MeshProfile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.BeginSectionBox("Target objects");
            meshButton.TargetTransform = HUXEditorUtils.DropDownComponentField<Transform> ("Transform", meshButton.TargetTransform, meshButton.transform);
            if (meshButton.TargetTransform != null && meshButton.TargetTransform == meshButton.transform)
            {
                HUXEditorUtils.WarningMessage("Button may behave strangely if scale & offset is applied to transform root. Consider choosing a child transform.");
            } else if (meshButton.TargetTransform != null)
            {
                // Check to see if offset & scale match any of the button defaults
                bool foundCloseState = false;
                foreach (CompoundButtonMesh.MeshButtonDatum datum in meshButton.MeshProfile.ButtonStates)
                {
                    if (meshButton.TargetTransform.localPosition == datum.Offset && meshButton.TargetTransform.localScale == datum.Scale)
                    {
                        foundCloseState = true;
                        break;
                    }
                }
                if (!foundCloseState)
                {
                    HUXEditorUtils.WarningMessage("Transform doesn't match the scale / offset of any button states. Button may appear different at runtime.");
                }
            }

            GUI.color = HUXEditorUtils.DefaultColor;
            meshButton.Renderer = HUXEditorUtils.DropDownComponentField<MeshRenderer>("Mesh Renderer", meshButton.Renderer, meshButton.transform);
            //meshButton.MeshFilter = HUXEditorUtils.DropDownComponentField<MeshFilter>("Mesh Filter", meshButton.MeshFilter, meshButton.transform);

            HUXEditorUtils.EndSectionBox();
            
            HUXEditorUtils.BeginSectionBox("Target material properties", (meshButton.Renderer == null ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor));
            if (meshButton.Renderer == null)
            {
                HUXEditorUtils.DrawSubtleMiniLabel("(No renderer specified)");
            } else
            {
                meshButton.MeshProfile.ColorPropertyName = HUXEditorUtils.MaterialPropertyName(
                    meshButton.MeshProfile.ColorPropertyName,
                    meshButton.Renderer.sharedMaterial,
                    ShaderUtil.ShaderPropertyType.Color);

                meshButton.MeshProfile.ValuePropertyName = HUXEditorUtils.MaterialPropertyName(
                    meshButton.MeshProfile.ValuePropertyName,
                    meshButton.Renderer.sharedMaterial,
                    ShaderUtil.ShaderPropertyType.Float);
                
            }

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("States", HUXEditorUtils.WarningColor);
            HUXEditorUtils.DrawSubtleMiniLabel("(Warning: this section edits the button profile. These changes will affect all buttons that use this profile.)");
            // Draw an editor for each state datum
            meshButton.MeshProfile.SmoothStateChanges = EditorGUILayout.Toggle("Smooth state changes", meshButton.MeshProfile.SmoothStateChanges);
            if (meshButton.MeshProfile.SmoothStateChanges)
            {
                meshButton.MeshProfile.AnimationSpeed = EditorGUILayout.Slider("Animation speed", meshButton.MeshProfile.AnimationSpeed, 0.01f, 1f);
            }
            meshButton.MeshProfile.StickyPressedEvents = EditorGUILayout.Toggle("'Sticky' pressed events", meshButton.MeshProfile.StickyPressedEvents);
            if (meshButton.MeshProfile.StickyPressedEvents)
            {
                meshButton.MeshProfile.StickyPressedTime = EditorGUILayout.Slider("'Sticky' pressed event time", meshButton.MeshProfile.StickyPressedTime, 0.01f, 1f);
            }

            // Validate our button states - ensure there's one for each button state enum value
            Button.ButtonStateEnum[] buttonStates = (Button.ButtonStateEnum[])System.Enum.GetValues(typeof(Button.ButtonStateEnum));
            List<CompoundButtonMesh.MeshButtonDatum> missingStates = new List<CompoundButtonMesh.MeshButtonDatum>();
            foreach (Button.ButtonStateEnum buttonState in buttonStates)
            {
                bool foundState = false;
                foreach (CompoundButtonMesh.MeshButtonDatum datum in meshButton.MeshProfile.ButtonStates)
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
                missingStates.AddRange(meshButton.MeshProfile.ButtonStates);
                meshButton.MeshProfile.ButtonStates = missingStates.ToArray();
            }

            foreach (CompoundButtonMesh.MeshButtonDatum datum in meshButton.MeshProfile.ButtonStates)
            {
                HUXEditorUtils.BeginSubSectionBox(datum.ActiveState.ToString());
                //datum.Name = EditorGUILayout.TextField("Name", datum.Name);
                if (meshButton.TargetTransform != null)
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
                else
                {
                    HUXEditorUtils.DrawSubtleMiniLabel("(No target transform specified for scale / offset)");
                }

                GUI.color = HUXEditorUtils.DefaultColor;
                if (meshButton.Renderer != null)
                {
                    if (!string.IsNullOrEmpty(meshButton.MeshProfile.ColorPropertyName))
                    {
                        datum.StateColor = EditorGUILayout.ColorField(meshButton.MeshProfile.ColorPropertyName + " value", datum.StateColor);
                    }
                    if (!string.IsNullOrEmpty(meshButton.MeshProfile.ValuePropertyName))
                    {
                        datum.StateValue = EditorGUILayout.FloatField(meshButton.MeshProfile.ValuePropertyName + " value", datum.StateValue);
                    }
                }
                else
                {
                    HUXEditorUtils.DrawSubtleMiniLabel("(No target renderer specified for color / value material properties)");
                }
                HUXEditorUtils.EndSubSectionBox();
            }

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target, meshButton.MeshProfile);
        }
    }
}
