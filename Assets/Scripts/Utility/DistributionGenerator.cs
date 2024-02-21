using System;
using System.Collections.Generic;
using System.Linq;
using Core.Runtime.DependencyInjection;

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

                var rangeSteps = GetHomogenousRangeSteps(m_totalOccurrenceCount, count);
                
                for (var k = 0; k < rangeSteps.Length; k++)
                {
                    var min = k == 0 ? 0 : rangeSteps[k - 1];
                    var max = rangeSteps[k];
                    
                    var index = GetUniqueIndexInRange(min, max);
                    distribution[index] = item;
                }
            }
            
            return distribution;
        }

        private int GetUniqueIndexInRange(int min, int max)
        {
            var subList = m_availableIndices.Where(index => index >= min && index < max).ToList();

            var index = subList.Count > 1 ? subList[m_random.Next(0, subList.Count)] : GetRandomIndex();
            
            m_availableIndices.Remove(index);
            return index;
        }

        private int GetRandomIndex()
        {
            var rand = m_random.Next(0, m_availableIndices.Count);
            return m_availableIndices[rand];
        }

        public static int[] GetHomogenousRangeSteps(int totalRange, int rangeCount)
        {
            if (rangeCount < 2)
                throw new Exception("Range count must be greater than 1.");
            
            if (totalRange < rangeCount)
                throw new Exception("Total range must be greater than range count.");
            
            var stepSize = totalRange / rangeCount;
            
            var rangeSteps = new int[rangeCount];

            for (int i = 0; i < rangeCount; i++)
            {
                rangeSteps[i] = i != rangeCount - 1 ? (i + 1) * stepSize : totalRange;
            }

            var extra = totalRange - stepSize * rangeCount;
            
            if (extra == 0)
                return rangeSteps;

            var extraPerRange = stepSize > rangeCount ? extra / (rangeCount - 1) : 1;
            
            for (int i = 0; i < rangeCount - 1; i++)
            {
                rangeSteps[i] += (i + 1) * extraPerRange;
            }

            return rangeSteps;
        }
    }
    
    [Serializable]
    public struct OccurrenceInfo<T>
    {
        public T Item;
        public int Occurrence;
    }
}