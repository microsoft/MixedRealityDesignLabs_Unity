//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

#if UNITY_WSA
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Sharing;
#endif

using System.Collections;
using System.Collections.Generic;
using HUX.Receivers;
using HUX.Dialogs.Debug;
using HUX.Utility;

public class PointOfReferenceManager : Singleton<PointOfReferenceManager>
{
    public GameObject m_PointOfReferenceCube;


#if UNITY_WSA
    List<string> m_MessagesAsync = new List<string>();
    public event System.Action OnPlacement;
    WorldAnchorStore store = null;
    private WorldAnchor m_PointOfReference = null;
    public WorldAnchor PointOfReference
    {
        get { return m_PointOfReference; }
    }
#endif
    public const string PointOfReferenceID = "PointOfReference";

    private bool m_LoadedAndPlaced = false;
    public bool LoadedAndPlaced
    {
        get { return m_LoadedAndPlaced; }
    }

    enum ManualPointOfReferenceAcquisitionStage
    {
        Idle,
        FirstPoint,
        SecondPoint,
        Acquired,
    }
    ManualPointOfReferenceAcquisitionStage m_ManualAnchorAcquisitionState = ManualPointOfReferenceAcquisitionStage.Idle;
    Vector3 m_ManualAnchorPos = Vector3.zero;
    Vector3 m_ManualAnchorForward = Vector3.forward;

#if UNITY_WSA
    // Use this for initialization
    void Start()
    {
        if (DebugMenu.Instance)
        {
            DebugMenu.Instance.AddButtonItem("Point Of Reference\\Generate Point of Reference", "Generate New", CreateManualPointOfReference);
        }
#if UNITY_EDITOR
        m_LoadedAndPlaced = true;
#else
        WorldAnchorStore.GetAsync(StoreLoaded);
#endif
        
		InputSources.Instance.hands.OnFingerPressed += OnFingerPressed;
    }
    
    // Update is called once per frame
    void Update ()
    {
        lock (m_MessagesAsync)
        {
            while (m_MessagesAsync.Count > 0)
            {
                Debug.Log("(async)" + m_MessagesAsync[0]);
                m_MessagesAsync.RemoveAt(0);
            }
        }

        if ((m_PointOfReference != null) && (m_PointOfReference.isLocated))
        {
            m_LoadedAndPlaced = true;
        }
    }
#endif

    public void CreatePointOfReference(Vector3 pos, Quaternion rot)
    {
        m_PointOfReferenceCube.transform.position = pos;
        m_PointOfReferenceCube.transform.rotation = rot;

#if UNITY_WSA
        if (store != null)
        {
            store.Delete(PointOfReferenceID);
        }

        if (m_PointOfReference)
        {
            DestroyImmediate(m_PointOfReference);
        }

        m_PointOfReference = m_PointOfReferenceCube.AddComponent<WorldAnchor>();
        if (StatusText.Instance)
        {
            StatusText.Instance.SetText("Point of Reference Created");
        }

        // SaveAnchorToStore(m_PointOfReferenceID, m_PointOfReference);
        StartCoroutine(SaveAnchor());
#endif
    }


#if UNITY_WSA
    void LogAsync(string message)
    {
        lock (m_MessagesAsync)
        {
            m_MessagesAsync.Add(message);
        }
    }

    private void StoreLoaded(WorldAnchorStore store)
    {
        this.store = store;

        // We've loaded.  Wait to place.
        StartCoroutine(PlaceWorldAnchor());
    }

    private IEnumerator PlaceWorldAnchor()
    {
        // Wait while the SR loads a bit.
        yield return new WaitForSeconds(0.5f);

        if (m_PointOfReference)
        {
            DestroyImmediate(m_PointOfReference);
        }

        m_PointOfReferenceCube.transform.position = Vector3.zero;
        m_PointOfReferenceCube.transform.localScale = Vector3.one * 0.1f;

        m_PointOfReference = store.Load(PointOfReferenceID, m_PointOfReferenceCube);
        if (m_PointOfReference == null)
        {
            if (StatusText.Instance)
            {
                StatusText.Instance.SetText("No Point of Reference created.");
            }
        }
        else
        {
            Debug.Log("Created anchor from WorldAnchorStore: " + PointOfReferenceID);
            if (StatusText.Instance)
            {
                StatusText.Instance.SetText("Loaded WorldAnchorStore");
            }
        }
    }

    private IEnumerator SaveAnchor()
    {
#if !UNITY_EDITOR
        while (!m_PointOfReference.isLocated)
        {
            yield return new WaitForEndOfFrame();
        }
#else
        yield return 0;
#endif

        SaveAnchorToStore(PointOfReferenceID, m_PointOfReference);
        Debug.Log("SaveAnchor: Point of Reference Saved.");
    }

    private void SaveAnchorToStore(string id, WorldAnchor worldAnchor)
    {
#if !UNITY_EDITOR
        if ((store != null) && (store.Save(id, worldAnchor)))
        {
            Debug.Log("WorldAnchor Saved: " + id);
            if (OnPlacement != null)
            {
                OnPlacement();
            }
        }
        else
        {
            Debug.Log("Failed to Save WorldAnchor " + id);
        }
#else
        if (OnPlacement != null)
        {
            OnPlacement();
        }
#endif
        if (StatusText.Instance)
        {
            StatusText.Instance.SetText("Anchor Saved");
        }
    }

