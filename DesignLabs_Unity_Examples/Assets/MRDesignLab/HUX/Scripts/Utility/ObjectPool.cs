//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HUX.Utility
{
	/// <summary>
	/// A simple implementation of a generic object pool.
	/// Access and creation time is O(N).
	/// Returning an object to the pool is O(1), and is done by calling IPoolable.ReturnToPool return false.
	/// </summary>
	/// <typeparam name="T">Type of objects in pool. Must implement IPoolable.</typeparam>
	public class ObjectPool<T> 
		where T : IPoolable
	{
		/// <summary>
		/// Default/initial size of the pool.
		/// </summary>
		protected const int DEFAULT_INITIAL_LENGTH = 50;

		/// <summary>
		/// Collection of objects.
		/// </summary>
		protected List<T> m_ObjectPool = null;

		/// <summary>
		/// Optional initializer function used to set up each object as it is created.
		/// </summary>
		protected Action<T> m_InitializeObject = null;

		/// <summary>
		/// Function to call to create a new pool object.
		/// </summary>
		protected Func<T> m_CreateObject = null;

		/// <summary>
		/// The current number of idle objects in the pool.
		/// </summary>
		public int Count
		{
			get
			{
				return m_ObjectPool.Count((T item) => { return !item.IsActive; });
			}
		}

		/// <summary>
		/// The current number of active pooled objects.
		/// </summary>
		public int ActiveItemCount
		{
			get
			{
				return m_ObjectPool.Count((T item) => { return item.IsActive; });
			}
		}

		/// <summary>
		/// Protected constructor to allow those inheriting to create their own constructors.
		/// </summary>
		protected ObjectPool()
		{
			m_ObjectPool = new List<T>();
		}

		/// <summary>
		/// Object pool constructor.
		/// </summary>
		/// <param name="createFunction">function to call to create a new pool item.</param>
		/// <param name="initialPoolSize">The initial pool size.</param>
		public ObjectPool(Func<T> createFunction, int initialPoolSize)
		{
			m_CreateObject = createFunction;
			m_ObjectPool = new List<T>(initialPoolSize);

			IncreasePool(initialPoolSize);
		}

		/// <summary>
		/// Object pool constructor.
		/// </summary>
		/// <param name="initFunction">Function to call on new pool items.</param>
		/// <param name="createFunction">function to call to create a new pool item.</param>
		/// <param name="initialPoolSize">The initial pool size.</param>
		public ObjectPool(Action<T> initFunction, Func<T> createFunction, int initialPoolSize)
		{
			m_CreateObject = createFunction;
			m_InitializeObject = initFunction;
			m_ObjectPool = new List<T>(initialPoolSize);
			IncreasePool(initialPoolSize);
		}

		/// <summary>
		/// Increases the number of objects in the pool.
		/// </summary>
		/// <param name="count">The number of objects to add to the pool.</param>
		public void IncreasePool(int count)
		{
			m_ObjectPool.Capacity = m_ObjectPool.Capacity + count;

			for (int i = 0; i < count; i++)
			{
				T newEntry = m_CreateObject();
				if (newEntry != null)
				{
					m_ObjectPool.Add(newEntry);
					if (m_InitializeObject != null)
					{
						m_InitializeObject(newEntry);
					}

					newEntry.ReturnToPool();
				}
			}
		}

		/// <summary>
		/// Returns an object from the pool.
		/// </summary>
		/// <returns>Object in the pool.</returns>
		public T GetObject()
		{
			for (int i = 0; i < m_ObjectPool.Count; i++)
			{
				if (!m_ObjectPool[i].IsActive)
				{
					return m_ObjectPool[i];
				}
			}

			// If list has no objects left to use, create a new one and return.
			this.IncreasePool(1);
			T obj = m_ObjectPool[m_ObjectPool.Count - 1];
			return obj;
		}
	}
}
