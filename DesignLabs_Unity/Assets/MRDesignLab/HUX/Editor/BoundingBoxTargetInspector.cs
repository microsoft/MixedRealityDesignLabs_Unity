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
        public override void OnInspectorGUI()
        {
            BoundingBoxTarget bbt = (BoundingBoxTarget)target;

            HUXEditorUtils.DrawFilterTagField(serializedObject, "TagOnSelected");
            HUXEditorUtils.DrawFilterTagField(serializedObject, "TagOnDeselected");
            bbt.PermittedOperations = (BoundingBoxManipulate.OperationEnum) HUXEditorUtils.EnumCheckboxField<BoundingBoxManipulate.OperationEnum>(
                "Permitted Operations",
                bbt.PermittedOperations,
                "Default",
                BoundingBoxManipulate.OperationEnum.ScaleUniform | BoundingBoxManipulate.OperationEnum.RotateY | BoundingBoxManipulate.OperationEnum.Drag,
                BoundingBoxManipulate.OperationEnum.Drag);

            bbt.ShowAppBar = EditorGUILayout.Toggle("Toolbar Display", bbt.ShowAppBar);


            HUXEditorUtils.SaveChanges(bbt);
        }
    }
}