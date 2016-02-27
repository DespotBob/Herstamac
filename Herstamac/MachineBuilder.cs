using System;
using System.Collections.Generic;

namespace Herstamac
{ 
    public class MachineBuilder<TInternalState>
    {
        public IMachineState<TInternalState> _MachineState = new MachineState<TInternalState>();

        private List<State<TInternalState>> RegisteredState = new List<State<TInternalState>>();

        private Dictionary<State<TInternalState>, State<TInternalState>> parentStates = new Dictionary<State<TInternalState>, State<TInternalState>>();

        private readonly List<Func<object, object>> EventInterceptors;

        public MachineBuilder() : base()
        {
            EventInterceptors = new List<Func<object, object>>();
        }

        public State<TInternalState> RegisterState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var newState = new State<TInternalState>(name);

            RegisteredState.Add(newState);

            return newState;
        }

        public void RegisterState(State<TInternalState> stateToRegister)
        {
            if (stateToRegister == null)
            {
                throw new ArgumentNullException("stateToRegister");
            }

            RegisteredState.Add(stateToRegister);
        }

        public StateBuilder<TInternalState> InState(State<TInternalState> stateToBuildWith)
        {
            if (stateToBuildWith == null)
            {
                throw new ArgumentNullException("stateToBuildWith");
            }

            if (RegisteredState.Contains(stateToBuildWith) == false)
            {
                throw new ApplicationException("Cannot build state on an unregistered state!");
            }

            if (_MachineState.CurrentState == null || Misc<TInternalState>.FindAllStates(this.parentStates, _MachineState.CurrentState).Count == 0)
            {
                _MachineState.ChangeState(stateToBuildWith);
            }

            return new StateBuilder<TInternalState>(stateToBuildWith);
        }

        public void RegisterParentStateFor(State<TInternalState> childState, Func<State<TInternalState>> parentState)
        {
            if (childState == null)
            {
                throw new ArgumentNullException("childState", "The child state in cannot be null in a sub-state relationship!");
            }

            if (parentState == null || parentState() == null)
            {
                throw new ArgumentNullException("parentState", "The parent state in cannot be null in a sub-state relationship!");
            }

            if (childState == parentState())
            {
                throw new ArgumentOutOfRangeException("childState", "The child state cannot be the same as the parent state in a sub-state relationship!");
            }

            parentStates.Add(childState, parentState());
        }

        public void RegisterHistoryState(State<TInternalState> historyState, State<TInternalState> initialState)
        {
            if (historyState == null)
            {
                throw new ArgumentNullException("historyState", "The historyState state in cannot be null when registering a history state");
            }

            if (initialState == null)
            {
                throw new ArgumentNullException("initialState", "The initialState state in cannot be null when defining a registering state!");
            }

            if (historyState == initialState)
            {
                throw new ArgumentOutOfRangeException("initialState", "The initialState state cannot be the same as the historyState state when registering a history start!");
            }

            _MachineState.StateHistory.Add(historyState, initialState);

        }

        public void AddEventInterceptor(Func<object, object> interceptor)
        {
            if (interceptor == null)
            {
                throw new ArgumentNullException("Interceptor", "Cannot attach a null as an EventInterceptor!");
            }

            EventInterceptors.Add(interceptor);
        }

        public MachineDefinition<TInternalState> GetMachineDefinition()
        {
            return new MachineDefinition<TInternalState>(RegisteredState, parentStates, EventInterceptors, new MachineConfiguration<TInternalState>() );
        }

        public MachineDefinition<TInternalState> GetMachineDefinition(Action<IMachineConfigure<TInternalState>> configure)
        {
            var config = new MachineConfigure<TInternalState>();

            configure(config);

            return new MachineDefinition<TInternalState>(RegisteredState, parentStates, EventInterceptors, config.Results);
        }

        public IMachineState<TInternalState> NewMachineState( TInternalState state)
        {
            IMachineState<TInternalState> t = new MachineState<TInternalState>( _MachineState.StateHistory, state,  _MachineState.CurrentState);

            return t;
        }

        public static State<TInternalState> NewState(string name)
        {
            return new State<TInternalState>(name);
        }
    } 
}
