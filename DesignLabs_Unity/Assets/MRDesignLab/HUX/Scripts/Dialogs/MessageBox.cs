//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HUX.Dialogs
{
    /// <summary>
    /// Message Box is an object for three line and dynamic background scaling messages.
    /// Supports delays between lines and line Audio
    /// </summary>
    public class MessageBox : MonoBehaviour
    {
        [Tooltip("First line text mesh")]
        public TextMesh LineOne;

        [Tooltip("Second line text mesh")]
        public TextMesh LineTwo;

        [Tooltip("Third line text mesh")]
        public TextMesh LineThree;

        [Tooltip("Game object with text lines")]
        public GameObject TextContainer;

        [Tooltip("Background object behind text")]
        public GameObject Background;

        [Tooltip("Time to fade between messages")]
        public float fadeTime = 1.0f;

        [Tooltip("Return true if playing message")]
        public bool playingMessage;

        [Tooltip("Offset from the Veil to display message")]
        public Vector3 VeilOffset;

        [System.Serializable]
        public class MessageDatum
        {
            public string LineOneText;
            public AudioClip AudioOne;
            public string LineTwoText;
            public AudioClip AudioTwo;
            public string LineThreeText;
            public AudioClip AudioThree;
            public float duration = 0.0f;
            public float lineDelay = 0.2f;
            public Vector3 messageOffset = Vector3.zero;
            public int MovieState = 0;
        }

        public Utility.MovieScreen TargetMovieScreen;


        private float _bg_height;
        private bool _txt_fading = false;
        private bool _indefinite_play = false;

        private float _duration = 0.0f;
        private float _lineDelay = 0.0f;
        private int _lineCount = 0;

        private AudioClip _lineOneAudio;
        private AudioClip _lineTwoAudio;
        private AudioClip _lineThreeAudio;
        private Vector3 _additionalOffset = Vector3.zero;

        private Transform head;


        private void Start()
        {
            if (Background != null)
                _bg_height = Background.transform.localScale.y;

            LineOne.text = LineTwo.text = LineThree.text = "";

            //First Line
            Color curColor = LineOne.GetComponent<Renderer>().material.color;
            curColor.a = 0.0f;
            LineOne.GetComponent<Renderer>().material.color = LineTwo.GetComponent<Renderer>().material.color = LineThree.GetComponent<Renderer>().material.color = curColor;

            if (TargetMovieScreen == null)
                TargetMovieScreen = this.transform.GetComponentInChildren<Utility.MovieScreen> ();

            head = Veil.Instance.HeadTransform;
        }

        public void FixedUpdate()
        {
            // Update message box based on screen offset
            this.transform.position = Vector3.Lerp(this.transform.position, head.TransformPoint(VeilOffset + _additionalOffset), Time.fixedDeltaTime * 5.0f);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, head.rotation, Time.fixedDeltaTime * 5.0f);
        }

        public float PlayMessage(MessageDatum message)
        {
            // Start Playing Message
            playingMessage = true;

            _lineCount = 0;

            _duration = message.duration;
            _lineDelay = message.lineDelay;

            _lineOneAudio = message.AudioOne;
            _lineTwoAudio = message.AudioTwo;
            _lineThreeAudio = message.AudioThree;
            _additionalOffset = message.messageOffset;

            LineOne.text = message.LineOneText;
            if (LineOne.text != "")
                _lineCount++;
            LineTwo.text = message.LineTwoText;
            if (LineTwo.text != "")
                _lineCount++;
            LineThree.text = message.LineThreeText;
            if (LineThree.text != "")
                _lineCount++;

            if(_lineCount > 0)
            {
                if(Background != null)
                    UpdateTextContainer(_lineCount);

                if (_duration > 0.0f)
                {
                    StartCoroutine("FadeInOutText", _duration);
                }
                else
                {
                    StartCoroutine("FadeText", false);
                    _indefinite_play = true;
                }
            }
            else
            {
                playingMessage = false;
            }

            if (TargetMovieScreen != null)
                TargetMovieScreen.SetState(message.MovieState);

            return message.duration + (fadeTime * 2.0f);
        }


        public IEnumerator FadeInOutText(float duration)
        {
            // Debug.Log("Fade In/Out");
            if (_lineCount > 0)
            {
                StartCoroutine("FadeText", false);
                while (_txt_fading)
                    yield return new WaitForEndOfFrame();

                yield return new WaitForSeconds(duration);

                StartCoroutine("FadeText", true);
                while (_txt_fading)
                    yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(duration);
            }

            playingMessage = false;
        }

        public IEnumerator FadeText(bool FadeOut)
        {
            _txt_fading = true;

            // Debug.Log("Fade Text: " + FadeOut);
            Color curColor = LineOne.GetComponent<Renderer>().material.color;
            float targetAlpha = FadeOut ? 0.0f : 1.0f;
            float timeStamp = Time.time;

            // Here we check for having independent delays for the separate lines only on fading in
            if (_lineDelay > 0.0f && !FadeOut)
            {
                if (LineOne.text != "")
                {
                    if (_lineOneAudio != null)
                        AudioManager.Instance.PlayClip(_lineOneAudio);

                    while (curColor.a != targetAlpha)
                    {
                        curColor.a = Mathf.Lerp(curColor.a, targetAlpha, (Time.time - timeStamp) / fadeTime);
                        LineOne.GetComponent<Renderer>().material.color = curColor;
                        yield return new WaitForFixedUpdate();
                    }
                }
                if (LineTwo.text != "")
                {
                    yield return new WaitForSeconds(_lineDelay);
                    if (_lineTwoAudio != null)
                        AudioManager.Instance.PlayClip(_lineTwoAudio);

                    timeStamp = Time.time;

                    curColor = LineTwo.GetComponent<Renderer>().material.color;
                    while (curColor.a != targetAlpha)
                    {
                        curColor.a = Mathf.Lerp(curColor.a, targetAlpha, (Time.time - timeStamp) / fadeTime);
                        LineTwo.GetComponent<Renderer>().material.color = curColor;
                        yield return new WaitForFixedUpdate();
                    }
                }
                if (LineThree.text != "")
                {
                    yield return new WaitForSeconds(_lineDelay);
                    if (_lineThreeAudio != null)
                        AudioManager.Instance.PlayClip(_lineThreeAudio);

                    timeStamp = Time.time;
                    curColor = LineThree.GetComponent<Renderer>().material.color;
                    while (curColor.a != targetAlpha)
                    {
                        curColor.a = Mathf.Lerp(curColor.a, targetAlpha, (Time.time - timeStamp) / fadeTime);
                        LineThree.GetComponent<Renderer>().material.color = curColor;
                        yield return new WaitForFixedUpdate();
                    }
                }
            }
            else
            {
                while (curColor.a != targetAlpha)
                {
                    // Debug.Log("Cur: " + curColor.a + " Target: " + targetAlpha);
                    curColor.a = Mathf.Lerp(curColor.a, targetAlpha, (Time.time - timeStamp) / fadeTime);
                    LineOne.GetComponent<Renderer>().material.color = LineTwo.GetComponent<Renderer>().material.color = LineThree.GetComponent<Renderer>().material.color = curColor;

                    // Even the the delay is 0.0 play the audio
                    if (_lineOneAudio != null)
                        AudioManager.Instance.PlayClip(_lineOneAudio);

                    if (_lineTwoAudio != null)
                        AudioManager.Instance.PlayClip(_lineTwoAudio);

                    if (_lineThreeAudio != null)
                        AudioManager.Instance.PlayClip(_lineThreeAudio);

                    yield return new WaitForFixedUpdate();
                }
            }

            _txt_fading = false;

            if (_indefinite_play)
            {
                _indefinite_play = false;
                playingMessage = false;
            }

            yield return null;
        }

        public void UpdateTextContainer(int lineNum)
        {
            // Set the offset of the text contatin
            float yOffset = 0;
            switch (lineNum)
            {
                case 0:
                    if (Background.activeSelf)
                        Background.SetActive(false);
                    break;
                case 1:
                    if (!Background.activeSelf)
                        Background.SetActive(true);
                    yOffset = -LineOne.transform.localPosition.y;
                    break;
                case 2:
                    if (!Background.activeSelf)
                        Background.SetActive(true);
                    yOffset = -(LineOne.transform.localPosition.y + LineTwo.transform.localPosition.y) / 2;
                    break;
                case 3:
                    if (!Background.activeSelf)
                        Background.SetActive(true);
                    yOffset = -LineTwo.transform.localPosition.y;
                    break;
            }

            if (TextContainer != null)
            {
                Vector3 newPos = TextContainer.transform.localPosition;
                newPos.y = yOffset;
                TextContainer.transform.localPosition = newPos;
            }

            // Update background height.
            if (Background != null)
            {
                Vector3 newScale = Background.transform.localScale;
                newScale.y = lineNum * _bg_height;
                Background.transform.localScale = newScale;
            }
        }
    }
}