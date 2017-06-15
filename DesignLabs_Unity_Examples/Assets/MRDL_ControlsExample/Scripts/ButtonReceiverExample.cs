//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Focus;
using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

namespace HUX
{
    public class ButtonReceiverExample : InteractionReceiver
    {
        public GameObject TextObjectState;
        public TextMesh txt;

        void Start()
        {
            txt = TextObjectState.GetComponentInChildren<TextMesh>();
        }

        protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            txt.text = obj.name + " : OnTapped";

            switch (obj.name)
            {
                case "ButtonMeshPrimitive":
                    // Do something on ButtonMeshPrimitive:OnTapped
                    break;

                case "ButtonCoffeeCup":
                    // Do something on ButtonCoffeeCup:OnTapped
                    break;

                case "ButtonPush":
                    // Do something on ButtonPush:OnTapped
                    break;

                case "ButtonMeshIcosa":
                    // Do something on ButtonMeshIcosa:OnTapped
                    break;

                case "ButtonMeshBucky":
                    // Do something on ButtonMeshBucky:OnTapped
                    break;

                case "ButtonHolographic":
                    // Do something on ButtonHolographic:OnTapped
                    break;

                case "ButtonTraditional":
                    // Do something on ButtonTraditional:OnTapped
                    break;

                case "ButtonBalloon":
                    // Do something on ButtonBalloon:OnTapped
                    break;

                case "ButtonCheese":
                    // Do something on ButtonCheese:OnTapped
                    break;
            }
            base.OnTapped(obj, eventArgs);
        }

        protected override void OnHoldStarted(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
        {
            txt.text = obj.name + " : OnHoldStarted";
            base.OnHoldStarted(obj, eventArgs);
        }

        protected override void OnFocusEnter(GameObject obj, FocusArgs args)
        {
            txt.text = obj.name + " : OnFocusEnter";
            base.OnFocusEnter(obj, args);
        }

        protected override void OnFocusExit(GameObject obj, FocusArgs args)
        {
            txt.text = obj.name + " : OnFocusExit";
            base.OnFocusExit(obj, args);
        }

    }
}
