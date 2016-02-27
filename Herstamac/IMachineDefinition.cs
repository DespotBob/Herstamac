using System;
using System.Collections.Generic;

namespace Herstamac
{
    public interface IMachineDefinition<TInternalState>
    {
        IReadOnlyList<Func<object, object>> EventInterceptors { get; }
        IReadOnlyDictionary<State<TInternalState>, State<TInternalState>> ParentStates { get; }
        IReadOnlyList<State<TInternalState>> RegisteredState { get; }
    }
}
