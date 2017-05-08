//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Spatial
{
	/// <summary>
	///   IgnoreParentScaling solver applies a scale to maintain a constant size regardless of inherited scale
	/// </summary>
	public class IgnoreParentScaling : MonoBehaviour
	{

		public Transform mirrorTransform;
		public bool lerpScale = false;
		public float lerpTime = 0.2f;

		private Vector3 baseScale;
		private float timeDelta = 0.0f;
		private float lastRatio;
		private bool scaleLerping = false;

		// Use this for initialization
		void Start()
		{
			baseScale = this.transform.root.lossyScale;
		}

		// Update is called once per frame
		void FixedUpdate()
		{

			float _curScaleRatio = baseScale.magnitude / this.transform.root.lossyScale.magnitude;
			Vector3 _targetScale = Vector3.one * _curScaleRatio;

			if (lerpScale)
			{
				if (Mathf.Abs(lastRatio - _curScaleRatio) > 0.01f)
				{
					scaleLerping = true;
				}

				if (scaleLerping)
				{
					timeDelta += (Time.fixedDeltaTime / lerpTime);
					this.transform.localScale = Vector3.Lerp(_targetScale, Vector3.one, timeDelta);

					if (timeDelta > lerpTime)
					{
						this.transform.localScale = Vector3.one;
						baseScale = this.transform.root.lossyScale;
						scaleLerping = false;
						timeDelta = 0.0f;
					}
				}

			}
			else
			{
				this.transform.localScale = _targetScale;
			}

			if (mirrorTransform != null)
			{
				this.transform.position = mirrorTransform.position;
				this.transform.rotation = mirrorTransform.rotation;
			}

			lastRatio = _curScaleRatio;
		}
	}
}
