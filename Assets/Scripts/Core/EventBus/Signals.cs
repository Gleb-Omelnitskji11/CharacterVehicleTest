namespace Core.EventBus
{
    public class ControlModeChangedSignal : IEvent
    {
        public readonly ControlMode NewMode;

        public ControlModeChangedSignal(ControlMode newMode)
        {
            NewMode = newMode;
        }
    }
}