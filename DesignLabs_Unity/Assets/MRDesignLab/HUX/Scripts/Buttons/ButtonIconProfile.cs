//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System.Collections.Generic;
using UnityEngine;

namespace HUX.Buttons
{
    public class ButtonIconProfile : ScriptableObject
    {
        /// <summary>
        /// The icon returned when a requested icon is not found
        /// (Icons starting with '_' will not be included in icon list)
        /// </summary>
        public Texture2D _IconNotFound_;
        /// <summary>
        /// Navigation icons
        /// </summary>
        public Texture2D GlobalNavButton;
        public Texture2D ChevronUp;
        public Texture2D ChevronDown;
        public Texture2D ChevronLeft;
        public Texture2D ChevronRight;
        public Texture2D Forward;
        public Texture2D Back;
        public Texture2D PageLeft;
        public Texture2D PageRight;

        /// <summary>
        /// Common action icons
        /// </summary>
        public Texture2D Add;
        public Texture2D Remove;
        public Texture2D Clear;
        public Texture2D Cancel;
        public Texture2D Zoom;
        public Texture2D Refresh;
        public Texture2D Lock;
        public Texture2D Accept;
        public Texture2D OpenInNewWindow;

        /// <summary>
        /// Common notification icons
        /// </summary>
        public Texture2D Completed;
        public Texture2D Error;

        /// <summary>
        /// Common object icons
        /// </summary>
        public Texture2D Contact;
        public Texture2D Volume;
        public Texture2D KeyboardClassic;
        public Texture2D Camera;
        public Texture2D Video;
        public Texture2D Microphone;

        /// <summary>
        /// Common gesture icons
        /// </summary>
        public Texture2D Ready;
        public Texture2D AirTap;
        public Texture2D PressHold;
        public Texture2D Drag;
        public Texture2D TapToPlaceArt;
        public Texture2D AdjustWithHand;
        public Texture2D AdjustHologram;
        public Texture2D RemoveHologram;

        /// <summary>
        /// Custom icons - these will override common icons by name
        /// </summary>
        public Texture2D[] CustomIcons;


        /// <summary>
        /// Returns an icon by name
        /// If icon is not found or icon is null, substitutes a default if useDefaultIfNotFound is true
        /// </summary>
        /// <param name="iconName"></param>
        /// <param name="icon"></param>
        /// <param name="useDefaultIfNotFound"></param>
        /// <returns></returns>
        public bool GetIcon (string iconName, out Texture2D icon, bool useDefaultIfNotFound)
        {
            Initialize();

            if (!iconLookup.TryGetValue(iconName, out icon) || icon == null)
            {
                // Substitute the default icon
                if (useDefaultIfNotFound)
                {
                    icon = _IconNotFound_;
                    return true;
                }
                // Report that we've had an error
                return false;
            }
            return true;
        }

        public List<string> GetIconKeys ()
        {
            Initialize();

            return new List<string> (iconKeys);
        }

        private void Initialize()
        {
            if (iconLookup != null)
                return;

            iconLookup = new Dictionary<string, Texture2D>();
            iconKeys = new List<string>();

            // Store all icons in iconLookup via reflection
            var properties = this.GetType().GetFields();
            foreach (var property in properties)
            {
                if (property.FieldType == typeof (Texture2D) && !property.Name.StartsWith ("_"))
                {
                    iconLookup.Add(property.Name, (Texture2D)property.GetValue(this));
                    iconKeys.Add(property.Name);
                }
            }

            // These icons will override the common icons if they exist, so do them last
            foreach (Texture2D icon in CustomIcons)
            {
                if (iconLookup.ContainsKey (icon.name))
                {
                    iconLookup[icon.name] = icon;
                } else
                {
                    iconLookup.Add(icon.name, icon);
                    iconKeys.Add(icon.name);
                }
            }
        }

        private bool initialized;
        private List<string> iconKeys;
        private Dictionary<string, Texture2D> iconLookup;
    }
}