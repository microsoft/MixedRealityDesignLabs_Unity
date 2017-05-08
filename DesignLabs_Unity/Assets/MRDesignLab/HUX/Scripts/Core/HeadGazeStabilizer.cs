//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;

public sealed class HeadGazeStabilizer : MonoBehaviour
{
    public Vector3 StableHeadPosition { get; private set; }
    public Quaternion StableHeadRotation { get; private set; }
    public Ray StableHeadRay { get; private set; }

    public int StoredStabilitySamples = 60;
    public float PositionDropOffRadius = 0.02f;
    public float DirectionDropOffRadius = 0.1f;
    public float PositionStrength = 0.66f;
    public float DirectionStrength = 0.83f;
    public float StabilityAverageDistanceWeight = 2.0f;
    public float StabilityVarianceWeight = 1.0f;

    public struct GazeSample
    {
        public Vector3 position;
        public Vector3 direction;
        public float timestamp;
    };

    private List<GazeSample> stabilitySamples;

    private Vector3 gazePosition;
    private Vector3 gazeDirection;

    // Most recent calculated instability values
    private float gazePositionInstability;
    private float gazeDirectionInstability;

    private bool gravityPointExists = false;
    private Vector3 gravityWellPosition;
    private Vector3 gravityWellDirection;

    // Transforms instability value into a modified drop off distance, modify with caution
    private float positionDestabilizationFactor = 0.02f; 
    private float directionDestabilizationFactor = 0.3f;

    public void Awake()
    {
         stabilitySamples = new List<GazeSample>(StoredStabilitySamples);
    }

    public void UpdateHeadStability(Vector3 position, Quaternion rotation)
    {
        gazePosition = position;
        gazeDirection = rotation * Vector3.forward;

        AddGazeSample(gazePosition, gazeDirection);
        UpdateInstability();

        // If we don't have a gravity point, just use the gaze position.
        if (!gravityPointExists)
        {
            gravityWellPosition = gazePosition;
            gravityWellDirection = gazeDirection;
            gravityPointExists = true;
        }

        UpdateGravityWellPositionDirection();
    }
    
    private void AddGazeSample(Vector3 positionSample, Vector3 directionSample)
    {
        // Record sample data
        GazeSample newStabilitySample;
        newStabilitySample.position = positionSample;
        newStabilitySample.direction = directionSample;
        newStabilitySample.timestamp = Time.time;

        // Strip from front if we exceed stored samples
        if (stabilitySamples.Count >= StoredStabilitySamples)
        {
            stabilitySamples.RemoveAt(0);
        }

        stabilitySamples.Add(newStabilitySample);
    }

    private void CalculateInstability(int sampleCount, out float positionInstability, out float directionInstability)
    {
        GazeSample mostRecentSample;

        float positionDeltaMin = 0.0f;
        float positionDeltaMax = 0.0f;
        float positionDeltaMean = 0.0f;

        float directionDeltaMin = 0.0f;
        float directionDeltaMax = 0.0f;
        float directionDeltaMean = 0.0f;

        float positionDelta;
        float directionDelta;

        // If we've got no samples, nothing to update
        if (stabilitySamples.Count <= 0)
        {
            positionInstability = 0.0f;
            directionInstability = 0.0f;

            return;
        }

        mostRecentSample = stabilitySamples[stabilitySamples.Count - 1];

        // Assert.IsTrue(sampleCount <= StoredStabilitySamples, "Requested instability sample count greater than number of stored samples");
        sampleCount = Mathf.Clamp(sampleCount, 0, stabilitySamples.Count);

        // If we only wants one sample, there can be no instability
        if (sampleCount <= 1)
        {
            positionInstability = 0.0f;
            directionInstability = 0.0f;
        }
        else
        {
            // All but most recent
            for (int i = stabilitySamples.Count - sampleCount; i < stabilitySamples.Count - 1; ++i) 
            {
                // Calculate difference between current sample and most recent sample
                positionDelta = Vector3.Magnitude(stabilitySamples[i].position - mostRecentSample.position);
                // The unity Vector3 dot product can occasionally return values slightly > 1.0, due to floating point error.
                // Vector3.Angle has a clamp built in, so let's use that instead.
                directionDelta = Vector3.Angle(stabilitySamples[i].direction, mostRecentSample.direction) * Mathf.Deg2Rad;

                // Initialize max and min on first sample
                if (i == stabilitySamples.Count - sampleCount)
                {
                    positionDeltaMin = positionDelta;
                    positionDeltaMax = positionDelta;
                    directionDeltaMin = directionDelta;
                    directionDeltaMax = directionDelta;
                }
                else
                {
                    // Update maximum, minimum and mean differences from most recent sample
                    positionDeltaMin = Mathf.Min(positionDelta, positionDeltaMin);
                    positionDeltaMax = Mathf.Max(positionDelta, positionDeltaMax);

                    directionDeltaMin = Mathf.Min(directionDelta, directionDeltaMin);
                    directionDeltaMax = Mathf.Max(directionDelta, directionDeltaMax);

                    positionDeltaMean = ((positionDeltaMean * i) + positionDelta) / (i + 1);
                    directionDeltaMean = ((directionDeltaMean * i) + directionDelta) / (i + 1);
                }
            }
        }

        // Calculate stability value for Gaze position and direction.  Note that stability values will be significantly different for position and
        // direction since the position value is based on values in CM while the direction stability is based on data in Radians
        positionInstability = StabilityVarianceWeight * (positionDeltaMax - positionDeltaMin) + StabilityAverageDistanceWeight * positionDeltaMean;
        directionInstability = StabilityVarianceWeight * (directionDeltaMax - directionDeltaMin) + StabilityAverageDistanceWeight * directionDeltaMean;
    }

