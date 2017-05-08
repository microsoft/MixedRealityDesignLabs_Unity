//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Spatial;
using System;

public class SpeechCommandSwitchMesh : SpeechCommand
{
    [UnityEngine.SerializeField]
    private SpatialRenderingConfiguration m_SpatialRenderingConfiguration = null;


    protected override void OnKeyword()
    {
#if UNITY_WSA
        SpatialRenderingConfiguration.RenderingState setting = SpatialRenderingConfiguration.Instance.CurrentRenderingState;

		if ((int)++setting >= Enum.GetValues(typeof(SpatialRenderingConfiguration.RenderingState)).Length)
		{
			setting = 0;
		}

        m_SpatialRenderingConfiguration.CurrentRenderingState = (SpatialRenderingConfiguration.RenderingState)setting;
        SetText("Process Mesh Mode: " + Enum.GetName(typeof(SpatialRenderingConfiguration.RenderingState), setting));
#endif
    }
}