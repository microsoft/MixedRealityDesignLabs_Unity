//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public class SpatialRenderingConfiguration : Singleton<SpatialRenderingConfiguration>
{
    /// <summary>
    /// Defines the materials available to be used for a surfaces render material.
    /// </summary>
    public enum RenderingState
    {
        /// <summary>
        /// Occluding material. Occludes objects but does not clear.
        /// </summary>
        Occlusion = 0,

        /// <summary>
        /// Default material, as set in editor.
        /// </summary>
        DefaultMaterial = 1,

        /// <summary>
        /// Normal Map
        /// </summary>
        NormalMap = 2,

        /// <summary>
        /// Will not render or occlude.
        /// </summary>
        None = int.MaxValue,
    }

    /// <summary>
    /// Reference to material used for occlusion.
    /// </summary>
    [SerializeField]
    [Tooltip("Specify the material to be used when the surfaces should occlude.")]
    private Material m_OcclusionMaterial = null;

    [SerializeField]
    private RenderingState m_CurrentRenderingState = RenderingState.DefaultMaterial;

    private Material m_SpatialMappingDefaultMaterial = null;
    private Material m_SpatialUnderstandingDefaultMaterial = null;

    private SpatialMappingManager m_SpatialMappingManager = null;
    private SpatialUnderstandingCustomMesh m_SpatialUnderstandingCustomMesh = null;

    public RenderingState CurrentRenderingState
    {
        get
        {
            return m_CurrentRenderingState;
        }

        set
        {
            m_CurrentRenderingState = value;
            switch (value)
            {
                case RenderingState.Occlusion:
                {
                    m_SpatialMappingManager.SurfaceMaterial = m_OcclusionMaterial;
                    m_SpatialUnderstandingCustomMesh.MeshMaterial = m_OcclusionMaterial;
                    break;
                }

                case RenderingState.DefaultMaterial:
                {
                    m_SpatialMappingManager.SurfaceMaterial = m_SpatialMappingDefaultMaterial;
                    m_SpatialUnderstandingCustomMesh.MeshMaterial = m_SpatialUnderstandingDefaultMaterial;
                    break;
                }

                case RenderingState.None:
                {
                    m_SpatialMappingManager.SurfaceMaterial = null;
                    m_SpatialMappingManager.DrawVisualMeshes = false;
                    m_SpatialUnderstandingCustomMesh.MeshMaterial = null;
                    m_SpatialUnderstandingCustomMesh.DrawProcessedMesh = false;
                    break;
                }
            }
        }
    }

    void Start()
    {
        m_SpatialMappingManager = SpatialMappingManager.Instance;
        m_SpatialUnderstandingCustomMesh = SpatialUnderstanding.Instance.UnderstandingCustomMesh;

        m_SpatialMappingDefaultMaterial = m_SpatialMappingManager.SurfaceMaterial;
        m_SpatialUnderstandingDefaultMaterial = m_SpatialUnderstandingCustomMesh.MeshMaterial;

        CurrentRenderingState = m_CurrentRenderingState;
    }

    void OnValidate()
    {
        if (Application.isPlaying && m_SpatialMappingManager && m_SpatialUnderstandingCustomMesh)
        {
            CurrentRenderingState = m_CurrentRenderingState;
        }
    }
}
