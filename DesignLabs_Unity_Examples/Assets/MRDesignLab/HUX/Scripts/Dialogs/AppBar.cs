//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Receivers;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace HUX.Interaction
{
    public class AppBar : InteractionReceiver
    {
        /// <summary>
        /// How many custom buttons can be added to the toolbar
        /// </summary>
        const int MaxCustomButtons = 5;

        /// <summary>
        /// Class used for building toolbar buttons
        /// (not yet in use)
        /// </summary>
        [Serializable]
        public struct ButtonTemplate
        {
            public ButtonTemplate (Transform parent, string name, string icon, string text)
            {
                Name = name;
                Icon = icon;
                Text = text;
                EventTarget = null;
                OnTappedEvent = null;
            }

            public bool IsEmpty
            {
                get
                {
                    return string.IsNullOrEmpty(Name);
                }
            }
            public string Name;
            public string Icon;
            public string Text;
            public GameObject EventTarget;
            public UnityEvent OnTappedEvent;
        }
        
        public enum DisplayEnum
        {
            None,
            Show,
        }

        public enum ToolbarStateEnum
        {
            Default,
            Manipulation,
            Hidden,
        }

        [Flags]
        public enum ButtonTypeEnum
        {
            Custom = 0,
            Remove = 1,
            Adjust = 2,
            Hide = 4,
            Show = 8,
            Done = 16
        }

        public BoundingBoxManipulate BoundingBox
        {
            get
            {
                return boundingBox;
            }
            set
            {
                boundingBox = value;
            }
        }

        public GameObject SquareButtonPrefab;

        public ToolbarStateEnum State = ToolbarStateEnum.Default;

        //[SerializeField]
        //private ButtonTemplate[] buttons = new ButtonTemplate[MaxCustomButtons];

        private ButtonTemplate[] defaultButtons;

        [SerializeField]
        private Transform buttonParent;

        [SerializeField]
        private GameObject baseRenderer;

        [SerializeField]
        private GameObject backgroundBar;

        [SerializeField]
        private BoundingBoxManipulate boundingBox;

        public void Reset()
        {
            State = ToolbarStateEnum.Default;
            if (boundingBox != null)
            {
                boundingBox.AcceptInput = false;
            }
            FollowBoundingBox(false);
            lastTimeTapped = Time.time + coolDownTime;
        }

        public void Start()
        {
            State = ToolbarStateEnum.Default;
            if (Interactibles.Count == 0)
            {
                // Create our base buttons
                // TODO - create custom buttons using templates
                CreateButton(ButtonTypeEnum.Remove);
                CreateButton(ButtonTypeEnum.Adjust);
                CreateButton(ButtonTypeEnum.Hide);
                CreateButton(ButtonTypeEnum.Show);
                CreateButton(ButtonTypeEnum.Done);
            }
        }

        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            if (Time.time < lastTimeTapped + coolDownTime)
                return;

            lastTimeTapped = Time.time;

            base.OnTapped(obj, eventArgs);
                        
            switch (obj.name)
            {
                case "Remove":
                    // Destroy the target object
                    GameObject.Destroy(boundingBox.Target);
                    // Set our bounding box to null so we'll disappear
                    boundingBox = null;
                    break;

                case "Adjust":
                    // Make the bounding box active so users can manipulate it
                    State = ToolbarStateEnum.Manipulation;
                    break;

                case "Hide":
                    // Make the bounding box inactive and invisible
                    State = ToolbarStateEnum.Hidden;
                    break;

                case "Show":
                    State = ToolbarStateEnum.Default;
                    break;

                case "Done":
                    State = ToolbarStateEnum.Default;
                    break;

                default:
                    break;
            }
        }

        private void CreateButton(ButtonTypeEnum type)
        {
            GameObject newButton = GameObject.Instantiate(SquareButtonPrefab, buttonParent);
            newButton.transform.localPosition = Vector3.zero;
            newButton.transform.localRotation = Quaternion.identity;
            AppBarButton mtb = newButton.AddComponent<AppBarButton>();
            mtb.Type = type;
            mtb.ParentToolbar = this;

            RegisterInteractible(newButton);
        }

        private void FollowBoundingBox(bool smooth)
        {
            if (boundingBox == null)
            {
                // Hide our buttons
                baseRenderer.SetActive(false);
                return;
            }

            // Show our buttons
            baseRenderer.SetActive(true);

            // Get positions for each side of the bounding box
            // Choose the one that's closest to us
            forwards[0] = boundingBox.transform.forward;
            forwards[1] = boundingBox.transform.right;
            forwards[2] = -boundingBox.transform.forward;
            forwards[3] = -boundingBox.transform.right;
            Vector3 scale = boundingBox.TargetBoundsLocalScale;
            float maxXYScale = Mathf.Max(scale.x, scale.y);
            float closestSoFar = Mathf.Infinity;
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = Camera.main.transform.position;

            for (int i = 0; i < forwards.Length; i++)
            {
                Vector3 nextPosition = boundingBox.transform.position +
                (forwards[i] * -maxXYScale) +
                (Vector3.up * (-scale.y * 0.25f));

                float distance = Vector3.Distance(nextPosition, headPosition);
                if (distance < closestSoFar)
                {
                    closestSoFar = distance;
                    finalPosition = nextPosition;
                }
            }

            // Follow our bounding box
            if (smooth)
            {
                transform.position = Vector3.Lerp(transform.position, finalPosition, 0.5f);
            } else
            {
                transform.position = finalPosition;
            }
            // Rotate on the y axis
            Vector3 eulerAngles = Quaternion.LookRotation((boundingBox.transform.position - finalPosition).normalized, Vector3.up).eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }

        private void Update()
        {
            FollowBoundingBox(true);

            switch (State)
            {
                case ToolbarStateEnum.Default:
                default:
                    targetBarSize = Vector3.one;
                    if (boundingBox != null)
                        boundingBox.AcceptInput = false;
                    break;

                case ToolbarStateEnum.Hidden:
                    targetBarSize = new Vector3(0.333f, 1f, 1f);
                    if (boundingBox != null)
                        boundingBox.AcceptInput = false;
                    break;

                case ToolbarStateEnum.Manipulation:
                    targetBarSize = new Vector3(0.666f, 1f, 1f);
                    if (boundingBox != null)
                        boundingBox.AcceptInput = true;
                    break;
            }

            backgroundBar.transform.localScale = Vector3.Lerp(backgroundBar.transform.localScale, targetBarSize, 0.5f);
        }

        #if UNITY_EDITOR
        protected override void OnDrawGizmos()
        {
            if (Application.isPlaying)
                return;

            FollowBoundingBox(true);
        }
        #endif

        private Vector3[] forwards = new Vector3[4];
        private Vector3 targetBarSize = Vector3.one;
        private float lastTimeTapped = 0f;
        private float coolDownTime = 0.5f;
    }
}
