using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Utility;

namespace Editor.Tests
{
    public class TestRunner
    {
        [Test]
        public void LineupCountsMatch()
        {
            var occurrences = GetActualDistributionInfo();
            var generatedLineups = GetGeneratedDistribution(occurrences);

            var expected = occurrences.ToDictionary(info => info.Item, info => info.Occurrence);
            
            var actual = new Dictionary<Lineup, int>();
            
            foreach (var lineup in generatedLineups)
            {
                if (actual.ContainsKey(lineup))
                    actual[lineup]++;
                else
                    actual.Add(lineup, 1);
            }
            
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LineupsAreEvenlyDistributed()
        {
            var occurrences = GetActualDistributionInfo();

            var generatedLineups = GetGeneratedDistribution(occurrences);

            var totalLineupCount = occurrences.Sum(info => info.Occurrence);
            
            Assert.AreEqual(totalLineupCount, generatedLineups.Length);
            
            var actualCountInfo = occurrences.ToDictionary(info => info.Item, info => info.Occurrence);
            
            var indexDictionary = new Dictionary<Lineup, List<int>>();
            
            for (var i = 0; i < generatedLineups.Length; i++)
            {
                var lineup = generatedLineups[i];

                if (indexDictionary.TryGetValue(lineup, out var list))
                    list.Add(i);
                else
                    indexDictionary.Add(lineup, new List<int>() { i });
            }
            
            
            foreach (var (lineup, list) in indexDictionary)
            {
                var count = actualCountInfo[lineup];
                var step = totalLineupCount / count;

                var rangeSteps = GetHomogenousSteps(generatedLineups.Length, list.Count);
                
                for (int i = 0; i < rangeSteps.Length; i++)
                {
                    var min = i == 0 ? 0 : rangeSteps[i - 1];
                    var max = rangeSteps[i];

                    var lineupCountInRange = list.Count(j => j >= min && j < max);
                    
                    Assert.LessOrEqual(lineupCountInRange, 3); // will explain 3.
                }
            }
        }

        private static OccurrenceInfo<Lineup>[] GetActualDistributionInfo()
        {
            var probDist = Resources.Load<ProbDistributionSO>("Data/ProbDistribution");
            return probDist.LineupOccurrences;
        }

        private static Lineup[] GetGeneratedDistribution(OccurrenceInfo<Lineup>[] occurrences)
        {
            var generator = new DistributionGenerator<Lineup>(occurrences);
            return generator.GetDistribution();
        }

        private static int[] GetHomogenousSteps(int totalRange, int rangeCount)
        {
            return DistributionGenerator<Lineup>.GetHomogenousRangeSteps(totalRange, rangeCount);
            // sorry for the ugly syntax :(
        }
    }
}
