//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using HUX.Interaction;
using HUX.Utility;
using UnityEngine.VR;
using HUX.Receivers;
using HUX.Focus;

namespace HUX
{
    /// <summary>
    /// The Veil Singleton is the central access point for all related objects and other 
    /// supporting tools for the HoloLens and other HMDs.
    /// </summary>
    public class Veil : Singleton<Veil>
    {
        #region Public Accessors
        public bool IsInitialized { get; set; }

        /// <summary>
        /// Returns the transform instance of the head or in this case the Gazer which is the same thing
        /// </summary>
        public Transform HeadTransform { get { return Camera.main.transform; } }

        /// <summary>
        /// Returns the hand position in WorldSpace.  To get the now world space hand ask for relative 
        /// </summary>
        public Vector3 HandPosition {
            get
            {
                return InputSources.Instance.hands.GetWorldPosition(0);
            }
        }
        public Vector3 HeadVelocity { get { return headVelocity; } }
        public Vector3 MoveDirection { get { return headMoveDirection; } }
        public bool HandVisible {
            get
            {
                return InputSources.Instance.hands.IsHandVisible();
            }
        }

        public float DeviceFOV { get { return m_deviceFOV; } }

        public float HeadZoneSizeIdle = 0.2f;
        public float HeadZoneSizeMin = 0.01f;
        public bool SetVeilFOVOnStartInEditorTrigger = true;

        static public bool SetVeilFOVOnStartInEditor = true;

        /// <summary>
        /// Reference mesh to use for rendering the veil
        /// </summary>
        public Mesh _veilMesh;

        /// <summary>
        /// Enum for Device Type
        /// </summary>
        public enum DeviceTypeEnum
        {
            Auto = -1,
            HoloLens,
            Oculus,
            Vive
        }

        /// <summary>
        /// When set to auto determine from VR settings
        /// </summary>
        public DeviceTypeEnum CurrentDevice = DeviceTypeEnum.Auto;
        #endregion

        #region Private members
        private Vector3 headVelocity;
        private Vector3 lastHeadPos;
        private Vector3 lastHeadZone;
        private Vector3 newHeadMoveDirection;
        private float headZoneSize = 1f;
        private Vector3 headMoveDirection = Vector3.one;
        private float m_deviceFOV;
        #endregion

        #region Public constants
        // For HDM FOV Consts...
        public const float HOLOLENS_FOV = 17.0f;
        public const float VIVE_FOV = 105.0f;
        public const float OCULUS_FOV = 90.0f;

        public const float c_horiz_ratio = (16.0f / 9.0f);
        #endregion

        protected override void Awake()
        {
            IsInitialized = false;
            base.Awake();
            SetDeviceFOV();
            SetVeilFOVOnStartInEditor = SetVeilFOVOnStartInEditorTrigger;
            StartCoroutine(CoViewFromVeilFOV());
        }

        public bool IsFingerPressed()
        {
            return InputSources.Instance.hands.NumFingersPressed > 0;
        }

        public RaycastHit RayCastFromHead()
        {
            //Transform gazerTransform, bool bDebugDrawGaze

            Transform head = HeadTransform;

            RaycastHit hitInfo;

            Ray gazeRay = new Ray(head.position, head.forward);

            if (Physics.Raycast(gazeRay, out hitInfo))
            {
                Transform hitTransform = hitInfo.collider.transform;

                // Traverse up parent path until interactible object found
                while (hitTransform != null)
                {
                    InteractibleObject interactableComponent = hitTransform.GetComponent<InteractibleObject>();

                    if (interactableComponent != null)
                    {
                        return hitInfo;
                    }

                    hitTransform = hitTransform.parent;
                }
            }

            return hitInfo;
        }

        public RaycastHit[] RayCastAllFromHead()
        {
            Transform head = HeadTransform;
            RaycastHit[] hitInfo;

            Ray gazeRay = new Ray(head.position, head.forward);
            hitInfo = Physics.RaycastAll(gazeRay);

            return hitInfo;
        }

        public RaycastHit SphereCastFromHead(float radius)
        {
            Transform head = HeadTransform;
            RaycastHit hitInfo;
            Ray gazeRay = new Ray(head.position, head.forward);

            if (Physics.SphereCast(gazeRay, radius, out hitInfo))
            {
                Transform hitTransform = hitInfo.collider.transform;

                // Traverse up parent path until interactible object found
                while (hitTransform != null)
                {
                    InteractibleObject interactableComponent = hitTransform.GetComponent<InteractibleObject>();

                    if (interactableComponent != null)
                    {
                        return hitInfo;
                    }

                    hitTransform = hitTransform.parent;
                }
            }

            return hitInfo;
        }

