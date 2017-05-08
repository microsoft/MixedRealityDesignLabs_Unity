//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections.Generic;
using HUX.Cursors;
using HUX.Utility;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HUX.Focus
{
	public abstract class AFocuser : MonoBehaviour
	{
		//--------------------------------------------------------------------------------------

		#region Private Variables
		/// <summary>
		/// The maximum expected number of hits when we do a raycast.
		/// </summary>
		protected const int ExpectedMaxRaycasts = 64;

		/// <summary>
		/// A pool of Focus Info so that we do not recreate them each frame.
		/// </summary>
		protected NewableObjectPool<FocusInfo> m_FocusInfoPool = new NewableObjectPool<FocusInfo>(ExpectedMaxRaycasts);

		/// <summary>
		/// A list for tracking hits with the focus ray.
		/// </summary>
		protected List<FocusInfo> m_FocusedItems = new List<FocusInfo>(ExpectedMaxRaycasts);

		/// <summary>
		/// The raycast hit array for the focus ray.  This is to avoid memory allocations each frame.
		/// </summary>
		protected RaycastHit[] m_ColliderRaycastHits = new RaycastHit[ExpectedMaxRaycasts];

		/// <summary>
		/// A list of raycast hit results for the Unity UI system
		/// </summary>
		protected List<UnityEngine.EventSystems.RaycastResult> m_UIRaycastResults = new List<UnityEngine.EventSystems.RaycastResult>();

		/// <summary>
		/// The Unity UI event systems pointer data.
		/// </summary>
		private WorldGraphicsRaycaster.RayEventData m_PointerData;

        /// <summary>
		/// The game object that focus has been locked to by calls to LockFocus
		/// </summary>
		private GameObject m_LockedFocus = null;

        protected bool m_IsManipulating = false;

		protected Transform m_ManipulationTransform = null;
		#endregion

		//--------------------------------------------------------------------------------------

		#region Object Accessors

		/// <summary>
		/// The cursor for this focuser.
		/// </summary>
		public HUX.Cursors.Cursor Cursor { get; protected set; }

        /// <summary>
        /// The cursor prefab for this focuser.
        /// </summary>
        [Tooltip("The prefab to use for the cursor.  Should have a cursor component at the base level.")]
        public HUX.Cursors.Cursor CursorPrefab;

        /// <summary>
        /// The animated cursor for this focuser.
        /// </summary>
        public StateWidget AnimatedCursor
		{
			get
			{
				AnimCursor cursor = Cursor.GetComponent<AnimCursor>();
				return cursor ? cursor.GetWidgetByName("Organize") as StateWidget : null;
			}
		}

		#endregion

		//--------------------------------------------------------------------------------------

		#region Focus Objects Accessors

		/// <summary>
		/// Position of the user's gaze.
		/// </summary>
		public Vector3 Position { get; protected set; }

		/// <summary>
		/// RaycastHit Normal direction.
		/// </summary>
		public Vector3 Normal { get; protected set; }

		/// <summary>
		/// Physics.Raycast result is true if it hits a Hologram.
		/// </summary>
		public bool Hit { get; protected set; }

		/// <summary>
		/// FocusHitInfo property gives access
		/// to FocusInfo public members.
		/// </summary>
		public FocusInfo FocusHitInfo { get; protected set; }

		/// <summary>
		/// FocusRay property gives access
		/// to Ray public members.
		/// </summary>
		public Ray FocusRay { get; protected set; }

		/// <summary>
		/// The Game Object of the current Prime Focus.
		/// </summary>
		public GameObject PrimeFocus { get; protected set; }

		/// <summary>
		/// The Game Object of the current Prime Focus.
		/// </summary>
		public GameObject OldPrimeFocus { get; protected set; }

		/// <summary>
		/// The Selectable UI object.
		/// </summary>
		public GameObject UIInteractibleFocus { get; protected set; }

		/// <summary>
		/// The Selectable UI object.
		/// </summary>
		public GameObject OldUIInteractibleFocus { get; protected set; }

		/// <summary>
		/// The list of objects being focused on.
		/// </summary>
		public List<GameObject> FocusList { get; protected set; }

		/// <summary>
		/// The list of objects that have lost focus this frame.
		/// </summary>
		public List<GameObject> FocusExitList { get; protected set; }

		/// <summary>
		/// The list of objects that have gained focus this frame.
		/// </summary>
		public List<GameObject> FocusEnterList { get; protected set; }

		/// <summary>
		/// Return true if the focuser is flagged as manipulating an object.
		/// </summary>
		public bool IsManipulating
		{
			get
			{
				return m_IsManipulating;
			}
		}
		#endregion

		//--------------------------------------------------------------------------------------

		#region Abstract Accessors

		/// <summary>
		/// The world origin of the targeting ray
		/// </summary>
		public abstract Vector3 TargetOrigin { get; }

		/// <summary>
		/// The forward direction of the targeting ray
		/// </summary>
		public abstract Vector3 TargetDirection { get; }

		/// <summary>
		/// The orientation of the focuser.
		/// </summary>
		public abstract Quaternion TargetOrientation { get; }

		/// <summary>
		/// Returns true if the select button for this focuser is held down.
		/// </summary>
		public abstract bool IsSelectPressed { get; }

		/// <summary>
		/// Return true if the focuser is ready to interact. (For example if the ready gesture is detected on a hololens hand.)
		/// </summary>
		public abstract bool IsInteractionReady { get; }

		/// <summary>
		/// Return true if this focuser allow for interaction and should be used for the main focus list.
		/// </summary>
		public abstract bool CanInteract { get; }

		#endregion

		//--------------------------------------------------------------------------------------

		#region Monobehaviour Functions
		protected virtual void Awake()
		{
			FocusList = new List<GameObject>();
			FocusExitList = new List<GameObject>();
			FocusEnterList = new List<GameObject>();
			FocusHitInfo = new FocusInfo();

            if(CursorPrefab != null)
            {
                SetCursor(CursorPrefab);
            }

        }

		protected virtual void Update()
		{
			if (m_IsManipulating && !IsSelectPressed)
			{
				this.StopManipulation();
			}
		}

        #endregion

        //--------------------------------------------------------------------------------------

        #region Public Functions

        /// <summary>
        /// Adds a widget to the cursor
        /// This enables devs to add widgets without modifying the global cursor prefab
        /// </summary>
        /// <param name="bActive"> Should the cursor be active. </param>		
        public void AddWidgetsToCursor(GameObject[] cursorWidgetPrefabs)
        {
            // Get cursor widget data from the prefabs
            Cursors.Cursor.CursorWidgetData[] widgets = new Cursors.Cursor.CursorWidgetData[cursorWidgetPrefabs.Length];
            for (int i = 0; i < cursorWidgetPrefabs.Length; i++)
            {
                CursorWidget cw = cursorWidgetPrefabs[i].GetComponent<CursorWidget>();
                Cursors.Cursor.CursorWidgetData widgetData = new Cursors.Cursor.CursorWidgetData();
                widgetData.CursorWidgetPrefab = cw;
                widgetData.Name = cw.name;
                widgets[i] = widgetData;
            }

            List<Cursors.Cursor.CursorWidgetData> widgetList = new List<Cursors.Cursor.CursorWidgetData>(widgets);
            widgetList.AddRange(Cursor.CursorWidgets);
            Cursor.CursorWidgets = widgetList.ToArray();

            // Now that we've added more cursor widgets, initialize the cursor again
            // This will instantiate any missing widgets
            Cursor.InitWidgets();
        }

        /// <summary>
        /// Set the display cursor for this focuser.
        /// </summary>
        /// <param name="cursorPrefab">The prefab to instantiate.</param>
        public void SetCursor(HUX.Cursors.Cursor cursorPrefab)
		{
			Vector3 currentPosition = Vector3.zero;
			if (Cursor != null)
			{
				currentPosition = Cursor.transform.position;
				DestroyObject(Cursor.gameObject);
				Cursor = null;
			}

			//Create the cursor and give ourselves to it as reference.
			if (cursorPrefab != null)
			{
				Cursor = GameObject.Instantiate(cursorPrefab);
				Cursor.SetFocuser(this);
				Cursor.transform.position = currentPosition;
			}
		}

		/// <summary>
		/// Sets if the cursor is visible.
		/// </summary>
		/// <param name="bActive"></param>
		public void SetCursorActive(bool bActive)
		{
			Cursor.gameObject.SetActive(bActive);
		}


		/// <summary>
		/// Locks the focus to the current Prime Focus.
		/// </summary>
		public void LockFocus()
		{
            m_LockedFocus = PrimeFocus;
		}

		/// <summary>
		/// Releases the Locked Focus if there is one.
		/// </summary>
		public void ReleaseFocus()
		{
            m_LockedFocus = null;
		}

		public void StartManipulation(Transform frame = null)
		{
			if (!m_IsManipulating)
			{
				m_IsManipulating = true;
				m_ManipulationTransform = frame;
				this.LockFocus();

				this.OnManipulationStarted(frame);
			}
		}

		public void StopManipulation()
		{
			if (m_IsManipulating)
			{
				this.ReleaseFocus();
				this.OnManipulationStopped();
				m_ManipulationTransform = null;
				m_IsManipulating = false;
			}
		}

		/// <summary>
		/// Calculates the Focus hit position and normal.
		/// </summary>
		public void UpdateRaycast()
		{
			OnPreRaycast();

			Hit = false;
			FocusInfo hitInfo = null;
			FocusRay = new Ray(TargetOrigin, TargetDirection);

			// This must be called at the start of the function, as functions called later in the frame rely on the data.
			CleanFocusedItemCollection();

			// Add UI items
			bool hitUI = GetUIRaycasts(ref m_FocusedItems);
			FocusInfo mainUIFocus = null;
			if (hitUI && m_FocusedItems.Count > 0)
			{
				mainUIFocus = m_FocusedItems[0];
			}

			// Perform a preemptive raycast to determine if we are looking at the SR or nothing.
			// If we are doing either, return out of the function early to avoid doing the extra raycast.
			// Bug-Fix: This also fixed a bug where the SR would not allow the pins panel to be opened.
			if (!hitUI)
			{
				RaycastHit hit;
				bool hitSomething = Physics.Raycast(FocusRay, out hit, FocusManager.Instance.MaxGazeDistance, FocusManager.Instance.RaycastLayerMask);
				if (!hitSomething || hit.transform.gameObject.layer == LayerMask.NameToLayer("SR"))
				{
					if (hitSomething)
					{
						Position = hit.point;
						Normal = hit.normal;
					}
					else
					{
						Position = TargetOrigin + TargetDirection * FocusManager.Instance.MaxGazeDistance;
						Normal = TargetDirection;
					}
					return;
				}
			}

			// Add colliders 
			bool hitCollider = GetColliderRaycasts(ref m_FocusedItems);

			// Check if we hit anything
			if (hitCollider || hitUI)
			{
				// Sort via distance
				m_FocusedItems.Sort((FocusInfo focusInfo1, FocusInfo focusInfo2) => {
					int ret = focusInfo1.distance.CompareTo(focusInfo2.distance);
					if (ret == 0)
					{
						Graphic graph1 = focusInfo1.gameObject.GetComponent<Graphic>();
						Graphic graph2 = focusInfo2.gameObject.GetComponent<Graphic>();
						if (graph1 != null && graph2 != null)
						{
							ret = graph2.depth.CompareTo(graph1.depth);
						}
					}

					return ret;
				});

				// Get the closest valid collider
				for (int i = 0; i < m_FocusedItems.Count; ++i)
				{
					FocusInfo focusInfo = m_FocusedItems[i];

					// Check if the transform has a valid filter
					if (FocusManager.Instance.ContainsValidFilter(focusInfo.transform))
					{
						if (focusInfo.isUI && mainUIFocus != null)
						{
							focusInfo = mainUIFocus;
						}

						hitInfo = focusInfo;
						Hit = true;
						break;
					}
				}
			}

			if (hitInfo != null)
			{
				// The raycast hit a valid hologram
				FocusHitInfo = hitInfo;
				Position = hitInfo.point;
				Normal = hitInfo.normal;
			}
			else
			{
				// If raycast did not hit a hologram... Save defaults
				Position = TargetOrigin + (TargetDirection * FocusManager.Instance.MaxGazeDistance);
				Normal = TargetDirection;
			}

			OnPostRaycast();
		}

		/// <summary>
		/// Update the game object being focused on
		/// </summary>
		public void UpdateFocus()
		{
			OldPrimeFocus = PrimeFocus;
			PrimeFocus = null;
			OldUIInteractibleFocus = UIInteractibleFocus;
			UIInteractibleFocus = null;

            if (m_LockedFocus != null)
            {
                PrimeFocus = m_LockedFocus;
            }
			else if (Hit && FocusHitInfo.transform != null)
			{
				PrimeFocus = FocusHitInfo.transform.gameObject;
			}

			// Bubble up from the old focused object and record all object that should get the GazeExited message
			if (OldPrimeFocus != null)
			{
				Transform trans = OldPrimeFocus.transform;
				do
				{
					FocusExitList.Add(trans.gameObject);
					GetNextValidParent(ref trans);

				} while (trans != null);
			}

			FocusList.Clear();

			if (PrimeFocus != null)
			{
				Transform trans = PrimeFocus.transform;
				do
				{
					bool found = FocusExitList.Remove(trans.gameObject);
					if (!found)
					{
						FocusEnterList.Add(trans.gameObject);
					}

					FocusList.Add(trans.gameObject);

					if (UIInteractibleFocus == null)
					{
						Selectable foundSelectable = trans.gameObject.GetComponent<Selectable>();
						IEventSystemHandler eventHandler = trans.gameObject.GetComponent<IEventSystemHandler>();
						if (eventHandler != null)
						{
							UIInteractibleFocus = (eventHandler as Component).gameObject;
						}
						else if (foundSelectable != null && foundSelectable.IsInteractable())
						{
							UIInteractibleFocus = foundSelectable.gameObject;
						}
					}

					GetNextValidParent(ref trans);
				} while (trans != null);
			}
		}

		public PointerEventData GetPointerData()
		{
			if (m_PointerData == null)
			{
				m_PointerData = new WorldGraphicsRaycaster.RayEventData(EventSystem.current, this);
			}

			m_PointerData.EventRay = new Ray(TargetOrigin, TargetDirection);
			m_PointerData.position = new Vector2(Camera.main.pixelWidth * 0.5f, Camera.main.pixelHeight * 0.5f);
			if (FocusHitInfo != null)
			{
				m_PointerData.pointerCurrentRaycast = FocusHitInfo.raycastResult;
				m_PointerData.pointerPressRaycast = FocusHitInfo.raycastResult;
				m_PointerData.position = Camera.main.WorldToScreenPoint(FocusHitInfo.raycastResult.worldPosition);
			}

			m_PointerData.pressPosition = m_PointerData.position;

			return m_PointerData;
		}

		#endregion

		//--------------------------------------------------------------------------------------

		#region Protected Functions

		protected virtual void OnPreRaycast()
		{

		}

		protected virtual void OnPostRaycast()
		{

		}

		protected virtual void OnManipulationStarted(Transform frame)
		{

		}

		protected virtual void OnManipulationStopped()
		{

		}

		#endregion


		//--------------------------------------------------------------------------------------

		#region Private Functions
		/// <summary>
		/// Clears out the focused item in preparation for the next pass.
		/// </summary>
		private void CleanFocusedItemCollection()
		{
			for (int i = 0; i < m_FocusedItems.Count; i++)
			{
				m_FocusedItems[i].ReturnToPool();
			}

			m_FocusedItems.Clear();
		}

		/// <summary>
		/// Gets the next parent of a transform that has a valid filter.
		/// </summary>
		/// <param name="transform"> The transform that will be set the it's next valid parent. </param>
		private void GetNextValidParent(ref Transform transform)
		{
			// Get next valid parent
			while (transform != null)
			{
				transform = transform.parent;

				// Check if the transform has a valid filter
				if (transform == null || FocusManager.Instance.ContainsValidFilter(transform))
				{
					break;
				}
			}
		}

		/// <summary>
		/// Will fill the list will all colliders from physics raycast.
		/// </summary>
		/// <param name="focusedItems">The list to be filled.</param>
		/// <returns>Did the raycast hit anything.</returns>
		private bool GetColliderRaycasts(ref List<FocusInfo> focusedItems)
		{
			// Get world collider raycasts
			int numberOfHits = Physics.RaycastNonAlloc(FocusRay, m_ColliderRaycastHits, FocusManager.Instance.MaxGazeDistance, FocusManager.Instance.RaycastLayerMask);

			bool hit = numberOfHits > 0;
			if (hit)
			{
				for (int i = 0; i < numberOfHits; ++i)
				{
					RaycastHit raycastHit = m_ColliderRaycastHits[i];
					FocusInfo focusInfo = m_FocusInfoPool.GetObject();
					focusInfo.Set(raycastHit.transform.gameObject, raycastHit.distance, raycastHit.point, raycastHit.normal, raycastHit.transform, raycastHit.textureCoord);
					focusedItems.Add(focusInfo);
				}
			}

			return hit;
		}

		/// <summary>
		/// Will fill the list with UI elements from EventSystem raycast.
		/// </summary>
		/// <param name="focusedItems">The list to be filled.</param>
		/// <returns>Did the raycast hit anything.</returns>
		private bool GetUIRaycasts(ref List<FocusInfo> focusedItems)
		{
			bool hit = false;

			// Get UI raycasts
			m_UIRaycastResults.Clear();
			if (EventSystem.current != null)
			{
				EventSystem.current.RaycastAll(GetPointerData(), m_UIRaycastResults);
				hit = hit ? hit : (m_UIRaycastResults != null && m_UIRaycastResults.Count > 0);

				if (hit)
				{
					// Add UI hits
					for (int i = 0; i < m_UIRaycastResults.Count; ++i)
					{
						UnityEngine.EventSystems.RaycastResult raycastResult = m_UIRaycastResults[i];

						float distance = raycastResult.distance;

						if (distance <= FocusManager.Instance.MaxGazeDistance)
						{
							FocusInfo focusInfo = m_FocusInfoPool.GetObject();
							focusInfo.SetUI(raycastResult.gameObject, true, raycastResult.index, distance, TargetOrigin + (TargetDirection.normalized * distance), -raycastResult.gameObject.transform.forward, raycastResult.gameObject.transform, Vector2.zero, raycastResult);
							focusedItems.Add(focusInfo);
						}
					}

					// Sort via render order
					focusedItems.Sort((FocusInfo a, FocusInfo b) => {
						if (a.raycastResult.sortingLayer != b.raycastResult.sortingLayer)
						{
							return a.raycastResult.sortingLayer.CompareTo(b.raycastResult.sortingLayer);
						}

						if (a.raycastResult.sortingOrder != b.raycastResult.sortingOrder)
						{
							return b.raycastResult.sortingOrder.CompareTo(a.raycastResult.sortingOrder);
						}

						bool distanceSimilar = Mathf.Approximately(a.distance, b.distance);
						if (distanceSimilar)
						{
							return a.raycastResult.index.CompareTo(b.raycastResult.index);
						}
						return a.distance.CompareTo(b.distance);
					});
				}
			}

			return hit;
		}
		#endregion
	}
}
