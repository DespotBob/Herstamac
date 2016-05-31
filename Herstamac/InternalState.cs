using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Herstamac
{
    [DebuggerDisplay("InternalState = {Name}")]
    public class InternalState<TInternalState>
    {
        public class ListOfHandlers
        {
            public IReadOnlyList<TransistionDefinition<TInternalState>> TransistionDefinitions { get; }

            public ListOfHandlers(List<TransistionDefinition<TInternalState>> handlers)
            {
                TransistionDefinitions = handlers.AsReadOnly();
            }
        }

        public InternalState(Fluent.BuilderState<TInternalState> state)
        {
            Name = state.Name;

            var t = state.Handlers.Select(x => new
            {
                type = x.Key,
                handlers = x.Value.TransistionDefinitions.Select(
                       y => new TransistionDefinition<TInternalState>(y.TypeGuardCondition, y.GuardCondition, y.Action, y?.TransitionTo?.Name))
                       .ToList()
            }).ToList();

            Handlers = t.ToDictionary(x => x.type, x => new ListOfHandlers(x.handlers));
        }

        public string Name { get; }

        public Dictionary<Type, ListOfHandlers> Handlers { get; }
    }
}
