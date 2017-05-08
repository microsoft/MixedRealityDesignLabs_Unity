//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

/// <summary>
/// Interface for Interaction Manager Hold messages.
/// </summary>
public interface IHold
{
    /// <summary>
    /// Called when a hold on an object starts.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnHoldStarted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a hold on an object completes.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnHoldCompleted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a hold on an object is cancelled.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnHoldCanceled(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);
}
