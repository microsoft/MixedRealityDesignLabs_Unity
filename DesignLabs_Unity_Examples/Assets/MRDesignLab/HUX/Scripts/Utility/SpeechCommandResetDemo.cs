//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpeechCommandResetDemo : SpeechCommand
{
    public int m_ResetToSceneIndex = 0;
    protected override void OnKeyword()
    {
        SceneManager.LoadScene(m_ResetToSceneIndex, LoadSceneMode.Single);
    }
}
