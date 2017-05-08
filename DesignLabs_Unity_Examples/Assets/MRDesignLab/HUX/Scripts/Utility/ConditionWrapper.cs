//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;

namespace HUX.Utility
{
	public class ConditionWrapper : MonoBehaviour
	{
		public InteractibleObject targetObject;
		public InteractionCondition.ConditionType triggerType;

		private InteractionCondition intCondition;

		private void Start()
		{
			// Create our first sequence condition
			intCondition = new InteractionCondition();
			intCondition.targetObject = targetObject;
			intCondition.targetCondition = triggerType;
			intCondition.RegisterCondition();
		}

        private void OnDestroy()
        {
            if (intCondition != null)
            {
                intCondition.UnregisterCondition();
            }
        }
    }
}

