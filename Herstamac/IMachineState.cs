using System.Collections.Generic;

namespace Herstamac
{
    public interface IMachineState<TInternalState>

    {
        State<TInternalState> CurrentState { get; }

        Dictionary<State<TInternalState>, State<TInternalState>> StateHistory { get; }

        TInternalState CurrentInternalState { get; }

        void ChangeState(State<TInternalState> newState);

        void AddInitialHistory(State<TInternalState> parentState, State<TInternalState> initialState);
    }
}
