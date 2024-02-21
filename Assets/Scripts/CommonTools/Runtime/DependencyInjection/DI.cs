using System;
using System.Collections.Generic;

namespace CommonTools.Runtime.DependencyInjection
{
    /// <summary>
    /// Simple dependency injector for general use, allowing one dependency per type.
    /// </summary>
    public static class DI
    {
        private static readonly Dictionary<Type, object> dictionary = new Dictionary<Type, object>(64);


        public static void Bind<T>(T dependency)
        {
            dictionary.TryAdd(typeof(T), dependency);
        }

        public static T Resolve<T>()
        {
            if (!dictionary.TryGetValue(typeof(T), out var dependency))
                throw new Exception($"No {typeof(T)} reference in container.");
            
            return ((T)dependency);
        }
    }
}
