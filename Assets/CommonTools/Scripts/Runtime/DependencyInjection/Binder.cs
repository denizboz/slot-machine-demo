using System.Linq;
using UnityEngine;

namespace CommonTools.Runtime.DependencyInjection
{
    public static class Binder
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BindDependencies()
        {
            var dependencies = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDependencyHandler>();

            foreach (var dependency in dependencies)
            {
                dependency.Bind();
            }
        }
    }
}