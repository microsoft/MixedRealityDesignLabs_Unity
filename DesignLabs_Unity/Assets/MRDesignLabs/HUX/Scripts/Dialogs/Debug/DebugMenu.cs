//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using HUX.Speech;
using HUX.Spatial;
using HUX.Utility;

namespace HUX.Dialogs.Debug
{
    /// <summary>
    /// Singleton instance for a HUX debug menu.
    /// This debug menu allows for adding of controls to be used during runtime.
    /// It also allows for those controls to be grouped together by providing a group name.
    /// </summary>
    public class DebugMenu : Singleton<DebugMenu>
    {
        /// <summary>
        /// The game object we can turn on and off to show/hide the debug menu.
        /// </summary>
        [SerializeField, Tooltip("The game object to turn on and off when hiding/showing the debug menu.")]
        private GameObject m_MenuContainer;

        /// <summary>
        /// The pins button to toggle when the debug menu has been pined in place.
        /// </summary>
        [SerializeField, Tooltip("The pins button to toggle when the debug menu has been pined in place.")]
        private Toggle m_PinButton;

        /// <summary>
        /// The 'prev' button to go to the previous page.
        /// </summary>
        [SerializeField, Tooltip("The 'prev' button to go to the previous page.")]
        private Button m_PrevButton;

        /// <summary>
        /// The 'next' button to go to the previous page.
        /// </summary>
        [SerializeField, Tooltip("The 'next' button to go to the previous page.")]
        private Button m_NextButton;

        /// <summary>
        /// Shows the current page number
        /// </summary>
        [SerializeField, Tooltip("Shows the current page number.")]
        private Text m_PageText;

        /// <summary>
        /// The SolverHandler that moves the debug menu around.
        /// </summary>
        [SerializeField, Tooltip("The SolverHandler that moves the debug menu around.")]
        private SolverHandler m_SolverHandler;

        [Header("Prefabs")]
        
        /// <summary>
        /// The prefab to use when creating new debug groups.
        /// </summary>
        public DebugItemGroup DebugItemGroupPrefab;

        /// <summary>
        /// The prefab to use when creating new page groups.
        /// </summary>
        public DebugPage DebugPagePrefab;

        /// <summary>
        /// The prefab to use when creating new toggle items.
        /// </summary>
        public DebugToggleItem DebugToggleItemPrefab;

        /// <summary>
        /// The prefab to use when creating new slider items. (Both integer and floating point.)
        /// </summary>
        public DebugSliderItem DebugSliderItemPrefab;

        /// <summary>
        /// The prefab to use when creating a new Vector 3 item.
        /// </summary>
        public DebugVector3Item DebugVector3ItemPrefab;
		
		/// <summary>
        /// The prefab to use when creating a new Vector 2 item.
        /// </summary>
        public DebugVector2Item DebugVector2ItemPrefab;

        /// <summary>
        /// The prefab to use when creating a new ENUM selection item.
        /// </summary>
        public DebugPulldownItem DebugPulldownItemPrefab;

        /// <summary>
        /// The prefab to use when creating a new button item.
        /// </summary>
        public DebugButtonItem DebugButtonItemPrefab;

        /// <summary>
        /// The prefab to use when creating a new label item.
        /// </summary>
        public DebugLabelItem DebugLabelItemPrefab;


        [Header("Activation/Deactivation")]
        /// <summary>
        /// The phrase to say to show the debug menu.
        /// </summary>
        public string OpenMenuKeyword = "open debug menu";

        /// <summary>
        /// The phrase to say to hide the debug menu.
        /// </summary>
        public  string CloseMenuKeyword = "close debug menu";

        /// <summary>
        /// The button to press to show the debug menu. (Only works in editor)
        /// </summary>
        [Tooltip("The button to press to show the debug menu. (Only works in the editor)")]
        public KeyCode EditorOpenKey = KeyCode.PageUp;

