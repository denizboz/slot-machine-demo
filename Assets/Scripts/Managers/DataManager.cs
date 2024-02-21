using CommonTools.Runtime.DependencyInjection;
using Events;
using Events.Implementations;
using UnityEngine;
using Utility;

namespace Managers
{
    public class DataManager : MonoBehaviour, IDependencyHandler
    {
        [SerializeField] private ProbDistributionSO m_probDistribution;
        
        public int CurrentRound { get; private set; }
        private int m_totalRoundCount;
        
        private const string keyForCurrentRound = "current_round";
        
        public void Bind()
        {
            DI.Bind(this);
            
            var distributionGenerator = new DistributionGenerator<Lineup>(m_probDistribution.LineupOccurrences);
            DI.Bind(distributionGenerator);
            
            var typeSequenceGenerator = new TypeSequenceGenerator();
            DI.Bind(typeSequenceGenerator);
        }

        private void Awake()
        {
            if (!PlayerPrefs.HasKey(keyForCurrentRound))
                PlayerPrefs.SetInt(keyForCurrentRound, 0);

            CurrentRound = PlayerPrefs.GetInt(keyForCurrentRound);
            m_totalRoundCount = m_probDistribution.GetTotalOccurenceCount();

            EventManager.AddListener<FullSpinStartedEvent>(ManageRound);
            
            if (CurrentRound == 0)
                RefreshData();
            else
                LoadData();
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<FullSpinStartedEvent>(ManageRound);
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
            var lineups = CreateNewDistribution();
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
        
        private Lineup[] CreateNewDistribution()
        {
            var generator = DI.Resolve<DistributionGenerator<Lineup>>();
            generator.Reset();
            
            return generator.GetDistribution();
        }
    }
}