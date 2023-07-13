using CommonTools.Runtime.DependencyInjection;
using UnityEngine;
using Utility;

namespace Managers
{
    public class SlotMachine : Manager
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;
        // prob dist can also be loaded by Resources.Load(), depending on preference.

        private int m_currentRound;
        private int m_totalRoundCount;

        private const string keyForCurrentRound = "current_round";
        
        protected override void Bind()
        {
            DI.Bind<SlotMachine>(this);
        }

        protected override void OnAwake()
        {
            if (!PlayerPrefs.HasKey(keyForCurrentRound))
                PlayerPrefs.SetInt(keyForCurrentRound, 0);

            m_currentRound = PlayerPrefs.GetInt(keyForCurrentRound);
            m_totalRoundCount = m_probDistribution.GetTotalOccurenceCount();

            m_currentRound %= m_totalRoundCount;
            
            if (m_currentRound == 0)
            {
                var newDistribution = CreateNewDistribution();
            }
        }

        private Lineup[] CreateNewDistribution()
        {
            var lineupOccurrences = m_probDistribution.LineupOccurrences;
            var generator = new DistributionGenerator<Lineup>(lineupOccurrences);

            return generator.GetDistribution();
        }
        
        protected override void Resolve()
        {
        }

        protected override void OnStart()
        {
        }
    }
}