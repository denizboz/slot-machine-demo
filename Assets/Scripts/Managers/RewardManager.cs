using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;

namespace Managers
{
    public class RewardManager : Manager
    {
        private int m_totalWheelCount;
        private int m_completedSpinCount;
        
        protected override void Bind()
        {
            DI.Bind<RewardManager>(this);
        }

        protected override void OnAwake()
        {
            GameEventSystem.AddListener<WheelsRegisteredEvent>(OnWheelsRegistered);
            GameEventSystem.AddListener<WheelSpinCompletedEvent>(OnOneSpinComplete);
        }

        private void OnWheelsRegistered(object count)
        {
            m_totalWheelCount = (int)count;
        }
        
        private void OnOneSpinComplete(object obj)
        {
            m_completedSpinCount++;
            
            if (m_completedSpinCount < m_totalWheelCount)
                return;

            m_completedSpinCount = 0;
            PlayRewardAnimation();
        }

        private void PlayRewardAnimation()
        {
            // after animation:
            GameEventSystem.Invoke<RewardingCompletedEvent>();
        }
    }
}