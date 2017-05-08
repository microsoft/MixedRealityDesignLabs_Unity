//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public static class BoundedPlaneExtensions
{
    public static float GetDistance(this BoundedPlane boundedPlane, Vector3 worldPos)
    {
        return Mathf.Abs((worldPos - GetClosestWorldPoint(boundedPlane, worldPos)).magnitude);
    }

    public static float GetSqrDistance(this BoundedPlane boundedPlane, Vector3 worldPos)
    {
        return Mathf.Abs((worldPos - GetClosestWorldPoint(boundedPlane, worldPos)).sqrMagnitude);
    }

    public static Vector3 GetClosestWorldPoint(this BoundedPlane boundedPlane, Vector3 worldPos)
    {
        float distanceToPlane = boundedPlane.Plane.GetDistanceToPoint(worldPos);
        Vector3 worldPosOnPlane = worldPos - boundedPlane.Plane.normal * distanceToPlane;

        Quaternion rotationToZero = Quaternion.Inverse(boundedPlane.Bounds.Rotation);
        Quaternion rotationBack = boundedPlane.Bounds.Rotation;

        worldPosOnPlane -= boundedPlane.Bounds.Center;

        worldPosOnPlane = rotationToZero * worldPosOnPlane;

        Bounds testBounds = new Bounds(Vector3.zero, boundedPlane.Bounds.Extents * 2);
        worldPosOnPlane = testBounds.ClosestPoint(worldPosOnPlane);

        worldPosOnPlane = rotationBack * worldPosOnPlane;
        worldPosOnPlane += boundedPlane.Bounds.Center;

        return worldPosOnPlane;
    }
}