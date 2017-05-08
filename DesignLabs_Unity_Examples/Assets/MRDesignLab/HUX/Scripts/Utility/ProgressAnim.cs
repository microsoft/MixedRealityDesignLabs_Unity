//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Utility
{
    public class ProgressAnim : MonoBehaviour
    {

        public int State;

        public void SetProgressState(int newState)
        {
            State = newState;
        }
    }
}
