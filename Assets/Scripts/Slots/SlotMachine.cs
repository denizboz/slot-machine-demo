using CommonTools.Runtime.DependencyInjection;
using CommonTools.Runtime.TaskManagement;
using DG.Tweening;
using Events;
using Events.Implementations;
using Managers;
using UnityEngine;
using Utility;

namespace Slots
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private ParametersSO m_parameters;
        // can also be loaded by Resources.Load(), depending on preference.

        [SerializeField] private Wheel[] m_wheels;

        private GameManager m_gameManager;
        private RewardManager m_rewardManager;
        
        private Lineup[] m_lineups;

        private float m_baseSpinDuration;
        private float m_lastSpinDuration;
        private float m_rewardedSpinDuration;

        private float m_spinDelayMin;
        private float m_spinDelayMax;
        
        private float m_wheelSpeed;
        
        
        protected void Awake()
        {
            DI.Bind(this);
            RegisterParameters();
            
            GameEventSystem.AddListener<DataLoadedEvent>(OnDataLoaded);
            GameEventSystem.AddListener<DataRefreshedEvent>(OnDataLoaded);
        }

        private void Start()
        {
            m_gameManager = DI.Resolve<GameManager>();
            m_rewardManager = DI.Resolve<RewardManager>();
            
            GameEventSystem.Invoke<WheelsRegisteredEvent>(m_wheels.Length);
        }

        private void OnDataLoaded(object lineupArray)
        {
            m_lineups = (Lineup[])lineupArray;
        }
        
        public void Spin()
        {
            var wheelCount = m_wheels.Length;
            var symbolTypes = m_lineups[m_gameManager.CurrentRound].GetSymbolTypes();
            
            var delays = new float[wheelCount];
            var durations = new float[wheelCount];
            var easings = new Ease[wheelCount];
            
            for (var i = 0; i < wheelCount; i++)
            {
                var delay = Random.Range(m_spinDelayMin, m_spinDelayMax);
                delays[i] = i == 0 ? delay : delays[i - 1] + delay;
            }

            for (int i = 0; i < wheelCount - 1; i++)
            {
                durations[i] = m_baseSpinDuration;
                easings[i] = Ease.Linear;
            }

            var isRewarding = m_rewardManager.IsLineupRewarding(symbolTypes);
            
            durations[wheelCount - 1] = isRewarding ? m_rewardedSpinDuration : m_lastSpinDuration;
            easings[wheelCount - 1] = Ease.OutQuart;
            
            for (var i = 0; i < wheelCount; i++)
            {
                var di = i;
                GameTask.Wait(delays[i]).Do(() =>
                    m_wheels[di].Spin(symbolTypes[di], durations[di], m_wheelSpeed, easings[di]));
            }

            GameEventSystem.Invoke<FullSpinStartedEvent>();
        }

        private void RegisterParameters()
        {
            m_baseSpinDuration = m_parameters.BaseSpinDuration;
            m_lastSpinDuration = m_baseSpinDuration + m_parameters.LastSpinStopDelay;
            m_rewardedSpinDuration = m_baseSpinDuration + m_parameters.RewardedSpinStopDelay;

            m_spinDelayMin = m_parameters.SpinDelayMin;
            m_spinDelayMax = m_parameters.SpinDelayMax;
            
            m_wheelSpeed = m_parameters.WheelSpeed;
        }
    }
}