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
        }

        protected override void Resolve()
        {
        }

        protected override void OnStart()
        {
            // debug purpose
            
            var generator = new DistributionGenerator<Lineup>(m_probDistribution.LineupOccurrences);
            var dist = generator.GetDistribution();
            
            for (var i = 0; i < dist.Length; i++)
            {
                var lineup = dist[i];
                Debug.Log($"{i}: {lineup.Left} | {lineup.Middle} | {lineup.Right}");
            }
        }
    }
}