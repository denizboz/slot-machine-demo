namespace Events.Implementations
{
    public class RewardingCompletedEvent : IEvent
    {
        public static RewardingCompletedEvent New()
        {
            return new RewardingCompletedEvent();
        }
    }
}