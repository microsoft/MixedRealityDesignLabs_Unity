//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

public static class OrientedBoundingBoxExtensions
{
    private enum Axis
    {
        X,
        Y,
        Z,
        Count
    }

    public static bool Equals(this OrientedBoundingBox thisBox, OrientedBoundingBox otherBox)
    {
        return thisBox.Center == otherBox.Center && thisBox.Extents == otherBox.Extents && thisBox.Rotation == otherBox.Rotation;
    }

    public static bool Contains(this OrientedBoundingBox thisBox, Vector3 pos)
    {
        Vector3 adjustedPos = Quaternion.Inverse(thisBox.Rotation) * (pos - thisBox.Center);

        return adjustedPos.x <= thisBox.Extents.x && adjustedPos.x >= -thisBox.Extents.x
            && adjustedPos.y <= thisBox.Extents.y && adjustedPos.y >= -thisBox.Extents.y
            && adjustedPos.z <= thisBox.Extents.z && adjustedPos.z >= -thisBox.Extents.z;
    }

    public static bool Intersects(this OrientedBoundingBox thisBox, Bounds AABB)
    {
        return Intersects(thisBox, AABB.center, AABB.extents, Quaternion.identity);
    }

    public static bool Intersects(this OrientedBoundingBox thisBox, OrientedBoundingBox otherBounds)
    {
        return Intersects(thisBox, otherBounds.Center, otherBounds.Extents, otherBounds.Rotation);
    }

    private static bool Intersects(this OrientedBoundingBox thisBox, Vector3 otherCenter, Vector3 otherExtents, Quaternion otherRot)
    {
        Vector3[] otherAxis = new Vector3[(int)Axis.Count];
        otherAxis[(int)Axis.X] = otherRot * Vector3.right;
        otherAxis[(int)Axis.Y] = otherRot * Vector3.up;
        otherAxis[(int)Axis.Z] = otherRot * Vector3.forward;

        Vector3[] otherSizeAxis = new Vector3[(int)Axis.Count];
        otherSizeAxis[(int)Axis.X] = otherAxis[(int)Axis.X] * otherExtents.x;
        otherSizeAxis[(int)Axis.Y] = otherAxis[(int)Axis.Y] * otherExtents.x;
        otherSizeAxis[(int)Axis.Z] = otherAxis[(int)Axis.Z] * otherExtents.x;

        Vector3[] ourAxis = new Vector3[(int)Axis.Count];
        ourAxis[(int)Axis.X] = thisBox.Rotation * Vector3.right;
        ourAxis[(int)Axis.Y] = thisBox.Rotation * Vector3.up;
        ourAxis[(int)Axis.Z] = thisBox.Rotation * Vector3.forward;

        Vector3[] sizeAxis = new Vector3[(int)Axis.Count];
        sizeAxis[(int)Axis.X] = ourAxis[(int)Axis.X] * thisBox.Extents.x;
        sizeAxis[(int)Axis.Y] = ourAxis[(int)Axis.Y] * thisBox.Extents.x;
        sizeAxis[(int)Axis.Z] = ourAxis[(int)Axis.Z] * thisBox.Extents.x;

        Vector3[] axis = new Vector3[15];

        axis[0] = ourAxis[(int)Axis.X];
        axis[1] = ourAxis[(int)Axis.Y];
        axis[2] = ourAxis[(int)Axis.Z];

        axis[3] = ourAxis[(int)Axis.X];
        axis[4] = ourAxis[(int)Axis.Y];
        axis[5] = ourAxis[(int)Axis.Z];

        axis[6] = Vector3.Cross(ourAxis[(int)Axis.X], otherAxis[(int)Axis.X]);
        axis[7] = Vector3.Cross(ourAxis[(int)Axis.X], otherAxis[(int)Axis.Y]);
        axis[8] = Vector3.Cross(ourAxis[(int)Axis.X], otherAxis[(int)Axis.Z]);

        axis[9] = Vector3.Cross(ourAxis[(int)Axis.Y], otherAxis[(int)Axis.X]);
        axis[10] = Vector3.Cross(ourAxis[(int)Axis.Y], otherAxis[(int)Axis.Y]);
        axis[11] = Vector3.Cross(ourAxis[(int)Axis.Y], otherAxis[(int)Axis.Z]);

        axis[12] = Vector3.Cross(ourAxis[(int)Axis.Z], otherAxis[(int)Axis.X]);
        axis[13] = Vector3.Cross(ourAxis[(int)Axis.Z], otherAxis[(int)Axis.Y]);
        axis[14] = Vector3.Cross(ourAxis[(int)Axis.Z], otherAxis[(int)Axis.Z]);

        Vector3 distBetween = otherCenter - thisBox.Center;

        bool intersects = true;
        for (int index = 0; index < axis.Length && intersects; index++)
        {
            float distProj = Mathf.Abs(Vector3.Dot(distBetween, axis[index]));

            float ourXProj = Mathf.Abs(Vector3.Dot(sizeAxis[(int)Axis.X], axis[index]));
            float ourYProj = Mathf.Abs(Vector3.Dot(sizeAxis[(int)Axis.Y], axis[index]));
            float ourZProj = Mathf.Abs(Vector3.Dot(sizeAxis[(int)Axis.Z], axis[index]));

            float otherXProj = Mathf.Abs(Vector3.Dot(otherSizeAxis[(int)Axis.X], axis[index]));
            float otherYProj = Mathf.Abs(Vector3.Dot(otherSizeAxis[(int)Axis.Y], axis[index]));
            float otherZProj = Mathf.Abs(Vector3.Dot(otherSizeAxis[(int)Axis.Z], axis[index]));

            intersects = distProj <= ourXProj + ourYProj + ourZProj + otherXProj + otherYProj + otherZProj;

        }

        return intersects;
    }

