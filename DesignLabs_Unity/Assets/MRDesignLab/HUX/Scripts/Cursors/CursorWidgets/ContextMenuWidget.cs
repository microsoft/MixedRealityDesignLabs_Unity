//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Cursors
{
    public class ContextMenuWidget : CursorWidget
    {
        public override bool ShouldBeActive()
        {
            if(this._curTarget == null)
            {
                return base.ShouldBeActive();
            }

            ContextMenu i = _curTarget.GetComponent<ContextMenu>();
            return i != null && i.Visible;
        }
    }
}