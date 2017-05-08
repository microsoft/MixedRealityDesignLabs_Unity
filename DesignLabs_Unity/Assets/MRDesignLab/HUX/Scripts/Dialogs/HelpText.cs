//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Speech;

namespace HUX.Dialogs
{
    public class HelpText : MonoBehaviour
    {
        #region Variables

        [Header("Text")]
        [SerializeField]
        public TextAsset m_DisplayTextAsset;

        [Header("Speech")]
        [SerializeField]
        private KeywordConfidenceLevel ConfidenceThreshold = KeywordConfidenceLevel.Medium;

        [Header("Input")]
        [SerializeField]
        private KeyCode m_ShowHelpKey = KeyCode.H;
        [SerializeField]
        private KeyCode m_HideHelpKey = KeyCode.J;

        [SerializeField]
        private string m_ShowHelpText = "help";
        [SerializeField]
        private string m_HideHelpText = "dismiss";

        private TextMesh m_TextMesh;
        private HUX.Spatial.SolverRectView m_SolverRectView;
        private bool m_bActive;
        #endregion

        // --------------------------------------------------------------------------------

        #region Monobehaviour Functions

        private void Awake()
        {
            m_TextMesh = GetComponentInChildren<TextMesh>();
            m_SolverRectView = GetComponent<HUX.Spatial.SolverRectView>();

            SetActive(false);
        }

        protected void Start()
        {
            m_TextMesh.text = m_DisplayTextAsset ? m_DisplayTextAsset.text : "";

            KeywordManager.Instance.AddKeyword(m_ShowHelpText, OnKeyWord, ConfidenceThreshold);
            KeywordManager.Instance.AddKeyword(m_HideHelpText, OnKeyWord, ConfidenceThreshold);
        }

        private void OnDestroy()
        {
            KeywordManager.Instance.RemoveKeyword(m_ShowHelpText, OnKeyWord);
            KeywordManager.Instance.RemoveKeyword(m_HideHelpText, OnKeyWord);
        }

        private void Update()
        {
            if (Input.GetKeyDown(m_ShowHelpKey))
            {
                SetActive(true);
            }

            if (Input.GetKeyDown(m_HideHelpKey))
            {
                SetActive(false);
            }
        }

        #endregion

        // --------------------------------------------------------------------------------

        #region Private Functions

        private void OnKeyWord(KeywordRecognizedEventArgs args)
        {
            SetActive(!m_bActive);

            if ( m_ShowHelpText.Equals(args.text) && (int)ConfidenceThreshold > (int)args.confidence)
            {
                SetActive(true);
            }
            else if (m_HideHelpText.Equals(args.text) && (int)ConfidenceThreshold > (int)args.confidence)
            {
                SetActive(false);
            }
        }

        private void SetActive(bool enabled)
        {
            m_bActive = enabled;

            if (enabled)
            {
                transform.position = HUX.Veil.Instance.HeadTransform.position + (HUX.Veil.Instance.HeadTransform.forward * m_SolverRectView.MinDistance);
            }

            m_TextMesh.gameObject.SetActive(enabled);
        }

        #endregion
    }
}
