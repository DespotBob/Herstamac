using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Herstamac
{
    public interface IUniqueIdConfigure<TInternalState>
    {
        /// <summary>
        /// Use a function to create your own unique id.
        /// </summary>
        /// <param name="UniqueIdentifier"></param>
        void From(Func<TInternalState, string> UniqueIdentifier);

        /// <summary>
        /// Get the unique Id from a property in the State.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        void FromProperty<TProperty>(Expression<Func<TInternalState, TProperty>> property);

        /// <summary>
        /// Use the given Id/
        /// </summary>
        /// <param name="uniqueId"></param>
        void UniqueId(string uniqueId);
    }

    public interface IMachineConfigure<TInternalState>
    {
        void Name(string name);

        /// <summary>
        /// Help tell instances of the same state machine apart with a Unique Id.
        /// </summary>
        IUniqueIdConfigure<TInternalState> UniqueId { get; }

        void Logger(Action<string> logger);
        void LogEventWith(Func<object, string> eventSerialiser);

    }

    public class MachineConfiguration<TInternalState>
    {
        internal Func<object,string> LogEvents = (x) => string.Empty ;
        internal Action<string> Logger = (x) => { };
        internal Func<TInternalState, string> GetUniqueId = (x) => "XXXXXX";
        internal string Name { get; set; }
    }

    public class MachineConfigure<TInternalState> : IMachineConfigure<TInternalState>, IUniqueIdConfigure<TInternalState>
    {
        MachineConfiguration<TInternalState> config = new MachineConfiguration<TInternalState>();

        public MachineConfiguration<TInternalState> Results
        {
            get
            {
                return config;
            }
        }

        void IUniqueIdConfigure<TInternalState>.From(Func<TInternalState, string> func)
        {
            config.GetUniqueId = func;
        }

        public void FromProperty<TProperty>(Expression<Func<TInternalState, TProperty>> property)
        {
            var t = property.Compile();
            config.GetUniqueId = (prop) => t(prop).ToString();
        }

        public void Logger(Action<string> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            config.Logger = logger;
        }

        public void Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            config.Name = name;
        }

        void IUniqueIdConfigure<TInternalState>.UniqueId(string uniqueId)
        {
            config.GetUniqueId = (x) => uniqueId;
        }

        public void LogEventWith(Func<object, string> eventSerialiser)
        {
            if( eventSerialiser == null)
            {
                throw new ArgumentNullException("eventSerialiser");
            }

            config.LogEvents = eventSerialiser;
        }

        IUniqueIdConfigure<TInternalState> IMachineConfigure<TInternalState>.UniqueId
        {
            get
            {
                return this;
            }
        }
    }



    public class MachineBuilder<TInternalState>
    {
        public IMachineState<TInternalState> _MachineState = new MachineState<TInternalState>();

        private List<State<TInternalState>> RegisteredState = new List<State<TInternalState>>();

        private Dictionary<State<TInternalState>, State<TInternalState>> parentStates = new Dictionary<State<TInternalState>, State<TInternalState>>();

        private readonly List<Func<object, object>> EventInterceptors;

        private Action<string> _log = (x) => { } ;

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
                Log( "Set Initial State: '{0}'", stateToBuildWith.Name);
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

        public void AddTransitionLog(Action<string> log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log", "Cannot attach a null as a transition logger!");
            }

            _log = log;
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

        private void Log(string format, params object[] args)
        {
            _log(string.Format(format, args));
        }
    } 
}
