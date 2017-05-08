//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace HUX.Dialogs.Debug
{
    /// <summary>
    /// A class to hold a page of groups together.
    /// </summary>
    public class DebugPage : MonoBehaviour
    {
        /// <summary>
        /// The Page title object.
        /// </summary>
        [SerializeField]
        private Text m_TitleText;

		/// <summary>
		/// The LayoutGroup to add the DebugItemGroups to.
		/// </summary>
		[SerializeField]
        private LayoutGroup m_ContentArea;

        /// <summary>
        /// A list of all the debug menu groups added.
        /// </summary>
        private List<DebugItemGroup> m_MenuGroups = new List<DebugItemGroup>();

        /// <summary>
        /// The title of this page.
        /// </summary>
        public string Title
        {
            get
            {
                return m_TitleText.text;
            }

            set
            {
                m_TitleText.text = value;
            }
        }
		
		/// <summary>
		/// The number of DebugMenuGroups in this group.
		/// </summary>
		public int GroupCount
        {
            get
            {
                return m_MenuGroups.Count;
            }
        }

        /// <summary>
        /// The list of groups in this page.  Changing this list does not effect the page.
        /// </summary>
        public List<DebugItemGroup> MenuGroups
        {
            get
            {
                return new List<DebugItemGroup>(m_MenuGroups);
            }
        }

		/// <summary>
		/// Call to add an group to this page.
		/// </summary>
		/// <param name="group">The group to add.</param>
		public void AddGroup(DebugItemGroup group)
        {
            if ((group != null) && (!m_MenuGroups.Contains(group)))
            {
				//group.transform.SetParent(this.transform, false);
				group.transform.SetParent(m_ContentArea.transform, false);
				m_MenuGroups.Add(group);
            }
        }

        /// <summary>
        /// Call to remove an group from this page and destroy it.
        /// </summary>
        /// <param name="group">The group to remove and destroy.</param>
        public void RemoveGroup(DebugItemGroup group)
        {
            if (m_MenuGroups.Contains(group))
            {
				m_MenuGroups.Remove(group);
                Destroy(group.gameObject);
            }
        }
    }
}
