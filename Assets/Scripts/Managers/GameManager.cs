using CommonTools.Runtime.DependencyInjection;
using UnityEngine;

namespace Managers
{
    public class GameManager : Manager
    {
        protected override void Bind()
        {
            DI.Bind<GameManager>(this);
        }

        protected override void OnAwake()
        {
            Application.targetFrameRate = 60;
        }

        protected override void Resolve()
        {
        }

        protected override void OnStart()
        {
        }
    }
}