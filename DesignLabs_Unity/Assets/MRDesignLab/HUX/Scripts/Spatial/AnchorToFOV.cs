//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class AnchorToFOV : MonoBehaviour
{
    public enum AnchorType
    {
        Top,
        Bottom,
        Left,
        Right,
		Middle
    }

    public AnchorType m_AnchorType;
    public Vector3 mViewSpaceOffset;

    private void LateUpdate()
    {
        Vector2 viewSpaceLocation = Vector2.zero;

        switch (m_AnchorType)
        {
            case AnchorType.Top:
            viewSpaceLocation = new Vector2(0.5f, 1.0f);
            break;
            case AnchorType.Bottom:
            viewSpaceLocation = new Vector2(0.5f, 0.0f);
            break;
            case AnchorType.Left:
            viewSpaceLocation = new Vector2(0.0f, 0.5f);
            break;
            case AnchorType.Right:
            viewSpaceLocation = new Vector2(1.0f, 0.5f);
            break;
			case AnchorType.Middle:
				viewSpaceLocation = new Vector2(0.5f, 0.5f);
			break;
        }

        // Add offset
        viewSpaceLocation.x += mViewSpaceOffset.x;
        viewSpaceLocation.y += mViewSpaceOffset.y;

        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(viewSpaceLocation.x, viewSpaceLocation.y, mViewSpaceOffset.z));
        transform.position = worldPosition;
        transform.rotation = Camera.main.transform.rotation;
    }
}