    public static void Encapsulate(this OrientedBoundingBox thisBox, Vector3 pos)
    {
        if (!Contains(thisBox, pos))
        {
            Vector3 adjustedPos = (pos - thisBox.Center);

            Vector3 newCenter = thisBox.Center;
            Vector3 newExtents = thisBox.Extents;

            float diff;
            if (adjustedPos.x > thisBox.Extents.x)
            {
                diff = adjustedPos.x - thisBox.Extents.x;
                newExtents.x += diff / 2;
                newCenter.x += diff / 2;

            }
            else if (adjustedPos.x < -thisBox.Extents.x)
            {
                diff = Mathf.Abs(-thisBox.Extents.x - adjustedPos.x);
                newExtents.x += diff / 2;
                newCenter.x -= diff / 2;
            }

            if (adjustedPos.y > thisBox.Extents.y)
            {
                diff = adjustedPos.y - thisBox.Extents.y;
                newExtents.y += diff / 2;
                newCenter.y += diff / 2;

            }
            else if (adjustedPos.y < -thisBox.Extents.y)
            {
                diff = Mathf.Abs(-thisBox.Extents.y - adjustedPos.y);
                newExtents.y += diff / 2;
                newCenter.y -= diff / 2;
            }

            if (adjustedPos.z > thisBox.Extents.z)
            {
                diff = adjustedPos.z - thisBox.Extents.z;
                newExtents.z += diff / 2;
                newCenter.z += diff / 2;

            }
            else if (adjustedPos.z < -thisBox.Extents.z)
            {
                diff = Mathf.Abs(-thisBox.Extents.z - adjustedPos.z);
                newExtents.z += diff / 2;
                newCenter.z -= diff / 2;
            }

            thisBox.Center = newCenter;
            thisBox.Extents = newExtents;
        }
    }

    public static void Encapsulate(this OrientedBoundingBox thisBox, OrientedBoundingBox otherBox)
    {
        Vector3 ftl = otherBox.Center + (otherBox.Rotation * new Vector3(-otherBox.Extents.x, otherBox.Extents.y, otherBox.Extents.z));
        Vector3 ftr = otherBox.Center + (otherBox.Rotation * new Vector3(otherBox.Extents.x, otherBox.Extents.y, otherBox.Extents.z));
        Vector3 fbl = otherBox.Center + (otherBox.Rotation * new Vector3(-otherBox.Extents.x, -otherBox.Extents.y, otherBox.Extents.z));
        Vector3 fbr = otherBox.Center + (otherBox.Rotation * new Vector3(otherBox.Extents.x, -otherBox.Extents.y, otherBox.Extents.z));

        Vector3 btl = otherBox.Center + (otherBox.Rotation * new Vector3(-otherBox.Extents.x, otherBox.Extents.y, -otherBox.Extents.z));
        Vector3 btr = otherBox.Center + (otherBox.Rotation * new Vector3(otherBox.Extents.x, otherBox.Extents.y, -otherBox.Extents.z));
        Vector3 bbl = otherBox.Center + (otherBox.Rotation * new Vector3(-otherBox.Extents.x, -otherBox.Extents.y, -otherBox.Extents.z));
        Vector3 bbr = otherBox.Center + (otherBox.Rotation * new Vector3(otherBox.Extents.x, -otherBox.Extents.y, -otherBox.Extents.z));

        Encapsulate(thisBox, ftl);
        Encapsulate(thisBox, ftr);
        Encapsulate(thisBox, fbl);
        Encapsulate(thisBox, fbr);

        Encapsulate(thisBox, btl);
        Encapsulate(thisBox, btr);
        Encapsulate(thisBox, bbl);
        Encapsulate(thisBox, bbr);
    }
};