    private void UpdateInstability()
    {
        CalculateInstability(stabilitySamples.Count, out gazePositionInstability, out gazeDirectionInstability);
    }

    private void UpdateGravityWellPositionDirection()
    {
        float stabilityModifiedPositionDropOffDistance;
        float stabilityModifiedDirectionDropOffDistance;
        float normalizedGazeToGravityWellPosition;
        float normalizedGazeToGravityWellDirection;

        // modify effective size of well based on gaze stability
        stabilityModifiedPositionDropOffDistance = Mathf.Max(0.0f, PositionDropOffRadius - (gazePositionInstability * positionDestabilizationFactor));
        stabilityModifiedDirectionDropOffDistance = Mathf.Max(0.0f, DirectionDropOffRadius - (gazeDirectionInstability * directionDestabilizationFactor));

        // Determine how far away from the well the gaze is, if that distance is zero push the normalized value above 1.0 to
        // force a gravity well position update
        normalizedGazeToGravityWellPosition = 2.0f;
        if (stabilityModifiedPositionDropOffDistance > 0.0f)
        {
            normalizedGazeToGravityWellPosition = Vector3.Magnitude(gravityWellPosition - gazePosition) / stabilityModifiedPositionDropOffDistance;
        }
    
        normalizedGazeToGravityWellDirection = 2.0f;
        if (stabilityModifiedDirectionDropOffDistance > 0.0f)
        {
            normalizedGazeToGravityWellDirection = Mathf.Acos(Vector3.Dot(gravityWellDirection, gazeDirection)) / stabilityModifiedDirectionDropOffDistance;
        }
    
        // Move gravity well with Gaze if necessary
        if (normalizedGazeToGravityWellPosition > 1.0f)
        {
            gravityWellPosition = gazePosition - Vector3.Normalize(gazePosition - gravityWellPosition) * stabilityModifiedPositionDropOffDistance;
        }

        if (normalizedGazeToGravityWellDirection > 1.0f)
        {
            gravityWellDirection = Vector3.Normalize(gazeDirection - Vector3.Normalize(gazeDirection - gravityWellDirection) * stabilityModifiedDirectionDropOffDistance);
        }

        // Adjust direction and position towards gravity well based on configurable strengths
        StableHeadPosition = Vector3.Lerp(gazePosition, gravityWellPosition, PositionStrength);
        StableHeadRotation = Quaternion.LookRotation(Vector3.Lerp(gazeDirection, gravityWellDirection, DirectionStrength));
        StableHeadRay = new Ray(StableHeadPosition, StableHeadRotation * Vector3.forward);
    }

    public float GetGazeDirectionStability()
    {
        return Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp(gazeDirectionInstability * directionDestabilizationFactor / DirectionDropOffRadius, 0.0f, 1.0f));
    }

    public float GetGazePositionStability()
    {
        return Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp(gazePositionInstability * positionDestabilizationFactor / PositionDropOffRadius, 0.0f, 1.0f));
    }

    public float GetCustomGazeDirectionStability(int sampleCount, float dropOffRadius)
    {
        float positionInstability, directionInstability;

        CalculateInstability(sampleCount, out positionInstability, out directionInstability);
        return Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp(directionInstability * directionDestabilizationFactor / dropOffRadius, 0.0f, 1.0f));
    }

    public float GetCustomGazePositionStability(int sampleCount, float dropOffRadius)
    {
        float positionInstability, directionInstability;

        CalculateInstability(sampleCount, out positionInstability, out directionInstability);
        return Mathf.Lerp(1.0f, 0.0f, Mathf.Clamp(positionInstability * positionDestabilizationFactor / dropOffRadius, 0.0f, 1.0f));
    }
}
