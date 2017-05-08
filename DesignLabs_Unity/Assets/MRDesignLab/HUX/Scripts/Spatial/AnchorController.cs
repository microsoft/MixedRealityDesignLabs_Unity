//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;

[RequireComponent(typeof(Anchor))]
public class AnchorController : MonoBehaviour
{
    #region Enumerations

    public enum Face
    {
        Left,
        Right,
        Top,
        Bottom,
        Front,
        Back
    }

    public enum Type
    {
        HeadPosition,
        GazeAlwaysOn,
        Gaze,
        Top,
        TopOnSide,
        COUNT
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Variables

    // ----- Anchor -----
    private Anchor m_Anchor;
    private Transform m_AnchorTransform;
    [SerializeField] private Type m_AnchorType;
    [SerializeField] private Vector3[] m_AnchorOffsets = new Vector3[4];

    // ----- Movement Damping -----
    public float m_PositionDamping = 15.0f;
    public float m_RotationDamping = 15.0f;

    private Vector3 m_TargetPosition = Vector3.zero;
    private Quaternion m_TargetRotation = Quaternion.identity;

    // ----- Face -----
    private Face m_CurrentFace = Face.Front;
    
    // ----- Holobar Objects ------
    [SerializeField] Renderer m_HolobarBacking;
    [SerializeField] GameObject m_HolobarButtons;
    [SerializeField] BoxCollider[] m_HolobarButtonColliders;

    #endregion

    // --------------------------------------------------------------------------------

    #region Monobehaviour Functions

    private void Start()
    {
        m_Anchor = GetComponent<Anchor>() as Anchor;
        m_AnchorTransform = m_Anchor.transform;

        // Disable the anchor position/rotation
        m_Anchor.AnchorPosition = false;
        m_Anchor.AnchorRotation = false;

        // Set initial target position/rotation
        m_TargetPosition = m_AnchorTransform.position;
        m_TargetRotation = m_AnchorTransform.rotation;

        // Set initial type
        SetType(Type.HeadPosition);
    }

