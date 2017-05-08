//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Utility
{
    /// <summary>
    /// This is a set of String Extensions mainly geared to the parsing JSON
    /// </summary>
    static class StringExtensions
    {
        public static string ColorToJSON(this Color color)
        {
            string M = "{ ";
            M += "\"r\" : \"" + (color.r * 255f).ToString() + "\" , ";
            M += "\"g\" : \"" + (color.g * 255f).ToString() + "\" , ";
            M += "\"b\" : \"" + (color.b * 255f).ToString() + "\" , ";
            M += "\"a\" : \"" + (color.a * 255f).ToString() + "\" } ";
            return M;
        }

        public static Color JSONToColor(this string node)
        {
            Color colorOut = JsonUtility.FromJson<Color>(node);

            colorOut.r /= 255f;
            colorOut.g /= 255f;
            colorOut.b /= 255f;
            colorOut.a /= 255f;

            return colorOut;
        }
    }
}