        private void FixedUpdate()
        {
            // Update headVelocity
            Vector3 newHeadPos = HeadTransform.position;
            Vector3 headDelta = newHeadPos - lastHeadPos;

            float moveThreshold = 0.01f;
            if (headDelta.sqrMagnitude < moveThreshold * moveThreshold)
            {
                headDelta = Vector3.zero;
            }

            if (Time.fixedDeltaTime > 0)
            {
                float adjustRate = 3f * Time.fixedDeltaTime;
                headVelocity = headVelocity * (1f - adjustRate) + headDelta * adjustRate / Time.fixedDeltaTime;

                float velThreshold = .1f;
                if (headVelocity.sqrMagnitude < velThreshold * velThreshold)
                {
                    headVelocity = Vector3.zero;
                }
            }

            lastHeadPos = HeadTransform.position;

            // Update headDirection
            float headVelIdleThresh = 0.5f;
            float headVelMoveThresh = 2f;

            float velP = Mathf.Clamp01(Mathf.InverseLerp(headVelIdleThresh, headVelMoveThresh, headVelocity.magnitude));
            float newHeadZoneSize = Mathf.Lerp(HeadZoneSizeIdle, HeadZoneSizeMin, velP);
            headZoneSize = Mathf.Lerp(headZoneSize, newHeadZoneSize, Time.fixedDeltaTime);

            Vector3 headZoneDelta = newHeadPos - lastHeadZone;
            if (headZoneDelta.sqrMagnitude >= headZoneSize * headZoneSize)
            {
                newHeadMoveDirection = Vector3.Lerp(newHeadPos - lastHeadZone, headVelocity, velP).normalized;
                lastHeadZone = newHeadPos;
            }

            {
                float adjustRate = Mathf.Clamp01(5f * Time.fixedDeltaTime);
                headMoveDirection = Vector3.Slerp(headMoveDirection, newHeadMoveDirection, adjustRate);
            }

            // Update head move direction
            /*float turnRate = 5f;
            headMoveDirection += headVelocity * turnRate;
            headMoveDirection.Normalize();*/

            Debug.DrawLine(lastHeadPos, lastHeadPos + headMoveDirection * 10f, Color.Lerp(Color.red, Color.green, velP));
            Debug.DrawLine(lastHeadPos, lastHeadPos + headVelocity, Color.yellow);
        }

        private void SetDeviceFOV()
        {
            m_deviceFOV = HOLOLENS_FOV;

            // Debug.Log(VRSettings.loadedDeviceName);

            switch (CurrentDevice)
            {
                case DeviceTypeEnum.Auto:
                switch (UnityEngine.XR.XRSettings.loadedDeviceName)
                {
                    case "Oculus":
                    m_deviceFOV = OCULUS_FOV;
                    break;
                    case "OpenVR":
                    m_deviceFOV = VIVE_FOV;
                    break;
                    case "HoloLens":
                    m_deviceFOV = HOLOLENS_FOV;
                    break;
                }
                break;
                case DeviceTypeEnum.HoloLens:
                m_deviceFOV = HOLOLENS_FOV;
                break;
                case DeviceTypeEnum.Vive:
                m_deviceFOV = VIVE_FOV;
                break;
                case DeviceTypeEnum.Oculus:
                m_deviceFOV = OCULUS_FOV;
                break;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gizmos to draw when the Collection is selected.
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            SetDeviceFOV();

            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawFrustum(Vector3.zero, m_deviceFOV, 5f, 0.1f, (16.0f / 9.0f));
        }

        /// <summary>
        /// On Draw Gizmo show the receiver icon
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = UnityEditor.Selection.activeGameObject == gameObject ? Color.cyan : Color.blue;
            Gizmos.DrawWireMesh(_veilMesh, this.transform.position, this.transform.rotation);
        }
#endif

        /// <summary>
        /// The view from veil coroutine makes sure the FOV matches in editor
        /// </summary>
        /// <returns></returns>
        public IEnumerator CoViewFromVeilFOV()
        {
            // StereoCamera singleton initializes after Veil (probably because it's a child?)
            while (Camera.main == null)
            {
                yield return null;
            }

            Camera camera = Camera.main;
            if (camera)
            {
#if UNITY_EDITOR || !UNITY_WSA
                if (SetVeilFOVOnStartInEditor)
                {
                    if (UnityEngine.XR.XRDevice.isPresent)
                    {
                        while (!UnityEngine.XR.XRSettings.enabled)
                        {
                            yield return null;
                        }
                    }

                    camera.fieldOfView = m_deviceFOV;
                }
#endif
            }

            IsInitialized = true;
        }
    }
}
