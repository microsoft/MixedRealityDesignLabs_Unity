//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;

namespace HUX.Utility
{
	/// <summary>
	/// An object pool for prefab objects.
	/// </summary>
	/// <typeparam name="T">The type of the prefab.</typeparam>
	public class PrefabObjectPool<T> : ObjectPool<T>
		where T : UnityEngine.Object, IPoolable
	{
		private T m_Prefab;
		private Transform m_PoolParent;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">The parent to parent the instance under.  Can be null.</param>
		public PrefabObjectPool(T prefab, Transform parent) : this(prefab, parent, null, DEFAULT_INITIAL_LENGTH)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">The parent to parent the instance under.  Can be null.</param>
		/// <param name="initialPoolSize">Initial size of the pool</param>
		public PrefabObjectPool(T prefab, Transform parent, int initialPoolSize) : this(prefab, parent, null, initialPoolSize)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">The parent to parent the instance under.  Can be null.</param>
		/// <param name="initFunction">Function to call when a new instance of the prefab is made.</param>
		public PrefabObjectPool(T prefab, Transform parent, Action<T> initFunction) : this(prefab, parent, initFunction, DEFAULT_INITIAL_LENGTH)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="parent">The parent to parent the instance under.  Can be null.</param>
		/// <param name="initFunction">Function to call when a new instance of the prefab is made.</param>
		/// <param name="initialPoolSize">Initial size of the pool</param>
		public PrefabObjectPool(T prefab, Transform parent, Action<T> initFunction, int initialPoolSize)
		{
			m_Prefab = prefab;
			m_PoolParent = parent;
			m_CreateObject = this.CreateFunction;
			m_InitializeObject = initFunction;
			this.IncreasePool(initialPoolSize);
		}

		/// <summary>
		/// Function to create new pool objects.
		/// </summary>
		/// <returns>The new instance of the prefab.</returns>
		protected T CreateFunction()
		{
			T newObj = GameObject.Instantiate(m_Prefab) as T;

			if (m_PoolParent != null)
			{
				if (newObj is Component)
				{
					(newObj as Component).transform.SetParent(m_PoolParent, false);
				}
				else if (newObj is GameObject)
				{
					(newObj as GameObject).transform.SetParent(m_PoolParent, false);
				}
			}

			return newObj;
		}
	}
}