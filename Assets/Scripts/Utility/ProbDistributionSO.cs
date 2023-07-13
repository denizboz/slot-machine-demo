using System;
using System.Linq;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "ProbDistribution", menuName = "Data/New Probability Distribution")]
    public class ProbDistributionSO : ScriptableObject
    {
        public OccurrenceInfo<Lineup>[] LineupOccurrences;

        // informative only -- can be made readonly with an inspector tool like Odin.
        [SerializeField] private int m_totalOccurrenceCount;
        
        public int GetTotalOccurenceCount()
        {
            return LineupOccurrences.Sum(info => info.Occurrence);
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            m_totalOccurrenceCount = GetTotalOccurenceCount();
        }
#endif
    }
}