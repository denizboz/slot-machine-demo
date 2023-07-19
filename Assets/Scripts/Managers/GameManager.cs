using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

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