//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using HUX.Interaction;
using HUX.Utility;
using HUX.Receivers;
using HUX.Focus;

namespace HUX.Cursors
{
    /// <summary>
    /// This is the base cursor abstract class.
    /// </summary>
    public abstract class Cursor : MonoBehaviour
    {

        /// <summary>
        /// Enum for cursor states based on Shell support
        /// </summary>
        public enum CursorState
        {
            /// <summary>
            /// Useful for releasing external override.
            /// See <c>CursorState.Contextual</c>
            /// </summary>
            None = -1,
            /// <summary>
            /// not HandVisible
            /// </summary>
            Observe,
            /// <summary>
            /// HandVisible AND not IsFingerPressed AND CurrentInteractibleObject is NULL
            /// </summary>
            Interact,
            /// <summary>
            /// HandVisible AND not IsFingerPressed AND CurrentInteractibleObject exists
            /// </summary>
            Hover,
            /// <summary>
            /// HandVisible AND IsFingerPressed
            /// </summary>
            Select,
            /// <summary>
            /// Available for use by classes that extend Cursor.
            /// No logic for setting Release state exists in the base Cursor class.
            /// </summary>
            Release,
            /// <summary>
            /// Allows for external override
            /// </summary>
            Contextual
        }

        /// <summary>
        /// Current State of the Cursor.
        /// </summary>
        [Tooltip("State of the cursor")]
        public CursorState currentState = CursorState.Observe;

        /// <summary>
        /// Distance to keep the cursor if nothing is hit.
        /// </summary>
        [Tooltip("If no surface is found place cursor at this distance")]
        public float hitNothingDistance = 1.5f;

        /// <summary>
        /// Cursor size to maintain when angular scaling the cursor.
        /// </summary>
        [Tooltip("Relative size to keep cursor when scaling based on distance")]
        public float cursorSize = 0.25f;

        /// <summary>
        /// Offset from the surface of hit target to keep the cursor.
        /// </summary>
        [Tooltip("Offset from the raycast hit location to place the cursor")]
        public float cursorHitOffset = 0.02f;

        /// <summary>
        /// Time to use for lerping to desired position.
        /// </summary>
        [Tooltip("Lerp time for position interpolation")]
        public float positionLerpTime = 0.05f;

        /// <summary>
        /// Time to use for lerping to desired rotation.
        /// </summary>
        [Tooltip("Lerp time for rotation interpolation")]
        public float rotationLerpTime = 0.05f;

        /// <summary>
        /// Time to use for lerping to desired rotation.
        /// </summary>
        [Tooltip("Lerp time for scale interpolation")]
        public float scaleLerpTime = 0.01f;

        /// <summary>
        /// Bool for whether or not to rotate to face surface normal
        /// </summary>
        [Tooltip("Should the cursor rotate to surface facing")]
        public bool RotateToSurface = false;

        /// <summary>
        /// Bool for whether or not to zero the Y hit normal
        /// </summary>
        [Tooltip("Should we zero out the Y axis on the hit normal")]
        public bool ignoreYHitNormal = true;

        /// <summary>
        /// Blend amount from hit normal to head facing.
        /// </summary>
        [Tooltip("Surface to user facing blend value")]
        public float surfaceBlendValue = 0.0f;


        /// <summary>
        /// Widget data container class, keeps name, prefab and object
        /// </summary>
        [System.Serializable]
        public class CursorWidgetData
        {
            public string Name;
            public CursorWidget CursorWidgetPrefab;
            public CursorWidget CursorWidgetObject { get; set; }
        }

        /// <summary>
        /// Collection of widget data for the cursor.
        /// </summary>
        [Header("Cursor Widget Data")]
        public CursorWidgetData[] CursorWidgets;

        /// <summary>
        /// Publically accessible but hidden hit info.
        /// </summary>
        [HideInInspector]
        public FocusInfo cursorHit;

        /// <summary>
        /// Publically accessible but hidden hit position info.
        /// </summary>
        [HideInInspector]
        public Vector3 hitPosition;

        /// <summary>
        /// Publically accessible but hidden hit direction info.
        /// </summary>
        [HideInInspector]
        public Quaternion hitDirection;

        /// <summary>
        /// Publically accessible but hidden hit scale info.
        /// </summary>
        [HideInInspector]
        public Vector3 hitScale = Vector3.one;

        /// <summary>
        /// String for cursor widget filtering.  ToDo: change to a better filter system.
        /// </summary>
        [Header("Cursor Flags")]
        [Tooltip("String for validating cursor widgets")]
        public string CursorWidgetFilter;

