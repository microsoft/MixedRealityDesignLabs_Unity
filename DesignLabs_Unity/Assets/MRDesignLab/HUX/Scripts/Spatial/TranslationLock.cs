//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HUX.Spatial
{
    public class TranslationLock : MonoBehaviour
	{
		public float speed = 1.0f;
		public Vector3 offset;
		public bool RotationTether = false;
		public float TetherAngleLimit = 20.0f;

		public bool lookAwayFromHead = false;

		private Transform head;
		private Quaternion origRot;

		private Vector3 mEndOfFramePosition = Vector3.zero;
		private Quaternion mEndOfFrameRotation = Quaternion.identity;

		void Start()
		{
			head = Veil.Instance.HeadTransform;

			mEndOfFramePosition = transform.position;
			mEndOfFrameRotation = transform.rotation;
		}

		void Update()
		{
			transform.position = mEndOfFramePosition;
			transform.rotation = mEndOfFrameRotation;

			Vector3 newPos = head.position + (this.transform.rotation * offset);

			if (RotationTether)
			{
				Vector3 scaleVec = new Vector3(1f, 0f, 1f);
				Vector3 headVec = Vector3.Scale(head.forward, scaleVec);
				Vector3 headRtVec = Vector3.Scale(head.right, scaleVec);
				Vector3 objVec = Vector3.Scale(this.transform.position - head.position, scaleVec);

				float angleDiff = Vector3.Angle(headVec, objVec);
				if (angleDiff > TetherAngleLimit)
				{
					float angleVal = Mathf.Lerp(0, TetherAngleLimit - angleDiff, Time.deltaTime * speed);
					angleVal = Vector3.Dot(headRtVec, objVec) < 0 ? -angleVal : angleVal;

					this.transform.RotateAround(head.position, new Vector3(0.0f, 1.0f, 0.0f), angleVal);
					newPos = head.position + this.transform.rotation * offset;
				}
			}

			this.transform.position = newPos;

			mEndOfFramePosition = transform.position;
			mEndOfFrameRotation = transform.rotation;

			if (lookAwayFromHead)
			{
				transform.LookAt(head);
				transform.Rotate(0.0f, 180.0f, 0.0f, Space.Self);
			}
		}
	}
}