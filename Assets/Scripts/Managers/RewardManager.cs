using System;
using System.Threading.Tasks;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;

namespace Managers
{
    public class RewardManager : Manager
    {
        [SerializeField] private ParticleSystem m_coinParticles;
        
        private int m_totalWheelCount;
        private int m_completedSpinCount;

        private const float particlePlayTime = 1.25f;


        protected override void Bind()
        {
            DI.Bind<RewardManager>(this);
        }

        protected override void OnAwake()
        {
            var main = m_coinParticles.main;
            main.duration = particlePlayTime;
            main.startLifetime = particlePlayTime;
            
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

        private async void PlayRewardAnimation()
        {
            m_coinParticles.Play();
            
            await Task.Delay(TimeSpan.FromSeconds(particlePlayTime));
            GameEventSystem.Invoke<RewardingCompletedEvent>();
        }
    }
}