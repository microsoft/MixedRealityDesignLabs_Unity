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
    /// A class to hold a group of DebugMenuItems together.
    /// </summary>
    public class DebugItemGroup : MonoBehaviour
    {
        /// <summary>
        /// The group title object.
        /// </summary>
        [SerializeField]
        private Text m_TitleText;

        /// <summary>
        /// The LayoutGroup to add the DebugMenuitems to.
        /// </summary>
        [SerializeField]
        private LayoutGroup m_ContentArea;

        /// <summary>
        /// A list of all the debug menu items added.
        /// </summary>
        private List<DebugMenuItem> m_MenuItems = new List<DebugMenuItem>();

		/// <summary>
		/// The page name.
		/// </summary>
		[SerializeField]
		private string m_PageName;

		/// <summary>
		/// The title of this group.
		/// </summary>
		public string PageName
		{
			get
			{
				return m_PageName;
			}

			set
			{
				m_PageName = value;
			}
		}

		/// <summary>
		/// The title of this group.
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
		/// The number of DebugMenuItems in this group.
		/// </summary>
		public int ItemCount
        {
            get
            {
                return m_MenuItems.Count;
            }
        }

        /// <summary>
        /// The list of item in this group.  Changing this list does not effect the group.
        /// </summary>
        public List<DebugMenuItem> MenuItems
        {
            get
            {
                return new List<DebugMenuItem>(m_MenuItems);
            }
        }

        /// <summary>
        /// Call to add an item to this grou.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddDebugItem(DebugMenuItem item)
        {
            if (item != null)
            {
                item.transform.SetParent(this.transform, false);
                m_MenuItems.Add(item);
            }
        }

        /// <summary>
        /// Call to remove an item from this group and destroy it.
        /// </summary>
        /// <param name="item">The item to remove and destroy.</param>
        public void RemoveDebugItem(DebugMenuItem item)
        {
            if (m_MenuItems.Contains(item))
            {
                m_MenuItems.Remove(item);
                Destroy(item.gameObject);
            }
        }
    }
}
