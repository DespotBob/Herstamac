using System;
using Herstamac.Fluent;

namespace Herstamac.Test
{
    public class OnOffInternalState {
        public int counter;
    }

    public class OnOffStateMachineBuilder : MachineBuilder<OnOffInternalState>
    {
    }

    public class UnitTestState : State
    {
        public UnitTestState(string name)
            : base(name)
        {
        }
    }
}
