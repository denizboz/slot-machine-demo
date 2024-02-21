namespace Events.Implementations
{
    public class WheelsRegisteredEvent : IEvent
    {
        public int WheelCount { get; }

        private WheelsRegisteredEvent(int wheelCount)
        {
            WheelCount = wheelCount;
        }

        public static WheelsRegisteredEvent New(int wheelCount)
        {
            return new WheelsRegisteredEvent(wheelCount);
        }
    }
}