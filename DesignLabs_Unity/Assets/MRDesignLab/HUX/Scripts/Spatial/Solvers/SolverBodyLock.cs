//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HUX.Spatial
{
    [RequireComponent(typeof(SolverHandler))]

    public class SolverBodyLock : Solver
    {

        public Vector3 offset;
        public bool RotationTether = false;
        public float TetherAngleLimit = 20.0f;

        private Transform head;
        private Quaternion origRot;

        void Start()
        {
            head = Veil.Instance.HeadTransform;
        }

        void Update()
        {

            Vector3 newPos = head.position + offset;
            this.transform.position = newPos;
        }

        public override void SolverUpdate()
        {
            Vector3 desiredPos = head != null ? head.position + offset : Vector3.zero;
            Quaternion desiredRot = Quaternion.identity;

            //if (RotationTether)
            //{
            //    Vector3 scaleVec = new Vector3(1f, 0f, 1f);
            //    Vector3 headVec = Vector3.Scale(head.forward, scaleVec);
            //    Vector3 headRtVec = Vector3.Scale(head.right, scaleVec);
            //    Vector3 objVec = Vector3.Scale(this.transform.position - head.position, scaleVec);

            //    float angleDiff = Vector3.Angle(headVec, objVec);
            //    if (angleDiff > TetherAngleLimit)
            //    {
            //        float angleVal = Mathf.Lerp(0, TetherAngleLimit - angleDiff, Time.deltaTime);
            //        angleVal = Vector3.Dot(headRtVec, objVec) < 0 ? -angleVal : angleVal;
            //        Quaternion.RotateAround(head.position, new Vector3(0.0f, 1.0f, 0.0f), angleVal);
            //        newPos = head.position + this.transform.rotation * offset;
            //    }
            //}

            this.GoalPosition = desiredPos;
            this.GoalRotation = desiredRot;

            UpdateWorkingToGoal();
        }
    }
}