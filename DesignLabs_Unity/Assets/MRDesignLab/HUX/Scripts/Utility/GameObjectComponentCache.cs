//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using UnityEngine;

#if (UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
#endif

/// <summary>
/// Extension methods to allow retrieving cached components from a GameObject without incurring the performance penalty of calling GameObject.GetComponent<>() all the time.
/// </summary>
public static class GameObjectComponentCache
{
#if (UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
    private static readonly ConditionalWeakTable<GameObject, Dictionary<Type, Component>> componentCache =
        new ConditionalWeakTable<GameObject, Dictionary<Type, Component>>();
#endif

    public static TComponent GetCachedComponent<TComponent>(this Component component) where TComponent : Component
    {
        if (object.ReferenceEquals(component, null))
        {
            throw new ArgumentNullException("component");
        }

        return component.gameObject.GetCachedComponent<TComponent>();
    }

    public static TComponent GetCachedComponent<TComponent>(this GameObject gameObject) where TComponent : Component
    {
        if (object.ReferenceEquals(gameObject, null))
        {
            throw new ArgumentNullException("gameObject");
        }

        if (typeof(TComponent) == typeof(Transform))
        {
            throw new InvalidOperationException("Shouldn't use GetCachedComponent<>() for Transform components. Use gameObject.transform property instead, which is faster.");
        }

#if (UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
        Dictionary<Type, Component> componentCache = GameObjectComponentCache.componentCache.GetOrCreateValue(gameObject);
        Component component;
        if (!componentCache.TryGetValue(typeof(TComponent), out component))
        {
            component = gameObject.GetComponent<TComponent>();
            componentCache.Add(typeof(TComponent), component);
        }

        return (TComponent)component;
#else
        // In the future we may want to invest in maintaining our own Dictionary with a coroutine sweeping through on a regular interval to remove all the destroyed game objects so that we can
        // do the same error checks in the editor that we're doing in the WSA build.
        
        return gameObject.GetComponent<TComponent>();
#endif
    }
}
