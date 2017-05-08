//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Spatial
{
    [RequireComponent(typeof(SolverHandler))]

	/// <summary>
	///  EdgeMagnetism solver which magnetizes a tag-along object to certain 'nodes' around the view direction
	/// </summary>
    public class SolverEdgeMagnetism : Solver
    {
        #region public enums
        public enum ReferenceDirectionEnum
        {
            HeadFacing,
            HeadMoveDirection
        }
        #endregion

        #region public members
        public float Depth = 1.8f;
        public float XAngle = 15.0f;
        public float YAngle = 10.0f;
        public float Threshold = 5.0f;

        public float IncrementAmount = 0.25f;
        public TextMesh debugText;

        public bool ActiveInput = false;
        public bool DrawSpheres = false;

        [Tooltip("Which direction to position the element relative to")]
        public ReferenceDirectionEnum ReferenceDirection = ReferenceDirectionEnum.HeadFacing;
        #endregion

        #region private members
        private Transform head;
        private Vector3 lastOffset;

        private Vector3[] offsetNodes;
        private GameObject[] HelperSpheres = new GameObject[4];

        private int nodeIdx;

        private float xVal;
        private float yVal;

        private float inputBuffer = 0.25f;
        private float lastInput;
        #endregion

        private void Start()
        {
            head = Veil.Instance.HeadTransform;
            ResetTransform();

            GenerateOffsets();
        }

        public void Reset()
        {
            if (!this.enabled)
                return;
            ResetTransform();
            UpdateDebug();
        }

        private void ResetTransform()
        {
            Vector3 refDir = GetRefDir();
            WorkingPos = head.position + refDir * Depth;
            WorkingRot = Quaternion.LookRotation((WorkingPos - head.position), Vector3.up);
            lastOffset = WorkingPos - head.position;
        }

        public void ToggleActive(bool bActive)
        {
            ActiveInput = bActive;

            for (int i = 0; i < HelperSpheres.Length; i++)
            {
                HelperSpheres[i].SetActive(bActive);
            }
        }

        private Vector3 GetRefDir()
        {
            if (ReferenceDirection == ReferenceDirectionEnum.HeadFacing)
            {
                return head.forward;
            }
            else if (ReferenceDirection == ReferenceDirectionEnum.HeadMoveDirection)
            {
                return Veil.Instance.MoveDirection;
            }
            return Vector3.one;
        }

        public override void SolverUpdate()
        {
            UpdateOffsets();

            Vector3 pos = WorkingPos;
            Vector3 vecTo = pos - head.position;
            Vector3 refDir = GetRefDir();

            float _toAngle = Vector3.Angle(refDir, vecTo);

            Matrix4x4 mat = Matrix4x4.TRS(head.position, Quaternion.LookRotation(refDir, Vector3.up), Vector3.one);

            Vector3 pinRelPos = mat.MultiplyPoint(pos);//head.TransformPoint(pos);
            float dispComp = Mathf.Abs(pinRelPos.x) + Mathf.Abs(pinRelPos.y);
            float compAngle = ((Mathf.Abs(pinRelPos.x) / dispComp) * XAngle) + ((Mathf.Abs(pinRelPos.y) / dispComp) * YAngle);

            if (_toAngle > (compAngle + Threshold))
            {
                this.GoalPosition = mat.MultiplyPoint(offsetNodes[nodeIdx]);
                lastOffset = pos - head.position;
            }
            else
            {
                this.GoalPosition = head.position + lastOffset;
            }

            this.GoalRotation = Quaternion.LookRotation(vecTo.normalized, Vector3.up);

            UpdateWorkingToGoal();


            // Controller debug controls
            if ((Time.time - lastInput > inputBuffer) && ActiveInput)
            {
                if (Input.GetAxis("360_HorizontalDPAD") > 0)
                {
                    lastInput = Time.time;
                    XAngle += IncrementAmount;
                    GenerateOffsets();
                    UpdateDebug();
                }

                if (Input.GetAxis("360_HorizontalDPAD") < 0)
                {
                    lastInput = Time.time;
                    XAngle -= IncrementAmount;
                    GenerateOffsets();
                    UpdateDebug();
                }

                if (Input.GetAxis("360_VerticalDPAD") > 0)
                {
                    lastInput = Time.time;
                    YAngle += IncrementAmount;
                    GenerateOffsets();
                    UpdateDebug();
                }

                if (Input.GetAxis("360_VerticalDPAD") < 0)
                {
                    lastInput = Time.time;
                    YAngle -= IncrementAmount;
                    GenerateOffsets();
                    UpdateDebug();
                }

                if (Input.GetButtonDown("360_RightBumper"))
                {
                    lastInput = Time.time;
                    Threshold += IncrementAmount;
                    UpdateDebug();
                }

                if (Input.GetButtonDown("360_LeftBumper"))
                {
                    lastInput = Time.time;
                    Threshold -= IncrementAmount;
                    UpdateDebug();
                }
            }
        }


        private void GenerateOffsets()
        {
            xVal = (Mathf.PI / 180f) * Depth * XAngle;
            yVal = (Mathf.PI / 180f) * Depth * YAngle;

            offsetNodes = new Vector3[] { new Vector3(0, yVal, Depth), new Vector3(xVal, 0, Depth), new Vector3(0, -yVal, Depth), new Vector3(-xVal, 0, Depth) };

            if (DrawSpheres)
            {
                for (int i = 0; i < offsetNodes.Length; i++)
                {
                    if (HelperSpheres[i] == null)
                    {
                        HelperSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        HelperSpheres[i].transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                        HelperSpheres[i].transform.parent = head;
                        HelperSpheres[i].transform.localPosition = offsetNodes[i];
                        HelperSpheres[i].SetActive(false);
                    }
                    else
                    {
                        HelperSpheres[i].transform.localPosition = offsetNodes[i];
                    }
                }
            }

        }

        private void UpdateOffsets()
        {
            Matrix4x4 mat = Matrix4x4.TRS(head.position, Quaternion.LookRotation(GetRefDir(), Vector3.up), Vector3.one);

            Vector3 curOffset = mat.inverse.MultiplyPoint(WorkingPos);//head.InverseTransformPoint(GetTransformPos());
            float bestDist = float.MaxValue;
            int newIdx = 0;

            for (int i = 0; i < offsetNodes.Length; i++)
            {
                float newDist = Vector3.Distance(curOffset, offsetNodes[i]);
                if (newDist < bestDist)
                {
                    newIdx = i;
                    bestDist = newDist;
                }
            }

            nodeIdx = newIdx;
        }

        private void UpdateDebug()
        {
            if (debugText != null)
            {
                debugText.text = "XAngle: <color=green>" + XAngle + "</color> YAngle: <color=red>" + YAngle + "</color> Threshold: <color=blue>" + Threshold + "</color>";
            }
        }

        public void ToggleDebug(bool active)
        {
            if (debugText != null)
            {
                debugText.gameObject.SetActive(active);
            }
        }
    }
}