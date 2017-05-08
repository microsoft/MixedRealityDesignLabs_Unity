//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Utility
{
    /// <summary>
    /// Interface for a sequencer condition.  Simple class to check for moving to the next object.
    /// </summary>
    public interface ISequencerCondition
    {
        void RegisterCondition();
        bool CheckCondition();
    }

}

