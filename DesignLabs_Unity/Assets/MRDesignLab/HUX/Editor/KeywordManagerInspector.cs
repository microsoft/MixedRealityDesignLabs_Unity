//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Speech;
using System.Collections.Generic;
using UnityEditor;

namespace HUX
{
    [CustomEditor(typeof(KeywordManager))]
    public class KeywordManagerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (!EditorApplication.isPlaying)
                return;

            KeywordManager keywordManager = (KeywordManager)target;
            
            HUXEditorUtils.BeginSectionBox("Registered keywords");
            foreach (KeyValuePair<string,List<string>> command in keywordManager.EditorCommandDescriptions)
            {
                HUXEditorUtils.BeginSubSectionBox(command.Key);
                foreach (string commandTarget in command.Value)
                {
                    EditorGUILayout.LabelField(commandTarget, EditorStyles.wordWrappedLabel);
                }
                HUXEditorUtils.EndSubSectionBox();
            }
            HUXEditorUtils.EndSectionBox();

            HUXEditorUtils.SaveChanges(target);
        }
    }
}