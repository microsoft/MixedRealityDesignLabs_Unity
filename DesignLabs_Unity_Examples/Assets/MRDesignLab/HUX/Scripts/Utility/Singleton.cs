//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
using UnityEngine;

namespace HUX.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] instances = FindObjectsOfType<T>();
                    if (instances.Length == 0)
                    {
                        Debug.LogError("No instance of singleton class " + typeof(T) + " found.");
                    }
                    else
                    {
                        instance = instances[0];
                        if (instances.Length > 1)
                        {
                            Debug.LogError("Multiple instances of singleton class " + typeof(T) + " found.");
                        }
                    }
                }
                return instance;
            }
        }

        private static T instance;

        protected virtual void Awake()
        {
            // Populate on startup
            T instanceCheck = Instance;
        }
    }
}
