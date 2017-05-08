//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
namespace HUX.Speech
{
    /// <summary>
    /// Condifence level that mirros the confidence level in the WSA.Speech namespace
    /// </summary>
    public enum KeywordConfidenceLevel
    {
        //
        // Summary:
        //     ///
        //     High confidence level.
        //     ///
        High = 0,
        //
        // Summary:
        //     ///
        //     Medium confidence level.
        //     ///
        Medium = 1,
        //
        // Summary:
        //     ///
        //     Low confidence level.
        //     ///
        Low = 2,
        //
        // Summary:
        //     ///
        //     Everything is rejected.
        //     ///
        Rejected = 3,

        /// <summary>
        /// The confidence level is unknown.
        /// </summary>
        Unknown = 4
    }
}