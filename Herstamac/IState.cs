using System;
using System.Collections.Generic;

namespace Herstamac
{
    public interface IState
    {
        string Name { get; }
    }

    public interface IInternalState<TInternalState> : IState
    {
        Dictionary<Type, InternalState<TInternalState>.ListOfHandlers> Handlers { get; }
    }
}
