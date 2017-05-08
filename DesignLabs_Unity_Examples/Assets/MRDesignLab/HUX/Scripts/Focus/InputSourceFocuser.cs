//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;

namespace HUX.Focus
{
    public class InputSourceFocuser : AFocuser
    {
        #region Editor Variables
        /// <summary>
        /// The input source to use for this focuser.
        /// </summary>
        [SerializeField]
        private InputSourceBase m_InputSource;
        #endregion

        //------------------------------------------------------

        #region Private Variables
        /// <summary>
        /// The targeting input source reference for our input source.
        /// </summary>
        private ITargetingInputSource m_TargetingInputSrc;
        #endregion

        //------------------------------------------------------

        #region Accessors
        public ITargetingInputSource TargetingInputSource
        {
            get
            {
                if (m_TargetingInputSrc != null)
                {
                    return m_TargetingInputSrc;
                }

                if (m_InputSource != null)
                {
                    m_TargetingInputSrc = m_InputSource as ITargetingInputSource;
                    if (m_TargetingInputSrc == null)
                    {
                        throw new InvalidCastException("Input source is not a ITargetingInputSource.");
                    }
                    return m_TargetingInputSrc;
                }
                else
                {
                    throw new NullReferenceException("No input source assigned.");
                }
            }
        }
        #endregion

        //------------------------------------------------------

        #region AFocuser Implementation
        public override bool CanInteract
        {
            get
            {
                return true;
            }
        }

        public override bool IsInteractionReady
        {
            get
            {
                return TargetingInputSource.IsReady();
            }
        }

        public override bool IsSelectPressed
        {
            get
            {
                return TargetingInputSource.IsSelectPressed();
            }
        }

        public override Vector3 TargetDirection
        {
            get
            {
                return TargetingInputSource.GetTargetRotation() * Vector3.forward;
            }
        }

        public override Quaternion TargetOrientation
        {
            get
            {
                return TargetingInputSource.GetTargetRotation();
            }
        }

        public override Vector3 TargetOrigin
        {
            get
            {
                return TargetingInputSource.GetTargetOrigin();
            }
        }

		protected override void OnManipulationStarted(Transform frame)
		{
			if (m_InputSource != null)
			{
				m_InputSource.StartManipulating(frame);
			}	
		}

		protected override void OnManipulationStopped()
		{
			if (m_InputSource != null)
			{
				m_InputSource.StopManipulating();
			}	
		}
        #endregion

        //------------------------------------------------------
        #region Monobehaviour Functions
		protected override void Update()
        {
			base.Update();
        }
        #endregion



        public static AFocuser GetFocuserForInputSource(InputSourceBase inputSource)
        {
            AFocuser focuser = null;
            if (FocusManager.Instance != null)
            {
                for (int index = 0; index < FocusManager.Instance.Focusers.Length; index++)
                {
                    InputSourceFocuser sourceFocuser = FocusManager.Instance.Focusers[index] as InputSourceFocuser;
                    if (sourceFocuser != null && sourceFocuser.m_InputSource == inputSource)
                    {
                        focuser = FocusManager.Instance.Focusers[index];
                        break;
                    }
                }

                //If we haven't found a specific focuser use the gaze.
                if (focuser == null)
                {
                    focuser = FocusManager.Instance.GazeFocuser;
                }
            }

            return focuser;
        }
    }
}
