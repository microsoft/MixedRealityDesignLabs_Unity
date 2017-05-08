//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

[System.Serializable]
public class ScrollMomentumPhysics
{
    public Vector3 scrollBounce = new Vector3(-0.1f, -0.1f, -0.05f);
    public Vector3 scrollLimitMin = new Vector3(0f, 0f, 0.5f);
    public Vector3 scrollLimitMax = new Vector3(0f, 0.9f, 1f);

    public Vector3 scrollPos;
    Vector3 accruedDelta;
    public Vector3 scrollVel;
    public Vector3 scrollFrictionVel = new Vector3(0.4f, 0.4f, 1.5f);
    public float scrollFrictionConst = 0.2f;

    public float avgVelRate = 0.1f;
    public Vector3 avgScrollVel;

    public Vector3 scrollPadSize;
    public Vector3 scrollScale = Vector3.one;

    public bool separateZ = true;

    public bool bDirectManipulating;
    bool wasDirectionManipulating;

    public void ApplyScrollDelta(Vector3 delta, bool noMomentum = false)
    {
        scrollPos += delta;
        if (noMomentum)
        {
            delta.x = 0;
            delta.y = 0;
        }
        accruedDelta += delta;
    }

    // Checks if pos exceeds minMax, and limits/bounces.  Adjusted is passed in to account for zoom (althought that needs work i think)
    void BouncePosVel(ref float pos, ref float vel, float bounce, Vector2 minMax, float scale, float padSize)
    {
        // If x < y, no clamping is performed
        if (minMax.x <= minMax.y)
        {
            minMax = minMax * scale;

            // Adjust minMax by 'adjust', to help account for viewable region
            minMax.y = Mathf.Max(minMax.x, minMax.y - 2f * padSize);

            float inPos = pos;
            pos = Mathf.Clamp(pos, minMax.x, minMax.y);
            if (inPos != pos)
            {
                vel *= bounce;
            }
        }
    }

    public void UpdateSim()
    {
        // Apply velocity if not manipulating
        if (!bDirectManipulating)
        {
            // Apply momentum if just finished manipulating
            if (bDirectManipulating != wasDirectionManipulating)
            {
                scrollVel += avgScrollVel;
            }

            float dt = Time.deltaTime;

            // Per-axis friction
            scrollVel -= Vector3.Scale(scrollVel, scrollFrictionVel) * dt;

            // Directional friction
            float magThresh = Mathf.Max(0.001f, scrollFrictionConst * dt);

            // If doing Z separate, start out with XY
            Vector3 vel = scrollVel;
            if (separateZ)
            {
                vel.z = 0;
            }

            // Constant directional friction
            if (vel.sqrMagnitude > magThresh * magThresh)
            {
                vel -= vel.normalized * scrollFrictionConst * dt;
            }
            else
            {
                vel = Vector3.zero;
            }

            // Z friction
            if (separateZ)
            {
                vel.z = scrollVel.z;
                if (vel.z * vel.z > magThresh * magThresh)
                {
                    vel.z -= vel.z * scrollFrictionConst * dt;
                }
                else
                {
                    vel.z = 0;
                }
            }

            scrollPos += vel * dt;
        }
        else
        {
            scrollVel = Vector3.zero;

            // If just started manipulating, initialize
            if (bDirectManipulating != wasDirectionManipulating)
            {
                avgScrollVel = Vector3.zero;
                accruedDelta = Vector3.zero;
            }
            else
            {
                // Use accrued delta as velocity calculation
                //Vector3 posDelta = scrollPos - lastScrollPos;
                Vector3 posDelta = accruedDelta;
                avgScrollVel = Vector3.Lerp(avgScrollVel, posDelta / Time.deltaTime, avgVelRate);

                accruedDelta = Vector3.zero;
            }
        }

        wasDirectionManipulating = bDirectManipulating;

        // Apply limit and bounce
        BouncePosVel(ref scrollPos.x, ref scrollVel.x, scrollBounce.x, new Vector2(scrollLimitMin.x, scrollLimitMax.x), scrollScale.x, scrollPadSize.x);
        BouncePosVel(ref scrollPos.y, ref scrollVel.y, scrollBounce.y, new Vector2(scrollLimitMin.y, scrollLimitMax.y), scrollScale.y, scrollPadSize.y);
        BouncePosVel(ref scrollPos.z, ref scrollVel.z, scrollBounce.z, new Vector2(scrollLimitMin.z, scrollLimitMax.z), scrollScale.z, scrollPadSize.z);
    }

    public void ApplyScrollDeltaWithMomentum(Vector3 delta)
    {
        scrollPos += delta;
        /*Vector3 newVel = Vector3.Lerp(scrollVel, delta / Time.deltaTime, 0.75f);

        delta.Normalize();
        scrollVel.x = Mathf.Lerp(scrollVel.x, newVel.x, Mathf.Abs(delta.x));
        scrollVel.y = Mathf.Lerp(scrollVel.y, newVel.y, Mathf.Abs(delta.y));
        scrollVel.z = Mathf.Lerp(scrollVel.z, newVel.z, Mathf.Abs(delta.z));*/
    }

    public void UpdateSimZ()
    {
        BouncePosVel(ref scrollPos.z, ref scrollVel.z, scrollBounce.z, new Vector2(scrollLimitMin.z, scrollLimitMax.z), scrollScale.z, scrollPadSize.z);
    }
}
