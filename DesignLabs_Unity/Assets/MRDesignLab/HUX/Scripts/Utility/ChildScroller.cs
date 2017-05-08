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
    /// Child slideshow iterates through children and makes a slideshow of the gameobjects.
    /// </summary>
    public class ChildScroller : MonoBehaviour
    {
        #region public members
        [Tooltip("Audio clip for transition In")]
        public AudioClip TransitionInAudio;

        [Tooltip("Audio clip for transition Out")]
        public AudioClip TransitionOutAudio;

        [Tooltip("Number of children shown")]
        public int ChildrenShown = 4;

        [Tooltip("When set to true requires input to scroll")]
        public bool Manual = false;

        [Tooltip("When true the time between is random")]
        public bool RandomDuration = true;

        [Tooltip("When RandomDuration is true uses random range around child duration")]
        public float RandomRange = 3.0f;

        [Tooltip("Duration between slide transitions")]
        public float ChildDuration = 5.0f;

        [Tooltip("Duration of the transition")]
        public float TransitionDuration = 1.0f;

        [Tooltip("Where to move relative when in transition")]
        public Vector3 RelativeMove = new Vector3(0.0f, 0.0f, 1.0f);

        [Tooltip("Key to use to scroll manually")]
        public KeyCode ScrollKey = KeyCode.Space;

        public enum TransitionTypeEnum
        {
            None,
            FadeInOut,
            MoveFade,
            Shrink
        }
        public TransitionTypeEnum TransitionType = TransitionTypeEnum.FadeInOut;

        [HideInInspector]
        public bool InTransition = false;
        #endregion

        #region private variables
        private int ChildIndex;
        private int LastChildIndex;
        private List<Transform> ChildList = new List<Transform>();
        private List<Transform> ActiveList = new List<Transform>();

        private Vector3 EntryPosition;
        private Vector3 MoveVector;
        private bool bCleanSet = false;
        private AudioSource Speaker;
        #endregion

        public void Awake()
        {
            ChildIndex = LastChildIndex = ChildrenShown - 1;
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                ChildList.Add(child);
                child.gameObject.SetActive(i < ChildrenShown);
                if (child.gameObject.activeSelf)
                {
                    ActiveList.Add(child);
                }
            }

            EntryPosition = ChildList[0].localPosition;
            MoveVector = ChildList[1].localPosition - ChildList[0].localPosition;

            // Add an audio source
            Speaker = this.gameObject.AddComponent<AudioSource>();

            if (!Manual)
            {
                StartCoroutine("Slideshow");
            }
        }

        // Check for moving forward
        private void Update()
        {
            if (!InTransition && Manual)
            {
                if (Input.GetKeyDown(ScrollKey))
                {
                    StartCoroutine("Transition");
                }
            }
        }

        public IEnumerator Slideshow()
        {
            while (true)
            {
                float duration = RandomDuration ? ChildDuration - (RandomRange / 2) + Random.Range(0.0f, RandomRange) : ChildDuration;
                yield return new WaitForSeconds(duration);

                StartCoroutine("Transition");
            }
        }

        public IEnumerator Transition()
        {
            // Get the timestamp for the transition start
            float TimeStamp = Time.fixedTime;

            // Set the bool that we are in transition
            InTransition = true;
            float halfDuration = (TransitionDuration / 2);

            // Here we transition out
            float transPercent;
            Vector3 origScale = ChildList[LastChildIndex].localScale;

            // Transition out audio
            if (TransitionOutAudio != null)
            {
                Speaker.PlayOneShot(TransitionOutAudio);
            }

            while (Time.fixedTime - TimeStamp < halfDuration)
            {
                transPercent = (Time.fixedTime - TimeStamp) / halfDuration;
                foreach (Transform child in ActiveList)
                {
                    child.localPosition = Vector3.Lerp(child.localPosition, child.localPosition + MoveVector, Time.deltaTime / halfDuration);
                }

                SetChildAlpha(LastChildIndex, 1 - transPercent);
                yield return new WaitForEndOfFrame();
            }

            // Hide the child renderer and go to the next index
            ChildList[LastChildIndex].gameObject.SetActive(false);
            ActiveList.Remove(ChildList[LastChildIndex]);

            //Reset to original values
            ChildList[LastChildIndex].localScale = origScale;

            // Check the direction we're going
            ChildIndex = ChildIndex < (ChildList.Count - 1) ? ChildIndex + 1 : 0;

            if (bCleanSet)
            {
                LastChildIndex = LastChildIndex < (ChildList.Count - 1) ? LastChildIndex + 1 : 0;
            }
            else
            {
                LastChildIndex = LastChildIndex > 0 ? LastChildIndex - 1 : ChildrenShown;
                if (LastChildIndex == ChildrenShown)
                {
                    bCleanSet = true;
                }
            }

            // Get new local pos and set active
            origScale = ChildList[ChildIndex].localScale;
            ChildList[ChildIndex].localPosition = EntryPosition;

            ChildList[ChildIndex].gameObject.SetActive(true);
            ActiveList.Add(ChildList[ChildIndex]);

            // Play transition In Audio
            if (TransitionInAudio != null)
            {
                Speaker.PlayOneShot(TransitionInAudio);
            }

            // Here we transition in
            while (Time.fixedTime - TimeStamp < TransitionDuration)
            {
                transPercent = ((Time.fixedTime - TimeStamp) - halfDuration) / halfDuration;
                switch (TransitionType)
                {
                    case TransitionTypeEnum.FadeInOut:
                        SetChildAlpha(ChildIndex, transPercent);
                        break;
                    case TransitionTypeEnum.MoveFade:
                        SetChildTranslation(EntryPosition, 1 - transPercent);
                        SetChildAlpha(ChildIndex, transPercent);
                        break;
                    case TransitionTypeEnum.Shrink:
                        SetChildScale(origScale, transPercent);
                        break;
                }
                yield return new WaitForEndOfFrame();
            }

            //Reset to original values
            ChildList[ChildIndex].localPosition = EntryPosition;
            ChildList[ChildIndex].localScale = origScale;

            InTransition = false;
            yield return null;
        }

        // Set the alpha for all children
        private void SetChildAlpha(int index, float alpha)
        {
            // grab all child objects
            Renderer[] rendererObjects = ChildList[index].GetComponentsInChildren<Renderer>();

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
        private void SetChildTranslation(Vector3 origPos, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newLoc = Vector3.Scale(RelativeMove, scaleVec);
            ChildList[ChildIndex].localPosition = origPos + newLoc;
        }

        // Take and set the scale based on the original scale and timescale
        private void SetChildScale(Vector3 origScale, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newScale = Vector3.Scale(origScale, scaleVec);
            ChildList[ChildIndex].localScale = newScale;
        }
    }
}