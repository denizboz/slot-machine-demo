using UnityEngine;

namespace Managers
{
    public abstract class Manager : MonoBehaviour
    {
        protected void Awake()
        {
            Bind();
            OnAwake();
        }

        protected abstract void Bind();
        protected abstract void OnAwake();
    }
}