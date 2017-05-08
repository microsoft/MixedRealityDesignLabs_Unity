//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class MathUtilities
{
    public static float ExpDecay(float from, float to, float hLife, float dTime)
    {
        return Mathf.Lerp(from, to, ExpCoefficient(hLife, dTime));
    }

    public static Vector2 ExpDecay(Vector2 from, Vector2 to, float hLife, float dTime)
    {
        return Vector2.Lerp(from, to, ExpCoefficient(hLife, dTime));
    }

    public static Vector3 ExpDecay(Vector3 from, Vector3 to, float hLife, float dTime)
    {
        return Vector3.Lerp(from, to, ExpCoefficient(hLife, dTime));
    }

    public static Quaternion ExpDecay(Quaternion from, Quaternion to, float hLife, float dTime)
    {
        return Quaternion.Slerp(from, to, ExpCoefficient(hLife, dTime));
    }

    public static Color ExpDecay(Color from, Color to, float hLife, float dTime)
    {
        return Color.Lerp(from, to, ExpCoefficient(hLife, dTime));
    }


    public static float ExpCoefficient(float hLife, float dTime)
    {
        if (hLife == 0)
            return 1;

        return 1.0f - Mathf.Pow(0.5f, dTime / hLife);
    }

    public static float DynamicExpCoefficient(float hLife, float delta)
    {
        if (hLife == 0)
            return 1;

        return 1.0f - Mathf.Pow(0.5f, delta / hLife);
    }

    public static float DynamicExpDecay(float from, float to, float hLife)
    {
        return Mathf.Lerp(from, to, DynamicExpCoefficient(hLife, Mathf.Abs(to - from)));
    }

    public static Vector2 DynamicExpDecay(Vector2 from, Vector2 to, float hLife)
    {
        return Vector2.Lerp(from, to, DynamicExpCoefficient(hLife, Vector3.Distance(to, from)));
    }

    public static Vector3 DynamicExpDecay(Vector3 from, Vector3 to, float hLife)
    {
        return Vector3.Lerp(from, to, DynamicExpCoefficient(hLife, Vector3.Distance(to, from)));
    }

    public static Quaternion DynamicExpDecay(Quaternion from, Quaternion to, float hLife)
    {
        return Quaternion.Slerp(from, to, DynamicExpCoefficient(hLife, Quaternion.Angle(to, from)));
    }
}
