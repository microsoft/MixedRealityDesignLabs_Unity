//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

public class DisableAfterDelay : MonoBehaviour
{
    public float Delay = 1.2f;
    protected float timeSinceLastEnabled = 0.0f;

    public void OnEnable()
    {
        timeSinceLastEnabled = 0.0f;
    }

    public void Update()
    {
        if (gameObject.activeSelf)
        {
            timeSinceLastEnabled += Time.deltaTime;

            if (timeSinceLastEnabled > Delay)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
