//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using HUX.Interaction;
using HUX.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HUX.Focus
{
    /// <summary>
    /// Focus Manager is a Singleton class for handling what is in focus for the user.
    /// </summary>
    public class FocusManager : Singleton<FocusManager>
	{
		#region Variables

		// Changing filters when gazing at objects could causes objects to not recieve FocusEnter or FocusExit calls.
		[Tooltip("Filter the current string tags used when selecting focus and sending messages.")]
		public FilterTags FilterTags;

		[Tooltip("Maximum gaze distance for calculating a hit.")]
		public float MaxGazeDistance = 5.0f;

		[Tooltip("Radius in meters of the user sphere on which apps float")]
		public float ShellRadius = 3.5f;

		[Tooltip("Select the layers raycast should target.")]
		public LayerMask RaycastLayerMask = Physics.DefaultRaycastLayers;

		[Tooltip("Use the focal point update with UnityEngine.VR.WSA.HolographicSettings.SetFocusPointForFrame")]
		public bool m_UseFocalPoint = true;

		[Tooltip("Use the fixed location or if null use the cursor position")]
		public Vector3 m_FixedFocalPoint;

		[Tooltip("Use active pointer focus or gaze focus")]
		[DebugBoolTunable]
		public bool m_VoiceGazeFocus = true;

		[Header("Focusers")]
		/// <summary>
		/// The gaze focuser.
		/// </summary>
		[SerializeField]
		private AFocuser m_GazeFocuser;

		/// <summary>
		/// The focusers being used
		/// </summary>
		[SerializeField]
		private AFocuser[] m_Focusers;

		public delegate void UpdateEvent();

		/// <summary>
		/// The update event that is fired after the focuser have been updated.
		/// </summary>
		public static event UpdateEvent OnUpdate;


		/// <summary>
		/// Events that trigger when a new Prime Focus is in view.
		/// </summary>
		/// <param name="GameObject"> The game object that has gained or lost focus. </param>
		public static event Action<GameObject, FocusArgs> OnFocusEnter;
		public static event Action<GameObject, FocusArgs> OnFocusExit;

		/// <summary>
		/// The list of currently focused objects.
		/// </summary>
		private List<GameObject> m_FocusedList = new List<GameObject>();

		/// <summary>
		/// The list of objects focused last frame.
		/// </summary>
		private List<GameObject> m_PrevFocusedList = new List<GameObject>();

		/// <summary>
		/// List of current focus objects and number of focusers referencing them.
		/// </summary>
		private Dictionary<GameObject, int> m_ObjectFocusCount = new Dictionary<GameObject, int>();
		#endregion

		//----------------------------------------------------------------------

		#region Accessors
		/// <summary>
		/// The focuser attached to the gaze.
		/// </summary>
		public AFocuser GazeFocuser
		{
			get
			{
				return m_GazeFocuser;
			}
		}

		/// <summary>
		/// All focusers currently active.
		/// </summary>
		public AFocuser[] Focusers
		{
			get
			{
				return m_Focusers;
			}
		}

		/// <summary>
		/// All objects that are currently focused by all focusers.
		/// </summary>
		public List<GameObject> FocusedObjects
		{
			get
			{
				return m_FocusedList;
			}
		}

		public PointerEventData UIEventPointerData { get; private set; }
		#endregion

		//----------------------------------------------------------------------

		#region Monobehaviour Functions
		/// <summary>
		/// Awake function for instancing objects and initialization 
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			FilterTags.AddTag(FilterTag.DefaultTag);
		}

		/// <summary>
		/// Update function to track focus and interaction
		/// </summary>
		private void Update()
		{
			// Update focus
			for (int index = 0; index < m_Focusers.Length; index++)
			{
				if (m_Focusers[index] != null)
				{
					m_Focusers[index].UpdateRaycast();
					m_Focusers[index].UpdateFocus();
				}
			}

			UpdateFocus();

#if UNITY_WSA
			// Add focus point set to cursor for frame
			if (m_UseFocalPoint && m_GazeFocuser != null && m_GazeFocuser.Cursor != null && m_FixedFocalPoint != Vector3.zero)
			{
				Vector3 focalPoint = m_FixedFocalPoint != Vector3.zero ? m_FixedFocalPoint : m_GazeFocuser.Cursor.transform.position;
				UnityEngine.VR.WSA.HolographicSettings.SetFocusPointForFrame(focalPoint);
			}
#endif

			if (OnUpdate != null)
				OnUpdate();
		}

		#endregion

		//----------------------------------------------------------------------

		#region Public Functions

		/// <summary>
		/// Sets all the cursors active or inactive.
		/// </summary>
		/// <param name="bActive"> Should the cursor be active. </param>		
		public void SetAllCursorsActive(bool bActive)
		{
			for (int index = 0; index < m_Focusers.Length; index++)
			{
				if (m_Focusers[index] != null && m_Focusers[index].Cursor != null)
				{
					m_Focusers[index].Cursor.gameObject.SetActive(bActive);
				}
			}
		}

		/// <summary>
		/// Get the snap position and rotation in front of the gaze
		/// </summary>
		/// <param name="pos"> The snap to position. </param>
		/// <param name="rot"> The snap to rotation. </param>
		public void GetSnapTo(out Vector3 pos, out Quaternion rot)
		{
			pos = GazeFocuser.FocusRay.origin + GazeFocuser.FocusRay.direction * ShellRadius;
			rot = Quaternion.LookRotation(GazeFocuser.FocusRay.direction, Vector3.up);
		}

		/// <summary>
		/// Does the transform contain a valid filter. No filter is a valid filter.
		/// </summary>
		/// <param name="transform"> The transform that will be tested for a valid filter. </param>
		/// <returns></returns>
		public bool ContainsValidFilter(Transform transform)
		{
			InteractibleObject interactibleObject = transform.GetComponent<InteractibleObject>();
			if (interactibleObject != null)
			{
				return FilterTags.ValidTags.Contains(interactibleObject.FilterTag.Tag);
			}
			return true;
		}

		public void ExecuteUIFocusEvent<T>(GameObject obj, PointerEventData eventData, ExecuteEvents.EventFunction<T> executeEvent)
			where T : IEventSystemHandler
		{
			if (obj != null)
			{
				UIEventPointerData = eventData;
				ExecuteEvents.Execute(obj, eventData, executeEvent);
				UIEventPointerData = null;
			}
		}

		#endregion

		//----------------------------------------------------------------------

		#region Private Functions

		/// <summary>
		/// Update the game object being focused on
		/// </summary>
		private void UpdateFocus()
		{
			m_PrevFocusedList.AddRange(m_FocusedList);
			m_FocusedList.Clear();

			for (int index = 0; index < m_Focusers.Length; index++)
			{
				AFocuser focuser = m_Focusers[index];
				if (focuser != null && focuser.CanInteract)
				{
					this.UpdateFocusList(focuser);
				}
			}

			for (int index = 0; index < m_Focusers.Length; index++)
			{
				AFocuser focuser = m_Focusers[index];
				if (focuser != null && focuser.CanInteract)
				{
					this.UpdateFocusEnter(focuser);
				}
			}

			for (int index = 0; index < m_Focusers.Length; index++)
			{
				AFocuser focuser = m_Focusers[index];
				if (focuser != null && focuser.CanInteract)
				{
					this.UpdateFocusExit(focuser);
				}
			}

			FocusArgs args = new FocusArgs();
			for (int index = 0; index < m_Focusers.Length; index++)
			{
				AFocuser focuser = m_Focusers[index];
				if (focuser != null && focuser.CanInteract && focuser.PrimeFocus != focuser.OldPrimeFocus)
				{
					args.Focuser = focuser;

					if (focuser.OldUIInteractibleFocus != focuser.UIInteractibleFocus)
					{
						PointerEventData pointerEventData = focuser.GetPointerData();

						ExecuteUIFocusEvent(focuser.OldUIInteractibleFocus, pointerEventData, ExecuteEvents.pointerExitHandler);
						pointerEventData.pointerEnter = focuser.UIInteractibleFocus;
						ExecuteUIFocusEvent(focuser.UIInteractibleFocus, pointerEventData, ExecuteEvents.pointerEnterHandler);
					}

					// Invoke events
					if (OnFocusExit != null && focuser.OldPrimeFocus != null)
					{
						args.CurNumFocusers = m_ObjectFocusCount[focuser.OldPrimeFocus];
						OnFocusExit(focuser.OldPrimeFocus, args);
					}

					if (OnFocusEnter != null && focuser.PrimeFocus != null)
					{
						args.CurNumFocusers = m_ObjectFocusCount[focuser.PrimeFocus];
						OnFocusEnter(focuser.PrimeFocus, args);
					}
				}
			}

			// Clean up
			m_ObjectFocusCount.Clear();

			m_PrevFocusedList.Clear();
		}

		private void UpdateFocusList(AFocuser focuser)
		{
			for (int index = 0; index < focuser.FocusList.Count; index++)
			{
				GameObject obj = focuser.FocusList[index];
				if (!m_FocusedList.Contains(obj))
				{
					m_FocusedList.Add(obj);
				}

				if (!focuser.FocusEnterList.Contains(obj))
				{
					UpdateObjectCount(obj, 1);
				}
			}
		}

		private int UpdateObjectCount(GameObject obj, int change)
		{
			int count = 0;
			if (m_ObjectFocusCount.ContainsKey(obj))
			{
				count = m_ObjectFocusCount[obj];
			}

			count += change;
			if (count < 0)
			{
				count = 0;
			}
			m_ObjectFocusCount[obj] = count;
			return count;
		}

		private void UpdateFocusEnter(AFocuser focuser)
		{
			FocusArgs arg = new FocusArgs();
			arg.Focuser = focuser;
			for (int index = 0; index < focuser.FocusEnterList.Count; index++)
			{
				GameObject obj = focuser.FocusEnterList[index];
				arg.CurNumFocusers = UpdateObjectCount(obj, 1);

				obj.SendMessage("FocusEnter", arg, SendMessageOptions.DontRequireReceiver);
			}

			focuser.FocusEnterList.Clear();
		}

		private void UpdateFocusExit(AFocuser focuser)
		{
			FocusArgs arg = new FocusArgs();
			arg.Focuser = focuser;
			for (int index = 0; index < focuser.FocusExitList.Count; index++)
			{
				GameObject obj = focuser.FocusExitList[index];
				arg.CurNumFocusers = UpdateObjectCount(obj, -1);

				obj.SendMessage("FocusExit", arg, SendMessageOptions.DontRequireReceiver);
			}

			focuser.FocusExitList.Clear();
		}
		#endregion
	}
}
