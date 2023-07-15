using System.Linq;
using CommonTools.Runtime.DependencyInjection;
using UnityEngine;
using Utility;

namespace Slots
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;
        // prob dist can also be loaded by Resources.Load(), depending on preference.

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

            m_currentRound %= m_totalRoundCount;
            
            if (m_currentRound == 0)
            {
                Debug.Log("FIRST SAVE");
                
                m_lineups = CreateNewDistribution();
                DataSystem.SaveBinary(m_lineups);

                foreach (var lineup in m_lineups)
                {
                    Debug.Log($"{lineup.Left} | {lineup.Middle} | {lineup.Right}");
                }
            }
            else
            {
                Debug.Log("LOAD");
                
                m_lineups = DataSystem.LoadBinary<Lineup>();
                
                foreach (var lineup in m_lineups)
                {
                    Debug.Log($"{lineup.Left} | {lineup.Middle} | {lineup.Right}");
                }
            }
        }

        public void Spin()
        {
            PlayerPrefs.SetInt(keyForCurrentRound, m_currentRound);
            
            var symbolTypes = m_lineups[m_currentRound].GetSymbolTypes();
            
            var delays = new float[m_wheels.Length];
            
            for (var i = 0; i < m_wheels.Length; i++)
            {
                var delay = Random.Range(0.2f, 0.5f);
                delays[i] = i == 0 ? delay : delays[i - 1] + delay;
            }
            
            for (var i = 0; i < m_wheels.Length; i++)
            {
                m_wheels[i].Spin(symbolTypes[i],2.5f, 3000f);
            }

            m_currentRound++;
        }

        private Lineup[] CreateNewDistribution()
        {
            var lineupOccurrences = m_probDistribution.LineupOccurrences;
            var generator = new DistributionGenerator<Lineup>(lineupOccurrences);

            return generator.GetDistribution();
        }
    }
}