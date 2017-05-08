//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

/// <summary>
/// Interface for Interaction Manager Navigate messages.
/// </summary>
public interface INavigate
{
    /// <summary>
    /// Called when a navigation event starts.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnNavigationStarted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a nagivation event is updated.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnNavigationUpdated(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a nagivation even completes.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnNavigationCompleted(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);

    /// <summary>
    /// Called when a nagivation event is cancelled.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnNavigationCanceled(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);
}
