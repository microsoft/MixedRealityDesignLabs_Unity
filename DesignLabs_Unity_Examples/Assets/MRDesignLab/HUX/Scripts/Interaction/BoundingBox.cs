//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Buttons;
using HUX.Focus;
using HUX.Receivers;
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Interaction
{
    /// <summary>
    /// Base class for bounding box objects
    /// </summary>
    public class BoundingBox : InteractionReceiver
    {
        #region public

        /// <summary>
        /// Layer for drawing & colliders
        /// </summary>
        public int PhysicsLayer = 0;

        /// <summary>
        /// Any renderers on this layer will be ignored when calculating object bounds
        /// </summary>
        public int IgnoreLayer = 2;//Ignore Raycast

        /// <summary>
        /// The target object being manipulated
        /// </summary>
        public virtual GameObject Target
        {
            get
            {
                return target;
            }
            set
            {
                if (target != value)
                {
                    // Send a message to the new / old targets
                    if (value != null)
                    {
                        value.SendMessage("OnTargetSelected", SendMessageOptions.DontRequireReceiver);
                    }
                    if (target != null)
                    {
                        target.SendMessage("OnTargetDeselected", SendMessageOptions.DontRequireReceiver);
                    }
                    target = value;
                }
                if (target != null)
                {
                    CreateTransforms();
                    // Set our transforms to the target immediately
                    targetStandIn.position = target.transform.position;
                    targetStandIn.rotation = target.transform.rotation;
                    targetStandIn.localScale = target.transform.lossyScale;
                    RefreshTargetBounds();
                }
            }
        }

        /// <summary>
        /// The world-space center of the target object's bounds
        /// </summary>
        public Vector3 TargetBoundsCenter
        {
            get
            {
                return targetBoundsWorldCenter;
            }
        }

        /// <summary>
        /// The local scale of the target object's bounds
        /// </summary>
        public Vector3 TargetBoundsLocalScale
        {
            get
            {
                return targetBoundsLocalScale;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (transformHelper != null)
            {
                GameObject.Destroy(transformHelper.gameObject);
            }
        }

        #endregion

        #region private

        /// <summary>
        /// Override so we're not overwhelmed by button gizmos
        /// </summary>
        #if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            // nothing
            if (!Application.isPlaying)
            {
                // Do this here to ensure continuous updates in editor
                RefreshTargetBounds();
            }
        }
        #endif

        protected void CreateTransforms() {
            // Create our transform helpers if they don't exist
            if (transformHelper == null) {
                transformHelper = new GameObject("BoundingBoxTransformHelper").transform;
                targetStandIn = new GameObject("TargetStandIn").transform;
                targetStandIn.parent = transformHelper;
            }
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying)
                return;

            RefreshTargetBounds();
        }

        protected virtual void RefreshTargetBounds()
        {
            if (target == null)
            {
                targetBoundsWorldCenter = Vector3.zero;
                targetBoundsLocalScale = Vector3.one;
                return;
            }

            // Get the new target bounds
            boundsPoints.Clear();

            MeshFilter[] mfs = target.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in mfs)
            {
                if (mf.gameObject.layer == IgnoreLayer)
                    continue;

                Vector3 v3Center = mf.sharedMesh.bounds.center;
                Vector3 v3Extents = mf.sharedMesh.bounds.extents;

                // Get the world-space corner points of the bounds
                // Add them to our global list of points
                mf.sharedMesh.bounds.GetCornerPositions(mf.transform, ref corners);
                boundsPoints.AddRange(corners);
            }

            if (boundsPoints.Count > 0)
            {
                // We now have a list of all points in world space
                // Translate them all to local space
                for (int i = 0; i < boundsPoints.Count; i++)
                {
                    boundsPoints[i] = target.transform.InverseTransformPoint(boundsPoints[i]);
                }

                // Encapsulate the points with a local bounds
                localTargetBounds.center = boundsPoints[0];
                localTargetBounds.size = Vector3.zero;
                foreach (Vector3 point in boundsPoints)
                {
                    localTargetBounds.Encapsulate(point);
                }
            }

            // Store the world center of the target bb
            targetBoundsWorldCenter = target.transform.TransformPoint(localTargetBounds.center);

            // Store the local scale of the target bb
            targetBoundsLocalScale = localTargetBounds.size;
            targetBoundsLocalScale.Scale(target.transform.localScale);
        }

        [SerializeField]
        protected GameObject target;

        /// <summary>
        /// These are used to make complex scaling operations simpler
        /// </summary>
        protected Transform transformHelper = null;
        protected Transform targetStandIn = null;

        protected Vector3 targetBoundsWorldCenter = Vector3.zero;
        protected Vector3 targetBoundsLocalScale = Vector3.zero;

        protected Vector3[] corners = null;
        protected Bounds localTargetBounds = new Bounds();
        protected List<Vector3> boundsPoints = new List<Vector3>();

        #endregion
    }
}