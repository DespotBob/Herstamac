using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Herstamac
{
    [DebuggerDisplay("State = {Name}")]
    public class State : IState
    {
        public State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }

    [DebuggerDisplay("InternalState = {Name}")]
    public class InternalState<TInternalState>
    {
        public class ListOfHandlers
        {
            public IReadOnlyList<ImmutableTransistionDefinition<TInternalState>> TransistionDefinitions { get; }

            public ListOfHandlers( List<ImmutableTransistionDefinition<TInternalState>> handlers)
            {
                TransistionDefinitions = handlers.AsReadOnly();
            }
        }

        public InternalState(BuilderState<TInternalState> state)
        {
            Name = state.Name;

            var t = state.Handlers.Select(x => new
            {
                type = x.Key,
                handlers = x.Value.TransistionDefinitions.Select(
                       y => new ImmutableTransistionDefinition<TInternalState>(y.TypeGuardCondition, y.GuardCondition, y.Action, y?.TransitionTo?.Name))
                       .ToList()
            }).ToList();

            Handlers = t.ToDictionary(x => x.type, x => new ListOfHandlers( x.handlers));
        }

        public string Name { get; }

        public Dictionary<Type, ListOfHandlers> Handlers { get; }
    }
}
