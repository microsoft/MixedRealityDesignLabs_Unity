//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Interaction
{
    /// <summary>
    /// Listens to a BoundingBoxManipulate object and draws a gizmo around the target object
    /// </summary>
    [ExecuteInEditMode]
    public class BoundingBoxGizmo : MonoBehaviour
    {
        /// <summary>
        /// How much to pad the scale of the box to fit around objects (as % of largest dimension)
        /// </summary>
        public float ScalePadding = 0.05f;

        /// <summary>
        /// Physics layer to use for rendering
        /// </summary>
        public int PhysicsLayer = 0;

        /// <summary>
        /// The bounding box we're following
        /// </summary>
        [SerializeField]
        protected BoundingBox boundingBox;

        /// <summary>
        /// The transform that follows the bounding box
        /// </summary>
        [SerializeField]
        protected Transform scaleTransform;

        /// <summary>
        /// Draws any custom gizmo elements around the manipulator's target object
        /// </summary>
        protected virtual void DrawGizmoObjects() {
            // empty
        }

        /// <summary>
        /// Updates bounding box to match target position etc
        /// </summary>
        protected virtual void UpdateGizmoPosition() {
            // If we don't have a target, nothing to do here
            if (boundingBox.Target == null)
                return;

            // Get position of object based on renderers
            transform.position = boundingBox.TargetBoundsCenter;
            Vector3 scale = boundingBox.TargetBoundsLocalScale;
            float largestDimension = Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z);
            scale.x += (largestDimension * ScalePadding);
            scale.y += (largestDimension * ScalePadding);
            scale.z += (largestDimension * ScalePadding);
            scaleTransform.localScale = scale;

            Vector3 rotation = boundingBox.Target.transform.eulerAngles;
            // Limit rotation by permitted operations
            // Get all our handle positions for rotation
            /*if ((manipulator.PermittedOperations & BoundingBoxManipulate.OperationEnum.RotateX) != BoundingBoxManipulate.OperationEnum.RotateX)
                rotation.x = 0f;

            if ((manipulator.PermittedOperations & BoundingBoxManipulate.OperationEnum.RotateY) != BoundingBoxManipulate.OperationEnum.RotateZ)
                rotation.z = 0f;

            if ((manipulator.PermittedOperations & BoundingBoxManipulate.OperationEnum.RotateZ) != BoundingBoxManipulate.OperationEnum.RotateY)
                rotation.y = 0f;*/

            transform.eulerAngles = rotation;
        }

        protected virtual void LateUpdate() {
            UpdateGizmoPosition();
            DrawGizmoObjects();
        }
    }
}