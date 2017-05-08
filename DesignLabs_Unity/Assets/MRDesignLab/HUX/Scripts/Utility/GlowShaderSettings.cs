//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Focus;
using UnityEngine;

/// <summary>
/// This class sets global shader variables for the 'Luminous' shader
/// </summary>
[ExecuteInEditMode]
public class GlowShaderSettings : MonoBehaviour {

    public Color GlowColor = Color.white;
    public Color EdgeColor = Color.white;
    public float GlowRadius = 1f;
    public Vector3 WorldPosition = Vector3.zero;

    /// <summary>
    /// Whether to use the focus manager's cursor position as the glow origin
    /// If this is set to false, it will be up to the user to set the glow origin manually
    /// </summary>
    public bool UseCursorPosition = true;
    
    void Update ()
    {
        if (Application.isPlaying && UseCursorPosition)
        {
            WorldPosition = FocusManager.Instance.GazeFocuser.Position;
        }

        Shader.SetGlobalColor("_HUXButtonGlowColor", GlowColor);
        Shader.SetGlobalColor("_HUXButtonEdgeColor", GlowColor);
        Shader.SetGlobalFloat("_HUXButtonGlowRadius", GlowRadius);
        Shader.SetGlobalVector("_HUXButtonGlowTarget", WorldPosition);
    }
}
