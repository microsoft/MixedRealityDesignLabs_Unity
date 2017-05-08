//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Spatial
{
    public class SolverHandler : MonoBehaviour
    {
        private List<Solver> m_Solvers = new List<Solver>();

        public Vector3 GoalPosition { get; set; }
        public Quaternion GoalRotation { get; set; }
        public Vector3 GoalScale { get; set; }
        public Vector3Smoothed AltScale { get; set; }
        public float DeltaTime { get; set; }

        private float m_LastUpdateTime { get; set; }

        private void Awake()
        {
            m_Solvers.AddRange(GetComponents<Solver>());

            GoalScale = Vector3.one;
            AltScale = new Vector3Smoothed(Vector3.one, 0.1f);
            DeltaTime = 0.0f;
        }

        private void Update()
        {
            DeltaTime = Time.realtimeSinceStartup - m_LastUpdateTime;
            m_LastUpdateTime = Time.realtimeSinceStartup;
        }

        private void LateUpdate()
        {
            for (int i = 0; i < m_Solvers.Count; ++i)
            {
                Solver solver = m_Solvers[i];

                if (solver.enabled)
                {
                    solver.SolverUpdate();
                }
            }
        }
    }

    [System.Serializable]
    public struct Vector3Smoothed
    {
        public Vector3 Current { get; set; }
        public Vector3 Goal { get; set; }
        public float SmoothTime { get; set; }

        public Vector3Smoothed(Vector3 value, float smoothingTime)
        {
            Current = value;
            Goal = value;
            SmoothTime = smoothingTime;
        }

        public void Update(float deltaTime)
        {
            Current = Vector3.Lerp(Current, Goal, (System.Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
        }

        public void SetGoal(Vector3 newGoal)
        {
            Goal = newGoal;
        }
    }

    [System.Serializable]
    public struct QuaternionSmoothed
    {
        public Quaternion Current { get; set; }
        public Quaternion Goal { get; set; }
        public float SmoothTime { get; set; }

        public QuaternionSmoothed(Quaternion value, float smoothingTime)
        {
            Current = value;
            Goal = value;
            SmoothTime = smoothingTime;
        }

        public void Update(float deltaTime)
        {
            Current = Quaternion.Slerp(Current, Goal, (System.Math.Abs(SmoothTime) < Mathf.Epsilon) ? 1.0f : deltaTime / SmoothTime);
        }

        public void SetGoal(Quaternion newGoal)
        {
            Goal = newGoal;
        }
    }
}