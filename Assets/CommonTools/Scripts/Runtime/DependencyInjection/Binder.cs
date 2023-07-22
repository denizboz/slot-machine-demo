using UnityEngine;

namespace CommonTools.Runtime.DependencyInjection
{
    public static class Binder
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BindDependencies()
        {
            var dependencies = Object.FindObjectsOfType<Dependency>();

            foreach (var dependency in dependencies)
            {
                dependency.Bind();
            }
        }
    }
}