//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;

namespace HUX.Speech
{
    public class KeywordRecognizedEventArgs
    {
        //
        // Summary:
        //     ///
        //     A measure of correct recognition certainty.
        //     ///
        public readonly KeywordConfidenceLevel confidence;
        //
        // Summary:
        //     ///
        //     The time it took for the phrase to be uttered.
        //     ///
        public readonly TimeSpan phraseDuration;
        //
        // Summary:
        //     ///
        //     The moment in time when uttering of the phrase began.
        //     ///
        public readonly DateTime phraseStartTime;
        //
        // Summary:
        //     ///
        //     A semantic meaning of recognized phrase.
        //     ///
        //SKYBOX - Find an equivelent to this outside of WSA or create one.
        //public readonly SemanticMeaning[] semanticMeanings;
        //
        // Summary:
        //     ///
        //     The text that was recognized.
        //     ///
        public readonly string text;

        public KeywordRecognizedEventArgs(string text, KeywordConfidenceLevel confidence, /*SemanticMeaning[] semanticMeanings,*/ DateTime phraseStartTime, TimeSpan phraseDuration)
        {
            this.confidence = confidence;
            this.phraseDuration = phraseDuration;
            this.phraseStartTime = phraseStartTime;
            //this.semanticMeanings = semanticMeanings;
            this.text = text;
        }
    }
}