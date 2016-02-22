using System;

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
        {}

        public Func<Events.EntryEvent, OnOffInternalState, Herstamac.EventHandledResponse<OnOffInternalState>> EntryEventHandler { get; set; }

        public Func<Events.ExitEvent, OnOffInternalState, Herstamac.EventHandledResponse<OnOffInternalState>> ExitEventHandler { get; set; }
    }
}
