using Utility;

namespace Events.Implementations
{
    public class DataRefreshedEvent : IEvent
    {
        public Lineup[] Lineups { get; }

        private DataRefreshedEvent(Lineup[] lineups)
        {
            Lineups = lineups;
        }
        
        public static DataRefreshedEvent New(Lineup[] lineup)
        {
            return new DataRefreshedEvent(lineup);
        }
    }
}