//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HUX.Interaction
{
    /// <summary>
    /// This is a simple interactible for toggling other interactible components.
    /// </summary>
    public class ToggleInteractible : MonoBehaviour
    {
        public interface IToggleable
        {
            bool enabled { get; set; }
        }

        [Tooltip("Target object to enable or disable components on")]
        public GameObject TargetObject;

        [Tooltip("Transition time between slides")]
        public Component[] EnableComponents;

        [Tooltip("Transition time between slides")]
        public Component[] DisableComponents;

        [Tooltip("Reset from prefab works in editor to revert back to prefab settings")]
        public bool ResetFromPrefab;

        private bool bToggled;

        private bool m_Targted;

        protected void OnTapped(InteractionManager.InteractionEventArgs args)
        {
            this.Triggered();
        }

        protected void OnHoldStarted(InteractionManager.InteractionEventArgs args)
        {
            m_Targted = true;
        }

        private void OnHoldCompleted(InteractionManager.InteractionEventArgs e)
        {
            if (m_Targted)
            {
                this.Triggered();
                m_Targted = false;
            }
        }

        private void OnHoldCanceled(InteractionManager.InteractionEventArgs e)
        {
            m_Targted = false;
        }

        protected void FocusExit()
        {
            m_Targted = false;
        }

        protected void Triggered()
        {
            if (TargetObject != null)
            {
#if UNITY_EDITOR
            	    if (ResetFromPrefab) { PrefabUtility.ResetToPrefabState(TargetObject); }
#endif

                bToggled = !bToggled;

                if (EnableComponents.Length > 0 || DisableComponents.Length > 0)
                {
                    // Enable the components in the enable array
                    foreach (var component in EnableComponents)
                    {
                        MonoBehaviour toggleComp = (MonoBehaviour)component;
                        if (toggleComp != null)
                        {
                            toggleComp.enabled = bToggled;
                        }
                    }

                    // Disable the components in the disable array
                    foreach (var component in DisableComponents)
                    {
                        MonoBehaviour toggleComp = (MonoBehaviour)component;
                        if (toggleComp != null)
                        {
                            toggleComp.enabled = !bToggled;
                        }
                    }
                }
                else
                {
                    var intComponents = TargetObject.GetComponents<IToggleable>();
                    foreach (var component in intComponents)
                    {
                        component.enabled = bToggled;
                    }
                }
            }
        }
    }
}