//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

public class MaterialMapperBox : MonoBehaviour {

	public int MaterialIdx = 0;
	public Material targetMat;
	public BoxCollider colliderBox;	// The box on which the content is mapped to

	Vector2 colliderBoxScale = Vector2.one;
	Vector2 originalTextureScale = Vector2.one;

	void Awake()
	{
		colliderBox = GetComponentInChildren<BoxCollider>();
		UpdateColliderBoxScale();

		if (targetMat == null)
		{
			targetMat = this.GetComponentInChildren<Renderer>().materials[MaterialIdx];
		}

		originalTextureScale = targetMat.mainTextureScale;

		//StartCoroutine(CoTest());
		//StartCoroutine(CoTestZoom());
	}

	/*IEnumerator CoTest()
	{
		float x = 0, y = 0;
		while (true)
		{
			Vector3 b = ContentToBounds(new Vector2(x, y));
			b = BoundsToWorld(b);
			Debug.DrawLine(b, b - transform.forward * 0.1f, Color.red, 0.1f);

			yield return new WaitForSeconds(0.1f);

			x += 0.25f;
			if (x >= 1f)
			{
				y += 0.25f;
				x = 0;
				if (y >= 1f)
				{
					y = 0;
				}
			}
		}
	}

	IEnumerator CoTestZoom()
	{
		while (true)
		{
			for (float zoom = 2f; zoom >= 0.5f; zoom -= 0.1f)
			{
				SetContentZoom(zoom);
				yield return new WaitForSeconds(0.5f);
			}
		}
	}*/

	// Get the offset of the content, in content space
	public Vector2 GetContentOffset()
	{
		return targetMat.mainTextureOffset;
	}

	// Get the current scale of the content
	public Vector2 GetContentScale()
	{
		return new Vector2(1f / targetMat.mainTextureScale.x, 1f / targetMat.mainTextureScale.y);
	}

	public Vector2 GetContentScaleInv()
	{
		return targetMat.mainTextureScale;
	}

	// Pan the content
	public void SetContentOffset(Vector2 offsetAmount)
	{
		Vector2 texRatio = targetMat.mainTextureScale;
		targetMat.mainTextureOffset = Vector2.Scale(offsetAmount, texRatio);
	}

	// Zoom the content (uniform). Returns the offset in content space that should be applied to center it
	// zoomCenterInBounds is in bounds space
	public Vector2 SetContentZoom(float zoom, Vector2 zoomCenterInBounds)
	{
		// Delta scale is difference between the new scale and the old scale
		Vector2 deltaScale = originalTextureScale.Inverse() / zoom - GetContentScale();

		targetMat.mainTextureScale = zoom * originalTextureScale;

		Vector2 offset = Vector2.Scale(zoomCenterInBounds, GetContentScaleInv());
		return Vector2.Scale(deltaScale, GetContentOffset() + offset);
	}

	// Convert from local position on the bounds, to content position
	public Vector2 BoundsToContent(Vector2 boundsPos)
	{
		Vector2 contentPos = boundsPos;

		contentPos.Scale(GetContentScaleInv());
		contentPos += GetContentOffset();

		return contentPos;
	}

	// Convert from content position, to bounds position
	public Vector2 ContentToBounds(Vector2 contentPos)
	{
		Vector2 boundsPos = contentPos;

		boundsPos -= GetContentOffset();
		boundsPos.Scale(GetContentScale());

		return boundsPos;
	}

	// Convert from bounds position to location in world space
	public Vector3 BoundsToWorld(Vector2 boundsPos)
	{
		boundsPos = boundsPos - 0.5f * Vector2.one;
		// Still account for center
		boundsPos.x *= colliderBoxScale.x;
		boundsPos.y *= colliderBoxScale.y;

		Vector3 worldPos = colliderBox.transform.TransformPoint(boundsPos);

		return worldPos;
	}

	// Convert from world position to bounds position
	public Vector3 WorldToBounds(Vector3 worldPos)
	{
		Vector3 localPos = colliderBox.transform.InverseTransformPoint(worldPos);

		// Normalize X,Y from 0,0 to 1,1.  Still need to account for center though :')
		localPos.x /= colliderBoxScale.x;
		localPos.y /= colliderBoxScale.y;

		localPos += Vector3.one * 0.5f;

		return localPos;
	}

	public float GetContentAspect()
	{
		return targetMat.mainTexture.width / targetMat.mainTexture.height;
	}

	void UpdateColliderBoxScale()
	{
		colliderBoxScale = (Vector2)colliderBox.size;
		colliderBoxScale.x *= colliderBox.transform.lossyScale.x;
		colliderBoxScale.y *= colliderBox.transform.lossyScale.y;
	}
}

public static class Vec2Extension
{
	public static Vector2 Inverse(this Vector2 vec)
	{
		return new Vector2(1f / vec.x, 1f / vec.y);
	}
}
