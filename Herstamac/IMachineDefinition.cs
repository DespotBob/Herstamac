using System;
using System.Collections.Generic;

namespace Herstamac
{
    public interface IMachineDefinition<TInternalState>
    {
        IReadOnlyList<Func<object, object>> EventInterceptors { get; }
        IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> ParentStates { get; }
        IReadOnlyList<InternalState<TInternalState>> RegisteredState { get; }
    }
}