		/// <summary>
		/// Callback for after cursor is positioned
		/// </summary>
		public System.Action OnPostUpdateTransform = () => { };

		/// <summary>
		/// The focuser for this cursor.
		/// </summary>
		private AFocuser m_Focuser;

		public AFocuser Focuser
		{
			get
			{
				return m_Focuser;
			}
		}

		/// <summary>
		/// Initialize cursor widgets and cache the gazer.
		/// </summary>
		public virtual void Start()
        {
            InitWidgets();

            // Trigger the initial state
            OnStateChange(currentState);
        }

        /// <summary>
        /// Clean up our widgets when this cursor is destroyed
        /// </summary>
        public void OnDestroy()
        {
            foreach (CursorWidgetData data in CursorWidgets)
            {
                if (data.CursorWidgetObject)
                {
                    Destroy(data.CursorWidgetObject.gameObject);
                }
            }
        }

        /// <summary>
        /// Init widgets goes through and validates if a widget
        /// needs to be spawned or already exists.
        /// </summary>
        public void InitWidgets()
        {
            foreach (CursorWidgetData data in CursorWidgets)
            {
                // Only instantiate widgets that don't exist yet, in case more have been added
                if (data.CursorWidgetPrefab != null && data.CursorWidgetObject == null)
                {
                    if (StaticExtensions.IsPrefab(data.CursorWidgetPrefab.gameObject))
                    {
                        data.CursorWidgetObject = Instantiate(data.CursorWidgetPrefab);
                    }
                    else
                    {
                        data.CursorWidgetObject = data.CursorWidgetPrefab;
                    }
                    data.CursorWidgetObject.CursorObj = this.gameObject;
                    data.CursorWidgetObject.SwitchAnchorType(data.CursorWidgetObject.AnchorType);
                    data.CursorWidgetObject.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="focuser"></param>
        public void SetFocuser(AFocuser focuser)
		{
			m_Focuser = focuser;
		}

		/// <summary>
		/// Init widgets goes through and validates if a widget
		/// needs to be spawned or already exists.
		/// </summary>
		public CursorWidget GetWidgetByName(string widgetName)
        {
            foreach (CursorWidgetData data in CursorWidgets)
            {
                if (data.Name == widgetName)
                    return data.CursorWidgetObject;
            }
            return null;
        }

        /// <summary>
        /// Late update for the cursor updating state and position.
        /// </summary>
        public virtual void LateUpdate()
        {
            if (m_Focuser != null)
            {
                cursorHit = m_Focuser.FocusHitInfo;
            }

			this.UpdateTransform();

            CursorState newState = CheckCursorState();
            if (currentState != newState)
            {
                OnStateChange(newState);
            }

            UpdateWidgets();

			OnPostUpdateTransform();
		}

        /// <summary>
        /// Virtual Function for updating the transform of the cursor
        /// </summary>
        public virtual void UpdateTransform()
        {
            Vector3 newPos = transform.position;
            Quaternion newRot = transform.rotation;
            Vector3 newScale = transform.localScale;

			
			GameObject primeFocus;
			Ray focusRay;
			Vector3 targetOrigin;

			if (m_Focuser != null)
			{
				primeFocus = m_Focuser.PrimeFocus;
				focusRay = m_Focuser.FocusRay;
				targetOrigin = m_Focuser.TargetOrigin;
			}
			else
			{
				return;
			}

			Quaternion newHeadRot = Quaternion.LookRotation(FocusManager.Instance.GazeFocuser.TargetOrigin - newPos, Vector3.up);

			//Over Nothing
			if (primeFocus == null)
            {
                RaycastHit newHit;

                if (Physics.Raycast(focusRay, out newHit))
                {
                    newPos = newHit.point + Vector3.Scale(newHit.normal, new Vector3(cursorHitOffset, cursorHitOffset, cursorHitOffset));
                    newHeadRot = Quaternion.LookRotation(targetOrigin - newPos, Vector3.up);
                    Vector3 newNorm = ignoreYHitNormal ? new Vector3(cursorHit.normal.x, 0.0f, cursorHit.normal.z) : cursorHit.normal;

                    newRot = RotateToSurface ? Quaternion.Lerp(Quaternion.FromToRotation(transform.forward, newNorm) * transform.rotation, newHeadRot, surfaceBlendValue) : newHeadRot;
                    newScale = new Vector3(newHit.distance * cursorSize, newHit.distance * cursorSize, newHit.distance * cursorSize);
                }
                else
                {
                    newPos = focusRay.GetPoint(hitNothingDistance);
                    newScale = new Vector3(hitNothingDistance * cursorSize, hitNothingDistance * cursorSize, hitNothingDistance * cursorSize);
                    newRot = Quaternion.LookRotation(targetOrigin - newPos, Vector3.up);
                }
            }
            else //Over an interactible object.
            {
                newPos = cursorHit.point + Vector3.Scale(cursorHit.normal, new Vector3(cursorHitOffset, cursorHitOffset, cursorHitOffset));
                newHeadRot = Quaternion.LookRotation(targetOrigin - newPos, Vector3.up);
                Vector3 newNorm = ignoreYHitNormal ? new Vector3(cursorHit.normal.x, 0.0f, cursorHit.normal.z) : cursorHit.normal;

                newRot = RotateToSurface ? Quaternion.Lerp(Quaternion.FromToRotation(transform.forward, newNorm) * transform.rotation, newHeadRot, surfaceBlendValue) : newHeadRot;
                newScale = new Vector3(cursorHit.distance * cursorSize, cursorHit.distance * cursorSize, cursorHit.distance * cursorSize);
            }

            transform.position = Vector3.Lerp(transform.position, newPos, Mathf.Clamp01(Time.deltaTime / positionLerpTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, Mathf.Clamp01(Time.deltaTime / rotationLerpTime));
            transform.localScale = Vector3.Slerp(transform.localScale, newScale, Mathf.Clamp01(Time.deltaTime / scaleLerpTime));
        }

        /// <summary>
        /// OnState Change callback for the cursor changing state.
        /// </summary>
        /// <param name="state">
        /// A <see cref="CursorState"/> for the State that it's currently bein changed to.
        /// </param>
        public virtual void OnStateChange(CursorState state)
        {
            currentState = state;
            foreach (CursorWidgetData widgetData in CursorWidgets)
            {
                if (widgetData.CursorWidgetObject != null)
                {
                    widgetData.CursorWidgetObject.OnStateChange(state);
                }
            }
        }

		/// <summary>
		/// Set the widgets based on widget filter
		/// </summary>
		private void UpdateWidgets()
		{
			bool cursorShouldBeVisible = true;
			foreach (CursorWidgetData widgetData in CursorWidgets)
			{
				if (widgetData.CursorWidgetObject != null)
				{
					GameObject target = null;

					if (m_Focuser != null)
					{
						target = m_Focuser.PrimeFocus;
					}
					widgetData.CursorWidgetObject.SetTargetObject(target);

					bool shouldBeActive = false;

					// Check that the cursor should be active.
					if ((CursorWidgetFilter == null || CursorWidgetFilter == "") || (CursorWidgetFilter != null && CursorWidgetFilter.Contains(widgetData.Name)))
					{
						shouldBeActive = widgetData.CursorWidgetObject.ShouldBeActive();
					}

					widgetData.CursorWidgetObject.gameObject.SetActive(shouldBeActive);

					if (shouldBeActive)
					{
						cursorShouldBeVisible = !widgetData.CursorWidgetObject.HideBaseCursor;
					}
				}
			}

			if (cursorShouldBeVisible != this.IsVisible())
			{
				//Debug.Log("Setting cursor active : " + cursorShouldBeVisible);
				this.SetVisible(cursorShouldBeVisible);
			}
		}

		public Vector3 GetCursorOrWidgetPosition()
		{
			if (IsVisible())
			{
				return transform.position;
			}

			foreach (CursorWidgetData widgetData in CursorWidgets)
			{
				if (widgetData.CursorWidgetObject != null && widgetData.CursorWidgetObject.ShouldBeActive())
				{
					return widgetData.CursorWidgetObject.transform.position;
				}
			}

			return transform.position;
		}

        public virtual bool IsVisible()
        {
            Renderer r = gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                return r.enabled;
            }
            return true;
        }

        /// <summary>
        /// Use this to hide the base cursor without desabling it
        /// </summary>
        /// <param name="visible"></param>
        public virtual void SetVisible(bool visible)
        {
            Renderer r = gameObject.GetComponent<Renderer>();
            if (r != null)
            {
                r.enabled = visible;
            }
        }

        /// <summary>
        /// Virtual function for checking state changess.
        /// </summary>
        public virtual CursorState CheckCursorState()
        {
            if (currentState != CursorState.Contextual)
            {
				if(m_Focuser != null)
				{
					if (m_Focuser.IsSelectPressed)
					{
						return CursorState.Select;
					}

					if (m_Focuser.PrimeFocus != null)
					{
						return CursorState.Hover;
					}

					if (m_Focuser.IsInteractionReady)
					{
						return CursorState.Interact;
					}
				}

				return CursorState.Observe;
            }
            return CursorState.Contextual;
        }
    }
}
