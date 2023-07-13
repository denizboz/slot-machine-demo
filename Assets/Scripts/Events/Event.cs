using System;
using System.Collections.Generic;

namespace Events
{
    public abstract class Event
    {
        private readonly List<Action<object>> actions = new List<Action<object>>();

        public void AddListener(Action<object> action)
        {
            actions.Add(action);
        }

        public void RemoveListener(Action<object> action)
        {
            actions.Remove(action);
        }

        public void Trigger(object item)
        {
            foreach (var action in actions)
            {
                action?.Invoke(item);
            }
        }
    }
}