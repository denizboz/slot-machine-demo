using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Managers
{
    public class RewardManager : Manager
    {
        [SerializeField] private ParametersSO m_parameters;
        [SerializeField] private ParticleSystem m_coinParticles;

        private SymbolType[] m_symbolTypeLineup;

        private Dictionary<SymbolType, int> m_coinParticleRates;
        
        private int m_totalWheelCount;
        private int m_completedSpinCount;

        private float particlePlayTime;


        protected override void Bind()
        {
            DI.Bind<RewardManager>(this);
            
        }

        protected override void OnAwake()
        {
            particlePlayTime = m_parameters.CoinAnimDuration;
            
            var main = m_coinParticles.main;
            main.duration = particlePlayTime;
            main.startLifetime = particlePlayTime;

            SetParticleRates(m_parameters.BaseCoinParticleRate);
            
            GameEventSystem.AddListener<WheelsRegisteredEvent>(OnWheelsRegistered);
            GameEventSystem.AddListener<WheelSpinCompletedEvent>(OnOneSpinComplete);
        }

        private void OnWheelsRegistered(object count)
        {
            var wheelCount = (int)count;
            
            m_totalWheelCount = wheelCount;
            m_symbolTypeLineup = new SymbolType[wheelCount];
        }
        
        private void OnOneSpinComplete(object symbolType)
        {
            m_symbolTypeLineup[m_completedSpinCount] = (SymbolType)symbolType;
            m_completedSpinCount++;

            if (m_completedSpinCount < m_totalWheelCount)
                return;

            m_completedSpinCount = 0;
            
            if (IsLineupRewarding(m_symbolTypeLineup))
                PlayRewardAnimation(m_symbolTypeLineup[0]);
            else
                GameEventSystem.Invoke<RewardingCompletedEvent>();
        }

        public bool IsLineupRewarding(SymbolType[] symbolTypeLineup)
        {
            var firstSymbol = symbolTypeLineup[0];
            var isRewarding = true;

            for (var i = 1; i < symbolTypeLineup.Length; i++)
            {
                isRewarding = isRewarding && symbolTypeLineup[i] == firstSymbol;
            }

            return isRewarding;
        }
        
        private async void PlayRewardAnimation(SymbolType symbolType)
        {
            var emissionModule = m_coinParticles.emission;
            emissionModule.rateOverTime = m_coinParticleRates[symbolType];
            
            m_coinParticles.Play();
            
            await Task.Delay(TimeSpan.FromSeconds(particlePlayTime));
            GameEventSystem.Invoke<RewardingCompletedEvent>();
        }

        private void SetParticleRates(int baseRate)
        {
            var allSymbolTypes = Enum.GetValues(typeof(SymbolType)) as SymbolType[];
            allSymbolTypes = allSymbolTypes.OrderBy(type => (int)type).ToArray();

            m_coinParticleRates = new Dictionary<SymbolType, int>(allSymbolTypes.Length);
            
            for (var i = 0; i < allSymbolTypes.Length; i++)
            {
                m_coinParticleRates.Add(allSymbolTypes[i], (i + 1) * baseRate);
            }
        }

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<WheelsRegisteredEvent>(OnWheelsRegistered);
            GameEventSystem.RemoveListener<WheelSpinCompletedEvent>(OnOneSpinComplete);
        }
    }
}