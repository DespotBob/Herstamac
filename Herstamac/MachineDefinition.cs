using System;
using System.Collections.Generic;
using System.Linq;

using Herstamac.Fluent;

namespace Herstamac
{
    /// <summary>
    /// Holds the definition of a StateMachine once it has been built.
    /// </summary>
    public sealed class MachineDefinition<TInternalState> 
    {
        private readonly Action<string> _log;

        internal MachineDefinition(
            IReadOnlyList<InternalState<TInternalState>> registeredStates,
            IReadOnlyDictionary<string, string> parentStates,
            IReadOnlyList<Func<object, object>> eventInterceptors,
            MachineConfiguration<TInternalState> config,
            IState initialState,
            Dictionary<BuilderState<TInternalState>, BuilderState<TInternalState>> stateHistories )            
        {
            if (registeredStates == null)
            {
                throw new ArgumentNullException(nameof(registeredStates));
            }

            if (parentStates == null)
            {
                throw new ArgumentNullException(nameof(parentStates));
            }

            if (eventInterceptors == null)
            {
                throw new ArgumentNullException(nameof(eventInterceptors));
            }

            if (initialState == null)
            {
                throw new ArgumentNullException(nameof(initialState));
            }

            if (stateHistories == null)
            {
                throw new ArgumentNullException(nameof(stateHistories));
            }

            _log = config.Logger;            

            RegisteredState = registeredStates;
            EventInterceptors = eventInterceptors;
            Config = config;
            InitialState =  LookupRegisteredState( initialState.Name);
            StateHistories = stateHistories.ToDictionary(x => LookupRegisteredState(x.Key.Name), x => LookupRegisteredState(x.Value.Name)); ;
            ParentStates = parentStates.ToDictionary(
                    x => LookupRegisteredState(x.Key),
                    x => LookupRegisteredState(x.Value ));

            Log(registeredStates);
        }


        /// <summary>
        /// Used to create a new StateMachine Instance
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IMachineState<TInternalState> NewMachineInstance(TInternalState state)
        {
            return new MachineState<TInternalState>(StateHistories, state, InitialState);
        }

        private IReadOnlyList<InternalState<TInternalState>> RegisteredState { get; }

        internal InternalState<TInternalState> LookupRegisteredState(string name) =>
            RegisteredState.ToDictionary(x => x.Name, x => x)[name];
        
        internal IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> ParentStates { get; }

        internal IReadOnlyList<Func<object, object>> EventInterceptors { get; }

        internal InternalState<TInternalState> InitialState { get;}

        internal MachineConfiguration<TInternalState> Config { get; }

        public Dictionary<InternalState<TInternalState>, InternalState<TInternalState>> StateHistories { get; }

        private void Log(IReadOnlyList<InternalState<TInternalState>> registeredStates )
        {
            registeredStates
                .ToList()
                .ForEach( x => Config.Logger(string.Format($"SM:{Config.Name} - Registered state: '{x.Name}', with {x.Handlers.Keys.Count()} Handlers")));
        }
    }
}
