//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

/// <summary>
/// Interface for Interaction Manager Manipulate messages.
/// </summary>
public interface IManipulate
{
    /// <summary>
    /// Called when a manipulation event starts.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnManipulationStarted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a manipulation event is updated.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnManipulationUpdated(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a manipulation event is completed.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnManipulationCompleted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a manipulation event is cancelled.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnManipulationCanceled(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);
}