        /// <summary>
        /// The button to press to hide the debug menu. (Only works in editor)
        /// </summary>
        [Tooltip("The button to press to hide the debug menu. (Only works in the editor)")]
        public KeyCode EditorDownKey = KeyCode.PageDown;


        /// <summary>
        /// The controller pressed to toggle the debug log.
        /// </summary>
        [Tooltip("The controller label for toggling menu")]
        public string LogToggleControllerMap = "Xbox_MenuButton";

        /// <summary>
        /// The default group name for items that have not been given a group name.
        /// </summary>
        public const string MISC_GROUP_NAME = "Misc";

        /// <summary>
        /// If true, fields tagged as [DebugAttribute] will automatically show in the menu
        /// </summary>
        [Tooltip("If true, fields tagged as [DebugAttribute] will automatically show in the menu")]
        public bool ShowDebugFields = true;

        [Header("VR Tuneables")]

        /// <summary>
        /// The scale when displayed on Oculus
        /// </summary>
        [Tooltip("The scale when displayed on Oculus")]
        public float m_ScaleForHMD = 4.0f;

        /// <summary>
        /// The max view degrees on Oculus
        /// </summary>
        [Tooltip("The max view degrees on Oculus")]
        public float m_MaxViewDegreesForHMD = 30.0f;

        /// <summary>
        /// The Page index.
        /// </summary>
        private int m_CurrentPage = 0;

        /// <summary>
        /// Sets whether the Pages need to be refreshed.
        /// </summary>
        private bool m_PageDirty = false;

        /// <summary>
        /// Struct information for a new debug item entry.
        /// </summary>
        public struct ItemCommonInfo
        {
            public DebugMenuItem mDebugMenuItem;
            public string mGroupName;
            public GameObject mOwner;
            public int mURID;
        }
     
        /// <summary>
        /// A list containing all of the debug items that have been added to the debug menu.
        /// </summary>
        private List<ItemCommonInfo> mMenuItems = new List<ItemCommonInfo>();

        /// <summary>
        /// Debug group name -> Debug group object mapping.
        /// </summary>
        private Dictionary<string, DebugItemGroup> m_ItemGroups = new Dictionary<string, DebugItemGroup>();

        /// <summary>
        /// Debug page name -> Debug page object mapping.
        /// </summary>
        private Dictionary<string, DebugPage> m_Pages = new Dictionary<string, DebugPage>();
        private List<string> m_PageKeys = new List<string>();

        /// <summary>
        /// The current ID that will be given to new Debug items for tracking and removal.
        /// </summary>
        private int mCurURID = 0;

        #region Accessors
        /// <summary>
        /// True if the debug menu is currently pinned.
        /// </summary>
        public bool IsPinned
        {
            get { return m_PinButton != null && m_PinButton.isOn; }
        }

        public bool IsActive
        {
            get { return m_MenuContainer.activeInHierarchy; }
        }
        #endregion

        #region Unity Methods
        /// <summary>
        /// Unity standard Awake method.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            this.Deactivate();
        }

