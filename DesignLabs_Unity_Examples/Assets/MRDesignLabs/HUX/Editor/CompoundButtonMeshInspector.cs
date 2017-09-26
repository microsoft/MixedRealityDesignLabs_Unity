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
    [CustomEditor(typeof(CompoundButtonMesh))]
    public class CompoundButtonMeshInspector : Editor
    {
        SerializedProperty profileProp;

        void OnEnable()
        {
            profileProp = serializedObject.FindProperty("Profile");
        }

        public override void OnInspectorGUI()
        {
            CompoundButtonMesh meshButton = (CompoundButtonMesh)target;

            GUI.color = HUXEditorUtils.DefaultColor;
            profileProp.objectReferenceValue = HUXEditorUtils.DrawProfileField<ButtonMeshProfile>(profileProp.objectReferenceValue as ButtonMeshProfile);

            if (meshButton.Profile == null)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            HUXEditorUtils.BeginSectionBox("Target objects");
            if (UnityEditor.Selection.gameObjects.Length == 1)
            {
                meshButton.TargetTransform = HUXEditorUtils.DropDownComponentField<Transform>("Transform", meshButton.TargetTransform, meshButton.transform);
                if (meshButton.TargetTransform != null && meshButton.TargetTransform == meshButton.transform)
                {
                    HUXEditorUtils.WarningMessage("Button may behave strangely if scale & offset is applied to transform root. Consider choosing a child transform.");
                }
                else if (meshButton.TargetTransform != null)
                {
                    // Check to see if offset & scale match any of the button defaults
                    bool foundCloseState = false;
                    foreach (CompoundButtonMesh.MeshButtonDatum datum in meshButton.Profile.ButtonStates)
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
            } else
            {
                EditorGUILayout.LabelField("(This section not supported for multiple objects)", EditorStyles.miniLabel);
            }

            HUXEditorUtils.EndSectionBox();
            
            HUXEditorUtils.BeginSectionBox("Target material properties", (meshButton.Renderer == null ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor));
            if (meshButton.Renderer == null)
            {
                HUXEditorUtils.DrawSubtleMiniLabel("(No renderer specified)");
            } else
            {
                meshButton.Profile.ColorPropertyName = HUXEditorUtils.MaterialPropertyName(
                    meshButton.Profile.ColorPropertyName,
                    meshButton.Renderer.sharedMaterial,
                    ShaderUtil.ShaderPropertyType.Color);

                meshButton.Profile.ValuePropertyName = HUXEditorUtils.MaterialPropertyName(
                    meshButton.Profile.ValuePropertyName,
                    meshButton.Renderer.sharedMaterial,
                    ShaderUtil.ShaderPropertyType.Float);
                
            }

            HUXEditorUtils.EndSectionBox();

            // Draw the profile
            HUXEditorUtils.DrawProfileInspector(meshButton.Profile, meshButton);

            HUXEditorUtils.SaveChanges(target, meshButton.Profile);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
