//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public static class SurfacePlaneExtensions
{
    /// <summary>
    /// Sets the renderer material.
    /// </summary>
    public static void SetPlaneMaterial(this SurfacePlane surfacePlane, Material mat)
    {
        Renderer renderer = surfacePlane.gameObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = mat;
        }
    }

    /// <summary>
    /// Updates the plane geometry to match the bounded plane found by SurfaceMeshesToPlanes.
    /// </summary>
    private static void SetPlaneGeometry(this SurfacePlane surfacePlane)
    {
        //surfacePlane.SurfaceNormal = surfacePlane.plane.normal;
        //
        //// Set the SurfacePlane object to have the same extents as the BoundingPlane object.
        //surfacePlane.gameObject.transform.position = surfacePlane.Bounds.Center;
        //surfacePlane.gameObject.transform.rotation = surfacePlane.Bounds.Rotation;
        //Vector3 extents = surfacePlane.Bounds.Extents * 2;
        //surfacePlane.gameObject.transform.localScale = extents;
    }
}
