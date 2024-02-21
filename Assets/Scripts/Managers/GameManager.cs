using Core.Runtime.DependencyInjection;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour, IDependency
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
        }
    }
}