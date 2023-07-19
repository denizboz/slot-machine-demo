using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Managers
{
    public class GameManager : Manager
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;
        
        public int CurrentRound { get; private set; }
        private int m_totalRoundCount;
        
        private const string keyForCurrentRound = "current_round";
        
        protected override void Bind()
        {
            DI.Bind(this);
        }

        protected override void OnAwake()
        {
            Application.targetFrameRate = 60;
            
            if (!PlayerPrefs.HasKey(keyForCurrentRound))
                PlayerPrefs.SetInt(keyForCurrentRound, 0);

            CurrentRound = PlayerPrefs.GetInt(keyForCurrentRound);
            m_totalRoundCount = m_probDistribution.GetTotalOccurenceCount();

            var generator = new DistributionGenerator<Lineup>(m_probDistribution.LineupOccurrences);
            DI.Bind(generator);
            
            GameEventSystem.AddListener<FullSpinStartedEvent>(ManageRound);
        }

        private void Start()
        {
            if (CurrentRound == 0)
                RefreshData();
            else
                LoadData();
        }

        private void ManageRound(object obj)
        {
            CurrentRound++;
            CurrentRound %= m_totalRoundCount;
            
            PlayerPrefs.SetInt(keyForCurrentRound, CurrentRound);

            if (CurrentRound == 0)
                RefreshData();
        }
        
        private void RefreshData()
        {
            var lineups = CreateNewDistribution();
            DataSystem.SaveBinary(lineups);
            
            GameEventSystem.Invoke<DataRefreshedEvent>(lineups);

            Debug.Log($"New lineup distribution created & saved.");
        }

        private void LoadData()
        {
            var lineups = DataSystem.LoadBinary<Lineup>();
            GameEventSystem.Invoke<DataLoadedEvent>(lineups);
            
            Debug.Log($"Lineup distribution loaded from saved data.");
        }
        
        private Lineup[] CreateNewDistribution()
        {
            var generator = DI.Resolve<DistributionGenerator<Lineup>>();
            generator.Reset();
            
            return generator.GetDistribution();
        }
    }
}