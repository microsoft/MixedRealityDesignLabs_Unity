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
    /// Listens to messages from attached BoundingBoxHandle objects and manipulates the target
    /// </summary>
    public class BoundingBoxManipulate : BoundingBox
    {
        [System.Flags]
        public enum OperationEnum
        {
            None = 0,
            Drag = 1,
            ScaleUniform = 2,
            RotateX = 4,
            RotateZ = 8,
            RotateY = 16,
            ScaleX = 32,
            ScaleY = 64,
            ScaleZ = 128,
        }

        #region public

        /// <summary>
        /// Makes bounding box manipulatable by user
        /// </summary>
        public bool AcceptInput
        {
            get
            {
                return acceptInput;
            }
            set
            {
                if (value & !acceptInput)
                {
                    // Reset to drag operation
                    CurrentOperation = OperationEnum.Drag;
                }
                acceptInput = value;
            }
        }

        /// <summary>
        /// Which operations this bounding box is allowed to perform
        /// </summary>
        public OperationEnum PermittedOperations
        {
            get
            {
                return permittedOperations;
            }
            set
            {
                if (permittedOperations != value)
                {
                    permittedOperations = value;
                    RefreshActiveHandles();
                }
            }
        }

        /// <summary>
        /// How quickly objects move when being dragged
        /// </summary>
        public float DragMultiplier = 10f;

        /// <summary>
        /// How much to scale rotation input
        /// </summary>
        public float RotateMultiplier = 10f;

        /// <summary>
        /// How much to scale scale input
        /// </summary>
        public float ScaleMultiplier = 10f;

        /// <summary>
        /// The smallest an object can be scaled with one gesture
        /// </summary>
        public float MinScalePercentage = 0.05f;

        /// <summary>
        /// The current operation of the bounding box
        /// </summary>
        public OperationEnum CurrentOperation
        {
            get
            {
                if (target == null || activeHandle == null)
                {
                    return OperationEnum.None;
                }

                return GetBoundingBoxOperationFromHandleType(activeHandle.HandleType);
            }
            set
            {
                SetHandleByOperation(value);
            }
        }

        /// <summary>
        /// Whether the user is manipulating the bb using a handle
        /// </summary>
        public bool ManipulatingNow
        {
            get
            {
                if (Application.isPlaying)
                {
                    return manipulatingNow;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    StartManipulating();
                }
                else
                {
                    StopManipulating();
                }
            }
        }

        /// <summary>
        /// The handle being manipulated by the user, if any
        /// </summary>
        public BoundingBoxHandle ActiveHandle
        {
            get
            {
                return activeHandle;
            }
            set
            {
                activeHandle = value;
            }
        }

        /// <summary>
        /// The target object being manipulated
        /// </summary>
        public override GameObject Target
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
                    // Reset active handle to drag
                    SetHandleByOperation(OperationEnum.Drag);
                    ManipulatingNow = false;
                }
                if (target != null)
                {
                    CreateTransforms();
                    // Set our transforms to the target immediately
                    targetStandIn.position = target.transform.position;
                    targetStandIn.rotation = target.transform.rotation;
                    targetStandIn.localScale = target.transform.lossyScale;
                    RefreshTargetBounds();
                } else
                {
                    ActiveHandle = null;
                }
            }
        }

        /// <summary>
        /// Convenience function to help determine what function a handle serves
        /// </summary>
        /// <param name="handleType"></param>
        /// <returns></returns>
        public static OperationEnum GetBoundingBoxOperationFromHandleType(BoundingBoxHandle.HandleTypeEnum handleType)
        {
            switch (handleType)
            {
                case BoundingBoxHandle.HandleTypeEnum.Drag:
                    return OperationEnum.Drag;

                //TODO - break this up into axis scales
                case BoundingBoxHandle.HandleTypeEnum.Scale_LBF:
                case BoundingBoxHandle.HandleTypeEnum.Scale_LBB:
                case BoundingBoxHandle.HandleTypeEnum.Scale_LTF:
                case BoundingBoxHandle.HandleTypeEnum.Scale_LTB:
                case BoundingBoxHandle.HandleTypeEnum.Scale_RBF:
                case BoundingBoxHandle.HandleTypeEnum.Scale_RBB:
                case BoundingBoxHandle.HandleTypeEnum.Scale_RTF:
                case BoundingBoxHandle.HandleTypeEnum.Scale_RTB:
                    return OperationEnum.ScaleUniform;

                case BoundingBoxHandle.HandleTypeEnum.Rotate_LTF_RTF:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_LBF_RBF:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_RTB_LTB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_RBB_LBB:
                    return OperationEnum.RotateX;

                case BoundingBoxHandle.HandleTypeEnum.Rotate_LTF_LBF:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_RTB_RBB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_LTB_LBB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_RTF_RBF:
                    return OperationEnum.RotateY;

                case BoundingBoxHandle.HandleTypeEnum.Rotate_RBF_RBB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_RTF_RTB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_LBF_LBB:
                case BoundingBoxHandle.HandleTypeEnum.Rotate_LTF_LTB:
                    return OperationEnum.RotateZ;

                default:
                    return OperationEnum.None;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            manipulatingNow = false;
        }

        #endregion

        #region manipulation events
        protected override void OnFocusEnter(GameObject obj, FocusArgs args)
        {
            if (!ManipulatingNow)
            {
                //TODO show handle mesh
            }
            base.OnFocusEnter(obj, args);
        }

        protected override void OnFocusExit(GameObject obj, FocusArgs args)
        {
            if (!ManipulatingNow)
            {
                //TODO hide handle mesh
            }
            base.OnFocusExit(obj, args);
        }

        /// <summary>
        /// Chooses our manipulation handle so we know how to interpret future input events
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eventArgs"></param>
        protected override void OnManipulationStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            base.OnManipulationStarted(obj, eventArgs);
            TryToSetHandle(obj, eventArgs.Position, eventArgs.Focuser);
        }

        protected override void OnManipulationCanceled(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            ManipulatingNow = false;
            base.OnManipulationCanceled(obj, eventArgs);
        }

        protected override void OnManipulationCompleted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            ManipulatingNow = false;
            base.OnManipulationCompleted(obj, eventArgs);
        }

        protected override void OnManipulationUpdated(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            base.OnManipulationUpdated(obj, eventArgs);

            if (!acceptInput)
                return;

            if (target == null)
                return;

            if (!manipulatingNow)
                return;


            Vector3 eventPos = eventArgs.Position;
            // Transform the direction if necessary
            if (eventArgs.IsPosRelative)
            {
                eventPos = Veil.Instance.HeadTransform.TransformDirection (eventPos);
            }
            // See how much our position has changed
            navigateVelocity = lastNavigatePos - eventPos;
            lastNavigatePos = eventPos;
            smoothVelocity = Vector3.Lerp(smoothVelocity, navigateVelocity, 0.5f);   
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
                UpdateTargetManipulation();
                RefreshTargetBounds();
            }
        }
        #endif

        protected override void Update()
        {
            if (!Application.isPlaying)
                return;

            // Check to see if our hands have exited the screen
            // If they have, stop manipulating
            if (!Veil.Instance.HandVisible) {
                ManipulatingNow = false;
            }

            UpdateUserManipulation();
            UpdateTargetManipulation();

            base.Update();
        }

        /// <summary>
        /// Applies changes gathered during manipulation udpate events
        /// </summary>
        private void UpdateUserManipulation()
        {
            if (ManipulatingNow)
            {
                // Change the transform helper based on the current operation
                // We're using some magic numbers in here to keep the multiplier ranges intuitive
                switch (CurrentOperation)
                {

                    case OperationEnum.Drag:
                        transformHelper.position -= (smoothVelocity * DragMultiplier);
                        break;

                    case OperationEnum.ScaleUniform:
                    case OperationEnum.ScaleX:
                    case OperationEnum.ScaleY:
                    case OperationEnum.ScaleZ:
                        // Translate velocity direction based on camera
                        Vector3 orientedVelocity = Camera.main.transform.TransformDirection(smoothVelocity);
                        // See whether handle is to left or right of gizmo center
                        Vector3 handleScreenPoint = Camera.main.WorldToScreenPoint(activeHandle.transform.position);
                        Vector3 gizmoScreenPoint = Camera.main.WorldToScreenPoint(targetBoundsWorldCenter);
                        float dragAmount = orientedVelocity.x;
                        if (handleScreenPoint.x > gizmoScreenPoint.x)
                        {
                            dragAmount = -dragAmount;
                        }
                        transformHelper.localScale += Vector3.one * (dragAmount * ScaleMultiplier);
                        break;

                    case OperationEnum.RotateX:
                        transformHelper.Rotate(-smoothVelocity.y * RotateMultiplier * 360, 0f, 0f, Space.World);
                        break;

                    case OperationEnum.RotateY:
                        transformHelper.Rotate(0f, smoothVelocity.x * RotateMultiplier * 360, 0f, Space.World);
                        break;

                    case OperationEnum.RotateZ:
                        transformHelper.Rotate(0f, 0f, smoothVelocity.x * RotateMultiplier * 360, Space.World);
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Alters the target based on changes applied to bounding box
        /// </summary>
        private void UpdateTargetManipulation()
        {
            // Goes without saying
            if (!acceptInput)
                return;

            // If we don't have a target, nothing to do here
            if (target == null)
                return;

            // If we're NOT actively manipulating the target, nothing to do here
            if (!ManipulatingNow)
                return;

            if (!Application.isPlaying)
                return;

            CreateTransforms();

            // Apply the target stand-in's transform info to the target
            Target.transform.position = targetStandIn.position;
            Target.transform.rotation = targetStandIn.rotation;
            Target.transform.localScale = targetStandIn.lossyScale;
        }

        /// <summary>
        /// Stores target under our transform helper and prepares for manipulation
        /// </summary>
        private void StartManipulating()
        {
            // Goes without saying
            if (!acceptInput)
                return;

            if (target == null)
                return;

            if (manipulatingNow)
                return;

            manipulatingNow = true;

            if (!Application.isPlaying)
                return;

            CreateTransforms();

            // Reset the transform helper to 1,1,1 / idenity
            transformHelper.localScale = Vector3.one;
            transformHelper.rotation = Quaternion.identity;
            adjustedScaleTarget = Vector3.one;
            smoothVelocity = Vector3.zero;
            
            // Set up our transforms and gestures based on the operation we're performing
            OperationEnum operation = GetBoundingBoxOperationFromHandleType(ActiveHandle.HandleType);
            switch (operation)
            {
                case OperationEnum.ScaleUniform:
                case OperationEnum.ScaleX:
                case OperationEnum.ScaleY:
                case OperationEnum.ScaleZ:
                    // If we're scaling, move the transform helper to the position OPPOSITE the scale handle
                    // That way the object will grow in the right direction
                    BoundingBoxHandle oppositeHandle = null;
                    BoundingBoxHandle.HandleTypeEnum oppositeHandleType = BoundingBoxHandle.GetOpposingHandle(ActiveHandle.HandleType);
                    foreach (GameObject bbhGo in Interactibles)
                    {
                        BoundingBoxHandle bbh = bbhGo.GetComponent<BoundingBoxHandle>();
                        if (bbh != null && bbh.HandleType == oppositeHandleType)
                        {
                            oppositeHandle = bbh;
                            break;
                        }
                    }
                    if (oppositeHandle == null)
                    {
                        Debug.LogWarning("Couldn't find opposing handle for type " + ActiveHandle.HandleType);
                        transformHelper.position = transform.position;
                        targetStandIn.position = target.transform.position;
                    }
                    else
                    {
                        transformHelper.position = oppositeHandle.transform.position;
                        targetStandIn.position = target.transform.position;
                    }
                    break;

                case OperationEnum.Drag:
                    // If we're rotating or moving, move the transform helper to the center of the gizmo
                    transformHelper.position = transform.position;
                    targetStandIn.position = target.transform.position;
                    break;

                case OperationEnum.RotateX:
                case OperationEnum.RotateY:
                case OperationEnum.RotateZ:
                default:
                    // Rotation
                    // If we're rotating or moving, move the transform helper to the center of the gizmo
                    transformHelper.position = transform.position;
                    targetStandIn.position = target.transform.position;
                    break;
            }

            scaleOnStartManipulation = targetStandIn.localScale;

            if (target != null)
            {
                // Set our transforms to the target immediately
                targetStandIn.position = target.transform.position;
                targetStandIn.rotation = target.transform.rotation;
                targetStandIn.localScale = target.transform.lossyScale;
            }
        }

        private void StopManipulating()
        {
            if (!manipulatingNow)
                return;

            manipulatingNow = false;
            if (focuser != null)
            {
                focuser.ReleaseFocus();
                focuser = null;
            }
        }

        private void SetHandleByOperation(OperationEnum operation)
        {
            switch (operation)
            {
                case OperationEnum.None:
                    ActiveHandle = null;
                    break;

                case OperationEnum.Drag:
                    foreach (GameObject obj in Interactibles)
                    {
                        BoundingBoxHandle h = obj.GetComponent<BoundingBoxHandle>();
                        if (h.HandleType == BoundingBoxHandle.HandleTypeEnum.Drag)
                        {
                            ActiveHandle = h;
                            break;
                        }
                    }
                    break;

                default:
                    //TODO link up other operations here
                    break;
            }

        }

        private void RefreshActiveHandles()
        {
            foreach (GameObject handleGo in Interactibles)
            {
                BoundingBoxHandle handle = handleGo.GetComponent<BoundingBoxHandle>();
                OperationEnum handleOperation = GetBoundingBoxOperationFromHandleType(handle.HandleType);
                handleGo.SetActive((handleOperation & permittedOperations) != 0);
            }
        }

        private void TryToSetHandle(GameObject obj, Vector3 position, AFocuser newFocuser)
        {
            if (!acceptInput)
            {
                Debug.Log("Not accepting input");
                return;
            }
            
            BoundingBoxHandle newHandle = obj.GetComponent<BoundingBoxHandle>();
            if (newHandle != null)
            {
                activeHandle = newHandle;
                lastNavigatePos = position;
                ManipulatingNow = true;
                focuser = newFocuser;
                focuser.LockFocus();
            }
        }

        [SerializeField]
        private BoundingBoxHandle activeHandle;

        [SerializeField]
        private bool acceptInput = true;

        [SerializeField]
        private bool manipulatingNow = false;

        [SerializeField]
        private OperationEnum permittedOperations = OperationEnum.Drag | OperationEnum.RotateY | OperationEnum.ScaleUniform;

        private Vector3 lastNavigatePos = Vector3.zero;
        private Vector3 navigateVelocity = Vector3.zero;

        private Vector3 smoothVelocity = Vector3.zero;
        private Vector3 adjustedScaleTarget = Vector3.one;
        private Vector3 targetPosition = Vector3.zero;
        
        private Vector3 scaleOnStartManipulation = Vector3.one;
        private AFocuser focuser = null;

        #endregion
    }
}