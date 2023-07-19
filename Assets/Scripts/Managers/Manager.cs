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

        protected virtual void Bind()
        {
            // bind self to DI or DC
        }

        protected virtual void OnAwake()
        {
            // code to execute on Awake
        }
    }
}