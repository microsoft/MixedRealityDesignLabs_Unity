//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Text;

public static class TransformExtensions
{
    /// <summary>
    /// An extension method that will get you the full path to an object.
    /// </summary>
    /// <param name="transform">The transform you wish a full path to.</param>
    /// <returns>A dot delimited string that is the full path to the game object in the hierarchy.</returns>
    public static string GetFullPath(this Transform transform)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (transform.parent == null)
        {
            stringBuilder.Append("/");
            stringBuilder.Append(transform.name);

        }
        else
        {
            stringBuilder.Append(transform.parent.GetFullPath());
            stringBuilder.Append(".");
            stringBuilder.Append(transform.name);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    ///  Reparents the given transform to a new parent and retains its local position, rotation, and scale.
    /// </summary>
    /// <param name="moving"></param>
    /// <param name="newParent"></param>
    public static void ReparentWithSameLocalTransform(this Transform moving, Transform newParent)
    {
        Vector3 localPosition = moving.localPosition;
        Quaternion localRotation = moving.localRotation;
        Vector3 localScale = moving.localScale;
        moving.parent = newParent;
        moving.localPosition = localPosition;
        moving.localRotation = localRotation;
        moving.localScale = localScale;
    }

    /// <summary>
    /// Returns the relative lossy scale between a child object and its ancestor.
    /// </summary>
    /// <remarks>
    /// Note that this will return inaccurate values for objects that are arbirtarily rotated
    /// relative to each other. See notes on Transform.lossyScale.
    /// </remarks>
    /// <param name="child"></param>
    /// <param name="ancestor"></param>
    public static Vector3 GetRelativeLossyScale(this Transform child, Transform ancestor)
    {
        Vector3 parentLossyScale = ancestor.lossyScale;
        Vector3 childLossyScale = child.lossyScale;
        return new Vector3(
            childLossyScale.x / parentLossyScale.x,
            childLossyScale.y / parentLossyScale.y,
            childLossyScale.z / parentLossyScale.z);
    }

    // Sets the parent of a transform, propagating position, rotation, and scale from the parent
    // to the child.
    public static void SetParentClearRelativeTransform(this Transform transform, Transform parent)
    {
        transform.SetParentClearRelativeTransform(parent, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    public static void SetParentClearRelativeTransform(this Transform transform, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        transform.parent = parent;
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
        transform.localScale = localScale;
    }

    /// <summary>
    /// Transforms a position from one local space to another.
    /// </summary>
    /// <param name="transform">The source local space.</param>
    /// <param name="position">The poistion to transform.</param>
    /// <param name="targetSpace">The target local space.</param>
    /// <returns>The transformed position.</returns>
    public static Vector3 TransformPoint(this Transform transform, Vector3 position, Transform targetSpace)
    {
        if (transform == null)
        {
            throw new ArgumentNullException("transform");
        }

        if (targetSpace == null)
        {
            throw new ArgumentNullException("targetSpace");
        }

        Vector3 worldPosition = transform.TransformPoint(position);
        return targetSpace.InverseTransformPoint(worldPosition);
    }

    public static Vector3 GetForwardDirection(Quaternion rotation)
    {
        return rotation * Vector3.forward;
    }

    /// <summary>
    /// Returns whether an odd number of the components
    /// of the transform or its parents' scales are negative.
    /// </summary>
    public static bool IsScaleNegative(this Transform transform)
    {
        Vector3 localScale = transform.localScale;
        bool negativeScale = (localScale.x * localScale.y * localScale.z < 0.0f ? true : false);

        // Traverse upwards through the hierarchy to check the parent scales
        if (transform.parent != null && IsScaleNegative(transform.parent))
        {
            negativeScale = !negativeScale;
        }

        return negativeScale;
    }
}
