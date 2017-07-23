namespace Herstamac.Lifts
{
    /// <summary>
    /// Probably not actually a lift.. Facade?
    /// </summary>
    /// <typeparam name="TInternalState"></typeparam>
    public class Lift<TInternalState>
    {
        private MachineDefinition<TInternalState> _machineDefinition;
        private IMachineState<TInternalState> _internalState;

        public Lift(IMachineState<TInternalState> state, MachineDefinition<TInternalState> machineState)
        {
            _internalState = state;
            _machineDefinition = machineState;
        }

        public bool IsInState(State state )
        {
            return MachineRunner.IsInState(_internalState, _machineDefinition, state);
        }

        public void Dispatch<T>(T @event)
        {
            MachineRunner.Dispatch(_machineDefinition, _internalState, @event);
        }
    }
}