    public void SetPointOfReference(byte[] pointOfReferenceData)
    {
        WorldAnchorTransferBatch.ImportAsync(pointOfReferenceData, OnImportComplete);
    }

    private void OnImportComplete(SerializationCompletionReason completionReason, WorldAnchorTransferBatch deserializedTransferBatch)
    {
        if (completionReason != SerializationCompletionReason.Succeeded)
        {
            LogAsync("Failed to import: " + completionReason.ToString());
            return;
        }

        string[] ids = deserializedTransferBatch.GetAllIds();
        if (ids.Length > 0)
        {
            if (m_PointOfReference)
            {
                DestroyImmediate(m_PointOfReference);
            }

            m_PointOfReferenceCube.transform.position = Vector3.zero;
            m_PointOfReferenceCube.transform.localScale = Vector3.one * 0.1f;

            m_PointOfReference = deserializedTransferBatch.LockObject(ids[0], m_PointOfReferenceCube);

            if (StatusText.Instance)
            {
                StatusText.Instance.SetText("Anchor Created");
            }
            Debug.Log("Anchor Created");

            if (store != null)
            {
                store.Delete(PointOfReferenceID);
            }

            StartCoroutine(SaveAnchor());

            if (StatusText.Instance)
            {
                StatusText.Instance.SetText("Anchor Saved to Store");
            }

            Debug.Log("Anchor Saved to Store");
        }
    }

    private void ClearAnchors()
    {
        if (store != null)
        {
            store.Clear();
        }

        Debug.Log("Cleared WorldAnchorStore");

        m_PointOfReference = null;

        if (StatusText.Instance)
        {
            StatusText.Instance.SetText("Anchors Cleared");
        }
    }
#endif

    public void CreateManualPointOfReference()
    {
        m_ManualAnchorAcquisitionState = ManualPointOfReferenceAcquisitionStage.FirstPoint;
        if (StatusText.Instance)
        {
            StatusText.Instance.SetTextUntimed("Click First Point for Point-of-Reference");
        }
    }

    public void OnFingerPressed(InputSourceHands.CurrentHandState state)
    {
        switch (m_ManualAnchorAcquisitionState)
        {
            case ManualPointOfReferenceAcquisitionStage.Idle:
            {
                break;
            }

            case ManualPointOfReferenceAcquisitionStage.FirstPoint:
            {
                if (StatusText.Instance)
                {
                    StatusText.Instance.SetTextUntimed("Click Second Point for Point-of-Reference");
                }
                m_ManualAnchorPos = HUX.Focus.FocusManager.Instance.GazeFocuser.Cursor.transform.position;
                m_ManualAnchorAcquisitionState = ManualPointOfReferenceAcquisitionStage.SecondPoint;
                break;
            }

            case ManualPointOfReferenceAcquisitionStage.SecondPoint:
            {
                if (StatusText.Instance)
                {
                    StatusText.Instance.SetText("Point of Reference Created!");
                }
                m_ManualAnchorForward = (HUX.Focus.FocusManager.Instance.GazeFocuser.Cursor.transform.position - m_ManualAnchorPos).normalized;
                m_ManualAnchorAcquisitionState = ManualPointOfReferenceAcquisitionStage.Acquired;

                CreatePointOfReference(m_ManualAnchorPos, Quaternion.LookRotation(m_ManualAnchorForward));
                break;
            }

            case ManualPointOfReferenceAcquisitionStage.Acquired:
            {
                break;
            }
        }
    }


    public void CalculateOffsetFromPointOfReference(Vector3 pos, Quaternion rot, out Vector3 posOffset, out Quaternion rotOffset)
    {
        if (m_PointOfReferenceCube != null)
        {
            Vector3 flattenedDir = m_PointOfReferenceCube.transform.forward;
            flattenedDir.y = 0.0f;

            rotOffset = Quaternion.Inverse(m_PointOfReferenceCube.transform.rotation) * rot;

            Quaternion localRot = Quaternion.LookRotation(flattenedDir, Vector3.up);
            posOffset = Quaternion.Inverse(localRot) * (pos - m_PointOfReferenceCube.transform.position);
        }
        else
        {
            posOffset = pos;
            rotOffset = rot;
        }
    }

    public void CalculatePosRotFromPointOfReference(Vector3 posOffset, Quaternion rotOffset, out Vector3 pos, out Quaternion rot)
    {
        if (m_PointOfReferenceCube != null)
        {
            Vector3 flattenedDir = m_PointOfReferenceCube.transform.forward;
            flattenedDir.y = 0.0f;

            rot = m_PointOfReferenceCube.transform.rotation * rotOffset;

            Quaternion localRot = Quaternion.LookRotation(flattenedDir, Vector3.up);
            pos = m_PointOfReferenceCube.transform.position + localRot * posOffset;
        }
        else
        {
            pos = posOffset;
            rot = rotOffset;
        }
    }
}
