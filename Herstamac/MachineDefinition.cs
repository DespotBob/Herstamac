using System;
using System.Collections.Generic;
using System.Linq;

namespace Herstamac
{
    /// <summary>
    /// Holds the definition of a StateMachine once it has been built.
    /// </summary>
    public class MachineDefinition<TInternalState>
    {
        IReadOnlyList<State<TInternalState>> _registeredState;
        IReadOnlyDictionary<State<TInternalState>, State<TInternalState>> _parentStates;
        IReadOnlyList<Func<object, object>> _eventInterceptors;

        private readonly Action<string> _log;

        MachineConfiguration<TInternalState> _config;

        internal MachineDefinition(
            IReadOnlyList<State<TInternalState>> registeredStates,
            IReadOnlyDictionary<State<TInternalState>, State<TInternalState>> parentStates,
            IReadOnlyList<Func<object, object>> eventInterceptors,
            MachineConfiguration<TInternalState> config )
        {
            if (registeredStates == null)
            {
                throw new ArgumentNullException("RegistererState");
            }

            if (parentStates == null)
            {
                throw new ArgumentNullException("parentStates");
            }

            if (eventInterceptors == null)
            {
                throw new ArgumentNullException("EventInterceptors");
            }


            _registeredState = registeredStates;
            _parentStates = parentStates;
            _eventInterceptors = eventInterceptors;
            _log = config.Logger;
            _config = config;

            Log();
        }

        internal IReadOnlyList<State<TInternalState>> RegisteredState
        {
            get
            {
                return _registeredState;
            }
        }

        internal IReadOnlyDictionary<State<TInternalState>, State<TInternalState>> ParentStates
        {
            get
            {
                return _parentStates;
            }
        }

        internal IReadOnlyList<Func<object, object>> EventInterceptors
        {
            get
            {
                return _eventInterceptors;
            }
        }

        internal MachineConfiguration<TInternalState> Config
        {
            get
            {
                return _config;
            }
        }

        private void Log()
        {
            _registeredState
                .ToList()
                .ForEach( x => _config.Logger(string.Format("SM:{0} - Registered state: '{1}', with {2} Handlers", _config.Name, x.Name, x.Handlers.Keys.Count())));
        }
    }
}
