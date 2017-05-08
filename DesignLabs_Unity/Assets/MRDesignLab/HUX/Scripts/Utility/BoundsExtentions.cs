//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public static class BoundsExtentions
{
    // Corners
    public const int LBF = 0;
    public const int LBB = 1;
    public const int LTF = 2;
    public const int LTB = 3;
    public const int RBF = 4;
    public const int RBB = 5;
    public const int RTF = 6;
    public const int RTB = 7;

    // X axis
    public const int LTF_RTF = 8;
    public const int LBF_RBF = 9;
    public const int RTB_LTB = 10;
    public const int RBB_LBB = 11;

    // Y axis
    public const int LTF_LBF = 12;
    public const int RTB_RBB = 13;
    public const int LTB_LBB = 14;
    public const int RTF_RBF = 15;

    // Z axis
    public const int RBF_RBB = 16;
    public const int RTF_RTB = 17;
    public const int LBF_LBB = 18;
    public const int LTF_LTB = 19;    

    #region Public Static Functions
    /// <summary>
    /// Returns an instance of the 'Bounds' class which is invalid. An invalid 'Bounds' instance 
    /// is one which has its size vector set to 'float.MaxValue' for all 3 components. The center
    /// of an invalid bounds instance is the zero vector.
    /// </summary>
    public static Bounds GetInvalidBoundsInstance()
    {
        return new Bounds(Vector3.zero, GetInvalidBoundsSize());
    }

    /// <summary>
    /// Checks if the specified bounds instance is valid. A valid 'Bounds' instance is
    /// one whose size vector does not have all 3 components set to 'float.MaxValue'.
    /// </summary>
    public static bool IsValid(this Bounds bounds)
    {
        return bounds.size != GetInvalidBoundsSize();
    }

    /// <summary>
    /// Gets all the corner points of the bounds in world space
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="positions"></param>
    /// <remarks>
    /// Use BoxColliderExtensions.{Left|Right}{Bottom|Top}{Front|Back} consts to index into the output
    /// corners array.
    /// </remarks>
    public static void GetCornerPositions(this Bounds bounds, Transform transform, ref Vector3[] positions)
    {
        // Calculate the local points to transform.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
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
        positions[BoundsExtentions.LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
        positions[BoundsExtentions.LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
        positions[BoundsExtentions.LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
        positions[BoundsExtentions.LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
        positions[BoundsExtentions.RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
        positions[BoundsExtentions.RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
        positions[BoundsExtentions.RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
        positions[BoundsExtentions.RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);
    }

    public static void GetCornerAndMidPointPositions (this Bounds bounds, Transform transform, ref Vector3[] positions)
    {
        // Calculate the local points to transform.
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;
        float leftEdge = center.x - extents.x;
        float rightEdge = center.x + extents.x;
        float bottomEdge = center.y - extents.y;
        float topEdge = center.y + extents.y;
        float frontEdge = center.z - extents.z;
        float backEdge = center.z + extents.z;

        // Allocate the array if needed.
        const int numPoints = BoundsExtentions.LTF_LTB + 1;
        if (positions == null || positions.Length != numPoints)
        {
            positions = new Vector3[numPoints];
        }

        // Transform all the local points to world space.
        positions[BoundsExtentions.LBF] = transform.TransformPoint(leftEdge, bottomEdge, frontEdge);
        positions[BoundsExtentions.LBB] = transform.TransformPoint(leftEdge, bottomEdge, backEdge);
        positions[BoundsExtentions.LTF] = transform.TransformPoint(leftEdge, topEdge, frontEdge);
        positions[BoundsExtentions.LTB] = transform.TransformPoint(leftEdge, topEdge, backEdge);
        positions[BoundsExtentions.RBF] = transform.TransformPoint(rightEdge, bottomEdge, frontEdge);
        positions[BoundsExtentions.RBB] = transform.TransformPoint(rightEdge, bottomEdge, backEdge);
        positions[BoundsExtentions.RTF] = transform.TransformPoint(rightEdge, topEdge, frontEdge);
        positions[BoundsExtentions.RTB] = transform.TransformPoint(rightEdge, topEdge, backEdge);

        positions[BoundsExtentions.LTF_RTF] = Vector3.Lerp(positions[BoundsExtentions.LTF], positions[BoundsExtentions.RTF], 0.5f);
        positions[BoundsExtentions.LBF_RBF] = Vector3.Lerp(positions[BoundsExtentions.LBF], positions[BoundsExtentions.RBF], 0.5f);
        positions[BoundsExtentions.RTB_LTB] = Vector3.Lerp(positions[BoundsExtentions.RTB], positions[BoundsExtentions.LTB], 0.5f);
        positions[BoundsExtentions.RBB_LBB] = Vector3.Lerp(positions[BoundsExtentions.RBB], positions[BoundsExtentions.LBB], 0.5f);

        positions[BoundsExtentions.LTF_LBF] = Vector3.Lerp(positions[BoundsExtentions.LTF], positions[BoundsExtentions.LBF], 0.5f);
        positions[BoundsExtentions.RTB_RBB] = Vector3.Lerp(positions[BoundsExtentions.RTB], positions[BoundsExtentions.RBB], 0.5f);
        positions[BoundsExtentions.LTB_LBB] = Vector3.Lerp(positions[BoundsExtentions.LTB], positions[BoundsExtentions.LBB], 0.5f);
        positions[BoundsExtentions.RTF_RBF] = Vector3.Lerp(positions[BoundsExtentions.RTF], positions[BoundsExtentions.RBF], 0.5f);

        positions[BoundsExtentions.RBF_RBB] = Vector3.Lerp(positions[BoundsExtentions.RBF], positions[BoundsExtentions.RBB], 0.5f);
        positions[BoundsExtentions.RTF_RTB] = Vector3.Lerp(positions[BoundsExtentions.RTF], positions[BoundsExtentions.RTB], 0.5f);
        positions[BoundsExtentions.LBF_LBB] = Vector3.Lerp(positions[BoundsExtentions.LBF], positions[BoundsExtentions.LBB], 0.5f);
        positions[BoundsExtentions.LTF_LTB] = Vector3.Lerp(positions[BoundsExtentions.LTF], positions[BoundsExtentions.LTB], 0.5f);
    }

    /// <summary>
    /// Transforms 'bounds' using the specified transform matrix.
    /// </summary>
    /// <remarks>
    /// Transforming a 'Bounds' instance means that the function will construct a new 'Bounds' 
    /// instance which has its center translated using the translation information stored in
    /// the specified matrix and its size adjusted to account for rotation and scale. The size
    /// of the new 'Bounds' instance will be calculated in such a way that it will contain the
    /// old 'Bounds'.
    /// </remarks>
    /// <param name="bounds">
    /// The 'Bounds' instance which must be transformed.
    /// </param>
    /// <param name="transformMatrix">
    /// The specified 'Bounds' instance will be transformed using this transform matrix. The function
    /// assumes that the matrix doesn't contain any projection or skew transformation.
    /// </param>
    /// <returns>
    /// The transformed 'Bounds' instance.
    /// </returns>
    public static Bounds Transform(this Bounds bounds, Matrix4x4 transformMatrix)
    {
        // We will need access to the right, up and look vector which are encoded inside the transform matrix
        Vector3 rightAxis = transformMatrix.GetColumn(0);
        Vector3 upAxis = transformMatrix.GetColumn(1);
        Vector3 lookAxis = transformMatrix.GetColumn(2);

        // We will 'imagine' that we want to rotate the bounds' extents vector using the rotation information
        // stored inside the specified transform matrix. We will need these when calculating the new size if
        // the transformed bounds.
        Vector3 rotatedExtentsRight = rightAxis * bounds.extents.x;
        Vector3 rotatedExtentsUp = upAxis * bounds.extents.y;
        Vector3 rotatedExtentsLook = lookAxis * bounds.extents.z;

        // Calculate the new bounds size along each axis. The size on each axis is calculated by summing up the 
        // corresponding vector component values of the rotated extents vectors. We multiply by 2 because we want
        // to get a size and curently we are working with extents which represent half the size.
        float newSizeX = (Mathf.Abs(rotatedExtentsRight.x) + Mathf.Abs(rotatedExtentsUp.x) + Mathf.Abs(rotatedExtentsLook.x)) * 2.0f;
        float newSizeY = (Mathf.Abs(rotatedExtentsRight.y) + Mathf.Abs(rotatedExtentsUp.y) + Mathf.Abs(rotatedExtentsLook.y)) * 2.0f;
        float newSizeZ = (Mathf.Abs(rotatedExtentsRight.z) + Mathf.Abs(rotatedExtentsUp.z) + Mathf.Abs(rotatedExtentsLook.z)) * 2.0f;

        // Construct the transformed 'Bounds' instance
        var transformedBounds = new Bounds();
        transformedBounds.center = transformMatrix.MultiplyPoint(bounds.center);
        transformedBounds.size = new Vector3(newSizeX, newSizeY, newSizeZ);

        // Return the instance to the caller
        return transformedBounds;
    }

    /// <summary>
    /// Returns the screen space corner points of the specified 'Bounds' instance.
    /// </summary>
    /// <param name="camera">
    /// The camera used for rendering to the screen. This is needed to perform the
    /// transformation to screen space.
    /// </param>
    public static Vector2[] GetScreenSpaceCornerPoints(this Bounds bounds, Camera camera)
    {
        Vector3 aabbCenter = bounds.center;
        Vector3 aabbExtents = bounds.extents;

        //  Return the screen space point array
        return new Vector2[]
        {
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z - aabbExtents.z)),

            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y - aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x + aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z)),
            camera.WorldToScreenPoint(new Vector3(aabbCenter.x - aabbExtents.x, aabbCenter.y + aabbExtents.y, aabbCenter.z + aabbExtents.z))
        };
    }

    /// <summary>
    /// Returns the rectangle which encloses the specifies 'Bounds' instance in screen space.
    /// </summary>
    public static Rect GetScreenRectangle(this Bounds bounds, Camera camera)
    {
        // Retrieve the bounds' corner points in screen space
        Vector2[] screenSpaceCornerPoints = bounds.GetScreenSpaceCornerPoints(camera);

        // Identify the minimum and maximum points in the array
        Vector3 minScreenPoint = screenSpaceCornerPoints[0], maxScreenPoint = screenSpaceCornerPoints[0];
        for (int screenPointIndex = 1; screenPointIndex < screenSpaceCornerPoints.Length; ++screenPointIndex)
        {
            minScreenPoint = Vector3.Min(minScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
            maxScreenPoint = Vector3.Max(maxScreenPoint, screenSpaceCornerPoints[screenPointIndex]);
        }

        // Return the screen space rectangle
        return new Rect(minScreenPoint.x, minScreenPoint.y, maxScreenPoint.x - minScreenPoint.x, maxScreenPoint.y - minScreenPoint.y);
    }

    /// <summary>
    /// Returns the volume of the bounds.
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public static float Volume(this Bounds bounds)
	{
		return bounds.size.x * bounds.size.y * bounds.size.z;
	}

	/// <summary>
	/// Returns bounds that contain both this bounds and the bounds passed in.
	/// </summary>
	/// <param name="originalBounds"></param>
	/// <param name="otherBounds"></param>
	/// <returns></returns>
	public static Bounds ExpandToContian(this Bounds originalBounds, Bounds otherBounds)
	{
		Bounds tmpBounds = originalBounds;

		//tmpBounds.Encapsulate(otherBounds.min);
		//tmpBounds.Encapsulate(otherBounds.max);

		tmpBounds.Encapsulate(otherBounds);

		return tmpBounds;
	}

	/// <summary>
	/// Checks to see if bounds contains the other bounds completely.
	/// </summary>
	/// <param name="bounds"></param>
	/// <param name="otherBounds"></param>
	/// <returns></returns>
	public static bool ContainsBounds(this Bounds bounds, Bounds otherBounds)
	{
		return bounds.Contains(otherBounds.min) && bounds.Contains(otherBounds.max);
	}

	public static bool CloserToPoint(this Bounds bounds, Vector3 point, Bounds otherBounds)
	{
		Vector3 distToClosestPoint1 = bounds.ClosestPoint(point) - point;
		Vector3 distToClosestPoint2 = otherBounds.ClosestPoint(point) - point;

		if (distToClosestPoint1.magnitude == distToClosestPoint2.magnitude)
		{
			Vector3 toCenter1 = point - bounds.center;
			Vector3 toCenter2 = point - otherBounds.center;
			return (toCenter1.magnitude <= toCenter2.magnitude);

		}
		else
		{
			return (distToClosestPoint1.magnitude <= distToClosestPoint2.magnitude);
		}
	}
    
    #endregion

    #region Private Static Functions
    /// <summary>
    /// Returns the vector which is used to represent and invalid bounds size.
    /// </summary>
    private static Vector3 GetInvalidBoundsSize()
    {
        return new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    }
    #endregion
}
