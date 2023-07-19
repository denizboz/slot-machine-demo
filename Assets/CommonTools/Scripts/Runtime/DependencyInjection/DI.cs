using System;
using System.Collections.Generic;

namespace CommonTools.Runtime.DependencyInjection
{
    /*Simple Dependency Injector for General Use*/
    public static class DI
    {
        private static readonly Dictionary<Type, object> dictionary = new Dictionary<Type, object>(64);

        public static void Bind<T>(T dependency)
        {
            var type = typeof(T);
            
            if (dictionary.ContainsKey(type))
                dictionary[type] = dependency;
            else
                dictionary.Add(type, dependency);
        }

        public static T Resolve<T>()
        {
            var type = typeof(T);

            if (!dictionary.ContainsKey(type))
                throw new Exception($"No {type} reference in container.");

            return (T)dictionary[type];
        }
    }
}
