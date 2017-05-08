//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX;
using HUX.Interaction;
using HUX.Focus;

namespace HUX.Receivers
{
    public class AnimProxReceiver : ProximityReceiver
    {
        public enum AnimStateEnum
        {
            Proximity,
            Observation,
            ObservationTargeted,
            Interactive,
            Targeted,
            Press,
            EnterManipulationMode,
            ExitManipulationMode
        }

        public AnimStateEnum AnimState = AnimStateEnum.Observation;

        public PlayMode AnimPlayMode = PlayMode.StopAll;

        private Animator targetAnimator;

        public void Start()
        {
            targetAnimator = gameObject.GetComponent<Animator>();

            if (targetAnimator != null)
                targetAnimator.SetInteger("CubeState", (int)AnimState);
        }


		protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs e)
		{
            SetState(AnimStateEnum.Press);
        }

        protected void FocusEnter(FocusArgs args)
        {
            if (args.CurNumFocusers == 1)
            {
                if (AnimState == AnimStateEnum.Proximity)
                    return;

                if (Veil.Instance.HandVisible)
                {
                    SetState(AnimStateEnum.Interactive);
                }
                else
                {
                    SetState(AnimStateEnum.ObservationTargeted);
                }
            }
        }

        protected void FocusExit(FocusArgs args)
        {
            if (args.CurNumFocusers == 0)
            {
                if (AnimState == AnimStateEnum.Proximity)
                    return;

                if (Veil.Instance.HandVisible)
                {
                    SetState(AnimStateEnum.Observation);
                }
                else
                {
                    SetState(AnimStateEnum.Observation);
                }
            }
        }

        protected override void OnProximityEnter()
        {
            SetState(AnimStateEnum.Proximity);
        }

        protected override void OnProximityExit()
        {
            SetState(AnimStateEnum.Observation);
        }

        private void SetState(AnimStateEnum NewAnimState)
        {
            if (targetAnimator != null)
            {
                if (AnimState != NewAnimState)
                {
                    AnimState = NewAnimState;
                    targetAnimator.SetInteger("CubeState", (int)AnimState);
                }
            }
        }
    }
}

