using CommonTools.Runtime.DependencyInjection;
using CommonTools.Runtime.TaskManagement;
using UnityEngine;
using Utility;

namespace Slots
{
    public class SlotMachine : MonoBehaviour
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;
        // prob dist can also be loaded by Resources.Load(), depending on preference.

        private int m_currentRound;
        private int m_totalRoundCount;

        private const string keyForCurrentRound = "current_round";
        
        
        protected void Awake()
        {
            DI.Bind<SlotMachine>(this);
            
            return;

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

        public void Spin()
        {
            var wheels = GetComponentsInChildren<Wheel>();

            var delays = new float[wheels.Length];
            
            for (var i = 0; i < delays.Length; i++)
            {
                var delay = Random.Range(0.2f, 0.5f);
                delays[i] = i == 0 ? delay : delays[i - 1] + delay;
            }
            
            for (var i = 0; i < wheels.Length; i++)
            {
                // var wheel = wheels[i];
                // GameTask.Wait(delays[i]).Do((() => wheel.Spin(2.5f, 5000f)));
                
                wheels[i].Spin(2.5f, 3000f);
            }
        }

        private Lineup[] CreateNewDistribution()
        {
            var lineupOccurrences = m_probDistribution.LineupOccurrences;
            var generator = new DistributionGenerator<Lineup>(lineupOccurrences);

            return generator.GetDistribution();
        }
    }
}