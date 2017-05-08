//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Utility;

namespace HUX.Focus
{
	public class FocusInfo : IPoolable
	{
		public GameObject gameObject;
		public bool isUI;
		public float uiIndex;
		public float distance;
		public Vector3 point;
		public Vector3 normal;
		public Transform transform;
		public Vector2 textureCoord;
		public UnityEngine.EventSystems.RaycastResult raycastResult;

		public bool IsActive
		{
			get
			{
				return (gameObject != null);
			}
		}

		public void ReturnToPool()
		{
			gameObject = null;
			isUI = false;
			uiIndex = 0;
			distance = 0;
			point = Vector3.zero;
			normal = Vector3.zero;
			transform = null;
			textureCoord = Vector2.zero;
			raycastResult.Clear();
		}

		internal void Set(
			GameObject nGameObject,
			float nDistance,
			Vector3 nPoint,
			Vector3 nNormal,
			Transform nTransform,
			Vector2 nTextureCoord)
		{
			this.gameObject = nGameObject;
			this.distance = nDistance;
			this.point = nPoint;
			this.normal = nNormal;
			this.transform = nTransform;
			textureCoord = nTextureCoord;
		}

		internal void SetUI(
			GameObject nGameObject,
			bool nIsUI,
			float nUiIndex,
			float nDistance,
			Vector3 nPoint,
			Vector3 nNormal,
			Transform nTransform,
			Vector2 nTextureCoord,
			UnityEngine.EventSystems.RaycastResult nRaycastResult)
		{
			this.gameObject = nGameObject;
			this.isUI = nIsUI;
			this.uiIndex = nUiIndex;
			this.distance = nDistance;
			this.point = nPoint;
			this.normal = nNormal;
			this.transform = nTransform;
			this.textureCoord = nTextureCoord;
			this.raycastResult = nRaycastResult;
		}
	}
}
