using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.EventBus
{
    public class EventBus : MonoBehaviour, IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _listeners = new();

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService<IEventBus>(this);
        }

        public void Subscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);

            if (!_listeners.ContainsKey(type))
                _listeners[type] = new List<Delegate>();

            _listeners[type].Add(callback);
        }

        public void Unsubscribe<T>(Action<T> callback) where T : IEvent
        {
            var type = typeof(T);

            if (_listeners.TryGetValue(type, out var list))
                list.Remove(callback);
        }

        public void Publish<T>(T evt) where T : IEvent
        {
            var type = typeof(T);

            if (!_listeners.TryGetValue(type, out var list))
                return;

            foreach (var listener in list)
                ((Action<T>)listener)?.Invoke(evt);
        }
    }


    public interface IEvent
    {
    }
}