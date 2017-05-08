//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HUX.Utility
{
    /// <summary>
    /// Toggle the visiblity of an object with a button press.
    /// </summary>
    public class ChildVisiblity : MonoBehaviour
    {
        #region public members
        [Tooltip("Audio clip for transition In")]
        public AudioClip TransitionInAudio;

        [Tooltip("Audio clip for transition Out")]
        public AudioClip TransitionOutAudio;

        [Tooltip("Where to move relative when in transition")]
        public Vector3 RelativeMove = new Vector3(0.0f, 0.0f, 1.0f);

        [Tooltip("Duration of the transition")]
        public float TransitionDuration = 1.0f;

        [Tooltip("Is object initially visible")]
        public bool InitialVisibility = false;

        [Tooltip("Key mapped to visibilty toggle")]
        public KeyCode ToggleKey = KeyCode.Space;

        public enum TransitionTypeEnum
        {
            None,
            FadeInOut,
            MoveFade,
            Shrink
        }

        [Tooltip("Type of transition for object")]
        public TransitionTypeEnum TransitionType = TransitionTypeEnum.FadeInOut;

        [HideInInspector]
        public bool InTransition = false;
        #endregion

        #region private variables
        private List<Transform> ChildList = new List<Transform>();
        private AudioSource Speaker;
        private bool bVisible;
        #endregion

        public void Awake()
        {
            bVisible = InitialVisibility;

            // Create a local list of the children transforms
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                ChildList.Add(child);
            }

            // Set initial visibility
            SetChildVisibility(bVisible);

            // Add an audio source
            Speaker = this.gameObject.AddComponent<AudioSource>();
        }

        // Check for moving forward
        private void Update()
        {
            if (!InTransition)
            {
                if (Input.GetKeyDown(ToggleKey))
                {
                    StartCoroutine("Transition");
                }
            }
        }

        public IEnumerator Transition()
        {
            // Set the bool that we are in transition
            InTransition = true;

            // Get the timestamp for the transition start
            float TimeStamp = Time.fixedTime;

            // Here we transition out
            float transPercent;
            Vector3 origScale = this.transform.localScale;
            Vector3 origPos = this.transform.localPosition;

            // Play transition In Audio
            AudioClip audio = bVisible ? TransitionOutAudio : TransitionInAudio;
            if (audio != null)
            {
                Speaker.PlayOneShot(audio);
            }

            // Make visible for fade-in
            if (!bVisible)
            {
                SetChildVisibility(true);
            }

            // Here we transition
            while (Time.fixedTime - TimeStamp < TransitionDuration)
            {

                transPercent = ((Time.fixedTime - TimeStamp)) / TransitionDuration;
                float alphaPercent = bVisible ? 1 - transPercent : transPercent;
                float movePercent = bVisible ? transPercent : 1 - transPercent;

                switch (TransitionType)
                {
                    case TransitionTypeEnum.FadeInOut:
                        SetChildrenAlpha(alphaPercent);
                        break;
                    case TransitionTypeEnum.MoveFade:
                        UpdateTranslation(origPos, movePercent);
                        SetChildrenAlpha(alphaPercent);
                        break;
                    case TransitionTypeEnum.Shrink:
                        UpdateScale(origScale, alphaPercent);
                        break;
                }
                yield return new WaitForEndOfFrame();


            }

            // Make visible for fade-in
            if (bVisible)
            {
                SetChildVisibility(false);
            } 
        
            // Toggle Our Visible Flag
            bVisible = !bVisible;

            //Reset to original values
            this.transform.localPosition = origPos;
            this.transform.localScale = origScale;

            // Reset Alpha
            SetChildrenAlpha(1.0f);

            InTransition = false;
            yield return null;
        }

        // Set the alpha for all children
        private void SetChildrenAlpha(float alpha)
        {
            // grab all child objects
            Renderer[] rendererObjects = this.transform.GetComponentsInChildren<Renderer>();

            // Now lets grab all the initial colors.
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Renderer render = rendererObjects[i];
                Color newColor = render.material.color;
                newColor.a = alpha;
                render.material.color = newColor;
            }
        }

        // Take and set the translation based on the original pos and timescale
        private void UpdateTranslation(Vector3 origPos, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newLoc = Vector3.Scale(RelativeMove, scaleVec);

            this.transform.localPosition = origPos + newLoc;
        }

        // Take and set the scale based on the original scale and timescale
        private void UpdateScale(Vector3 origScale, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newScale = Vector3.Scale(origScale, scaleVec);

            this.transform.localScale = newScale;
        }

        // Take and set the scale based on the original scale and timescale
        private void SetChildVisibility(bool visible)
        {
            foreach (Transform child in ChildList)
            {
                child.gameObject.SetActive(visible);
            }
        }
    }
}
