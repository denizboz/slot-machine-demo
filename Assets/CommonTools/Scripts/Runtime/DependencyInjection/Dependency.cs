using UnityEngine;

namespace CommonTools.Runtime.DependencyInjection
{
    public abstract class Dependency : MonoBehaviour
    {
        public abstract void Bind();
    }
}