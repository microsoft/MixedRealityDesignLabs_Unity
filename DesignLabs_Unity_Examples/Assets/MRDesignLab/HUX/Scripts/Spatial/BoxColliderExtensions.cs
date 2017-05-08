//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public static class BoxColliderExtensions
{
    public const int LeftBottomFront = 0;
    public const int LeftBottomBack = 1;
    public const int LeftTopFront = 2;
    public const int LeftTopBack = 3;
    public const int RightBottomFront = 4;
    public const int RightBottomBack = 5;
    public const int RightTopFront = 6;
    public const int RightTopBack = 7;

    /// <summary>
    /// Gets all the corner points of the box collider in world space if it had the given local transform.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="positions"></param>
    /// <remarks>
    /// Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
    /// corners array.
    /// </remarks>
    public static void GetCornerPositions(this BoxCollider collider, ref Vector3[] positions)
    {
        // Calculate the local points to transform.
        Vector3 center = collider.center;
        Vector3 extents = collider.size * 0.5f;
        float leftEdge = center.x - extents.x;
        float rightEdge = center.x + extents.x;
        float bottomEdge = center.y - extents.y;
        float topEdge = center.y + extents.y;
        float frontEdge = center.z - extents.z;
        float backEdge = center.z + extents.z;

        // Allocate the array if needed.
        const int numPoints = 8;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        // Transform all the local points to world space.
        Transform boxColliderTransform = collider.transform;
        positions[BoxColliderExtensions.LeftBottomFront] = boxColliderTransform.TransformPoint(new Vector3(leftEdge, bottomEdge, frontEdge));
        positions[BoxColliderExtensions.LeftBottomBack] = boxColliderTransform.TransformPoint(new Vector3(leftEdge, bottomEdge, backEdge));
        positions[BoxColliderExtensions.LeftTopFront] = boxColliderTransform.TransformPoint(new Vector3(leftEdge, topEdge, frontEdge));
        positions[BoxColliderExtensions.LeftTopBack] = boxColliderTransform.TransformPoint(new Vector3(leftEdge, topEdge, backEdge));
        positions[BoxColliderExtensions.RightBottomFront] = boxColliderTransform.TransformPoint(new Vector3(rightEdge, bottomEdge, frontEdge));
        positions[BoxColliderExtensions.RightBottomBack] = boxColliderTransform.TransformPoint(new Vector3(rightEdge, bottomEdge, backEdge));
        positions[BoxColliderExtensions.RightTopFront] = boxColliderTransform.TransformPoint(new Vector3(rightEdge, topEdge, frontEdge));
        positions[BoxColliderExtensions.RightTopBack] = boxColliderTransform.TransformPoint(new Vector3(rightEdge, topEdge, backEdge));
    }
}
