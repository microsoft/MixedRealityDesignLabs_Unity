//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Utility;
using UnityEngine;

namespace HUX.Dialogs
{
    /// <summary>
    /// Singleton for displaying a simple loading dialog
    /// Can be combined with a radial solver to keep locked to the HMD
    /// </summary>
    public class LoadingDialog : Singleton<LoadingDialog>
    {
        const float SmoothProgressSpeed = 10f;

        public enum IndicatorStyleEnum
        {
            None,           // Don't display an activity indicator
            StaticIcon,     // Display a static icon
            AnimatedOrbs,   // Display animated orbs
            Prefab,         // Display a custom prefab
        }

        public enum ProgressStyleEnum
        {
            None,           // Don't display a progress number
            Percentage,     // Display progress as a 0-100%
            ProgressBar     // Display progress as a progress bar
        }

        public enum MessageStyleEnum
        {
            None,           // Don't display a message
            Visible,        // Display a message
        }

        public bool IsLoading
        {
            get
            {
                return Instance.gameObject.activeSelf;
            }
        }

        public IndicatorStyleEnum DefaultIndicatorStyle = IndicatorStyleEnum.AnimatedOrbs;
        public ProgressStyleEnum DefaultProgressStyle = ProgressStyleEnum.Percentage;
        public MessageStyleEnum DefaultMessageStyle = MessageStyleEnum.Visible;

        // The default prefab used by the 'Prefab' indicator style
        [SerializeField]
        private GameObject defaultPrefab;

        // The default icon used by the 'StaticIcon' indicator style
        [SerializeField]
        private Texture2D defaultIcon;

        // The animated orbs object used by the 'AnimatedOrbs' indicator style
        [SerializeField]
        private GameObject orbsObject;

        // The progress bar container object
        [SerializeField]
        private GameObject progressBarContainer;

        // The animated progress bar object
        [SerializeField]
        private Transform progressBar;

        // The icon renderer used by the 'StaticIcon' indicator style
        [SerializeField]
        private Renderer iconRenderer;

        // The message text used by the 'Visible' message style
        [SerializeField]
        private TextMesh messageText;

       
        // The progress text used by all non-'None' progress styles
        [SerializeField]
        private TextMesh progressText;

        [SerializeField]
        private Animator animator;

        public float Progress {
            get {
                return smoothProgress;
            }
        }

        private float smoothProgress = 0f;
        private float targetProgress = 0f;
        private bool closing = false;
        private GameObject instantiatedCustomObject;

        /// <summary>
        /// Format to be used for the progress number
        /// </summary>
        public string ProgressFormat = "0.0";

        /// <summary>
        /// Opens the dialog with full custom options
        /// </summary>
        /// <param name="indicatorStyle"></param>
        /// <param name="progressStyle"></param>
        /// <param name="messageStyle"></param>
        /// <param name="message"></param>
        /// <param name="icon"></param>
        public void Open (IndicatorStyleEnum indicatorStyle, ProgressStyleEnum progressStyle, MessageStyleEnum messageStyle, string message = "", Texture2D icon = null, GameObject prefab = null)
        {
            if (gameObject.activeSelf)
                return;

            // Make sure we aren't parented under anything
            transform.parent = null;

            // Make sure we aren't destroyed on load
            // Just in case the user is loading a scene
            DontDestroyOnLoad(transform);

            // Turn our common objects on 
            closing = false;
            gameObject.SetActive(true);
            progressText.gameObject.SetActive(progressStyle == ProgressStyleEnum.Percentage);
            progressBarContainer.gameObject.SetActive(progressStyle == ProgressStyleEnum.ProgressBar);
            messageText.gameObject.SetActive(messageStyle != MessageStyleEnum.None);
            messageText.text = message;

            // Reset our loading progress
            smoothProgress = 0f;
            targetProgress = 0f;

            // Turn the style objects off
            iconRenderer.enabled = false;
            orbsObject.SetActive(false);
            
            /*if (displayParent != null && transform.parent != displayParent)
            {
                transform.parent = displayParent;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }*/
            
            // Re-enable objects based on our style
            switch (indicatorStyle)
            {
                case IndicatorStyleEnum.None:
                    break;

                case IndicatorStyleEnum.StaticIcon:
                    iconRenderer.enabled = true;
                    if (defaultIcon == null && icon == null)
                    {
                        UnityEngine.Debug.LogError("No icon available in loading dialog, spawning without one");
                        iconRenderer.enabled = false;
                    } else
                    {
                        // The icon sent in the function overrides the icon set in the inspector 
                        iconRenderer.material.mainTexture = (icon == null) ? defaultIcon : icon;
                    }
                    break;

                case IndicatorStyleEnum.AnimatedOrbs:
                    iconRenderer.enabled = false;
                    orbsObject.SetActive(true);
                    orbsObject.GetComponent<LoadingAnimation>().StartLoader();
                    break;

                case IndicatorStyleEnum.Prefab:
                    // Instantiate our custom object under our animator
                    if (defaultPrefab == null && prefab == null)
                    {

                        UnityEngine.Debug.LogError("No prefab available in loading dialog, spawning without one");
                    }
                    else
                    {
                        // The prefab sent in the function overrides the prefab set in the inspector 
                        instantiatedCustomObject = GameObject.Instantiate((prefab == null) ? defaultPrefab : prefab, animator.transform) as GameObject;
                        instantiatedCustomObject.transform.localPosition = Vector3.zero;
                        instantiatedCustomObject.transform.localRotation = Quaternion.identity;
                    }
                    break;
            }
            animator.SetTrigger("Open");
        }


        /// <summary>
        /// Opens the dialog with default settings for indicator and progress
        /// </summary>
        /// <param name="message"></param>
        public void Open (string message)
        {
            Open(DefaultIndicatorStyle, DefaultProgressStyle, MessageStyleEnum.Visible, message, null);
        }

        /// <summary>
        /// Updates message.
        /// Has no effect until Open is called.
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(string message)
        {
            if (!gameObject.activeSelf)
                return;

            messageText.text = message;
        }

        /// <summary>
        /// Updates progress.
        /// Has no effect until Open is called.
        /// </summary>
        /// <param name="progress"></param>
        public void SetProgress (float progress)
        {
            targetProgress = Mathf.Clamp01(progress) * 100;
            // If progress is 100, assume we want to snap to that value
            if (targetProgress == 100)
            {
                smoothProgress = targetProgress;
            }
        }

        /// <summary>
        /// Initiates the process of closing the dialog.
        /// </summary>
        public void Close ()
        {
            if (!gameObject.activeSelf)
                return;

            closing = true;
            progressText.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
            animator.SetTrigger("Close");
        }

        private void Start()
        {
            gameObject.SetActive(false);
            progressText.gameObject.SetActive(false);
            messageText.gameObject.SetActive(false);
        }

        private void Update ()
        {
            smoothProgress = Mathf.Lerp(smoothProgress, targetProgress, Time.deltaTime * SmoothProgressSpeed);
            progressBar.localScale = new Vector3(smoothProgress / 100, 1f, 1f);
            progressText.text = smoothProgress.ToString(ProgressFormat) + "%";
            // If we're closing, wait for the animator to reach the closed state
            if (closing)
            {
                if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Closed"))
                {
                    // Once we've reached the cloesd state shut down completely
                    closing = false;
                    transform.parent = null;
                    gameObject.SetActive(false);
                    // Destroy our custom object if we made one
                    if (instantiatedCustomObject != null)
                        GameObject.Destroy(instantiatedCustomObject);
                }
            }
        }
    }
}
