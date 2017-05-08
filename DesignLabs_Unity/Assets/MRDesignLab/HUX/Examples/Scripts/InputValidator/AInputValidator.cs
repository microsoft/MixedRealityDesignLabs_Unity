//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.InputValidator
{
	public abstract class AInputValidator : Interaction.InteractibleObject
	{
		
		#region Editor Variables
		[SerializeField]
		protected TextItem[] m_Messages;
		#endregion

		//--------------------------------------------------------------

		#region Unity Methods

		protected virtual void Start()
		{
			AddInputListeners();
		}

		protected virtual void OnDestroy()
		{
			RemoveInputListeners();
		}

		#endregion

		//--------------------------------------------------------------

		#region Message Handling
		protected void AddMessage(string str)
		{
			for (int i = m_Messages.Length - 1; i >= 1; --i)
			{
				m_Messages[i].CopyFrom(m_Messages[i - 1]);
			}
			m_Messages[0].SetText(str);
			m_Messages[0].Highlight();
		}
		#endregion

		//--------------------------------------------------------------

		#region Abstract Functions
		/// <summary>
		/// Method to Add all needed listeners for input in.
		/// </summary>
		protected abstract void AddInputListeners();

		/// <summary>
		/// Method to remove all needed listeners for input in.
		/// </summary>
		protected abstract void RemoveInputListeners();
		#endregion
	}
}
