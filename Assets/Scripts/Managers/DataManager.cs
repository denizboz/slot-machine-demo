using Core.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Managers
{
    public class DataManager : MonoBehaviour, IDependency
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;

        private DistributionGenerator<Lineup> m_distributionGenerator;
        
        public int CurrentRound { get; private set; }
        private int m_totalRoundCount;
        
        private const string keyForCurrentRound = "current_round";
        
        [Construct]
        public void Construct()
        {
            m_distributionGenerator = new DistributionGenerator<Lineup>(m_probDistribution.LineupOccurrences);
            
            if (!PlayerPrefs.HasKey(keyForCurrentRound))
                PlayerPrefs.SetInt(keyForCurrentRound, 0);

            CurrentRound = PlayerPrefs.GetInt(keyForCurrentRound);
            m_totalRoundCount = m_probDistribution.GetTotalOccurenceCount();
        }

        private void OnEnable()
        {
            EventManager.AddListener<FullSpinStartedEvent>(ManageRound);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<FullSpinStartedEvent>(ManageRound);
        }

        private void Start()
        {
            if (CurrentRound == 0)
                RefreshData();
            else
                LoadData();
        }

        private void ManageRound(FullSpinStartedEvent _)
        {
            CurrentRound++;
            CurrentRound %= m_totalRoundCount;
            
            PlayerPrefs.SetInt(keyForCurrentRound, CurrentRound);

            if (CurrentRound == 0)
                RefreshData();
        }
        
        private void RefreshData()
        {
            var lineups = m_distributionGenerator.GetDistribution();
            DataSystem.SaveBinary(lineups);
            EventManager.Invoke(DataRefreshedEvent.New(lineups));
            
            Debug.Log($"New lineup distribution created & saved.");
        }

        private void LoadData()
        {
            var lineups = DataSystem.LoadBinary<Lineup>();
            EventManager.Invoke(DataRefreshedEvent.New(lineups));
            
            Debug.Log($"Lineup distribution loaded from saved data.");
        }
    }
}