    private void Update()
    {
        // Set type the the shell global type
/*        Type shellType = PrototypeShell.Instance.AnchorType;
        if (m_AnchorType != shellType)
        {
            SetType(shellType);
        }
*/
        // Update anchor type
        switch (m_AnchorType)
        {
            case Type.HeadPosition:
                UpdateToHeadPosition();
                break;

            case Type.GazeAlwaysOn:
                UpdateToGaze(true);
                break;

            case Type.Gaze:
                UpdateToGaze(false);
                break;

            case Type.Top:
                UpdateToTop();
                break;

            case Type.TopOnSide:
                UpdateToTopOnSide();
                break;

            default:
                break;
        }
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region State Functions

    public void SetType(Type anchorType)
    {
        // Set type
        m_AnchorType = anchorType;
        switch (anchorType)
        {
            case Type.HeadPosition:
                SetToHeadPosition();
                break;

            case Type.GazeAlwaysOn:
                SetToGazeAlwaysOn();
             break;

            case Type.Gaze:
                SetToGaze();
                break;

            case Type.Top:
                SetToTop();
                break;

            case Type.TopOnSide:
                SetToTopOnSide();
                break;

            default:
                break;
        }
    }

    private void SetToHeadPosition()
    {
        // Set holobar - enabled
        SetHolobarActive(true);

        // Set for head
        SetAnchorUpdateStates(false, false);
        SetAnchorOffset(m_AnchorOffsets[(int)Type.HeadPosition]);
        SetAnchorAxes(m_Anchor.m_XAxisAnchor, Anchor.YAxis.Bottom, m_Anchor.m_ZAxisAnchor);
    }

    private void SetToGazeAlwaysOn()
    {
        // Set holobar - enabled
        SetHolobarActive(true);

        // Set to gaze
        SetAnchorUpdateStates(false, false);
        SetAnchorOffset(m_AnchorOffsets[(int)Type.GazeAlwaysOn]);
        SetAnchorAxes(m_Anchor.m_XAxisAnchor, Anchor.YAxis.Bottom, m_Anchor.m_ZAxisAnchor);
    }

    private void SetToGaze()
    {
        // Set holobar - Diabled
        SetHolobarActive(false);

        // Set to gaze
        SetAnchorUpdateStates(false, false);
        SetAnchorOffset(m_AnchorOffsets[(int)Type.Gaze]);
        SetAnchorAxes(m_Anchor.m_XAxisAnchor, Anchor.YAxis.Bottom, m_Anchor.m_ZAxisAnchor);
    }

    private void SetToTop()
    {
        // Set holobar - enabled
        SetHolobarActive(true);

        // Set to top
        SetAnchorUpdateStates(true, false);
        SetAnchorOffset(m_AnchorOffsets[(int)Type.Top]);
        SetAnchorAxes(Anchor.XAxis.Middle, Anchor.YAxis.Top, Anchor.ZAxis.Middle);
    }

    private void SetToTopOnSide()
    {
        // Set holobar - enabled
        SetHolobarActive(true);

        // Set for head
        SetAnchorUpdateStates(false, false);
        SetAnchorOffset(m_AnchorOffsets[(int)Type.TopOnSide]);
        SetAnchorAxes(m_Anchor.m_XAxisAnchor, Anchor.YAxis.Top, m_Anchor.m_ZAxisAnchor);
    }

    private void UpdateToHeadPosition()
    {
        // Use head position
        Vector3 forward = (m_Anchor.AnchorObj.position - HUX.Veil.Instance.HeadTransform.position).normalized;
        m_CurrentFace = GetClosestFace(forward, m_Anchor.AnchorObj.rotation);

        // Set position/rotation
        SetPositionToFace(m_CurrentFace);
        SetRotationToFace(m_CurrentFace);

        // Set target position
        m_TargetPosition = m_Anchor.GetAnchorWorldPosition();

        // Update position/rotation
        UpdatePositionAndRotation();
    }

    private void UpdateToGaze(bool alwaysOn)
    {
        // Use head direction
        bool hit = GetLookAtFace(m_Anchor.AnchorObj.rotation, ref m_CurrentFace);

        if(!alwaysOn)
        {
            // Enable/disable holobar
            SetHolobarActive(hit);
        }

        // Set position/rotation
        SetPositionToFace(m_CurrentFace);
        SetRotationToFace(m_CurrentFace);

        // Set target position
        m_TargetPosition = m_Anchor.GetAnchorWorldPosition();

        // Update position/rotation
        UpdatePositionAndRotation();
    }

    private void UpdateToTop()
    {
        // Set target rotation
        m_TargetRotation = m_Anchor.AnchorObj.rotation;

        // Face the user
        Vector3 normal = m_Anchor.AnchorObj.up;
        Vector3 fromUser = m_Anchor.AnchorObj.position - HUX.Veil.Instance.HeadTransform.position;

        // Get vector to user - flattend - in local space
        fromUser = m_Anchor.AnchorObj.InverseTransformDirection(fromUser);
        fromUser.y = 0.0f;

        // Move to world space - apply rotation
        fromUser = m_Anchor.AnchorObj.rotation * fromUser;
        transform.rotation = Quaternion.LookRotation(fromUser, normal);
    }

    private void UpdateToTopOnSide()
    {
        UpdateToHeadPosition();
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region State Helper Functions

    private void SetAnchorUpdateStates(bool anchorPosition, bool anchorRotation)
    {
        // Set the anchor position/rotation
        m_Anchor.AnchorPosition = anchorPosition;
        m_Anchor.AnchorRotation = anchorRotation;
    }

    private void SetAnchorAxes(Anchor.XAxis xAxis, Anchor.YAxis yAxis, Anchor.ZAxis zAxis)
    {
        // Set the axes
        m_Anchor.m_XAxisAnchor = xAxis;
        m_Anchor.m_YAxisAnchor = yAxis;
        m_Anchor.m_ZAxisAnchor = zAxis;
    }

    private void SetAnchorOffset(Vector3 anchorOffset)
    {
        // Set the offset
        m_Anchor.m_LocalPositionFromAnchor = anchorOffset;
    }

    #endregion

    // --------------------------------------------------------------------------------

    #region Private Functions

    private void UpdatePositionAndRotation()
    {
        if (!m_Anchor.AnchorPosition)
        {
            // Update position
            m_AnchorTransform.position = Vector3.Lerp(m_AnchorTransform.position, m_TargetPosition, Time.deltaTime * m_PositionDamping);
        }

        if (!m_Anchor.AnchorRotation)
        {
            // Update rotation
            m_AnchorTransform.rotation = Quaternion.Slerp(m_AnchorTransform.rotation, m_TargetRotation, Time.deltaTime * m_RotationDamping);
        }
    }

    private bool GetLookAtFace(Quaternion anchorRotation, ref Face face)
    {
        HUX.Focus.FocusManager focusManager = HUX.Focus.FocusManager.Instance;
        HUX.Focus.FocusInfo focusHitInfo = focusManager.GazeFocuser.FocusHitInfo;

        // Early out if did not hit or did not hit anchor
        if (focusHitInfo.gameObject == null || focusHitInfo.gameObject != m_Anchor.AnchorObj.gameObject)
        {
            face = m_CurrentFace;
            return focusHitInfo.gameObject != null ? IsCollidingWithAnchor(focusHitInfo.gameObject) : false;
        }

        // Return closest face to normal
        Vector3 faceNormal = focusHitInfo.normal;
        face = GetClosestFace(-faceNormal, anchorRotation);
        return true;
    }

    private Face GetClosestFace(Vector3 forward, Quaternion anchorRotation)
    {
        float dot = 0.0f;
        Face closestFace = m_CurrentFace;

        Vector3 anchorForward = anchorRotation * Vector3.forward;
        Vector3 anchorRight = anchorRotation * Vector3.right;
        Vector3 anchorUp = anchorRotation * Vector3.up;

        CheckAxis(forward, -anchorForward, Face.Front, ref dot, ref closestFace);
        CheckAxis(forward, anchorForward, Face.Back, ref dot, ref closestFace);

        CheckAxis(forward, anchorRight, Face.Right, ref dot, ref closestFace);
        CheckAxis(forward, -anchorRight, Face.Left, ref dot, ref closestFace);

        CheckAxis(forward, anchorUp, Face.Top, ref dot, ref closestFace);
        CheckAxis(forward, -anchorUp, Face.Bottom, ref dot, ref closestFace);

        return closestFace;
    }

    private void CheckAxis(Vector3 direction, Vector3 axisNormal, Face currentFace, ref float dot, ref Face closestFace)
    {
        float newDot = Vector3.Dot(direction, -axisNormal);
        if (newDot > dot)
        {
            dot = newDot;
            closestFace = currentFace;
        }
    }

    private void SetPositionToFace(Face face)
    {
        // Set anchor to closest face - Do not set to Top/Bottom
        switch (face)
        {
            case Face.Left:
                m_Anchor.m_XAxisAnchor = Anchor.XAxis.Left;
                m_Anchor.m_ZAxisAnchor = Anchor.ZAxis.Middle;
                break;

            case Face.Right:
                m_Anchor.m_XAxisAnchor = Anchor.XAxis.Right;
                m_Anchor.m_ZAxisAnchor = Anchor.ZAxis.Middle;
                break;

            case Face.Front:
                m_Anchor.m_XAxisAnchor = Anchor.XAxis.Middle;
                m_Anchor.m_ZAxisAnchor = Anchor.ZAxis.Front;
                break;

            case Face.Back:
                m_Anchor.m_XAxisAnchor = Anchor.XAxis.Middle;
                m_Anchor.m_ZAxisAnchor = Anchor.ZAxis.Back;
                break;

            default:
                break;
        }
    }

    private void SetRotationToFace(Face face)
    {
        Vector3 faceForward = Vector3.zero;

        // Get face forward
        switch (face)
        {
            case Face.Left:
                faceForward = Vector3.right;
                break;

            case Face.Right:
                faceForward = -Vector3.right;
                break;

            case Face.Front:
                faceForward = Vector3.forward;
                break;

            case Face.Back:
                faceForward = -Vector3.forward;
                break;

            default:
                // Return - Do not set rotation
                return;
        }

        // Set target rotation
        m_TargetRotation = m_Anchor.AnchorObj.rotation * Quaternion.LookRotation(m_Anchor.AnchorObj.localRotation * faceForward);
    }

    private void SetHolobarActive(bool enabled)
    {
        m_HolobarBacking.enabled = enabled;
        m_HolobarButtons.SetActive(enabled);
    }

    private bool IsCollidingWithAnchor(GameObject gameObject)
    {
        // Check anchor
        if(gameObject == m_Anchor.AnchorObj.gameObject)
        {
            return true;
        }

        // Check backign collider
        if (gameObject == m_HolobarBacking.gameObject)
        {
            return true;
        }

        // Check button colliders
        foreach (BoxCollider collider in m_HolobarButtonColliders)
        {
            if(collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
