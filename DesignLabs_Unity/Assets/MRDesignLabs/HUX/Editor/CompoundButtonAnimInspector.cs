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
    [CustomEditor(typeof(CompoundButtonAnim))]
    public class CompoundButtonAnimInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CompoundButtonAnim acb = (CompoundButtonAnim)target;

            if (UnityEditor.Selection.gameObjects.Length == 1)
            {
                acb.TargetAnimator = HUXEditorUtils.DropDownComponentField<Animator>("Target animator", acb.TargetAnimator, acb.transform);

                if (acb.TargetAnimator == null)
                {
                    GUI.color = HUXEditorUtils.ErrorColor;
                    EditorGUILayout.LabelField("You must chooes a target animator.");
                    HUXEditorUtils.SaveChanges(target);
                    return;

                }
            } else
            {
                EditorGUILayout.LabelField("(This section not supported for multiple objects)", EditorStyles.miniLabel);
            }

            Animator animator = acb.TargetAnimator;
            AnimatorControllerParameter[] animParams = null;

            if (!animator.isInitialized)
            {
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            // Validate the AnimButton controls - make sure there's one control for each button state
            Button.ButtonStateEnum[] buttonStates = (Button.ButtonStateEnum[])System.Enum.GetValues(typeof(Button.ButtonStateEnum));
            if (acb.AnimActions == null || acb.AnimActions.Length != buttonStates.Length)
            {
                acb.AnimActions = new CompoundButtonAnim.AnimatorControllerAction[buttonStates.Length];
            }

            // Don't allow user to change setup during play mode
            if (!Application.isPlaying)
            {
                // Get the available animation parameters
                animParams = animator.parameters;

                for (int i = 0; i < buttonStates.Length; i++)
                {
                    acb.AnimActions[i].ButtonState = buttonStates[i];
                }

                // Now make sure all animation parameters are found
                for (int i = 0; i < acb.AnimActions.Length; i++)
                {
                    if (!string.IsNullOrEmpty(acb.AnimActions[i].ParamName))
                    {
                        bool invalidParam = true;
                        foreach (AnimatorControllerParameter animParam in animParams)
                        {
                            if (acb.AnimActions[i].ParamName == animParam.name)
                            {
                                // Update the type while we're here
                                invalidParam = false;
                                acb.AnimActions[i].ParamType = animParam.type;
                                break;
                            }
                        }

                        // If we didn't find it, mark it as invalid
                        acb.AnimActions[i].InvalidParam = invalidParam;
                    }
                }
            }

            if (!acb.gameObject.activeInHierarchy)
            {
                //GUI.color = HUX.Editor.EditorStyles.ColorWarning;
                EditorGUILayout.LabelField("(Gameobject must be active to view animation actions)");
                HUXEditorUtils.SaveChanges(target);
                return;
            }

            // Draw the editor for all the anim actions
            HUXEditorUtils.BeginSectionBox("Animation actions");
            for (int i = 0; i < acb.AnimActions.Length; i++)
            {
                acb.AnimActions[i] = DrawAnimActionEditor(acb.AnimActions[i], animParams);
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target);
        }

        CompoundButtonAnim.AnimatorControllerAction DrawAnimActionEditor (CompoundButtonAnim.AnimatorControllerAction action, AnimatorControllerParameter [] animParams)
        {
            bool actionIsEmpty = string.IsNullOrEmpty(action.ParamName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(action.ButtonState.ToString(), GUILayout.MaxWidth(150f), GUILayout.MinWidth(150f));

            if (animParams != null && animParams.Length > 0)
            {
                // Show a dropdown
                string[] options = new string[animParams.Length + 1];
                options[0] = "(None)";
                int currentIndex = 0;
                for (int i = 0; i < animParams.Length; i++)
                {
                    options[i + 1] = animParams[i].name;
                    if (animParams[i].name == action.ParamName)
                    {
                        currentIndex = i + 1;
                    }
                }
                GUI.color = actionIsEmpty ? Color.gray : Color.white;
                int newIndex = EditorGUILayout.Popup(currentIndex, options, GUILayout.MinWidth(150f), GUILayout.MaxWidth(150f));
                if (newIndex == 0)
                {
                    action.ParamName = string.Empty;
                }
                else
                {
                    action.ParamName = animParams[newIndex - 1].name;
                    action.ParamType = animParams[newIndex - 1].type;
                }
            } else
            {
                // Just show a label
                GUI.color = action.InvalidParam ? Color.yellow : Color.white; 
                EditorGUILayout.LabelField(actionIsEmpty ? "(None)" : action.ParamName, GUILayout.MinWidth (75f), GUILayout.MaxWidth (75f));
            }

            GUI.color = Color.white;

            if (!actionIsEmpty)
            {
                EditorGUILayout.LabelField(action.ParamType.ToString(), EditorStyles.miniLabel, GUILayout.MinWidth(75f), GUILayout.MaxWidth(75f));
                switch (action.ParamType)
                {
                    case AnimatorControllerParameterType.Bool:
                        action.BoolValue = EditorGUILayout.Toggle(action.BoolValue);
                        break;

                    case AnimatorControllerParameterType.Float:
                        action.FloatValue = EditorGUILayout.FloatField(action.FloatValue);
                        break;

                    case AnimatorControllerParameterType.Int:
                        action.IntValue = EditorGUILayout.IntField(action.IntValue);
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        break;

                    default:
                        break;

                }
            }

            EditorGUILayout.EndHorizontal();

            return action;
        }
    }
}
