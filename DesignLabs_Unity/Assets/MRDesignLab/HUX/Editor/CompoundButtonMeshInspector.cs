//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
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

            // Draw the profile
            HUXEditorUtils.DrawProfileInspector(meshButton.MeshProfile, meshButton);

            HUXEditorUtils.SaveChanges(target, meshButton.MeshProfile);
        }
    }
}
