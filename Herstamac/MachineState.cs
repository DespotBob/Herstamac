using System;
using System.Collections.Generic;

namespace Herstamac
{
    public sealed class MachineState<TInternalState> : IMachineState<TInternalState>
    {
        private InternalState<TInternalState> _currentState;

        private readonly TInternalState _currentInternalState;

        private readonly Dictionary<InternalState<TInternalState>, InternalState<TInternalState>> _stateHistories;

        internal MachineState(
              Dictionary<InternalState<TInternalState>, InternalState<TInternalState>> stateHistories
            , TInternalState state
            , InternalState<TInternalState> currentState )
        {
            _stateHistories = stateHistories;
            _currentInternalState = state;
            _currentState = currentState;   
        }

        InternalState<TInternalState> IMachineState<TInternalState>.CurrentState
        { 
            get 
            {
                return _currentState;
            } 
        }

        Dictionary<InternalState<TInternalState>, InternalState<TInternalState>> IMachineState<TInternalState>.StateHistory 
        {
            get
            {
                return _stateHistories;
            }
        }

        TInternalState IMachineState<TInternalState>.CurrentInternalState 
        { 
            get 
            { 
                return _currentInternalState; 
            } 
        }

        void IMachineState<TInternalState>.ChangeState(InternalState<TInternalState> newState)
        {
            _currentState = newState;
        }

        void IMachineState<TInternalState>.AddInitialHistory(InternalState<TInternalState> parentState, InternalState<TInternalState> initialState)
        {
            if( parentState==null)
            {
                throw new ArgumentNullException("parentState", "The Parent state cannot be null when trying to Add initial histories");
            }

            if (initialState== null)
            {
                throw new ArgumentNullException("initialState", "The Initial State cannot be null when trying to Add initial histories");
            }

            _stateHistories[parentState] = initialState;
        }
    }
}
