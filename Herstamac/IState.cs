using System;
using System.Collections.Generic;

namespace Herstamac
{
    public interface IState<TInternalState>
    {
        string Name { get; }
        Dictionary<Type, State<TInternalState>.ListOfHandlers> Handlers { get; }
    }
}
