//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using UnityEngine;

namespace HUX.Buttons
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CompoundButton))]
    public class CompoundButtonIcon : MonoBehaviour
    {
        /// <summary>
        /// How quickly to animate changing icon alpha at runtime
        /// </summary>
        const float AlphaTransitionSpeed = 0.25f;

        /// <summary>
        /// Turns off the icon entirely
        /// </summary>
        public bool DisableIcon = false;

        /// <summary>
        /// Disregard the icon in the profile
        /// </summary>
        public bool OverrideIcon = false;

        /// <summary>
        /// Icon to use for override
        /// </summary>
        public Texture2D IconOverride
        {
            get
            {
                return iconOverride;
            }
            set
            {
                iconOverride = value;
                SetIconName(string.Empty);
            }
        }

        /// <summary>
        /// Property to use in IconMaterial for alpha control
        /// Useful for animating icon transparency
        /// </summary>
        public float Alpha
        {
            get
            {
                return alphaTarget;
            }
            set
            {
                if (alphaTarget != value)
                {
                    alphaTarget = value;
                    if (Application.isPlaying)
                    {                        
                        if (Mathf.Abs (alpha - alphaTarget) < 0.01f)
                            return;

                        if (updatingAlpha)
                            return;

                        if (gameObject.activeSelf && gameObject.activeInHierarchy)
                        {
                            // Update over time
                            updatingAlpha = true;
                            StartCoroutine(UpdateAlpha());
                        }
                        else
                        {
                            // If we're not active, just set the alpha immediately
                            alpha = alphaTarget;
                            RefreshAlpha();
                        }
                    }
                    else
                    {
                        alphaTarget = value;
                        alpha = alphaTarget;
                        RefreshAlpha();
                    }
                }
            }
        }

        public ButtonIconProfile IconProfile;
        public MeshRenderer IconRenderer;
        public Material IconMaterial;

        /// <summary>
        /// Property used to modify icon alpha
        /// If this is null alpha will not be appiled
        /// </summary>
        public string AlphaColorProperty = "_Color";

        #if UNITY_EDITOR
        /// <summary>
        /// Called by CompoundButtonSaveInterceptor
        /// Prevents saving a scene with instanced materials
        /// </summary>
        public void OnWillSaveScene()
        {
            if (IconRenderer != null && instantiatedMaterial != null)
            {
                IconRenderer.sharedMaterial = IconMaterial;
                GameObject.DestroyImmediate(instantiatedMaterial);
            }
        }
        #endif

        public string IconName
        {
            get
            {
                return iconName;
            }
            set
            {
                SetIconName(value);
            }
        }

        private void SetIconName(string newName)
        {
            if (IconRenderer == null)
            {
                Debug.LogError("Icon renderer null in in CompoundButtonIcon " + name);
                return;
            }

            if (DisableIcon)
            {
                IconRenderer.enabled = false;
                return;
            }

            // Instantiate our local material now, if we don't have one
            if (instantiatedMaterial == null)
            {
                instantiatedMaterial = new Material(IconMaterial);
            }
            else if (!instantiatedMaterial.name.Contains(IconMaterial.name))
            {
                if (!Application.isPlaying)
                {
                    // Prevent material leaks
                    GameObject.DestroyImmediate(instantiatedMaterial);
                }
                instantiatedMaterial = new Material(IconMaterial);
                instantiatedMaterial.name = IconMaterial.name;
            }

            IconRenderer.sharedMaterial = instantiatedMaterial;

            if (OverrideIcon)
            {
                IconRenderer.enabled = true;
                instantiatedMaterial.mainTexture = iconOverride;
                return;
            }

            if (IconProfile == null)
            {
                Debug.LogError("Icon profile was null in CompoundButtonIcon " + name);
                return;
            }

            if (string.IsNullOrEmpty(newName))
            {
                IconRenderer.enabled = false;
                iconName = newName;
                return;
            }

            Texture2D icon = null;
            if (!IconProfile.GetIcon(newName, out icon, true))
            {
                Debug.LogError("Icon " + newName + " not found in icon profile " + IconProfile.name + " in CompoundButtonIcon " + name);
                return;
            }

            IconRenderer.enabled = true;
            iconName = newName;

            instantiatedMaterial.mainTexture = icon;
            RefreshAlpha();

        }

        private void OnDisable()
        {
            // Prevent material leaks
            if (instantiatedMaterial != null)
            {
                GameObject.DestroyImmediate(instantiatedMaterial);
            }

            // Restore the icon material to the renderer
            IconRenderer.sharedMaterial = IconMaterial;
            // Reset our alpha to the alpha target
            alpha = alphaTarget;
        }

        private void OnEnable()
        {
            // Prevent material leaks
            if (instantiatedMaterial != null)
            {
                GameObject.DestroyImmediate(instantiatedMaterial);
            }

            SetIconName(iconName);
        }

        private void RefreshAlpha()
        {
            if (instantiatedMaterial != null && !string.IsNullOrEmpty(AlphaColorProperty))
            {
                Color c = instantiatedMaterial.GetColor(AlphaColorProperty);
                c.a = alpha;
                instantiatedMaterial.SetColor(AlphaColorProperty, c);
            }
        }

        private void OnDrawGizmos()
        {
            if (DisableIcon)
            {
                if (IconRenderer != null)
                {
                    IconRenderer.enabled = false;
                }
            }
            else
            {
                IconRenderer.enabled = true;
                if (IconProfile != null)
                {
                    SetIconName(iconName);
                }
            }
        }

        private IEnumerator UpdateAlpha()
        {
            float startTime = Time.time;
            Color color = Color.white;
            if (instantiatedMaterial != null && !string.IsNullOrEmpty(AlphaColorProperty))
            {
                color = instantiatedMaterial.GetColor(AlphaColorProperty);
                color.a = alpha;
            }
            while (Time.time < startTime + AlphaTransitionSpeed)
            {
                alpha = Mathf.Lerp(alpha, alphaTarget, (Time.time - startTime) / AlphaTransitionSpeed);
                if (instantiatedMaterial != null && !string.IsNullOrEmpty(AlphaColorProperty))
                {
                    color.a = alpha;
                    instantiatedMaterial.SetColor(AlphaColorProperty, color);
                }
                yield return null;
            }
            alpha = alphaTarget;
            RefreshAlpha();
            updatingAlpha = false;
            yield break;
        }

        [SerializeField]
        private Texture2D iconOverride;

        [SerializeField]
        private string iconName;

        [SerializeField]
        private float alpha = 1f;

        [SerializeField]
        private Material instantiatedMaterial;

        private bool updatingAlpha = false;
        private float alphaTarget = 1f;
    }
}
