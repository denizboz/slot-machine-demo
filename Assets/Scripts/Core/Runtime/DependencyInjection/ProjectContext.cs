using System;
using System.Collections.Generic;

namespace Core.Runtime.DependencyInjection
{
    /// <summary>
    /// Simple dependency injector for all project, allowing one dependency per type.
    /// </summary>
    internal static class ProjectContext
    {
        private static readonly Dictionary<Type, IDependency> dictionary = new Dictionary<Type, IDependency>();
        
        internal static void Bind(IDependency dependency)
        {
            dictionary.TryAdd(dependency.GetType(), dependency);
        }

        internal static IDependency Resolve(Type type)
        {
            if (!dictionary.TryGetValue(type, out var dependency))
                throw new Exception($"No {type} reference in container.");
            
            return dependency;
        }

        internal static void Remove(IDependency dependency)
        {
            var type = dependency.GetType();
            
            if (!dictionary.Remove(type))
                throw new Exception($"No {type} reference in container.");
        }
    }
}