        /// <summary>
        /// Unity standard Start method.
        /// </summary>
        private void Start()
        {
            KeywordManager.Instance.AddKeyword(OpenMenuKeyword, OnWord, KeywordConfidenceLevel.Low);
            KeywordManager.Instance.AddKeyword(CloseMenuKeyword, OnWord, KeywordConfidenceLevel.Low);

            // Add the DebugMenuAttributeList component, which takes care of enumerating and listing [Debug] fields
            if (ShowDebugFields)
            {
                gameObject.AddComponent<DebugMenuAttributeList>();
            }

            if (m_MenuContainer && (UnityEngine.XR.XRSettings.loadedDeviceName == "Oculus" || UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR"))
            {
                // Scale up -- We have to do this every time the page is dirty.
                transform.localScale = Vector3.one * m_ScaleForHMD;

                SolverRectView solverRectView = m_MenuContainer.GetComponent<SolverRectView>();
                if (solverRectView)
                {
                    solverRectView.MaxViewDegrees = new Vector2(m_MaxViewDegreesForHMD, m_MaxViewDegreesForHMD);
                }
            }

        }

        /// <summary>
        /// Unity Standard OnDestroy method.
        /// </summary>
        private void OnDestroy()
        {
            KeywordManager.Instance.RemoveKeyword(OpenMenuKeyword, OnWord);
            KeywordManager.Instance.RemoveKeyword(CloseMenuKeyword, OnWord);
        }

        /// <summary>
        /// Unity Standard Update method.
        /// </summary>
        private void Update()
        {
#if !UNITY_METRO || UNITY_EDITOR
            if (Input.GetKeyDown(EditorOpenKey))
            {
                Activate();
            }
            else if (Input.GetKeyDown(EditorDownKey))
            {
                Deactivate();
            }
#endif
            if (Input.GetButtonDown(LogToggleControllerMap))
            {
                if (m_MenuContainer.activeSelf)
                {
                    Deactivate();
                }
                else
                {
                    Activate();
                }
            }

            // Needs updating
            if (m_PageDirty)
            {
                for (int i = 0; i < m_PageKeys.Count; ++i)
                {
                    DebugPage page;
                    m_Pages.TryGetValue(m_PageKeys[i], out page);
                    if (page)
                    {
                        page.gameObject.SetActive(i == m_CurrentPage);
                    }
                }
                m_PageText.text = (m_CurrentPage + 1) + "/" + m_PageKeys.Count;

                m_PageDirty = false;
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Shows the debug menu.
        /// </summary>
        public void Activate()
        {
            m_MenuContainer.SetActive(true);
            UnPin();
        }

        /// <summary>
        /// Hides the debug menu.
        /// </summary>
        public void Deactivate()
        {
            m_MenuContainer.SetActive(false);
        }

        /// <summary>
        /// Unpins the debug menu so that it moves into the users gaze.
        /// </summary>
        public void UnPin()
        {
            if (m_PinButton != null)
            {
                m_PinButton.isOn = false;
            }
        }

        /// <summary>
        /// Pins the debug menu so that it stays in place while the user looks around.
        /// </summary>
        public void Pin()
        {
            if (m_PinButton != null)
            {
                m_PinButton.isOn = true;
            }
        }

        /// <summary>
        /// Callback for when the Toggle Button toggles the pin state.
        /// </summary>
        /// <param name="toggleBtn"></param>
        public void PinBtnPressed(Toggle toggleBtn)
        {
            m_SolverHandler.enabled = !toggleBtn.isOn;
        }

        /// <summary>
        /// Callback for when the Prev Button is pressed.
        /// </summary>
        public void PrevBtnPressed()
        {
            if (m_Pages.Count > 1)
            {
                m_CurrentPage = (m_CurrentPage + m_Pages.Count - 1) % m_Pages.Count;
            }
            m_PageDirty = true;
        }

        /// <summary>
        /// Callback for when the Next Button is pressed.
        /// </summary>
        public void NextBtnPressed()
        {
            if (m_Pages.Count > 1)
            {
                m_CurrentPage = (m_CurrentPage + 1) % m_Pages.Count;
            }
            m_PageDirty = true;
        }

        /// <summary>
        /// Splits an item display name of the format "Group Name\Display Name".  "Group Name" will be
        /// returned, and the passed in string will be modified to contain "Display Name".
        /// </summary>
        /// <param name="fullName">Reference to the name string.  The group name will be trimmed</param>
        /// <returns>The parsed group name</returns>
        string ParseGroupName(ref string fullName)
        {
            string groupName = MISC_GROUP_NAME;

            int idx = fullName.IndexOf('\\');
            if( idx >= 0 )
            {
                groupName = fullName.Substring(0, idx);
                fullName = fullName.Substring(idx + 1, fullName.Length - (idx + 1));
            }
            return groupName;
        }

        #region Page Methods
        /// <summary>
        /// Sets a group to a page.  If no page exists, then a page is created.
        /// </summary>
        /// <param name="groupName">The group to assign to a page.</param>
        /// <param name="pageName">The name of the page to assign the group to.</param>
        public void SetGroupPage(string groupName, string pageName)
        {
            DebugItemGroup group = GetItemGroup(groupName, false);
            DebugPage page = GetPage(pageName, true);

            if (group != null)
            {
                // Remove from old group (ie, "Miscellaneous Items")
                DebugPage oldPage = GetPage(pageName, false);
                if (oldPage != null)
                {
                    oldPage.RemoveGroup(group);
                }

                // Add to the new group
                page.AddGroup(group);
            }
        }

        #region Add Methods
        /// <summary>
        /// Adds a DebugMenu item to the correct group and tracking.
        /// </summary>
        /// <param name="groupName">The group to add to.</param>
        /// <param name="owner">The owner to associate with.</param>
        /// <param name="item">The debug menu item to add.</param>
        /// <returns>The unique id for this item/</returns>
        public int AddItem(string groupName, GameObject owner, DebugMenuItem item)
        {
            DebugItemGroup group = GetItemGroup(groupName, true);

            ItemCommonInfo info = new ItemCommonInfo();
            info.mDebugMenuItem = item;
            info.mGroupName = groupName;
            info.mOwner = owner;
            info.mURID = mCurURID++;

            group.AddDebugItem(item);
            mMenuItems.Add(info);

            return info.mURID;
        }

        /// <summary>
        ///  Adds a floating point entry into the debug menu that goes between min and max by the step provided.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="callback">The function to call when the value changes.</param>
        /// <param name="valueFunc">The function to call each update to get the current value.</param>
        /// <param name="initValue">The initial value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <param name="step">The value to step by when we increase or decrease</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddItem("Group Name", "Variable Name", delegate (float val) { FloatVariable = val;}, () => { return FloatVariable; }, 0.0f, 1.0f, gameObject, 0.1f);</example>
        public int AddFloatItem(string displayName, System.Action<float> callback, Func<float> valueFunc, float initValue, float min, float max, GameObject owner = null, float step = 0.1f)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugSliderItem sliderItem = Instantiate(DebugSliderItemPrefab) as DebugSliderItem;
            sliderItem.Setup(displayName, callback, valueFunc, initValue, min, max, step);
            return this.AddItem(groupName, owner, sliderItem);
        }

        /// <summary>
        /// Adds an integer entry into the debug menu that goes between min and max by the step provided.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="callback">The function to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to get the current value.</param>
        /// <param name="initValue">The initial value.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <param name="step">The value to step by when we increase or decrease</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddItem("Group Name", "Variable Name", delegate (int val) { IntVariable = val;}, () => { return IntVariable; }, 0, 1, gameObject);</example>
        public int AddIntItem(string displayName, System.Action<int> callback, Func<int> valueFunc, int initValue, int min, int max, GameObject owner = null, int step = 1)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugSliderItem sliderItem = Instantiate(DebugSliderItemPrefab) as DebugSliderItem;
            sliderItem.Setup(displayName, callback, valueFunc, initValue, min, max, step);
            return this.AddItem(groupName, owner, sliderItem);
        }

        /// <summary>
        /// Adds a vector3 item to the debug menu.
        /// </summary>
        /// <param name="displayname">The display name to display for this item.</param>
        /// <param name="callback">The callback to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to get the current value. Can be null.</param>
        /// <param name="initValue">The initial value of the item.</param>
        /// <param name="owner">The owner of this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <remarks>The vector will change by 1 with each inc/dec.</remarks>
        public int AddVector3Item(string displayname, Action<Vector3> callback, Func<Vector3> valueFunc, Vector3 initValue, GameObject owner = null)
        {
            return this.AddVector3Item(displayname, callback, valueFunc, initValue, Vector3.one, owner);
        }

        /// <summary>
        /// Adds a vector3 item to the debug menu.
        /// </summary>
        /// <param name="displayname">The display name to display for this item.</param>
        /// <param name="callback">The callback to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to get the current value. Can be null.</param>
        /// <param name="initValue">The initial value of the item.</param>
        /// <param name="step">The amount to change X,Y,Z by when inc/dec is pressed.</param>
        /// <param name="owner">The owner of this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        public int AddVector3Item(string displayname, Action<Vector3> callback, Func<Vector3> valueFunc, Vector3 initValue, Vector3 step, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayname);
            DebugVector3Item vector3Item = Instantiate(DebugVector3ItemPrefab) as DebugVector3Item;
            vector3Item.Setup(displayname, callback, valueFunc, initValue, step);
            return this.AddItem(groupName, owner, vector3Item);
        }
		
		/// <summary>
        /// Adds a vector2 item to the debug menu.
        /// </summary>
        /// <param name="displayname">The display name to display for this item.</param>
        /// <param name="callback">The callback to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to get the current value. Can be null.</param>
        /// <param name="initValue">The initial value of the item.</param>
        /// <param name="owner">The owner of this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <remarks>The vector will change by 1 with each inc/dec.</remarks>
        public int AddVector2Item(string displayname, Action<Vector2> callback, Func<Vector2> valueFunc, Vector2 initValue, GameObject owner = null)
        {
            return this.AddVector2Item(displayname, callback, valueFunc, initValue, Vector2.one, owner);
        }

        /// <summary>
        /// Adds a vector2 item to the debug menu.
        /// </summary>
        /// <param name="displayname">The display name to display for this item.</param>
        /// <param name="callback">The callback to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to get the current value. Can be null.</param>
        /// <param name="initValue">The initial value of the item.</param>
        /// <param name="step">The amount to change X,Y,Z by when inc/dec is pressed.</param>
        /// <param name="owner">The owner of this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        public int AddVector2Item(string displayname, Action<Vector2> callback, Func<Vector2> valueFunc, Vector2 initValue, Vector2 step, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayname);
            DebugVector2Item vector2Item = Instantiate(DebugVector2ItemPrefab) as DebugVector2Item;
            vector2Item.Setup(displayname, callback, valueFunc, initValue, step);
            return this.AddItem(groupName, owner, vector2Item);
        }

