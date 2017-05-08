//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HUX.Utility
{
	/// <summary>
	/// Tag a Unity component with the SendsEvent attribute to allow the editor to pick up and add
	/// its the event to the dropdown list for the EventSender.
	/// </summary>
	/// <example>
	/// [SendsEvent("PageLoadCompleted")]
	/// [SendsEvent("LinkClicked")]
	/// class WebBrowser : MonoBehaviour
	/// {
	///		void ClickHandler()
	///		{
	///			gameObject.SendMessage("HandleEvent", "LinkClicked", SendMessageOptions.DontRequireReceiver);
	///		}
	/// }
	/// </example>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	public class SendsEvent : Attribute
	{
		/// <summary>The name of the event to send, handled by the EventSender class.</summary>
		public string EventToSend {get;set;}

		/// <summary>Constructor</summary>
		/// <param name="eventToSend"></param>
		public SendsEvent(string eventToSend)
		{
			EventToSend = eventToSend;
		}
	}

	/// <summary>
	/// Handles sending of an event (e.g. "LinkClicked" or" ButtonPressed") to a target object, and calling
	/// a function on that object, specified in editor.
	/// </summary>
	public class EventSender : MonoBehaviour
	{
		/// <summary>List of registered events to send and their targets</summary>
		[SerializeField]
		private List<EventEntry> eventEntries = new List<EventEntry>();

		/// <summary>
		/// Called by a component with the SendsEvent attribute specified. Will call a function on a target object
		/// if specified.
		/// </summary>
		/// <param name="eventName"></param>
		private void HandleEvent(string eventName)
		{
			for (int i = 0; i < eventEntries.Count; ++i)
			{
				EventEntry entry = eventEntries[i];
				if ((entry.eventName == eventName) && (entry.targetObject != null))
				{
					entry.targetObject.SendMessage(entry.funtionToCall, (entry.userData == null) ? string.Empty : entry.userData);
				}
			}
		}

		/// <summary>
		/// Class that holds data for each instance of an event to send
		/// </summary>
		[Serializable]
		public class EventEntry
		{
			/// <summary>The event to send, from the available components with SendsEvent attributes. Used by EventSender.HandleEvent</summary>
			public string eventName;
			/// <summary>The object to call SendMessage on</summary>
			public GameObject targetObject;
			/// <summary>The function name to pass as a parameter of SendMessage</summary>
			public string funtionToCall;
			/// <summary>An additional string for user-supplied data</summary>
			public string userData;
		}
	}
}
