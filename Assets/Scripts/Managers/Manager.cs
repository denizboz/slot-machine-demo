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

        protected void Start()
        {
            Resolve();
            OnStart();
        }

        protected abstract void Bind();
        protected abstract void OnAwake();
        protected abstract void Resolve();
        protected abstract void OnStart();
    }
}