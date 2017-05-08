//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using UnityEditor;
using HUX.Collections;

[CustomEditor(typeof(KeyCollection))]
public class KeyCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default
        base.OnInspectorGUI();

        // Place the button at the bottom
        KeyCollection myScript = (KeyCollection)target;
        if(GUILayout.Button("Generate Keys"))
        {
            myScript.GenerateKeys();
        }
    }
}
