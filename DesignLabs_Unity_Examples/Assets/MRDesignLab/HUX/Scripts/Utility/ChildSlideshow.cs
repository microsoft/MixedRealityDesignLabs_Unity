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
    public class ChildSlideshow : MonoBehaviour
    {
        #region public members

        [Tooltip("Duration between slide transitions")]
        public float SlideDuration = 4.0f;

        [Tooltip("Duration of the transition")]
        public float TransitionDuration = 1.0f;

        [Tooltip("Where to move relative when in transition")]
        public Vector3 RelativeMove = new Vector3(0.0f, 0.0f, 1.0f);

        [Tooltip("Key to use to scroll manually")]
        public KeyCode ForwardKey = KeyCode.Period;

        [Tooltip("Key to use to scroll manually")]
        public KeyCode BackKey = KeyCode.Comma;

        [Tooltip("When true scroll manually")]
        public bool Manual = false;

        [Tooltip("When true scroll forward when false go in reverse")]
        public bool Forward = true;

        public enum TransitionTypeEnum
        {
            None,
            FadeInOut,
            MoveFade,
            Shrink
        }

        [Tooltip("Type of transition between slides")]
        public TransitionTypeEnum TransitionType = TransitionTypeEnum.FadeInOut;

        [HideInInspector]
        public bool InTransition = false;
        #endregion

        #region private variables
        private int ChildIndex = 0;
        private List<Transform> ChildList = new List<Transform>();
        #endregion

        public void Awake()
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Transform child = this.transform.GetChild(i);
                ChildList.Add(child);
                child.gameObject.SetActive(i == ChildIndex);
            }
        }

        public void OnEnable()
        {
            if(!Manual)
            {
                StartCoroutine("Slideshow");
            }
        }

        public void OnDisable()
        {
            if (!Manual)
            {
                StopCoroutine("Slideshow");
            }
        }

        private void Update()
        {
            if (!InTransition)
            {
                if (Input.GetKeyDown(ForwardKey))
                {
                    Forward = true;
                    StartCoroutine("Transition");
                }
                else if (Input.GetKeyDown(BackKey))
                {
                    Forward = false;
                    StartCoroutine("Transition");
                }
            }
        }

        public IEnumerator Slideshow()
        {
            while (true)
            {
                yield return new WaitForSeconds(SlideDuration);
                if (TransitionType == TransitionTypeEnum.None)
                {
                    ChildList[ChildIndex].gameObject.SetActive(false);
                    ChildIndex = ChildIndex < (ChildList.Count - 1) ? ChildIndex + 1 : 0;
                    ChildList[ChildIndex].gameObject.SetActive(true);
                }
                else
                {
                    StartCoroutine("Transition");
                    yield return new WaitForSeconds(TransitionDuration);
                }
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
            Vector3 origLocPos = ChildList[ChildIndex].localPosition;
            Vector3 origScale = ChildList[ChildIndex].localScale;

            // Transition Our
            while(Time.fixedTime - TimeStamp < halfDuration)
            {
                transPercent = (Time.fixedTime - TimeStamp) / halfDuration;
                switch (TransitionType)
                {
                    case TransitionTypeEnum.FadeInOut:
                        SetChildrenAlpha(1 - transPercent);
                        break;
                    case TransitionTypeEnum.MoveFade:
                        SetChildTranslation(origLocPos, transPercent);
                        SetChildrenAlpha(1 - transPercent);
                        break;
                    case TransitionTypeEnum.Shrink:
                        SetChildScale(origScale, 1 - transPercent);
                        break;
                }
                yield return new WaitForEndOfFrame();
            }

            // Hide the child renderer and go to the next index
            ChildList[ChildIndex].gameObject.SetActive(false);

            //Reset to original values
            ChildList[ChildIndex].localPosition = origLocPos;
            ChildList[ChildIndex].localScale = origScale;

            // Check the direction we're going
            if(Forward)
            {
                ChildIndex = ChildIndex < (ChildList.Count - 1) ? ChildIndex + 1 : 0;
            }
            else 
            {
                ChildIndex = ChildIndex > 0 ? ChildIndex - 1 : ChildList.Count - 1;
            }

            // Get new local pos and set active
            origLocPos = ChildList[ChildIndex].localPosition;
            origScale = ChildList[ChildIndex].localScale;
            ChildList[ChildIndex].gameObject.SetActive(true);
            // Here we transition in
            while (Time.fixedTime - TimeStamp < TransitionDuration)
            {
                transPercent = ((Time.fixedTime - TimeStamp) - halfDuration) / halfDuration;
                switch (TransitionType)
                {
                    case TransitionTypeEnum.FadeInOut:
                        SetChildrenAlpha(transPercent);
                        break;
                    case TransitionTypeEnum.MoveFade:
                        SetChildTranslation(origLocPos, 1 - transPercent);
                        SetChildrenAlpha(transPercent);
                        break;
                    case TransitionTypeEnum.Shrink:
                        SetChildScale(origScale, transPercent);
                        break;
                }
                yield return new WaitForEndOfFrame();
            }

            //Reset to original values
            ChildList[ChildIndex].localPosition = origLocPos;
            ChildList[ChildIndex].localScale = origScale;

            InTransition = false;
            yield return null;
        }

        // Set the alpha for all children
        public void SetChildrenAlpha(float alpha)
        {
            // grab all child objects
            Renderer[] rendererObjects = ChildList[ChildIndex].GetComponentsInChildren<Renderer>();

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
        public void SetChildTranslation(Vector3 origPos, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newLoc = Vector3.Scale(RelativeMove, scaleVec);
            ChildList[ChildIndex].localPosition = origPos + newLoc;
        }

        // Take and set the scale based on the original scale and timescale
        public void SetChildScale(Vector3 origScale, float delta)
        {
            Vector3 scaleVec = new Vector3(delta, delta, delta);
            Vector3 newScale = Vector3.Scale(origScale, scaleVec);
            ChildList[ChildIndex].localScale = newScale;
        }
    
    }
}
