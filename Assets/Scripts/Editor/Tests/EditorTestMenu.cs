using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public static class EditorTestMenu
    {
        private const string soPath = "Data/ProbDistribution";
        private const string jsonPath = "TestData/data.json";
        
        [MenuItem("Tools/Test Runner/Create Distribution and Debug")]
        public static void CreateDistributionAndDebug()
        {
            var probDistribution = Resources.Load<ProbDistributionSO>(soPath);
            var generator = new DistributionGenerator<Lineup>(probDistribution.LineupOccurrences);

            var lineups = generator.GetDistribution();
            
            for (var i = 0; i < lineups.Length; i++)
            {
                var lineup = lineups[i];
                Debug.Log($"{i}: {lineup.Left} | {lineup.Middle} | {lineup.Right}");
            }
            
            DebugOccurrences(lineups, probDistribution.LineupOccurrences);
        }

        [MenuItem("Tools/Test Runner/Create Distribution and Open Json")]
        public static void CreateDistributionAndOpenJson()
        {
            var probDistribution = Resources.Load<ProbDistributionSO>(soPath);
            var generator = new DistributionGenerator<Lineup>(probDistribution.LineupOccurrences);

            var lineups = generator.GetDistribution();
            
            var savePath = Path.Combine(Application.dataPath, jsonPath);
            DataSystem.SaveJson(lineups, savePath);

            Process.Start(savePath);
            DebugOccurrences(lineups, probDistribution.LineupOccurrences);
        }

        private static void DebugOccurrences(Lineup[] items, OccurrenceInfo<Lineup>[] occurrenceArray)
        {
            Debug.Log("======= OCCURENCES =======");

            var commonToRare = occurrenceArray.OrderByDescending(oc => oc.Occurrence).ToArray();
            var lineupTypes = commonToRare.Select(oc => oc.Item).ToArray();
            
            foreach (var type in lineupTypes)
            {
                var count = items.Count(l => l.Equals(type));
                Debug.Log($"{type.Left} | {type.Middle} | {type.Right} : {count} occurrences");
            }
        }
    }
}