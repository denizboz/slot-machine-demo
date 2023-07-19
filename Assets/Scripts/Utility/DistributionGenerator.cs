using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public class DistributionGenerator<T>
    {
        private readonly OccurrenceInfo<T>[] m_occurrences;
        private readonly int m_totalOccurrenceCount;
        
        private readonly Random m_random;

        private readonly List<int> m_availableIndices;


        public DistributionGenerator(OccurrenceInfo<T>[] occurrences)
        {
            m_occurrences = occurrences;
            m_totalOccurrenceCount = occurrences.Sum(info => info.Occurrence);
            
            m_random = new Random();
            m_availableIndices = new List<int>(m_totalOccurrenceCount);
            
            Reset();
        }

        public void Reset()
        {
            m_availableIndices.Clear();

            for (int i = 0; i < m_totalOccurrenceCount; i++)
            {
                m_availableIndices.Add(i);
            }
        }
        
        public T[] GetDistribution()
        {
            var distribution = new T[m_totalOccurrenceCount];
            var rareToCommon = m_occurrences.OrderBy(info => info.Occurrence).ToArray();
            
            for (var i = 0; i < rareToCommon.Length; i++)
            {
                var item = rareToCommon[i].Item;
                var count = rareToCommon[i].Occurrence;

                var extender = m_totalOccurrenceCount % count == 0 ? 0 : 1;
                var rangeStep = m_totalOccurrenceCount / count + extender;
                var extra = m_totalOccurrenceCount % rangeStep;

                for (int j = 0; j < count; j++)
                {
                    var inExtraRange = j == count - 1 && extra != 0;

                    var min = j * rangeStep;
                    var max = inExtraRange ? m_totalOccurrenceCount : (j + 1) * rangeStep;

                    var index = GetUniqueIndexInRange(min, max);
                    distribution[index] = item;
                }
            }
            
            return distribution;
        }

        private int GetUniqueIndexInRange(int min, int max)
        {
            var subList = m_availableIndices.Where(index => index >= min && index < max).ToList();

            var index = subList.Count > 1 ? subList[m_random.Next(0, subList.Count)] : GetClosestOutOfRange(min, max);
            
            m_availableIndices.Remove(index);
            return index;
        }

        private int GetClosestOutOfRange(int min, int max)
        {
            int closest = m_availableIndices.Max();
            
            var closestDistToMax = Math.Abs(max - closest);
            var closestDistToMin = Math.Abs(closest - min);
            
            foreach (var index in m_availableIndices)
            {
                var distToMax = Math.Abs(max - index);
                var distToMin = Math.Abs(index - min);
            
                if (distToMax < closestDistToMax || distToMin < closestDistToMin)
                    closest = index;
            }

            return closest;
        }
    }
    
    [Serializable]
    public struct OccurrenceInfo<T>
    {
        public T Item;
        public int Occurrence;
    }
}