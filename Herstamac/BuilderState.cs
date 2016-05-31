using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Herstamac
{

    [DebuggerDisplay("State = {Name}")]
    public class BuilderState<TInternalState> : IState
    {
        public class ListOfHandlers
        {
            public List<BuilderTransitionDefinition<TInternalState>> TransistionDefinitions = new List<BuilderTransitionDefinition<TInternalState>>();
        }

        public BuilderState(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "A state must have a name.");
            }

            Name = name;
            Handlers = new Dictionary<Type, ListOfHandlers>();
        }

        public string Name { get; }

        public Dictionary<Type, ListOfHandlers> Handlers { get; }

        internal BuilderTransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent, TAsHandler>()
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent, TAsHandler>((s, e) => true);
        }

        internal BuilderTransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent>()
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent>((s, e) => true);
        }

        internal BuilderTransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent>(Func<TInternalState, TEvent, bool> guard)
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent, TEvent>(guard);
        }

        internal BuilderTransitionDefinition<TInternalState> AddTransitionDefinitionToState<T1, TAsEvent>(Func<TInternalState, TAsEvent, bool> guard)
              where T1 : class
        {
            if (Handlers.Keys.Contains(typeof(T1)) == false)
            {
                Handlers.Add(typeof(T1), new ListOfHandlers());
            }

            var h = Handlers[typeof(T1)];

            var transDefinition = new BuilderTransitionDefinition<TInternalState>(TypedGuard<TAsEvent>, (s, o) => guard(s, (TAsEvent)o), null, null);

            h.TransistionDefinitions.Add(transDefinition);

            return transDefinition;
        }

        private static bool TypedGuard<TEvent>(object obj)
        {
            return obj.GetType() == typeof(TEvent);
        }
    }
}
