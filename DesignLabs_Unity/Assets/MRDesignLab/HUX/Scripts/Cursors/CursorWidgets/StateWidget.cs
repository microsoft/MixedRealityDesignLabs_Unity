//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Cursors
{

    /// <summary>
    /// The state Widget is a simple container widget that can be set to different states
    /// with external calls that hide and show prefabs that are spawned or not.
    /// </summary>
    public class StateWidget : CursorWidget
    {
        [System.Serializable]
        public class WidgetStateData
        {
            public string Name;
            public GameObject UpStatePrefab;
            public GameObject DownStatePrefab;

            public GameObject UpStateObject { get; set; }
            public GameObject DownStateObject { get; set; }
        }

        [SerializeField]
        public List<WidgetStateData> WidgetData = new List<WidgetStateData>();

        protected WidgetStateData m_CurrentStateData;

        // private active info 
        private bool m_bActive = false;  // Is the widget active?  
        private bool m_ActiveState = false;  // This is the "Up" state.

        /// <summary>
        /// OnStateChange gets state change calls from the parent
        /// </summary>
        /// <param name="ParentState"></param>
        public override void OnStateChange(Cursor.CursorState ParentState)
        {
            //  Veil.Instance.IsFingerPressed
            switch (ParentState)
            {
                case Cursor.CursorState.Release:
                    m_ActiveState = false;
                    break;
                case Cursor.CursorState.Select:
                    m_ActiveState = true;
                    break;
            }
        }

        /// <summary>
        /// Instantiate, reposition and reparent new game objects.
        /// </summary>
        /// <param name="prefab">Prefab reference, to be instantiated.</param>
        /// <param name="objName">Name for the newly instantiated object.</param>
        /// <returns>The newly instantiated object.</returns>
        public GameObject MakeObject(GameObject prefab, string objName)
        {
            if (prefab != null)
            {
                GameObject go = (GameObject)GameObject.Instantiate(prefab);
                go.SetActive(false);
                go.name = "Cursor_" + objName;
                go.transform.parent = this.gameObject.transform;
                CopyTransform(go);
                go.transform.localScale = new Vector3(1, 1, 1);
                return go;
            }
            else
            {
                return null;
            }
        }

        /// <param name="name">The name of the cursor.</param>
        public void SetStateByName(string name)
        {
            foreach(WidgetStateData data in WidgetData)
            {
                if(data.Name.Equals(name))
                {
                    SetStateData(data);
                    return;
                }
            }
        }

        public void Activate(bool active)
        {
            m_bActive = active;
        }

        public override bool ShouldBeActive()
        {
            return m_bActive;
        }

        /// <summary>
        /// Disable the supplied active custom cursor.
        /// </summary>
        /// <param name="c">Cursor data reference.</param>
        public void HideCustomCursor(WidgetStateData c)
        {
            if (c != null)
            {
                SafeSetActive(c.UpStateObject, false);
                SafeSetActive(c.DownStateObject, false);
            }
        }

        /// <summary>
        /// Enable the supplied inactive custom cursor.
        /// </summary>
        /// <param name="c">Cursor data reference.</param>
        public void ShowCustomCursor(WidgetStateData wsd)
        {
            if (wsd != null)
            {
                if (wsd.UpStateObject == null)
                {
                    wsd.UpStateObject = MakeObject(wsd.UpStatePrefab, wsd.Name + "Up");
                }
                if (wsd.DownStateObject == null)
                {
                    wsd.DownStateObject = MakeObject(wsd.DownStatePrefab, wsd.Name + "Down");
                }


                SafeSetActive(wsd.UpStateObject, !m_ActiveState);
                SafeSetActive(wsd.DownStateObject, m_ActiveState);
            }
        }


        /// <summary>
        /// Set a custom state by data reference.
        /// </summary>
        /// <param name="wsd"></param>
        public void SetStateData(WidgetStateData wsd)
        {
            HideCustomCursor(m_CurrentStateData);
            if (wsd == null)
            {
                // Standard cursor
                m_CurrentStateData = null;
                return;
            }
            else
            {
                // Custom cursor
                ShowCustomCursor(wsd);
                m_CurrentStateData = wsd;
            }
        }

        /// <summary>
        /// Copy transform values from this script's parent object, to a supplied object.
        /// </summary>
        /// <param name="go">GameObject to copy transform to.</param>
        public void CopyTransform(GameObject go)
        {
            if (go != null)
            {
                go.transform.position = this.transform.position;
                go.transform.rotation = this.transform.rotation;
                //go.transform.localScale = new Vector3(1, 1, 1); // this.transform.localScale;	// ?  Why is the scale strange?
            }
        }

        /// <summary>
        /// Try to set the state of the supplied game object.
        /// </summary>
        /// <param name="go">Game object to toggle state of.</param>
        /// <param name="activeFlag">Target state.</param>
        public void SafeSetActive(GameObject go, bool activeFlag)
        {
            if (go != null)
            {
                go.SetActive(activeFlag);
            }
        }
    }
}
