﻿using System;

namespace Herstamac.Test
{
    public class OnOffInternalState {
        public int counter;
    }

    public class OnOffStateMachineBuilder : MachineBuilder<OnOffInternalState>
    {
    }

    public class UnitTestState : State<OnOffInternalState>
    {
        public UnitTestState(string name)
            : base(name)
        {
        }
    }
}