        /// <summary>
        /// Adds a boolean toggle to the debug menu.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="callback">The function to call when the value changes.</param>
        /// <param name="valueFunc">The current value.</param>
        /// <param name="initValue">The initial value.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddItem("Group Name", "Variable Name", delegate (BOOL val) { BoolVariable = val;}, () => { return BoolVariable; }, gameObject);</example>
        public int AddBoolItem(string displayName, System.Action<bool> callback, Func<bool> valueFunc, bool initValue, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugToggleItem toggleItem = Instantiate(DebugToggleItemPrefab) as DebugToggleItem;
            toggleItem.Setup(displayName, callback, valueFunc, initValue);
            return this.AddItem(groupName, owner, toggleItem);
        }

        /// <summary>
        /// Adds a button item to the debug menu.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="btnName">The string to put on the button.</param>
        /// <param name="callback">The function to call when the button is pressed.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddItem("Group Name", "Variable Name", delegate() { DoSomething(); }, gameObject);</example>
        public int AddButtonItem(string displayName, string btnName, Action callback, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugButtonItem btnItem = Instantiate(DebugButtonItemPrefab) as DebugButtonItem;
            btnItem.Setup(displayName, btnName, callback);
            return this.AddItem(groupName, owner, btnItem);
        }

        /// <summary>
        /// Adds a button item with argument to the debug menu.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="btnName">The string to put on the button.</param>
        /// <param name="callback">The function to call when the button is pressed.</param>
        /// <param name="argument">The argument to pass in.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddItem("Group Name", "Variable Name", delegate() { DoSomething(); }, "Argument", gameObject);</example>
        public int AddButtonItem(string displayName, string btnName, Action<string> callback, string argument, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugButtonItem btnItem = Instantiate(DebugButtonItemPrefab) as DebugButtonItem;
            btnItem.Setup(displayName, btnName, callback, argument);
            return this.AddItem(groupName, owner, btnItem);
        }

