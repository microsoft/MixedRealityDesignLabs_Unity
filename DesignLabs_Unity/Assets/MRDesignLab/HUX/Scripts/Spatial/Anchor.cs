//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class Anchor : MonoBehaviour
{
    #region Enumerations

    public enum XAxis
    {
        Left,
        Middle,
        Right
    }

    public enum YAxis
    {
        Top,
        Middle,
        Bottom
    }

    public enum ZAxis
    {
        Front,
        Middle,
        Back
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Variables

    public Transform m_Anchor;
    private BoxCollider m_AnchorCollider;
    private MeshFilter m_AnchorMeshFilter;
    private Anchor m_ParentAnchor;

    public XAxis m_XAxisAnchor;
    public YAxis m_YAxisAnchor;
    public ZAxis m_ZAxisAnchor;

    public Vector3 m_LocalPositionFromAnchor = Vector3.zero;
    public Vector3 m_ScaleOffset = Vector3.zero;

    public bool m_AnchorPosition = true;
    public bool m_AnchorRotation = true;
 
    public bool m_AnchorScale = false;
    public bool m_AnchorScaleBasedOnParentY = false;
    public bool m_AnchorScaleBasedOnParentX = false;
	public bool m_IgnoreParentCollider = false;
	#endregion

	// --------------------------------------------------------------------------------

	#region Accessors

	public Transform AnchorObj
    {
        get { return m_Anchor; }
    }

    public bool AnchorPosition
    {
        get { return m_AnchorPosition; }
        set { m_AnchorPosition = value; }
    }

    public bool AnchorRotation
    {
        get { return m_AnchorRotation; }
        set { m_AnchorRotation = value; }
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Monobehaviour Functions

    private void Awake()
    {
        m_AnchorCollider = m_Anchor.GetComponent<BoxCollider>();
        m_AnchorMeshFilter = m_Anchor.GetComponent<MeshFilter>();
        m_ParentAnchor = m_Anchor.GetComponent<Anchor>();

        ForceAnchorUpdate();
    }

    private void Update()
    {
        if (m_AnchorPosition)
        {
            // Set the position
            transform.position = GetAnchorWorldPosition();
        }

        if (m_AnchorRotation)
        {
            // Set the rotation
            transform.rotation = m_Anchor.rotation;
        }

        if (m_AnchorScale)
        {
            // Set scale
            transform.localScale = m_Anchor.localScale + m_ScaleOffset;

            if (m_AnchorCollider != null)
            {
                transform.localScale = Vector3.Scale(transform.localScale, m_AnchorCollider.size);
            }
        }

        if (m_AnchorScaleBasedOnParentY)
        {
            // Set scale
            transform.localScale = Vector3.one * m_Anchor.localScale.y + m_ScaleOffset;

            if (m_AnchorCollider != null && !m_IgnoreParentCollider)
            {
                transform.localScale = Vector3.Scale(transform.localScale, Vector3.one * m_AnchorCollider.size.y);
            }
        }

        if (m_AnchorScaleBasedOnParentX)
        {
            // Set scale
            transform.localScale = Vector3.one * m_Anchor.localScale.x + m_ScaleOffset;

            if (m_AnchorCollider != null && !m_IgnoreParentCollider)
            {
                transform.localScale = Vector3.Scale(transform.localScale, Vector3.one * m_AnchorCollider.size.x);
            }
        }
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Public Functions

    public void ForceAnchorUpdate()
    {
        // Force update the parent anchor first
        if (m_ParentAnchor)
        {
            m_ParentAnchor.ForceAnchorUpdate();
        }

        Update();
    }

    public Vector3 GetAnchorWorldPosition()
    {
        // Get local position
        Vector3 localAnchorPosition = GetAnchorLocalPosition();

        // Return transformed
        return m_Anchor.position + m_Anchor.TransformDirection(localAnchorPosition);
    }

    public Vector3 GetAnchorLocalPosition()
    {
        Vector3 anchorPosition = Vector3.zero;
        Vector3 halfBounds = m_Anchor.lossyScale * 0.5f;

        if(m_AnchorCollider != null)
        {
            halfBounds = Vector3.Scale(halfBounds, m_AnchorCollider.size);
        }

        if(m_AnchorMeshFilter != null)
        {
            halfBounds = Vector3.Scale(halfBounds, m_AnchorMeshFilter.mesh.bounds.size);
        }

        // Set x position
        switch (m_XAxisAnchor)
        {
            case XAxis.Left:
            anchorPosition.x = halfBounds.x;
            break;

            case XAxis.Middle:
            anchorPosition.x = 0.0f;
            break;

            case XAxis.Right:
            anchorPosition.x = -halfBounds.x;
            break;
        }

        // Set y position
        switch (m_YAxisAnchor)
        {
            case YAxis.Top:
            anchorPosition.y = halfBounds.y;
            break;

            case YAxis.Middle:
            anchorPosition.y = 0.0f;
            break;

            case YAxis.Bottom:
            anchorPosition.y = -halfBounds.y;
            break;
        }

        // Set z position
        switch (m_ZAxisAnchor)
        {
            case ZAxis.Front:
            anchorPosition.z = -halfBounds.z;
            break;

            case ZAxis.Middle:
            anchorPosition.z = 0.0f;
            break;

            case ZAxis.Back:
            anchorPosition.z = halfBounds.z;
            break;
        }

        Vector3 finalPosition = anchorPosition + m_LocalPositionFromAnchor;
        if (m_AnchorCollider)
        {
            finalPosition += Vector3.Scale(m_Anchor.transform.lossyScale, m_AnchorCollider.center);
        }

        return finalPosition;
    }

    public void ChangeAnchor(Transform newAnchor, Vector3 positionOffset, Vector3 scaleOffset)
    {
        m_Anchor = newAnchor;
        m_AnchorCollider = m_Anchor.GetComponent<BoxCollider>();

        m_LocalPositionFromAnchor = positionOffset;
        m_ScaleOffset = scaleOffset;

        ForceAnchorUpdate();
    }

    public void OffsetPosition(Vector3 positionOffset)
    {
        m_LocalPositionFromAnchor += positionOffset;
        ForceAnchorUpdate();
    }

    #endregion
}
