//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Interaction;
using HUX.Receivers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(AppBar))]
    public class AppBarInspector : Editor
    {
        static Vector2 scrollPosition;
        static AppBar.AppBarStateEnum previewState = AppBar.AppBarStateEnum.Default;
        static GUIStyle buttonPreviewStyle;

        const float previewButtonSize = 65f;

        public override void OnInspectorGUI() {
            AppBar appBar = (AppBar)target;

            appBar.DisplayType = (AppBar.AppBarDisplayTypeEnum)EditorGUILayout.EnumPopup("Display Type", appBar.DisplayType);

            if (appBar.DisplayType == AppBar.AppBarDisplayTypeEnum.Manipulation)
            {
                HUXEditorUtils.BeginSectionBox("Bounding box");
                appBar.BoundingBox = HUXEditorUtils.SceneObjectField<BoundingBoxManipulate>(null, appBar.BoundingBox);
                if (appBar.BoundingBox == null)
                {
                    HUXEditorUtils.WarningMessage("Manipulation state will not function correctly at runtime without a bounding box. (If you're using BoundingBoxTarget this is not a problem.)");
                }
                HUXEditorUtils.EndSectionBox();
            }

            HUXEditorUtils.BeginSectionBox("App bar options");
            appBar.SquareButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Button Prefab", appBar.SquareButtonPrefab, typeof(GameObject));
            GUI.color = (appBar.CustomButtonIconProfile == null) ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor;
            appBar.CustomButtonIconProfile = (ButtonIconProfile)EditorGUILayout.ObjectField("Custom Icon Profile", appBar.CustomButtonIconProfile, typeof(ButtonIconProfile));
            HUXEditorUtils.DrawSubtleMiniLabel("(Leave blank to use the button's default profile)");
            GUI.color = HUXEditorUtils.DefaultColor;

            if (appBar.SquareButtonPrefab == null) {
                HUXEditorUtils.ErrorMessage("You must specify a button prefab");
                HUXEditorUtils.EndSectionBox();
                return;
            }

            CompoundButtonIcon icon = appBar.SquareButtonPrefab.GetComponent<CompoundButtonIcon>();
            if (icon == null) {
                HUXEditorUtils.ErrorMessage("You must use a button prefab that has a CompoundButtonIcon component");
                HUXEditorUtils.EndSectionBox();
                return;
            }

            HUXEditorUtils.BeginSubSectionBox("Default buttons");
            GUI.color = appBar.UseHide ? HUXEditorUtils.DefaultColor : HUXEditorUtils.DisabledColor;
            appBar.UseHide = EditorGUILayout.Toggle("Show / Hide Buttons", appBar.UseHide);
            GUI.color = appBar.UseAdjust ? HUXEditorUtils.DefaultColor : HUXEditorUtils.DisabledColor;
            appBar.UseAdjust = EditorGUILayout.Toggle("Adjust / Done Buttons", appBar.UseAdjust);
            GUI.color = appBar.UseRemove ? HUXEditorUtils.DefaultColor : HUXEditorUtils.DisabledColor;
            appBar.UseRemove = EditorGUILayout.Toggle("Remove Button", appBar.UseRemove);
            GUI.color = HUXEditorUtils.DefaultColor;
            HUXEditorUtils.EndSubSectionBox();

            HUXEditorUtils.BeginSubSectionBox("Custom buttons");
            HUXEditorUtils.DrawSubtleMiniLabel("Up to " + AppBar.MaxCustomButtons + " allowed. Un-named buttons will be ignored.");
            //HUXEditorUtils.DrawProfileField <ButtonIconProfile> (appBar.CustomButtonIconProfile)

            // Get the profile we'll be using for our icons
            ButtonIconProfile profile = appBar.CustomButtonIconProfile;
            if (profile == null) {
                profile = icon.IconProfile;
                if (profile == null) {
                    HUXEditorUtils.ErrorMessage("The button prefab does not specify an icon profile. Can't continue.");
                    HUXEditorUtils.EndSectionBox();
                    return;
                }
            }

            AppBar.ButtonTemplate[] buttons = appBar.Buttons;
            if (buttons.Length != AppBar.MaxCustomButtons) {
                System.Array.Resize<AppBar.ButtonTemplate>(ref buttons, AppBar.MaxCustomButtons);
            }
            int numCustomButtons = appBar.UseHide ? 0 : -1;
            for (int i = 0; i < buttons.Length; i++) {
                buttons[i] = DrawButtonEditor(buttons[i], profile, ref numCustomButtons, "buttons", i);
            }
            appBar.Buttons = buttons;
            HUXEditorUtils.EndSubSectionBox();

            // Force the buttons to refresh based on the options we've specified
            appBar.EditorRefreshTemplates();

            HUXEditorUtils.BeginSubSectionBox("App bar preview");
            HUXEditorUtils.DrawSubtleMiniLabel("An approximation of what the final bar will look like. 'Hidden' and 'Manipulation' states depend on default button settings and may not be available.");
            previewState = (AppBar.AppBarStateEnum)EditorGUILayout.EnumPopup(previewState);
            List<AppBar.ButtonTemplate> buttonList = new List<AppBar.ButtonTemplate>();
            buttonList.AddRange(appBar.DefaultButtons);
            buttonList.AddRange(buttons);

            if (previewState == AppBar.AppBarStateEnum.Default) {
                buttonList.Sort(delegate (AppBar.ButtonTemplate b1, AppBar.ButtonTemplate b2) { return b1.DefaultPosition.CompareTo(b2.DefaultPosition); });
            } else {
                buttonList.Sort(delegate (AppBar.ButtonTemplate b1, AppBar.ButtonTemplate b2) { return b1.ManipulationPosition.CompareTo(b2.ManipulationPosition); });
            }


            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.MaxHeight (previewButtonSize + 15f));
            EditorGUILayout.BeginHorizontal();
            bool drewOneButton = false;
            for (int i = 0; i < buttonList.Count; i++) {
                drewOneButton |= DrawPreviewButton(buttonList[i], previewState, appBar.UseHide, appBar.UseAdjust, appBar.UseRemove);
            }
            if (!drewOneButton) {
                HUXEditorUtils.WarningMessage("This state has no buttons due to the options you've chosen. It won't be permitted during play mode.");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            HUXEditorUtils.EndSubSectionBox();

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(appBar);
        }

        private AppBar.ButtonTemplate DrawButtonEditor(AppBar.ButtonTemplate template, ButtonIconProfile profile, ref int numCustomButtons, string arrayName, int templateIndex) {
            // Set the button's position immediately
            template.DefaultPosition = 0;
            if (!template.IsEmpty) {
                numCustomButtons++;
                template.DefaultPosition = numCustomButtons;
            }

            HUXEditorUtils.BeginSectionBox(template.IsEmpty ? "(Empty)" : template.Name + " (" + template.DefaultPosition + ")", HUXEditorUtils.ObjectColor);
            template.Type = AppBar.ButtonTypeEnum.Custom;
            GUI.color = template.IsEmpty ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor;
            EditorGUILayout.BeginHorizontal();
            template.Name = EditorGUILayout.TextField("Button Name", template.Name);
            if (GUILayout.Button ("Clear", EditorStyles.miniButton)) {
                template.Name = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            if (!template.IsEmpty) {
                if (string.IsNullOrEmpty (template.Text)) {
                    GUI.color = HUXEditorUtils.WarningColor;
                }
                template.Text = EditorGUILayout.TextField("Label Text", template.Text);
                GUI.color = HUXEditorUtils.DefaultColor;
                string[] keys = profile.GetIconKeys().ToArray();
                int selectedIndex = 0;
                for (int i = 0; i < keys.Length; i++) {
                    if (keys[i].Equals(template.Icon)) {
                        selectedIndex = i;
                        break;
                    }
                }
                selectedIndex = EditorGUILayout.Popup("Icon", selectedIndex, keys);
                template.Icon = keys[selectedIndex];

                template.EventTarget = HUXEditorUtils.SceneObjectField<InteractionReceiver>("Interaction Receiver", template.EventTarget);
                if (template.EventTarget == null) {
                    HUXEditorUtils.WarningMessage("This button will have no effect until you choose an interaction receiver to register it with.");
                }
            }
            
            // Draw the unity message section
            /*string propertyName = string.Format("{0}.Array.data[{1}].OnTappedEvent", arrayName, templateIndex);
            SerializedProperty buttonEvent = serializedObject.FindProperty(propertyName);
            EditorGUILayout.PropertyField(buttonEvent);
            if (GUI.changed) {
                serializedObject.ApplyModifiedProperties();
            }*/

            HUXEditorUtils.EndSectionBox();
            return template;
        }

        private bool DrawPreviewButton (AppBar.ButtonTemplate template, AppBar.AppBarStateEnum state, bool useHide, bool useAdjust, bool useRemove) {
            bool drewButton = false;

            if (template.IsEmpty)
                return drewButton;

            if (buttonPreviewStyle == null) {
                buttonPreviewStyle = new GUIStyle(EditorStyles.toolbarButton);
                buttonPreviewStyle.fontSize = 8;
                buttonPreviewStyle.fixedHeight = previewButtonSize;
                buttonPreviewStyle.fixedWidth = previewButtonSize;
            }

            GUI.color = Color.Lerp(Color.gray, HUXEditorUtils.DefaultColor, 0.5f);

            switch (state) {
                case AppBar.AppBarStateEnum.Default:
                    switch (template.Type) {
                        case AppBar.ButtonTypeEnum.Custom:
                            GUILayout.Button(template.Text, buttonPreviewStyle);
                            drewButton = true;
                            break;

                        case AppBar.ButtonTypeEnum.Adjust:
                            if (useAdjust) {
                                GUILayout.Button(template.Text, buttonPreviewStyle);
                                drewButton = true;
                            }
                            break;

                        case AppBar.ButtonTypeEnum.Hide:
                            if (useHide) {
                                GUILayout.Button(template.Text, buttonPreviewStyle);
                                drewButton = true;
                            }
                            break;

                        case AppBar.ButtonTypeEnum.Remove:
                            if (useRemove) {
                                GUILayout.Button(template.Text, buttonPreviewStyle);
                                drewButton = true;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case AppBar.AppBarStateEnum.Hidden:
                   switch (template.Type) {
                        case AppBar.ButtonTypeEnum.Show:
                            if (useHide) {
                                GUILayout.Button(template.Text, buttonPreviewStyle);
                                drewButton = true;
                            }
                            break;

                        default:
                            break;
                    }
                    break;

                case AppBar.AppBarStateEnum.Manipulation:
                    switch (template.Type) {
                        default:
                            break;

                        case AppBar.ButtonTypeEnum.Done:
                            GUILayout.Button(template.Text, buttonPreviewStyle);
                            drewButton = true;
                            break;

                        case AppBar.ButtonTypeEnum.Remove:
                            if (useRemove) {
                                GUILayout.Button(template.Text, buttonPreviewStyle);
                                drewButton = true;
                            }
                            break;
                    }
                    break;
            }

            GUI.color = HUXEditorUtils.DefaultColor;

            return drewButton;
        }
    }
}