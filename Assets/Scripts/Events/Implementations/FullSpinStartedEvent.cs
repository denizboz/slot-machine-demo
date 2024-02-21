namespace Events.Implementations
{
    public class FullSpinStartedEvent : IEvent
    {
        public static FullSpinStartedEvent New()
        {
            return new FullSpinStartedEvent();
        }
    }
}