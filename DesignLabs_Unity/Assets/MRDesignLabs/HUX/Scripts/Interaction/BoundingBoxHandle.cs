//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Buttons
{
    public class BoundingBoxHandle : Button {
                
        /// <summary>
        /// Convenience enum to store BoundsExtensions const values as an enum
        /// </summary>
        public enum HandleTypeEnum
        {
            Scale_LBF = BoundsExtentions.LBF,
            Scale_LBB = BoundsExtentions.LBB,
            Scale_LTF = BoundsExtentions.LTF,
            Scale_LTB = BoundsExtentions.LTB,
            Scale_RBF = BoundsExtentions.RBF,
            Scale_RBB = BoundsExtentions.RBB,
            Scale_RTF = BoundsExtentions.RTF,
            Scale_RTB = BoundsExtentions.RTB,
            // X axis
            Rotate_LTF_RTF = BoundsExtentions.LTF_RTF,
            Rotate_LBF_RBF = BoundsExtentions.LBF_RBF,
            Rotate_RTB_LTB = BoundsExtentions.RTB_LTB,
            Rotate_RBB_LBB = BoundsExtentions.RBB_LBB,
            // Y axis
            Rotate_LTF_LBF = BoundsExtentions.LTF_LBF,
            Rotate_RTB_RBB = BoundsExtentions.RTB_RBB,
            Rotate_LTB_LBB = BoundsExtentions.LTB_LBB,
            Rotate_RTF_RBF = BoundsExtentions.RTF_RBF,
            // Z axis
            Rotate_RBF_RBB = BoundsExtentions.RBF_RBB,
            Rotate_RTF_RTB = BoundsExtentions.RTF_RTB,
            Rotate_LBF_LBB = BoundsExtentions.LBF_LBB,
            Rotate_LTF_LTB = BoundsExtentions.LTF_LTB,
            Drag,
        }

        public HandleTypeEnum HandleType;

        /// <summary>
        /// The handle on the opposite side of the bounding box
        /// This is guaranteed to be set to a value
        /// In the case of drag, it is set to itself
        /// </summary>
        public BoundingBoxHandle OpposingHandle;

        void OnEnable()
        {
            name = HandleType.ToString();
            transform.localPosition = GetHandlePositionFromType(HandleType);
        }

        /// <summary>
        /// Convenience function used to correctly place handles.
        /// Helps prevent prefab corruption.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Vector3 GetHandlePositionFromType (HandleTypeEnum type)
        {
            float x = 0f;
            float y = 0f;
            float z = 0f;

            switch (type)
            {
                case HandleTypeEnum.Drag:
                default:
                    break;

                case HandleTypeEnum.Scale_LTB:
                    x = -0.5f;
                    y = 0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Scale_LTF:
                    x = -0.5f;
                    y = 0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Scale_LBF:
                    x = -0.5f;
                    y = -0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Scale_LBB:
                    x = -0.5f;
                    y = -0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Scale_RBF:
                    x = 0.5f;
                    y = -0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Scale_RBB:
                    x = 0.5f;
                    y = -0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Scale_RTB:
                    x = 0.5f;
                    y = 0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Scale_RTF:
                    x = 0.5f;
                    y = 0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Rotate_LTF_LTB:
                    x = -0.5f;
                    y = 0.5f;
                    z = 0.0f;
                    break;

                case HandleTypeEnum.Rotate_RTF_RTB:
                    x = 0.5f;
                    y = 0.5f;
                    z = 0.0f;
                    break;

                case HandleTypeEnum.Rotate_LTF_RTF:
                    x = 0.0f;
                    y = 0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Rotate_RTB_LTB:
                    x = 0.0f;
                    y = 0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Rotate_LBF_LBB:
                    x = -0.5f;
                    y = -0.5f;
                    z = 0.0f;
                    break;

                case HandleTypeEnum.Rotate_RBF_RBB:
                    x = 0.5f;
                    y = -0.5f;
                    z = 0.0f;
                    break;

                case HandleTypeEnum.Rotate_LBF_RBF:
                    x = 0.0f;
                    y = -0.5f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Rotate_RBB_LBB:
                    x = 0.0f;
                    y = -0.5f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Rotate_LTF_LBF:
                    x = -0.5f;
                    y = 0.0f;
                    z = -0.5f;
                    break;

                case HandleTypeEnum.Rotate_RTB_RBB:
                    x = 0.5f;
                    y = 0.0f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Rotate_LTB_LBB:
                    x = -0.5f;
                    y = 0.0f;
                    z = 0.5f;
                    break;

                case HandleTypeEnum.Rotate_RTF_RBF:
                    x = 0.5f;
                    y = 0.0f;
                    z = -0.5f;
                    break;

            }

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Convenience function to get an opposing handle
        /// This could be done by assigning an inspector value but this is cleaner
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static HandleTypeEnum GetOpposingHandle (HandleTypeEnum type)
        {
            switch (type)
            {
                case HandleTypeEnum.Drag:
                default:
                    // Only type with no opposite
                    return HandleTypeEnum.Drag;

                    // Scale handles are diagonally opposing on all axis

                case HandleTypeEnum.Scale_LTB:
                    return HandleTypeEnum.Scale_RBF;

                case HandleTypeEnum.Scale_LTF:
                    return HandleTypeEnum.Scale_RBB;

                case HandleTypeEnum.Scale_LBF:
                    return HandleTypeEnum.Scale_RTB;

                case HandleTypeEnum.Scale_LBB:
                    return HandleTypeEnum.Scale_RTF;

                case HandleTypeEnum.Scale_RBF:
                    return HandleTypeEnum.Scale_LTB;

                case HandleTypeEnum.Scale_RBB:
                    return HandleTypeEnum.Scale_LTF;

                case HandleTypeEnum.Scale_RTB:
                    return HandleTypeEnum.Scale_LBF;

                case HandleTypeEnum.Scale_RTF:
                    return HandleTypeEnum.Scale_LBB;

                // Rotation handles are diagonally opposing on same axis

                case HandleTypeEnum.Rotate_LTF_RTF:
                    return HandleTypeEnum.Rotate_RBB_LBB;

                case HandleTypeEnum.Rotate_LBF_RBF:
                    return HandleTypeEnum.Rotate_RTB_LTB;

                case HandleTypeEnum.Rotate_RTB_LTB:
                    return HandleTypeEnum.Rotate_LBF_RBF;

                case HandleTypeEnum.Rotate_RBB_LBB:
                    return HandleTypeEnum.Rotate_LTF_RTF;
            }
        }

        #if UNITY_EDITOR
        void OnDrawGizmos ()
        {
            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                BoundingBoxHandle [] bbhs = GameObject.FindObjectsOfType<BoundingBoxHandle>();
                HandleTypeEnum oppositeHandle = GetOpposingHandle(HandleType);
                foreach (BoundingBoxHandle bbh in bbhs)
                {
                    if (bbh.HandleType == oppositeHandle)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(transform.position, 0.025f);
                        Gizmos.DrawLine(transform.position, bbh.transform.position);
                        break;
                    }
                }                    
            }
        }
        #endif
    }
}
