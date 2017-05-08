//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Focus;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(CompoundButton))]
    public class CompoundButtonInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CompoundButton cb = (CompoundButton)target;

            // Don't perform this check at runtime
            if (!Application.isPlaying)
            {
                // First, check our colliders
                // Get the components we need for the button to be visible
                Rigidbody parentRigidBody = cb.GetComponent<Rigidbody>();
                Collider parentCollider = cb.GetComponent<Collider>();
                // Get all child colliders that AREN'T the parent collider
                HashSet<Collider> childColliders = new HashSet<Collider>(cb.GetComponentsInChildren<Collider>());
                childColliders.Remove(parentCollider);

                bool foundError = false;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (parentCollider == null)
                {
                    if (childColliders.Count == 0)
                    {
                        foundError = true;
                        GUI.color = HUXEditorUtils.ErrorColor;
                        EditorGUILayout.LabelField("Error: Button must have at least 1 collider to be visible, preferably on the root transform.", EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button("Fix now"))
                        {
                            cb.gameObject.AddComponent<BoxCollider>();
                        }
                    }
                    else if (parentRigidBody == null)
                    {
                        foundError = true;
                        GUI.color = HUXEditorUtils.ErrorColor;
                        EditorGUILayout.LabelField("Error: Button requires a rigidbody if colliders are only present on child transforms.", EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button("Fix now"))
                        {
                            Rigidbody rb = cb.gameObject.AddComponent<Rigidbody>();
                            rb.isKinematic = true;
                        }
                    }
                    else if (!parentRigidBody.isKinematic)
                    {
                        foundError = true;
                        GUI.color = HUXEditorUtils.WarningColor;
                        EditorGUILayout.LabelField("Warning: Button rigid body is not kinematic - this is not recommended.", EditorStyles.wordWrappedLabel);
                        if (GUILayout.Button("Fix now"))
                        {
                            parentRigidBody.isKinematic = true;
                        }
                    }
                }

                // If our colliders were fine, next check the interaction manager to make sure the button will be visible
                if (!foundError)
                {
                    FocusManager fm = GameObject.FindObjectOfType<FocusManager>();
                    if (fm != null)
                    {
                        if (fm.RaycastLayerMask != (fm.RaycastLayerMask | (1 << cb.gameObject.layer)))
                        {
                            foundError = true;
                            GUI.color = HUXEditorUtils.WarningColor;
                            EditorGUILayout.LabelField("Warning: Button is not on a layer that the focus manager can see. Button will not receive input until either the button's layer or the FocusManager's RaycastLayerMask is changed. (This may be intentional.)", EditorStyles.wordWrappedLabel);
                            if (fm.RaycastLayerMask.value != 0)
                            {
                                if (GUILayout.Button("Fix now"))
                                {
                                    // Put the button on the first layer that the layer mask can see
                                    for (int i = 0; i < 32; i++)
                                    {
                                        if (fm.RaycastLayerMask == (fm.RaycastLayerMask | (1 << i)))
                                        {
                                            cb.gameObject.layer = i;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField("(Couldn't find a focus manager to perform visibility check)", EditorStyles.wordWrappedLabel);
                    }
                }

                if (!foundError)
                {
                    GUI.color = HUXEditorUtils.SuccessColor;
                    EditorGUILayout.LabelField("Button is good to go!", EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.EndVertical();
            }

            GUI.color = HUXEditorUtils.DefaultColor;
            HUXEditorUtils.BeginSectionBox("Button properties", HUXEditorUtils.DefaultColor);
            // Draw the filter tag field
            HUXEditorUtils.DrawFilterTagField(serializedObject, "FilterTag");
            cb.RequireGaze = EditorGUILayout.Toggle("Require Gaze", cb.RequireGaze);
            cb.ButtonState = (Button.ButtonStateEnum)EditorGUILayout.EnumPopup("Button State", cb.ButtonState);
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.BeginSectionBox("Commonly referenced components", HUXEditorUtils.DefaultColor);
            cb.MainCollider = HUXEditorUtils.DropDownComponentField<Collider>("Main collider", cb.MainCollider, cb.transform);
            cb.MainRenderer = HUXEditorUtils.DropDownComponentField<MeshRenderer>("Main renderer", cb.MainRenderer, cb.transform);
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(cb);
        }
    }
}
