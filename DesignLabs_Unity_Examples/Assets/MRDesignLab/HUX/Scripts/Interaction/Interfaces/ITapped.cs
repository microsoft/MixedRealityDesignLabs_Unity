//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

/// <summary>
/// Interface for Interaction Manager Tapped message.
/// </summary>
public interface ITapped
{
    /// <summary>
    /// Called when object receives a tap.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnTapped(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);
}
