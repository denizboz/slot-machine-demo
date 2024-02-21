namespace Events.Implementations
{
    public class SpinButtonClickedEvent : IEvent
    {
        public static SpinButtonClickedEvent New()
        {
            return new SpinButtonClickedEvent();
        }
    }
}