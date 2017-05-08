//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
namespace HUX.Utility
{
	/// <summary>
	/// Interface for any object which must be poolable.
	/// </summary>
	public interface IPoolable
	{
		bool IsActive { get; }

		void ReturnToPool();
	}
}
