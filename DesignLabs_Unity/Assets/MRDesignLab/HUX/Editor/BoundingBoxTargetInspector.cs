//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Interaction;
using UnityEditor;
using UnityEngine;

namespace HUX
{
    [CustomEditor(typeof(BoundingBoxTarget))]
    public class BoundingBoxTargetInspector : Editor
    {
        const string BoundingBoxPrefabPath = "Assets/MRDesignLab/HUX/Prefabs/Dialogs/BoundingBox.prefab";
        const string ToolbarPrefabPath = "Assets/MRDesignLab/HUX/Prefabs/Dialogs/ManipulationToolbar.prefab";

        public override void OnInspectorGUI()
        {
            BoundingBoxTarget bbt = (BoundingBoxTarget)target;

            // See if there's a bounding box yet
            BoundingBoxManipulate bbm = GameObject.FindObjectOfType<BoundingBoxManipulate>();
            ManipulationToolbar toolbar = GameObject.FindObjectOfType<ManipulationToolbar>();
            if (bbm == null || toolbar == null)
            {                
                HUXEditorUtils.ErrorMessage(
                    "Couldn't find a bounding box prefab and/or manipulation toolbar in the scene. Bounding box target won't work without them.",
                    AddBoundingBox);
            }

            HUXEditorUtils.DrawFilterTagField(serializedObject, "TagOnSelected");
            HUXEditorUtils.DrawFilterTagField(serializedObject, "TagOnDeselected");
            bbt.PermittedOperations = (BoundingBoxManipulate.OperationEnum) HUXEditorUtils.EnumCheckboxField<BoundingBoxManipulate.OperationEnum>(
                "Permitted Operations",
                bbt.PermittedOperations,
                "Default",
                BoundingBoxManipulate.OperationEnum.ScaleUniform | BoundingBoxManipulate.OperationEnum.RotateY | BoundingBoxManipulate.OperationEnum.Drag,
                BoundingBoxManipulate.OperationEnum.Drag);

            bbt.ManipulationDisplay = (ManipulationToolbar.DisplayEnum)EditorGUILayout.EnumPopup("Toolbar Display", bbt.ManipulationDisplay);


            HUXEditorUtils.SaveChanges(bbt);
        }

        private void AddBoundingBox ()
        {
            BoundingBoxManipulate bbm = GameObject.FindObjectOfType<BoundingBoxManipulate>();
            ManipulationToolbar toolbar = GameObject.FindObjectOfType<ManipulationToolbar>();
            if (bbm == null)
            {
                Object prefab = AssetDatabase.LoadAssetAtPath(BoundingBoxPrefabPath, typeof(GameObject));
                GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                clone.name = prefab.name;
            }
            if (toolbar == null)
            {
                Object prefab = AssetDatabase.LoadAssetAtPath(ToolbarPrefabPath, typeof(GameObject));
                GameObject clone = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                clone.name = prefab.name;
            }
        }
    }
}