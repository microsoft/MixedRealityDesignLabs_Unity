//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System;
using HUX.Interaction;
using HUX.Focus;

namespace HUX.Receivers
{
	/// <summary>
	/// Animate the target when the correct conditions are met.
	/// </summary>
	public class AnimationReceiver : InteractionReceiver
	{
		#region public
		public enum StartType
		{
			OnGaze,
			OnSelect
		}

		[Tooltip("Condition to start the Animation")]
		public StartType AnimCondition = StartType.OnGaze;

		public GameObject TargetObject;
		public string AnimName;
		public float BlendTime;
		public bool LoopAnimation;

		public PlayMode AnimPlayMode = PlayMode.StopAll;

		//public float AnimationRate = 1.0f;
		#endregion

		#region private
		private bool bPlaying;
		#endregion

		// When if set to gaze trigger then play anim when gazing
		protected void FocusEnter(FocusArgs args)
		{
            if (args.CurNumFocusers == 1)
            {
                Animation targetAnim = TargetObject.GetComponent<Animation>();
                if (AnimCondition == StartType.OnGaze && TargetObject != null && targetAnim != null)
                {
                    var time = BlendTime;
                    if (time < 0.001f)
                    {
                        targetAnim.Play(AnimName, AnimPlayMode);
                    }
                    else
                    {
                        targetAnim.CrossFade(AnimName, time, AnimPlayMode);
                    }

                    bPlaying = true;
                }
            }
		}

		// When if gaze then stop animation if gaze ended
		protected void FocusExit(FocusArgs args)
		{
            if (args.CurNumFocusers == 0)
            {
                Animation targetAnim = TargetObject.GetComponent<Animation>();

                if (AnimCondition == StartType.OnGaze && TargetObject != null && targetAnim != null)
                {
                    targetAnim.Stop(AnimName);
                    bPlaying = false;
                }
            }
		}

		protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
		{
			Animation targetAnim = TargetObject.GetComponent<Animation>();

			if (AnimCondition == StartType.OnSelect)
			{
				if (bPlaying)
				{
					targetAnim.Stop(AnimName);
					bPlaying = false;
				}
				else
				{
					var time = BlendTime;
					if (time < 0.001f)
					{
						targetAnim.Play(AnimName, AnimPlayMode);
					}
					else
					{
						targetAnim.CrossFade(AnimName, time, AnimPlayMode);
					}

					bPlaying = true;
				}
			}
		}
	}
}
