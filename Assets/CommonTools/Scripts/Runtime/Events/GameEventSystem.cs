using System;
using System.Collections.Generic;

namespace CommonTools.Runtime.Events
{
    public static class GameEventSystem
    {
        private static readonly Dictionary<Type, Event> dictionary = new Dictionary<Type, Event>();

        public static void AddListener<T>(Action<object> action) where T : Event, new()
        {
            var type = typeof(T);
            var hasType = dictionary.TryGetValue(type, out Event evt);

            if (!hasType)
            {
                evt = new T();
                dictionary.Add(type, evt);
            }
            
            evt.AddListener(action);
        }

        public static void RemoveListener<T>(Action<object> action) where T : Event
        {
            var hasType = dictionary.TryGetValue(typeof(T), out Event evt);

            if (!hasType) return;

            evt.RemoveListener(action);
        }
        
        public static void Invoke<T>(object obj = null) where T : Event
        {
            var type = typeof(T);
            var hasType = dictionary.TryGetValue(type, out Event evt);

            if (!hasType) return;
            
            evt.Trigger(obj);
        }
    }
}