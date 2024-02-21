using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Core.Runtime.DependencyInjection
{
    [DefaultExecutionOrder(-500)]
    public class SceneContext : MonoBehaviour
    {
        private MonoBehaviour[] m_behaviours;
        private IDependency[] m_dependencies;

        private void Awake()
        {
            BindDependencies();
            InstallDependencies();
        }

        private void OnDestroy()
        {
            RemoveDependencies();
        }

        private void BindDependencies()
        {
            m_behaviours = FindObjectsOfType<MonoBehaviour>();
            m_dependencies = m_behaviours.OfType<IDependency>().ToArray();
            
            foreach (var dependency in m_dependencies)
            {
                ProjectContext.Bind(dependency);
            }
        }

        private void InstallDependencies()
        {
            foreach (var monoBehaviour in m_behaviours)
            {
                var methods = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                foreach (var method in methods)
                {
                    if (Attribute.IsDefined(method, typeof(ConstructAttribute)))
                    {
                        var parameters = method.GetParameters();
                        var dependencies = new object[parameters.Length];

                        for (var i = 0; i < parameters.Length; i++)
                        {
                            var type = parameters[i].ParameterType;

                            if (type.IsSubclassOf(typeof(MonoBehaviour)))
                            {
                                dependencies[i] = ProjectContext.Resolve(type);
                            }
                            else
                            {
                                dependencies[i] = Activator.CreateInstance(type);
                            }
                        }

                        method.Invoke(monoBehaviour, dependencies);
                    }
                }
            }
        }

        private void RemoveDependencies()
        {
            foreach (var dependency in m_dependencies)
            {
                ProjectContext.Remove(dependency);
            }
        }
    }
}