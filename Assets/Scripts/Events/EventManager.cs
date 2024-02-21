using System;
using System.Collections.Generic;

namespace Events
{
    public static class EventManager
    {
        public static void AddListener<T>(Action<T> action) where T : IEvent
        {
            EventHandler<T>.Instance.AddListener(action);
        }

        public static void RemoveListener<T>(Action<T> action) where T : IEvent
        {
            EventHandler<T>.Instance.RemoveListener(action);
        }
        
        public static void Invoke<T>(T eventData) where T : IEvent
        {
            EventHandler<T>.Instance.Trigger(eventData);
        }
        
        private class EventHandler<T> where T : IEvent
        {
            private static EventHandler<T> m_instance;
            private readonly List<Action<T>> m_actions = new List<Action<T>>();

            public static EventHandler<T> Instance
            {
                get
                {
                    m_instance ??= new EventHandler<T>();
                    return m_instance;
                }
            }

            public void AddListener(Action<T> action)
            {
                if (m_actions.Contains(action))
                    return;

                m_actions.Add(action);
            }

            public void RemoveListener(Action<T> action)
            {
                if (!m_actions.Contains(action))
                    return;
                
                m_actions.Remove(action);
            }

            public void Trigger(T eventData)
            {
                for (var i = m_actions.Count - 1; i >= 0; i--)
                {
                    m_actions[i].Invoke(eventData);
                }
            }
        }
    }
}