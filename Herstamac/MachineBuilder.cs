using System;
using System.Collections.Generic;
using System.Linq;

namespace Herstamac
{ 
    public class MachineBuilder<TInternalState>
    {
        private IState _currentState = null;

        private Dictionary<BuilderState<TInternalState>, BuilderState<TInternalState>> _stateHistory { get; } = new Dictionary<BuilderState<TInternalState>, BuilderState<TInternalState>>();

        private List<BuilderState<TInternalState>> RegisteredStates = new List<BuilderState<TInternalState>>();

        private Dictionary<BuilderState<TInternalState>, BuilderState<TInternalState>> ParentStates 
            = new Dictionary<BuilderState<TInternalState>, BuilderState<TInternalState>>();

        private readonly List<Func<object, object>> EventInterceptors;

        public MachineBuilder() 
        {
            EventInterceptors = new List<Func<object, object>>();
        }

        private BuilderState<TInternalState> RegisterState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var newState = new BuilderState<TInternalState>(name);

            RegisteredStates.Add(newState);

            return newState;
        }

        public BuilderState<TInternalState> RegisterState(State stateToRegister)
        {
            return RegisterState(stateToRegister.Name);
        }

        public StateBuilder<TInternalState> InState(State stateToBuildWith)
        {
            if (stateToBuildWith == null)
            {
                throw new ArgumentNullException("stateToBuildWith");
            }

            var lookedUp = Lookup(stateToBuildWith);

            if (RegisteredStates.Contains(lookedUp) == false)
            {
                throw new ApplicationException("Cannot build state on an unregistered state!");
            }

            if (_currentState == null)
            { 
                _currentState = stateToBuildWith;
            }

            return new StateBuilder<TInternalState>(lookedUp, Lookup);
        }

        public void RegisterParentStateFor(State childState, Func<State> parentState)
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

            ParentStates.Add( Lookup(childState), Lookup(parentState()));
        }

        public void RegisterHistoryState(State historyState, State initialState)
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

            _stateHistory.Add( Lookup( historyState), Lookup(initialState));
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
            var states = ConvertStates();
            var parentStates = ConvertParentStates();

            return new MachineDefinition<TInternalState>(states
                , parentStates
                , EventInterceptors
                , new MachineConfiguration<TInternalState>()
                , _currentState
                , _stateHistory );
        }

        public MachineDefinition<TInternalState> GetMachineDefinition(Action<IMachineConfigure<TInternalState>> configure)
        {
            var config = new MachineConfigure<TInternalState>();

            configure(config);

            var states = ConvertStates();
            var parentStates = ConvertParentStates();

            return new MachineDefinition<TInternalState>(states
                , parentStates
                , EventInterceptors
                , config.Results
                , _currentState
                , _stateHistory);
        }

        public static State NewState(string name)
        {
            return new State(name);
        }

        public BuilderState<TInternalState> Lookup(IState state)
        {
            return RegisteredStates.First(x => x.Name == state.Name);
        }

        private IReadOnlyDictionary<string, string> ConvertParentStates()
        {
            return ParentStates.ToDictionary(x => x.Key.Name, x => x.Value.Name);
        }

        private IReadOnlyList<InternalState<TInternalState>> ConvertStates()
        {
            return RegisteredStates.Select(x => new InternalState<TInternalState>(x))
                .ToList().AsReadOnly();
        }
    } 
}
