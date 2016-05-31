using System;
using Herstamac.Fluent;

namespace Herstamac.Test.DefaultHandlers
{
    public class AnyHandlerState 
    {
        private Guid _guid = Guid.NewGuid();

        public Guid Id { get { return _guid; } }

        public int Counter1 { get; set; }

        public int Counter2 { get; set; }

    }

    public class AnyHandlerBuilder : MachineBuilder<AnyHandlerState>
    {
        /* Create some states to use in our state machine - The names are the most important thing really. There here inside this builder - just cos */
        public State Outer = NewState("Outer");
        public State Inner = NewState("Inner");
        public State Error = NewState("Error");

        /* Events - Can be any class, and can be located anywhere, here there inside the builder - just cos.. */
        public class GoFaster { }

        public AnyHandlerBuilder()
        {
            RegisterState(Inner);
            RegisterState(Outer);
            RegisterState(Error);

            RegisterParentStateFor(Inner, () => Outer );

            InState(Outer)
                .When<GoFaster>()
                .Then()
                .TransitionTo(Inner);

            InState(Outer)
                .OnAnyEvent()
                .Then((s,e) => s.Counter1++);

            InState(Inner)
                .OnAnyEvent()
                .Then((s,e) => s.Counter2++ )
                .TransitionTo(Error);

            InState(Inner)
                .OnDefaultEvent()
                .Then()
                .TransitionTo(Error);
        }
    }
}
