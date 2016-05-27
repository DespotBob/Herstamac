using System;

namespace Herstamac.Test.DefaultHandlers
{
    public class DefaultHandlerState : Herstamac.MachineState<object>
    {
        private Guid _guid = Guid.NewGuid();

        public Guid Id { get { return _guid; } }

        public int Counter { get; set; }
    }

    public class DefaultHandlerBuilder : MachineBuilder<DefaultHandlerState>
    {
        /* Create some states to use in our state machine - The names are the most important thing really. There here inside this builder - just cos */
        public State Outer = NewState("Outer");
        public State Inner = NewState("Inner");
        public State Error = NewState("Error");

        /* Events - Can be any class, and can be located anywhere, here there inside the builder - just cos.. */
        public class GoFaster { }
        public class GoSlower  { }
        public class GoStop { }

        public DefaultHandlerBuilder()
        {
            RegisterState(Inner);
            RegisterState(Outer);
            RegisterState(Error);

            RegisterParentStateFor(Inner, () => Outer );

            InState(Inner)
                .When<GoFaster>()
                .Then(() => { });

            InState(Inner)
                .OnDefaultEvent()
                .Then((s,e) => s.Counter++ )
                .TransitionTo(Error);

            InState(Inner)
                .OnDefaultEvent()
                .Then()
                .TransitionTo(Error);


        }
    }
}
