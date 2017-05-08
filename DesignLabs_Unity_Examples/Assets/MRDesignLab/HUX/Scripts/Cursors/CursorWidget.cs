//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System.Collections;

namespace HUX.Cursors
{
    public abstract class CursorWidget : MonoBehaviour
    {
        public enum AnchorTypeEnum
        {
            Cursor,
            TargetLock,
            TargetRelative,
            WorldLock
        }

        public string ActiveStates;

		public GameObject CursorObj;

		public AnchorTypeEnum AnchorType = AnchorTypeEnum.Cursor;

        public bool IgnoreParentScale;
        public bool HideBaseCursor = false;

        protected GameObject _curTarget;

        [System.Serializable]
        public struct TransformCache
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public void cache(Transform t)
            {
                this.position = t.localPosition;
                this.rotation = t.localRotation;
                this.scale = t.localScale;
            }
        }

        private TransformCache _transformCache;

        public virtual  void Awake()
        {
            _transformCache.cache(transform);
        }

        public virtual void Start() { }

        public virtual void OnEnable()
        {
			if (CursorObj != null)
			{
				SwitchAnchorType(AnchorType);
			}
        }

        public virtual bool ShouldBeActive()
        {
            return false;
        }

        public virtual void OnStateChange(Cursor.CursorState ParentState)
        {
            switch (ParentState)
            {
                case Cursor.CursorState.Hover:
                    break;
                case Cursor.CursorState.Interact:
                    break;
                case Cursor.CursorState.Observe:
                    break;
                case Cursor.CursorState.Release:
                    break;
                case Cursor.CursorState.Select:
                    break;
            }
        }

        public virtual void SetTargetObject(GameObject target)
        {
            _curTarget = target != null ? target : CursorObj;
        }


        public virtual void SwitchAnchorType(AnchorTypeEnum newAnchor)
        {
            AnchorType = newAnchor;
			Transform cursorTransform = CursorObj.transform;
			switch (AnchorType)
            {
                case AnchorTypeEnum.Cursor:
                    this.transform.parent = cursorTransform;
                    this.transform.localPosition = _transformCache.position;
                    this.transform.localRotation = _transformCache.rotation;
                    this.transform.localScale = _transformCache.scale;
                    break;
                case AnchorTypeEnum.TargetLock:
                    if (_curTarget != null)
                    {
                        this.transform.parent = null;
                        this.transform.position = _curTarget.transform.position + _transformCache.position;
                        this.transform.rotation = _transformCache.rotation * _curTarget.transform.rotation;
                        this.transform.localScale = _transformCache.scale;
                        this.transform.parent = _curTarget.transform;
                    }
                    break;
                case AnchorTypeEnum.TargetRelative:
                    if (_curTarget != null) {
                        this.transform.parent = null;
                        this.transform.position = cursorTransform.position;
                        this.transform.localScale = cursorTransform.lossyScale;
                        this.transform.localScale.Scale(_transformCache.scale);
                        this.transform.localRotation = _transformCache.rotation * cursorTransform.rotation;
                        this.transform.parent = _curTarget.transform;
                    }
                    break;
                case AnchorTypeEnum.WorldLock:
                    this.transform.parent = null;
                    this.transform.position = cursorTransform.position;
                    this.transform.localScale = cursorTransform.lossyScale;
                    this.transform.localScale.Scale(_transformCache.scale);
                    this.transform.localRotation = _transformCache.rotation * cursorTransform.rotation;
                    break;
            }
        }
    }
}
