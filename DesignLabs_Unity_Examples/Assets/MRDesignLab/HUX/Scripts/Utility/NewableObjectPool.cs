//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;
using System;

namespace HUX.Utility
{
	/// <summary>
	/// An objects pool for types that can be newed
	/// </summary>
	/// <typeparam name="T">The type of the object.  Must implement new().</typeparam>
	public class NewableObjectPool<T> : ObjectPool<T>
		where T : IPoolable, new()
	{
		/// <summary>
		/// Constructor with default initial size.
		/// </summary>
		public NewableObjectPool() : this(null, DEFAULT_INITIAL_LENGTH)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="initialPoolSize">The initial size of the pool.</param>
		public NewableObjectPool(int initialPoolSize) : this(null, initialPoolSize)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initFunction">The function to call when a new pool item is created.</param>
		public NewableObjectPool(Action<T> initFunction) : this(initFunction, DEFAULT_INITIAL_LENGTH)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="initFunction">The function to call when a new pool item is created.</param>
		/// <param name="initialPoolSize">The initial size of the pool.</param>
		public NewableObjectPool(Action<T> initFunction, int initialPoolSize)
		{
			m_CreateObject = this.CreateFunction;
			m_InitializeObject = initFunction;
			this.IncreasePool(initialPoolSize);
		}

		/// <summary>
		/// Creates a new pool item.
		/// </summary>
		/// <returns>A new instance of the object type.</returns>
		protected T CreateFunction()
		{
			return new T();
		}
	}
}
