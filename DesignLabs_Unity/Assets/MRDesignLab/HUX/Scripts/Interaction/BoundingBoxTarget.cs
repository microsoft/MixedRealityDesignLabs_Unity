//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using UnityEngine;

namespace HUX.Interaction
{
    /// <summary>
    /// This script assists in using a bounding box to target objects
    /// Bounding boxes and manipulation toolbars can both be used without this script
    /// But this makes it easier to use a single bounding box to target multiple objects
    /// as well as to specify per-target display options and operations
    /// </summary>
    [RequireComponent (typeof (CompoundButton))]
    public class BoundingBoxTarget : MonoBehaviour {

        /// <summary>
        /// Tags to use when selected / deselected
        /// This should be set to something the FocusManager will ignore
        /// Otherwise the colliders from this object may occlude bounding box
        /// </summary>
        public FilterTag TagOnSelected;
        public FilterTag TagOnDeselected;

        /// <summary>
        /// Which operations will be permitted when the bounding box targets this object
        /// </summary>
        [HideInInspector]
        public BoundingBoxManipulate.OperationEnum PermittedOperations = BoundingBoxManipulate.OperationEnum.Drag | BoundingBoxManipulate.OperationEnum.ScaleUniform | BoundingBoxManipulate.OperationEnum.RotateY;

        /// <summary>
        /// Whether to show the manipulation display when the bounding box targets this object
        /// </summary>
        [HideInInspector]
        public bool ShowAppBar = true;

        /// <summary>
        /// Bounding box to use. If this is not set, the first bounding box in the scene will be used.
        /// </summary>
        private BoundingBoxManipulate boundingBox;

        /// <summary>
        /// Manipulation toolbar to use. If this is not set, the first toolbar in the scene will be uesd.
        /// </summary>
        private AppBar toolbar;

        private void Start()
        {
            Button button = GetComponent<Button>();
            button.FilterTag = TagOnDeselected;
        }

        public void OnTargetSelected()
        {
            //Debug.Log("Selecting target" + name);
            GetComponent<Button>().FilterTag = TagOnSelected;
        }

        public void OnTargetDeselected ()
        {
            //Debug.Log("Deselecting target " + name);
            GetComponent<Button>().FilterTag = TagOnDeselected;
        }

        public void Tapped()
        {

            // Return if there isn't a Manipulation Manager
            if (ManipulationManager.Instance == null)
            {
                Debug.LogError("No manipulation manager for " + name);
                return;
            }

            // Try to find our bounding box
            if (boundingBox == null)
            {
                boundingBox = ManipulationManager.Instance.ActiveBoundingBox;
            }

            // Try to find our toolbar
            if (toolbar == null)
            {
                toolbar = ManipulationManager.Instance.ActiveAppBar;
            }

            // If we've already got a bounding box and it's pointing to us, do nothing
            if (boundingBox != null && boundingBox.Target == this.gameObject)
                return;
            
            // Set the bounding box's target and permitted operations
            boundingBox.PermittedOperations = PermittedOperations;
            boundingBox.Target = gameObject;
            
            if (ShowAppBar)
            {
                // Show it and set its bounding box object
                toolbar.BoundingBox = boundingBox;
                toolbar.Reset();
            } else if (toolbar != null)
            {
                // Set its bounding box to null to hide it
                toolbar.BoundingBox = null;
                // Set to accept input immediately
                boundingBox.AcceptInput = true;
            }
        }

        private void OnDestroy ()
        {
            if (boundingBox != null && boundingBox.Target == this)
                boundingBox.Target = null;
        }
    }
}
