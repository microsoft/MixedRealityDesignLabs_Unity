//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HUX.Utility
{
	public class StateController : MonoBehaviour
	{

		StateComponent[] stateComponents;

		public StateOverride[] StateOverrides;
		public string ActiveStateName = "";

		public bool ignoreComponentStateNames = false;
		public bool ignoreStateOverrides = false;

		void Awake()
		{
			CacheStateComponents();
			DeactivateAllStates();

			// Disable explicit states if no explicit states have been defined anyway
			if (StateOverrides == null || StateOverrides.Length == 0)
			{
				ignoreStateOverrides = true;
			}
		}

		public void CacheStateComponents()
		{
			StateComponent[] comps = GetComponents<StateComponent>();

			// Count only state components with a state name
			int count = 0;
			foreach (StateComponent sc in comps)
			{
				if (sc.stateName.Length > 0)
				{
					++count;
				}
			}

			// Add only state components with a state name to the list
			int i = 0;
			stateComponents = new StateComponent[count];
			foreach (StateComponent sc in comps)
			{
				if (sc.stateName.Length > 0)
				{
					stateComponents[i++] = sc;
				}
			}
		}

		StateOverride findState(string name)
		{
			foreach (StateOverride so in StateOverrides)
			{
				if (so.name == name)
				{
					return so;
				}
			}
			return null;
		}

		void deactivateState(string name)
		{
			if (name != null && name.Length > 0)
			{
				StateOverride so = findState(name);
				if (so != null)
				{
					so.SetActive(false);
				}
			}
		}

		void setStates(string name, bool bActive)
		{
			foreach (StateComponent s in stateComponents)
			{
				setState(s, name, bActive);
			}
		}

		void setState(StateComponent sc, string stateName, bool bActive)
		{
			if (stateName == null || (stateName.Length > 0 && sc.stateName.Contains(stateName)))
			{
				sc.enabled = bActive;
			}
		}

		public virtual void ActivateState(string name)
		{
			if (name == ActiveStateName)
			{
				return;
			}

			// Apply state changes based on component state names
			if (!ignoreComponentStateNames)
			{
				// Deactivate the previous active states
				setStates(ActiveStateName, false);

				// Activate the new ones
				setStates(name, true);
			}

			// Then perform any state overrides if desired
			if (!ignoreStateOverrides)
			{
				StateOverride so = findState(name);
				if (so != null)
				{
					deactivateState(ActiveStateName);
					so.SetActive(true);
				}
			}

			ActiveStateName = name;
		}

		public void DeactivateState(string name = null)
		{
			// Apply state changes based on state names
			if (!ignoreComponentStateNames)
			{
				setStates(name != null ? name : ActiveStateName, false);
			}

			// Then perform any state overrides if desired
			if (!ignoreStateOverrides)
			{
				deactivateState(name != null ? name : ActiveStateName);
			}

			ActiveStateName = "";
		}

		public void DeactivateAllStates()
		{
			setStates(null, false);
		}
	}

	[System.Serializable]
	public class StateOverride
	{
		public string name;
		public StateComponent[] stateComponents;

		public void SetActive(bool bActive)
		{
			foreach (StateComponent s in stateComponents)
			{
				s.enabled = bActive;
			}
		}
	}
}
