using System;
using System.Collections.Generic;

namespace Herstamac
{
    public interface IInternalState<TInternalState>
    {
        string Name { get; }
        Dictionary<Type, InternalState<TInternalState>.ListOfHandlers> Handlers { get; }
    }
}