        /// <summary>
        /// Adds an ENUM item to the debug menu.
        /// </summary>
        /// <typeparam name="T">The ENUM Type being added.</typeparam>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="callback">The function to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to update the value.</param>
        /// <param name="initValue">The initial ENUM value.</param>
        /// <param name="min">The minimum ENUM value.</param>
        /// <param name="max">The maximum ENUM value.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <remarks>The ENUM is cast to an integer for ease of use.</remarks>
        /// <example>DebugMenu.Instance.AddEnumItem<EnumType>("Group Name", "Variable Name", delegate (int val) { EnumVariable = (EnumType)val;}, (int)EnumVariable, 0, 1, gameObject);</example>
        public int AddEnumItem<T>(string displayName, System.Action<int> callback, Func<int> valueFunc, int initValue, int min, int max, GameObject owner = null)
        {
            return AddEnumItem(displayName, typeof(T), callback, valueFunc, initValue, min, max, owner);
        }

        /// <summary>
        /// Adds an ENUM item to the debug menu.
        /// </summary>
        /// <typeparam name="T">The ENUM Type being added.</typeparam>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="enumType">The system Type of the ENUM.</param>
        /// <param name="callback">The function to call when the value changes.</param>
        /// <param name="valueFunc">The function to call to update the value.</param>
        /// <param name="initValue">The initial ENUM value.</param>
        /// <param name="min">The minimum ENUM value.</param>
        /// <param name="max">The maximum ENUM value.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <remarks>The ENUM is cast to an integer for ease of use.</remarks>
        /// <example>DebugMenu.Instance.AddEnumItem<EnumType>("Group Name", "Variable Name", delegate (int val) { EnumVariable = (EnumType)val;}, (int)EnumVariable, 0, 1, gameObject);</example>
        public int AddEnumItem(string displayName, Type enumType, System.Action<int> callback, Func<int> valueFunc, int initValue, int min, int max, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugPulldownItem pulldownItem = Instantiate(DebugPulldownItemPrefab) as DebugPulldownItem;
            pulldownItem.Setup(displayName, enumType, callback, valueFunc, initValue, min, max);
            return this.AddItem(groupName, owner, pulldownItem);
        }

