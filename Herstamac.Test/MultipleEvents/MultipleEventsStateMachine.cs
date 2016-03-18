using System;

namespace Herstamac.Test.MultipleEvents
{
    public class MultipleEventsMachineState : MachineState<object>
    {
        public Guid Id { get; set; }
    }

    public class MultipleEventsStateMachine : MachineBuilder<MultipleEventsMachineState>
    {
        public InternalState<MultipleEventsMachineState> Sane = NewState("Sane");
        public InternalState<MultipleEventsMachineState> Insane = NewState("Insane");
        public InternalState<MultipleEventsMachineState> Sucidal = NewState("Sucidal");

        public class TelephoneCall  { }
        public class EmailMessage { }
        public class PressTrigger { }

        public MultipleEventsStateMachine( )
        {
            RegisterState(Sane);
            RegisterState(Insane);
            RegisterState(Sucidal);

            RegisterParentStateFor(Sucidal, () => Insane);

            InState(Sane);

            InState(Sane)
                .When<EmailMessage, TelephoneCall>((eb) => 
                    eb.WithGuard((s, e) => true)
                    .Then((s, e) => { })
                    .TransitionTo(Insane) 
                );

            InState(Sucidal)
                .When<EmailMessage, TelephoneCall>(b => b.TransitionTo(Insane));

            InState(Sucidal)
                .When<TelephoneCall>()
                .Then((state, evnt) => Console.WriteLine("I cannae go nae faster..."));

        }
    }
}
