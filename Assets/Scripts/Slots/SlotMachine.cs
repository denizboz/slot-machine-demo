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
    public class SlotMachine : Dependency
    {
        [SerializeField] private ParametersSO m_parameters;
        // can also be loaded by Resources.Load(), depending on preference.
        
        [SerializeField] private Wheel[] m_wheels;

        private DataManager m_dataManager;
        private RewardManager m_rewardManager;
        
        private Lineup[] m_lineups;

        private float m_baseSpinDuration;
        private float m_lastSpinDuration;
        private float m_rewardedSpinDuration;

        private float m_spinDelayMin;
        private float m_spinDelayMax;
        
        private float m_wheelSpeed;


        public override void Bind()
        {
            DI.Bind(this);
        }
        
        protected void Awake()
        {
            RegisterParameters();
            
            m_dataManager = DI.Resolve<DataManager>();
            m_rewardManager = DI.Resolve<RewardManager>();
            
            GameEventSystem.AddListener<DataLoadedEvent>(OnDataLoaded);
            GameEventSystem.AddListener<DataRefreshedEvent>(OnDataLoaded);
            GameEventSystem.AddListener<SpinButtonClickedEvent>(OnSpinButtonClicked);
        }

        private void Start()
        {
            GameEventSystem.Invoke<WheelsRegisteredEvent>(m_wheels.Length);
        }

        private void OnDataLoaded(object lineupArray)
        {
            m_lineups = (Lineup[])lineupArray;
        }
        
        private void OnSpinButtonClicked(object obj)
        {
            var wheelCount = m_wheels.Length;
            var symbolTypes = m_lineups[m_dataManager.CurrentRound].GetSymbolTypes();
            
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

        private void OnDestroy()
        {
            GameEventSystem.RemoveListener<DataLoadedEvent>(OnDataLoaded);
            GameEventSystem.RemoveListener<DataRefreshedEvent>(OnDataLoaded);
            GameEventSystem.RemoveListener<SpinButtonClickedEvent>(OnSpinButtonClicked);
        }
    }
}