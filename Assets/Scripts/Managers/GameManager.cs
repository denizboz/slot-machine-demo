using UnityEngine;

namespace Managers
{
    public class GameManager : Manager
    {
        protected override void OnAwake()
        {
            Application.targetFrameRate = 60;
        }
    }
}