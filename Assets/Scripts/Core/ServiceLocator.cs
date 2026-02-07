using System;
using System.Collections.Generic;

namespace Game.Core
{
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private static ServiceLocator _instance;

        public static ServiceLocator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ServiceLocator();
                }
                return _instance;
            }
        }

        public void RegisterService<T>(T service)
        {
            _services[typeof(T)] = service;
        }

        public static T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service of type {typeof(T)} is not registered.");
        }

        public static bool HasService<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public void ClearServices()
        {
            _services.Clear();
        }
    }
}
