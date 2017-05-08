//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using HUX.Focus;

namespace HUX.Cursors
{
    /// <summary>
    /// This is the base abstract class for cursors and their defined states.
    /// </summary>
    public class OrganizeCursor : Cursor
    {
		[System.Serializable]
		public class CursorData
		{
			public string Name;
			public GameObject UpStatePrefab;
			public GameObject DownStatePrefab;

			public GameObject UpStateObject { get; set; }
			public GameObject DownStateObject { get; set; }
		}

		[Header("Custom Cursor Data")]
		public CursorData[] CustomCursors;

		[Header("Standard Cursor")]
		public GameObject NormalPrefab;
		public GameObject UpPrefab;
        public GameObject DownPrefab;

		public GameObject FOGPrefab;

		protected GameObject NormalObject;
		protected GameObject UpObject;
		protected GameObject DownObject;
		protected GameObject FOGObject;

		protected string CurrentCustomCursorName = "";		// "" = normal (no adornments)
		protected CursorData CurrentCustomCursor;

		// Move / size locked to object
		public Vector3 LockedToObjectPosition;			// If NULL, not locked to any object
		public bool LockedToObjectFlag;

		// State
		public bool HandInView = false;
		public bool FingerUp = true;
		public bool FOGMode = false;

        public bool UseGazerRay;

        private Animator cursorAnim;
        private Light cursorLight;

        public Light CursorLight
        {
            get { return cursorLight; }
        }

        //rotation, transform, scale are all driven by FoundationDriver
        public override void Start()
        {
            cursorAnim = gameObject.GetComponentInChildren<Animator>();
            cursorLight = gameObject.GetComponentInChildren<Light>();
            base.Start();

			// Create & hide our objects
			NormalObject= MakeObject(NormalPrefab, "Normal");
			UpObject	= MakeObject(UpPrefab, "Up");
			DownObject	= MakeObject(DownPrefab, "Down");
		}

		public GameObject MakeObject( GameObject prefab, string objName)
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


		public void CheckState()
		{
			HandInView = Veil.Instance.HandVisible;
			FingerUp = false;
			if (HandInView == true)
			{
				FingerUp = !Veil.Instance.IsFingerPressed();
			}
		}


		//
		// CALL THIS FROM EXTERNAL CODE TO SET YOUR CURSOR EXTRAS.
		//
		public void SetCursorByName(string name)
		{
			// In our list?
			//
			foreach (CursorData c in this.CustomCursors)
			{
				if (c != null)
				{
					if (c.Name == name)
					{
						// Got it
#if CHATTY
						Debug.Log("[ Setting custom cursor '" + name + "' ]");
#endif // CHATTY
						SetCursor(c);
						return;
					}
				}
			}

			// Not found?  Clear cursor
			SetCursor(null);
		}

		// Internal function
		public void SetCursor(CursorData c)
		{
			HideCustomCursor(CurrentCustomCursor);
			if (c == null)
			{
				// Standard cursor
				CurrentCustomCursor = null;
				CurrentCustomCursorName = "";
				return;
			}
			else
			{
				// Custom cursor
				ShowCustomCursor(c);
				CurrentCustomCursor = c;
				CurrentCustomCursorName = c.Name;
			}
		}

		public void HideCustomCursor(CursorData c)
		{
			if (c != null)
			{
				SafeSetActive(c.UpStateObject,false);
				SafeSetActive(c.DownStateObject,false);
			}
		}

		public void ShowCustomCursor(CursorData c)
		{
			if (c != null)
			{
				if (c.UpStateObject == null)
				{
					c.UpStateObject = MakeObject(c.UpStatePrefab, c.Name + "Up");
				}
				if (c.DownStateObject == null)
				{
					c.DownStateObject = MakeObject(c.DownStatePrefab, c.Name + "Down");
				}
				SafeSetActive(c.UpStateObject, FingerUp);
				SafeSetActive(c.DownStateObject,!FingerUp);
			}
		}

		public void SetCustomCursorPosition()
		{
			if (CurrentCustomCursor != null)
			{
				CopyTransform(CurrentCustomCursor.UpStateObject);
				CopyTransform(CurrentCustomCursor.DownStateObject);
			}
		}

		public void SetCursorPosition()
		{
			CopyTransform(this.NormalObject);
			CopyTransform(this.UpObject);
			CopyTransform(this.DownObject);
		}

		public void CopyTransform(GameObject go)
		{
			if (go != null)
			{
				go.transform.position = this.transform.position;
				go.transform.rotation = this.transform.rotation;
				//go.transform.localScale = new Vector3(1, 1, 1); // this.transform.localScale;	// ?  Why is the scale strange?
			}
		}

		public void SafeSetActive(GameObject go, bool activeFlag)
		{
			if (go != null)
			{
				go.SetActive(activeFlag);
			}
		}

