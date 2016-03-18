using System.Collections.Generic;

namespace Herstamac
{
    public interface IMachineState<TInternalState>

    {
        InternalState<TInternalState> CurrentState { get; }

        Dictionary<InternalState<TInternalState>, InternalState<TInternalState>> StateHistory { get; }

        TInternalState CurrentInternalState { get; }

        void ChangeState(InternalState<TInternalState> newState);

        void AddInitialHistory(InternalState<TInternalState> parentState, InternalState<TInternalState> initialState);
    }
}
