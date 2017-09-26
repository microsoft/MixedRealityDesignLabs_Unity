//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Collections;
using HUX.Dialogs;
using HUX.Receivers;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(SimpleMenuCollection))]
    public class SimpleMenuCollectionInspector : Editor
    {
        static Vector2 scrollPosition;
        static GUIStyle buttonPreviewStyle;
        const float previewButtonSizeX = 185f;
        const float previewButtonSizeY = 55f;

        public override void OnInspectorGUI() {
            SimpleMenuCollection menu = (SimpleMenuCollection)target;

            HUXEditorUtils.BeginSectionBox("Menu settings");

            menu.DisplayTitle = EditorGUILayout.Toggle("Display Title", menu.DisplayTitle);
            if (menu.DisplayTitle) {
                //menu.TitleText.font = (Font)EditorGUILayout.ObjectField("Title font", menu.TitleText.font, typeof(Font), false);
                menu.Title = EditorGUILayout.TextField("Title", menu.Title);
                menu.TitleText = HUXEditorUtils.DropDownComponentField<TextMesh>("Title TextMesh", menu.TitleText, menu.transform);
            }
            menu.ButtonPrefab = (GameObject)EditorGUILayout.ObjectField("Button prefab", menu.ButtonPrefab, typeof(GameObject), false);
            menu.ParentCollection = HUXEditorUtils.DropDownComponentField<ObjectCollection>("Collection parent", menu.ParentCollection, menu.transform);

            if (menu.ButtonPrefab == null) {
                HUXEditorUtils.ErrorMessage("You must specify a button prefab");
                HUXEditorUtils.EndSectionBox();
                return;
            }

            if (menu.ParentCollection == null)
            {
                HUXEditorUtils.ErrorMessage("This menu needs a collection component to work", AddCollection, "Fix");
                HUXEditorUtils.EndSectionBox();
                return;
            }

            bool showIcon = false;
            bool showText = false;

            CompoundButtonIcon icon = menu.ButtonPrefab.GetComponent<CompoundButtonIcon>();
            showIcon = icon != null;
                

            CompoundButtonText text = menu.ButtonPrefab.GetComponent<CompoundButtonText>();
            showText = text != null;

            ButtonIconProfile profile = null;
            if (icon != null)
            {
                profile = icon.Profile;
            }

            HUXEditorUtils.BeginSubSectionBox("Buttons");
            HUXEditorUtils.DrawSubtleMiniLabel("Up to " + SimpleMenuCollection.MaxButtons + " allowed. Un-named buttons will be ignored.");

            SimpleMenuCollectionButton[] buttons = menu.Buttons;
            HashSet<string> buttonNamesSoFar = new HashSet<string>();
            int numButtons = 0;
            for (int i = 0; i < buttons.Length; i++) {
                if (!buttons[i].IsEmpty)
                {
                    numButtons++;
                }
                DrawButtonEditor(buttons[i], showIcon, showText, profile, "buttons", i, buttonNamesSoFar);
            }
            HUXEditorUtils.EndSubSectionBox();

            menu.EditorRefreshButtons();

            HUXEditorUtils.BeginSubSectionBox("Menu preview");
            HUXEditorUtils.DrawSubtleMiniLabel("An approximation of what the menu will look like.");

            List<SimpleMenuCollectionButton> buttonsList = new List<SimpleMenuCollectionButton>(buttons);
            buttonsList.Sort(delegate (SimpleMenuCollectionButton b1, SimpleMenuCollectionButton b2) { return b1.Index.CompareTo(b2.Index); });
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.MinHeight (previewButtonSizeY * numButtons + 50f));
            EditorGUILayout.BeginVertical();
            bool drewOneButton = false;
            foreach (SimpleMenuCollectionButton button in buttonsList)
            {
                drewOneButton = DrawPreviewButton(button, showText);
            }
            if (!drewOneButton) {
                HUXEditorUtils.WarningMessage("This state has no buttons due to the options you've chosen. It won't be permitted during play mode.");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            HUXEditorUtils.EndSubSectionBox();

            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(menu);
        }

        private void AddCollection ()
        {
            SimpleMenuCollection menu = (SimpleMenuCollection)target;
            ObjectCollection collection = menu.GetComponentInChildren<ObjectCollection>();
            if (collection == null)
            {
                collection = menu.gameObject.AddComponent<ObjectCollection>();
            }
            menu.ParentCollection = collection;
        }

        private void DrawButtonEditor(SimpleMenuCollectionButton template, bool showIcon, bool showText, ButtonIconProfile profile, string arrayName, int templateIndex, HashSet<string> buttonNamesSoFar) {

            HUXEditorUtils.BeginSectionBox(template.IsEmpty ? "(Empty)" : template.Name + " (" + template.Index + ")", HUXEditorUtils.ObjectColor);
            GUI.color = template.IsEmpty ? HUXEditorUtils.DisabledColor : HUXEditorUtils.DefaultColor;
            EditorGUILayout.BeginHorizontal();
            template.Name = EditorGUILayout.TextField("Button Name", template.Name);
            if (GUILayout.Button ("Clear", EditorStyles.miniButton)) {
                template.Name = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(template.Name) && buttonNamesSoFar.Contains(template.Name))
            {
                HUXEditorUtils.ErrorMessage("Buttons must have unique names.");
            }
            else
            {
                buttonNamesSoFar.Add(template.Name);
            }
            if (!template.IsEmpty) {
                if (showText)
                {
                    if (string.IsNullOrEmpty(template.Text))
                    {
                        GUI.color = HUXEditorUtils.WarningColor;
                    }
                    template.Text = EditorGUILayout.TextField("Label Text", template.Text);
                }

                if (showIcon)
                {
                    GUI.color = HUXEditorUtils.DefaultColor;
                    string[] keys = profile.GetIconKeys().ToArray();
                    int selectedIndex = 0;
                    for (int i = 0; i < keys.Length; i++)
                    {
                        if (keys[i].Equals(template.Icon))
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                    selectedIndex = EditorGUILayout.Popup("Icon", selectedIndex, keys);
                    template.Icon = keys[selectedIndex];
                }

                if (!Application.isPlaying) {
                    template.Target = HUXEditorUtils.SceneObjectField<InteractionReceiver>("Interaction Receiver", template.Target);
                    if (template.Target == null) {
                        HUXEditorUtils.WarningMessage("This button will have no effect until you choose an interaction receiver to register it with.");
                    }
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
        }

        private bool DrawPreviewButton (SimpleMenuCollectionButton template, bool showText) {
            bool drewButton = false;

            if (template.IsEmpty)
                return drewButton;

            if (buttonPreviewStyle == null) {
                buttonPreviewStyle = new GUIStyle(EditorStyles.toolbarButton);
                buttonPreviewStyle.fontSize = 12;
                buttonPreviewStyle.fixedHeight = previewButtonSizeY;
                buttonPreviewStyle.fixedWidth = previewButtonSizeX;
            }

            GUI.color = Color.white;

            GUILayout.Button(showText ? template.Text : template.Name, buttonPreviewStyle);

            GUI.color = HUXEditorUtils.DefaultColor;

            return drewButton;
        }
    }
}