		/*public void OnGUI()
		{
			return;

			int x = 20;
			int y = 30;
			int h = 30;
			int w = 200;

			string s = "Cursor: '" + CurrentCustomCursorName + "'";
			GUI.Box(new Rect(x, y, w, h), s);
			y += h;
			s = "Hand in view: " + HandInView + ", " + (FingerUp ? "Up" : "Down");
			GUI.Box(new Rect(x, y, w, h), s);
		}*/


		public void UpdateCursorBasedOnState()
		{
			// Show appropriate accent cursor
			if (CurrentCustomCursor != null)
			{
				if (FingerUp == true)
				{
					SafeSetActive(CurrentCustomCursor.UpStateObject, true);
					SafeSetActive(CurrentCustomCursor.DownStateObject, false);
				}
				else
				{
					SafeSetActive(CurrentCustomCursor.UpStateObject, false);
					SafeSetActive(CurrentCustomCursor.DownStateObject, true);
				}
			}

			// Set state for main cursor
			bool normalShow = false;
			bool upShow = false;
			bool downShow = false;
			bool FOGshow = false;
			if (HandInView == false)
			{
				// Normal cursor
				normalShow = true;
			}
			else
			{
				// Hand cursors
				upShow = FingerUp;
				downShow = !FingerUp;
			}
			FOGshow = this.FOGMode;

			SafeSetActive(NormalObject, normalShow);
			SafeSetActive(UpObject, upShow);
			SafeSetActive(DownObject, downShow);
			SafeSetActive(FOGObject, FOGshow);
		}

        // For the General Cursor we want to simply override these states
        public override void OnStateChange(CursorState state)
        {
			if (cursorAnim != null)
			{
				cursorAnim.SetInteger("CursorState", (int)state);
			}
            base.OnStateChange(state);
        }


		void FixedUpdate()
        {
            Vector3 newPos = transform.position;
            Quaternion newRot = transform.rotation;

            //Over Nothing
            if (Focuser.PrimeFocus == null)
            {
                RaycastHit newHit;
                Ray newRay = Focuser.FocusRay;

				SetCursorByName("none");

				if (!UseGazerRay && Physics.Raycast(newRay, out newHit))
                {
                    newPos = newHit.point + Vector3.Scale(newHit.normal, new Vector3(cursorHitOffset, cursorHitOffset, cursorHitOffset));
                    newRot = Quaternion.FromToRotation(transform.forward, newHit.normal) * transform.rotation;
                    transform.localScale = new Vector3(newHit.distance * cursorSize, newHit.distance * cursorSize, newHit.distance * cursorSize);
				}
                else
                {
                    Transform head = Veil.Instance.HeadTransform;
                    Ray middleEyeRay;
                    middleEyeRay = new Ray(head.position, head.rotation * Vector3.forward);
                    newPos = middleEyeRay.GetPoint(hitNothingDistance);
                    newRot = Quaternion.LookRotation(head.position - transform.position, Vector3.up);
                    transform.localScale = new Vector3(hitNothingDistance * cursorSize, hitNothingDistance * cursorSize, hitNothingDistance * cursorSize);

					//////// Uncomment for done cursor functionality. PrototypeShell must exist in order for it to work
					//if (PrototypeShell.Instance.InOrganizeMode == true)
					//{
					//	SetCursorByName("Done");
					//}
                }
            }
            else //Over an interactible object.
            {
                newPos = cursorHit.point + Vector3.Scale(cursorHit.normal, new Vector3(cursorHitOffset, cursorHitOffset, cursorHitOffset));
				Transform head = Veil.Instance.HeadTransform;
				Quaternion lookRotation = Quaternion.LookRotation(head.position - transform.position, Vector3.up);
				Quaternion facingRotation = Quaternion.FromToRotation(transform.forward, cursorHit.normal) * transform.rotation;
				newRot = Quaternion.Slerp(lookRotation, facingRotation, 0.0f);	//
                transform.localScale = new Vector3(cursorHit.distance * cursorSize, cursorHit.distance * cursorSize, cursorHit.distance * cursorSize);

				// The PinsSlateInteractible tells the cursor whether it's looking at the object, one of its corners, or nothing
				ICursorInterface cursorObject = Focuser.PrimeFocus.GetComponent<ICursorInterface>();
				if (cursorObject != null)
				{
					SetCursorByName(cursorObject.GetCursor());
				}
            }

#if CLASSIC_INTERP
			transform.position = Vector3.Slerp(transform.position, newPos, Time.fixedDeltaTime * 25.0f);
			transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.fixedDeltaTime * 25.0f);
#else
			float blendValue = 0.82f;
			transform.position = blendValue * transform.position + (1 - blendValue) * newPos;
			transform.rotation = Quaternion.Slerp(transform.rotation, newRot, blendValue);
#endif // CLASSIC_INTERP

			// Basic cursor
			CheckState();
			UpdateCursorBasedOnState();
			SetCustomCursorPosition();
			SetCursorPosition();
        }
    }
}
