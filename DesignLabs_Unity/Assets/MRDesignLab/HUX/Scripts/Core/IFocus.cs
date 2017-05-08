//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
namespace HUX.Focus
{
    /// <summary>
    /// Interface for FocusManager focus messages.
    /// </summary>
    public interface IFocus
    {
        /// <summary>
        /// Called when object receives user gaze.
        /// </summary>
        void FocusEnter(FocusArgs args);

        /// <summary>
        /// Called when object loses user gaze.
        /// </summary>
        void FocusExit(FocusArgs args);
    }
}
