using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class ServiceLocator : MonoBehaviour
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static ServiceLocator Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void RegisterService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
        }

        public bool HasService<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public void ClearServices()
        {
            _services.Clear();
        }
    }
}
