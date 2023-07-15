using System.Linq;
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
        [SerializeField] private ProbDistributionSO m_probDistribution;
        // above objects can also be loaded by Resources.Load(), depending on preference.

        private RewardManager m_rewardManager;
        
        private Wheel[] m_wheels;
        private Lineup[] m_lineups;

        private float m_baseSpinDuration;
        private float m_lastSpinDuration;
        private float m_rewardedSpinDuration;

        private float m_spinDelayMin;
        private float m_spinDelayMax;
        
        private float m_wheelSpeed;

        private int m_currentRound;
        private int m_totalRoundCount;

        private const string keyForCurrentRound = "current_round";
        
        
        protected void Awake()
        {
            DI.Bind<SlotMachine>(this);
            RegisterParameters();

            m_wheels = GetComponentsInChildren<Wheel>().OrderBy(wheel => (int)wheel.Location).ToArray();
            
            if (!PlayerPrefs.HasKey(keyForCurrentRound))
                PlayerPrefs.SetInt(keyForCurrentRound, 0);

            m_currentRound = PlayerPrefs.GetInt(keyForCurrentRound);
            m_totalRoundCount = m_probDistribution.GetTotalOccurenceCount();
            
            if (m_currentRound == 0)
                RefreshData();
            else
                LoadData();
        }

        private void Start()
        {
            m_rewardManager = DI.Resolve<RewardManager>();
            GameEventSystem.Invoke<WheelsRegisteredEvent>(m_wheels.Length);
        }

        public void Spin()
        {
            var wheelCount = m_wheels.Length;
            var symbolTypes = m_lineups[m_currentRound].GetSymbolTypes();
            
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
            
            ManageRound();
        }

        private void ManageRound()
        {
            m_currentRound++;
            m_currentRound %= m_totalRoundCount;
            
            PlayerPrefs.SetInt(keyForCurrentRound, m_currentRound);

            if (m_currentRound == 0)
                RefreshData();
        }

        private void RefreshData()
        {
            m_lineups = CreateNewDistribution();
            DataSystem.SaveBinary(m_lineups);

            Debug.Log($"New lineup distribution created & saved.");
        }

        private void LoadData()
        {
            m_lineups = DataSystem.LoadBinary<Lineup>();
            Debug.Log($"Lineup distribution loaded from saved data.");
        }
        
        private Lineup[] CreateNewDistribution()
        {
            var lineupOccurrences = m_probDistribution.LineupOccurrences;
            var generator = new DistributionGenerator<Lineup>(lineupOccurrences);

            return generator.GetDistribution();
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