//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HUX.Buttons;
using HUX.Focus;

namespace HUX.Collections
{
    /// <summary>
    /// The key collection is used for creating keyboards of differnt types and configuration for input.
    /// To create a key collection define the number of rows and then add the characters to the proper array positions
    /// and hit Generate Keys.
    /// </summary>
    [ExecuteInEditMode]
    public class KeyCollection : MonoBehaviour
    {
        #region public members
        public enum SurfaceTypeEnum
        {
            Plane,
            Cylinder
        }

        [Tooltip("Type of surface to maps keys to")]
        public SurfaceTypeEnum SurfaceType = SurfaceTypeEnum.Plane;

        [Tooltip("Game object prefab for keys")]
        public GameObject KeyPrefab;

        [Tooltip("First key to test angles of for screen space angle")]
        public GameObject TestKey;

        [Tooltip("Second key to test angles of for screen space angle")]
        public GameObject TestKeyTwo;

        [Tooltip("Number of rows for key collection")]
        public int Rows = 3;

        [Tooltip("Number of columns for key collection")]
        public int Columns = 8;

        [Tooltip("Margin between keys")]
        public float Margin = 0.02f;

        [Tooltip("Whether keycollection is being looked at or any of the keys are being gazed at (read only)")]
        public bool InFocus;

        [Serializable]
        public class KeyRow
        {
            public string[] stringArray = new string[0];

            public string this[int index]
            {
                get { return stringArray[index]; }
                set { stringArray[index] = value; }
            }

            public int Length
            {
                get { return stringArray.Length; }
            }
        }

        [Tooltip("Key objects referenced by rows")]
        public KeyRow[] keyRows;

        public event Action<KeyButton> KeyPressed;

        [Tooltip("Target textmesh to update with key presses")]
        public TextMesh TargetText;

        [Tooltip("Debug text target for angle debug text")]
        public TextMesh DebugText;
        #endregion

        #region private variables
        private float KeyWidth;
        private float KeyHeight;

        private float TestKeyWidth;
        private float TestKeyHeight;
        private Vector3 KeyScale = Vector3.one;

        private List<GameObject> rowContainers = new List<GameObject>();
        #endregion


        public void Update()
        {

#if UNITY_EDITOR
            bool bKeyUpdate = false;
            int i;

            bKeyUpdate = keyRows == null ? true : keyRows.Length != Rows;
            if(!bKeyUpdate)
                bKeyUpdate = keyRows[0].Length != Columns;

            if (bKeyUpdate)
            {
                keyRows = new KeyRow[Rows];
                for (i = 0; i < Rows; i++)
                {
                    keyRows[i] = new KeyRow();
                    keyRows[i].stringArray = new string[Columns];
                }
            }
#endif
            // To Do: Add a method for getting screenspace size between keys 
            // Use our two test keys to determine our overall size.
            if (TestKey != null && TestKeyTwo != null && Veil.Instance != null && Veil.Instance.HeadTransform != null)
            {
                Vector3 headPos = Veil.Instance.HeadTransform.position;
                Vector3 keyOneVec = TestKey.transform.position - headPos;
                Vector3 keyTwoVec = TestKeyTwo.transform.position - headPos;

                float keyAngle = Vector3.Angle(keyOneVec, keyTwoVec);

                if (DebugText != null)
                {
                    DebugText.text = "KeyAngle: " + keyAngle;
                }
            }
        }

        public void Awake()
        {
            if (Application.isPlaying)
            {
                foreach (KeyButton keyBut in this.gameObject.GetComponentsInChildren<KeyButton>())
                {
                    keyBut.OnButtonPressed += OnKeyPressed;
                }

                Transform curTrans = TestKey.transform;
                while (curTrans != null)
                {
                    KeyScale.Scale(curTrans.localScale);
                    curTrans = curTrans.parent;
                }
            }
        }


        public virtual void OnKeyPressed(GameObject selectedObject)
        {
            KeyButton keyBut = selectedObject.GetComponent<KeyButton>();

            if (keyBut != null && KeyPressed != null)
            {
                KeyPressed(keyBut);
            }
        }


        public void FixedUpdate()
        {
			InFocus = false;

			for (int index = 0; index < FocusManager.Instance.FocusedObjects.Count && !InFocus; index++)
			{
				GameObject focusedObj = FocusManager.Instance.FocusedObjects[index];
				InFocus = focusedObj.transform == this.transform || focusedObj.transform.IsChildOf(this.transform);
			}
        }


        public void GenerateKeys()
        {
            int i, j, idx;

            // First lets wipe out our old children.
            for (i = gameObject.transform.childCount; i > 0; i--)
            {
                Transform child = gameObject.transform.GetChild(i - 1);
                GameObject.DestroyImmediate(child.gameObject);
            }

            rowContainers.Clear();

            // Create the rows
            for (i = 0; i < Rows; i++)
            {
                GameObject newRow = new GameObject();
                newRow.transform.parent = this.transform;
                newRow.name = "Row_" + i;

                rowContainers.Add(newRow);

                float rowOffset = (Rows / 2) - i;

                // Create the keys
                for (j = 0; j < Columns; j++)
                {
                    string keyText = keyRows[i][j];

                    if (keyText != "")
                    {
                        GameObject newKey = GameObject.Instantiate(KeyPrefab) as GameObject;
                        newKey.transform.parent = newRow.transform;
                        idx = (i * Columns) + j;
                        newKey.name = "Key_" + idx;

                        KeyButton keyBut = newKey.GetComponent<KeyButton>();
                        keyBut.Character = keyBut.KeyText.text = keyText;

                        KeyWidth = newKey.GetComponent<Collider>().bounds.size.x;
                        KeyHeight = newKey.GetComponent<Collider>().bounds.size.y;

                        float colOffset = j - (Columns / 2);
                        float xOffset = colOffset * (KeyWidth + (Margin / 2));

                        newKey.transform.localPosition = new Vector3(xOffset, 0.0f, 0.0f);
                    }
                }

                float yOffset = rowOffset * (KeyHeight + (Margin / 2));
                newRow.transform.localPosition = new Vector3(0.0f, yOffset, 0.0f);
            }
        }
    }
}