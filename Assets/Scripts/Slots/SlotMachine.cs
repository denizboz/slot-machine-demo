using System.Linq;
using CommonTools.Runtime.DependencyInjection;
using CommonTools.Runtime.TaskManagement;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Slots
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private ParametersSO m_parameters;
        [SerializeField] private ProbDistributionSO m_probDistribution;
        // scriptable objects can also be loaded by Resources.Load(), depending on preference.

        private Wheel[] m_wheels;
        private Lineup[] m_lineups;

        private int m_currentRound;
        private int m_totalRoundCount;

        private const string keyForCurrentRound = "current_round";
        
        
        protected void Awake()
        {
            DI.Bind<SlotMachine>(this);

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
            GameEventSystem.Invoke<WheelsRegisteredEvent>(3);
        }

        public void Spin()
        {
            var symbolTypes = m_lineups[m_currentRound].GetSymbolTypes();
            
            var delays = new float[m_wheels.Length];
            
            for (var i = 0; i < m_wheels.Length; i++)
            {
                var delay = Random.Range(0.1f, 0.33f);
                delays[i] = i == 0 ? delay : delays[i - 1] + delay;
            }
            
            for (var i = 0; i < m_wheels.Length; i++)
            {
                var di = i;
                GameTask.Wait(delays[i]).Do(() => m_wheels[di].Spin(symbolTypes[di], 2.5f, 3000f));
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
    }
}