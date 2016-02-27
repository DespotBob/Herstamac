using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Herstamac
{
    public class MachineState<TInternalState> : IMachineState<TInternalState>
    {
        private State<TInternalState> _currentState;

        private readonly TInternalState _currentInternalState;

        private Dictionary<State<TInternalState>, State<TInternalState>> _stateHistories = new Dictionary<State<TInternalState>, State<TInternalState>>();

        public MachineState()
        {
        }

        public MachineState(TInternalState state) : this()
        {
            _currentInternalState = state;
        }

        public MachineState(Dictionary<State<TInternalState>, State<TInternalState>> stateHistories, TInternalState state, State<TInternalState> currentState )
            : this()
        {
            _stateHistories = stateHistories;
            _currentInternalState = state;
            _currentState = currentState;
        }

        State<TInternalState> IMachineState<TInternalState>.CurrentState
        { 
            get 
            {
                return _currentState;
            } 
        }

        Dictionary<State<TInternalState>, State<TInternalState>> IMachineState<TInternalState>.StateHistory 
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

        void IMachineState<TInternalState>.ChangeState(State<TInternalState> newState)
        {
            _currentState = newState;
        }

        void IMachineState<TInternalState>.AddInitialHistory(State<TInternalState> parentState, State<TInternalState> initialState)
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