        /// <summary>
        /// Adds a label item to the debug menu.
        /// </summary>
        /// <param name="displayName">The display name of the item.</param>
        /// <param name="labelText">The string to put on the label.</param>
        /// <param name="callback">The function to call for label string.</param>
        /// <param name="owner">The owner to associate with this item.</param>
        /// <returns>A unique id that can be used for removal.</returns>
        /// <example>DebugMenu.Instance.AddLabelItem("Group Name", "Label Text", delegate() { return "Label Text"; }, gameObject);</example>
        public int AddLabelItem(string displayName, string labelText, Func<string> stringFunc, GameObject owner = null)
        {
            string groupName = ParseGroupName(ref displayName);
            DebugLabelItem labelItem = Instantiate(DebugLabelItemPrefab) as DebugLabelItem;
            labelItem.Setup(displayName, labelText, stringFunc);
            return this.AddItem(groupName, owner, labelItem);
        }

        #endregion

            #region Remove Methods
            /// <summary>
            /// Removed the item with the provided id.
            /// </summary>
            /// <param name="urid">The id passed back by an add call.</param>
        public void RemoveItem(int urid)
        {
            string groupToRemove = null;
            for (int i = 0; i < mMenuItems.Count; ++i)
            {
                if (mMenuItems[i].mURID == urid)
                {
                    DebugItemGroup group = GetItemGroup(mMenuItems[i].mGroupName, false);
                    group.RemoveDebugItem(mMenuItems[i].mDebugMenuItem);
                    
                    if (group.ItemCount == 0)
                    {
                        groupToRemove = mMenuItems[i].mGroupName;
                    }

                    mMenuItems.RemoveAt(i);
                    break;
                }
            }

            if (groupToRemove != null)
            {
                this.RemoveGroup(groupToRemove);
            }
        }

