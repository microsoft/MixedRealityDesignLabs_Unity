//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class DisableUnlessEditor : MonoBehaviour
{
    public void OnEnable()
    {
#if !UNITY_METRO
        gameObject.SetActive(false);
#endif
    }
}
