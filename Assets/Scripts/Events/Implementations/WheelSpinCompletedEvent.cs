using Utility;

namespace Events.Implementations
{
    public class WheelSpinCompletedEvent : IEvent
    {
        public SymbolType SymbolType { get; }

        private WheelSpinCompletedEvent(SymbolType type)
        {
            SymbolType = type;
        }
        
        public static WheelSpinCompletedEvent New(SymbolType type)
        {
            return new WheelSpinCompletedEvent(type);
        }
    }
}