        /// <summary>
        /// Removes all the item that are associated with the owner.
        /// </summary>
        /// <param name="owner">The owner to look for.</param>
        public void RemoveItemsWithOwner(GameObject owner)
        {
            List<string> groupsToRemove = new List<string>();
            // Remove in reverse
            for (int i = mMenuItems.Count - 1; i >= 0; --i)
            {
                if (mMenuItems[i].mOwner == owner)
                {
                    DebugItemGroup group = GetItemGroup(mMenuItems[i].mGroupName, false);
                    group.RemoveDebugItem(mMenuItems[i].mDebugMenuItem);

                    if (group.ItemCount == 0 && !groupsToRemove.Contains(mMenuItems[i].mGroupName))
                    {
                        groupsToRemove.Add(mMenuItems[i].mGroupName);
                    }
                    mMenuItems.RemoveAt(i);
                }
            }

            for (int index = 0; index < groupsToRemove.Count; index++)
            {
                this.RemoveGroup(groupsToRemove[index]);
            }
        }

        /// <summary>
        /// Removed all item within a group.
        /// </summary>
        /// <param name="groupName">The group name to look for.</param>
        public void RemoveGroup(string groupName)
        {
            DebugItemGroup group = GetItemGroup(groupName, false);
            if (group != null)
            {
                List<DebugMenuItem> children = group.MenuItems;
                for (int index = 0; index < children.Count; index++)
                {
                    for (int i = mMenuItems.Count - 1; i >= 0; --i)
                    {
                        if (mMenuItems[i].mDebugMenuItem == children[index])
                        {
                            mMenuItems.RemoveAt(i);
                        }
                    }

                    Destroy(mMenuItems[index].mDebugMenuItem.gameObject);
                }

                m_ItemGroups.Remove(groupName);
                Destroy(group.gameObject);
            }
        }
        #endregion
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the group item or creates one if it does not exist.
        /// </summary>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="allowCreate">If true will create a group if it does not exist.  If fall it will return null if a group does not exist.</param>
        /// <returns>The group item or null if the group does not exist and allowCreate is false.</returns>
        private DebugItemGroup GetItemGroup(string groupName, bool allowCreate)
        {
            DebugItemGroup group = null;

            if (m_ItemGroups.ContainsKey(groupName))
            {
                group = m_ItemGroups[groupName];
            }
            else if (allowCreate)
            {
                group = Instantiate(DebugItemGroupPrefab) as DebugItemGroup;
                group.Title = groupName;

                m_ItemGroups[groupName] = group;

                // Set to the default page
                DebugPage page = GetPage("Miscellaneous Items", true);
                page.AddGroup(group);
            }

            return group;
        }

        /// <summary>
        /// Gets a page or creates one if it does not exist.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="allowCreate">If true will create a page if it does not exist.  If fall it will return null if a group does not exist.</param>
        /// <returns>The page or null if the page does not exist and allowCreate is false.</returns>
        private DebugPage GetPage(string pageName, bool allowCreate, bool startDisabled = true)
        {
            DebugPage page = null;

            if (m_Pages.ContainsKey(pageName))
            {
                page = m_Pages[pageName];
            }
            else if (allowCreate)
            {
                UnityEngine.Debug.Log("Creating new page: " + pageName);
                page = Instantiate(DebugPagePrefab) as DebugPage;
                page.Title = pageName;

                m_Pages[pageName] = page;
                page.transform.SetParent(m_MenuContainer.transform, false);

                m_PageKeys.Add(pageName);

                m_PageDirty = true;
            }

            return page;
        }
        #endregion

        /// <summary>
        /// The Speech callback.
        /// </summary>
        /// <param name="args"></param>
        private void OnWord(KeywordRecognizedEventArgs args)
        {
            if (args.text == OpenMenuKeyword)
            {
                Activate();
            }
            else if (args.text == CloseMenuKeyword)
            {
                Deactivate();
            }
        }
        #endregion
    }
}
