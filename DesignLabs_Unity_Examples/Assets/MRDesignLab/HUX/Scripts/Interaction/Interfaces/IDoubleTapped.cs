//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

/// <summary>
/// Interface for Interaction Manager Double Tapped message.
/// </summary>
public interface IDoubleTapped
{
    /// <summary>
    /// Called when object receives a double tap.
    /// </summary>
    /// <param name="eventArgs">Relevant event information</param>
    void OnDoubleTapped(HUX.Interaction.InteractionManager.InteractionEventArgs eventArgs);
}
