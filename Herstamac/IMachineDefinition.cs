using System;

namespace Herstamac
{
    public interface IMachineDefinition<TInternalState>
    {
        System.Collections.Generic.IReadOnlyList<Func<object, object>> EventInterceptors { get; }
        System.Collections.Generic.IReadOnlyDictionary<State<TInternalState>, State<TInternalState>> ParentStates { get; }
        System.Collections.Generic.IReadOnlyList<State<TInternalState>> RegisteredState { get; }

       // Action<string>  Log { get; }
       // string MachineIdentifier { get; }
    }
}
