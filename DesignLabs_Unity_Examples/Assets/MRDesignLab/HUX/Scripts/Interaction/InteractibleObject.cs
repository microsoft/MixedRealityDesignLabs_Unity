//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

namespace HUX.Interaction
{
    /// <summary>
    /// Objects wanting to recieve messages can inherit from this class.
    /// They will only recieve a message from the Focus Manager or Interaction Manager
    /// if their custom Filter Tag is valid on the Focus Manager.
    /// </summary>
    public class InteractibleObject : MonoBehaviour
    {
#region public members
        public FilterTag FilterTag;
#endregion
    }

    /// <summary>
    /// The following functions can be declared. They will automatically be called via messages sent from the Focus Manager or Interaction Manager.
    /// These messages can be declared in any script, you do NOT need to inherit from InteractibleObject.
    /// 
    /// FocusEnter() & FocusExit() will bubble up through the hierarchy, starting from the Prime Focus collider.
    /// 
    /// All other messages will only be sent to the Prime Focus collider.
    /// 
    /// Note: PrimeFocus means the collider or UI element hit first with a raycast from Gaze Origin. It can be found in the Focus Manager.
    /// </summary>

    /// protected void FocusEnter() { }
    /// protected void FocusExit() { }

    /// protected void OnNavigationStarted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnNavigationUpdated(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnNavigationCompleted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnNavigationCanceled(InteractionManager.InteractionEventArgs eventArgs) { }

    /// protected void OnTapped(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnDoubleTapped(InteractionManager.InteractionEventArgs eventArgs) { }

    /// protected void OnHoldStarted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnHoldCompleted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnHoldCanceled(InteractionManager.InteractionEventArgs eventArgs) { }

    /// protected void OnManipulationStarted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnManipulationUpdated(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnManipulationCompleted(InteractionManager.InteractionEventArgs eventArgs) { }
    /// protected void OnManipulationCanceled(InteractionManager.InteractionEventArgs eventArgs) { }
}
