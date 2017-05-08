//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TagDialog : MonoBehaviour {

    public GameObject Background;
    public TextMesh Text;
    public Vector3 Margin;

    // Update is called once per frame
    public void Update () {
        if ( Background != null && Text != null)
        {
            MeshRenderer renderer = Text.GetCachedComponent<MeshRenderer>();
            Background.transform.localScale = Vector3.Scale(transform.lossyScale, renderer.bounds.size + Margin);
        }
    }
}
