using System;

namespace Herstamac.Test.SlowFastStopped
{
    public class GoFaster : Event { }
    public class GoSlower : Event { }
    public class Stop : Event { }

    public class SlowFastStoppedStateMachineBuilder : MachineBuilder<SlowFastStoppedInternalState>
    {
        /* Create some states to use in our state machine - The names are the most important thing really. There here inside this builder - just cos */
        public State<SlowFastStoppedInternalState> Moving = NewState("Moving");
        public State<SlowFastStoppedInternalState> Slow = NewState("Slow");
        public State<SlowFastStoppedInternalState> Fast = NewState("Fast");
        public State<SlowFastStoppedInternalState> Stopped = NewState("Stopped");

        /* Events - Can be any class, and can be located anywhere, here there inside the builder - just cos.. */
        public class GoFaster : Event { }
        public class GoSlower : Event { }
        public class GoStop : Event { }

        public SlowFastStoppedStateMachineBuilder()
        {
            /* Register the states before use.. */
            RegisterState(Stopped);
            RegisterState(Slow);
            RegisterState(Fast);
            RegisterState(Stopped);

            /* Define a state Hierachy....*/
            RegisterParentStateFor(Slow, () => Moving);
            RegisterParentStateFor(Fast, () => Moving);

            /* Use a style of fluent api to define your behaviour */

            /* Some code that runs when the Stopped state is entered */
            InState(Stopped)
                .OnEntry()
                .Then((state, @event, log) =>
                {
                    log("Entered log in the stopped state!");
                });

            /* Some code that runs when the Stopped state is exited */
            InState(Stopped)
                .OnExit()
                .Then((state, @event) => {
                    Console.WriteLine("Exiting Stopped!");
                });

            /* In the stopped state, when a GoFasterEvent arrives - transition to the slow state */
            InState(Stopped)
                .When<GoFaster>()
                .TransitionTo(Slow);

            InState(Slow)
                .OnExit()
                .Then((state, @event) => {
                    Console.WriteLine("Entering Slow!");
                });

            InState(Slow)
                .When<GoFaster>()
                .TransitionTo(Fast);

            /* Right - just going to use (s,e) for state and @event from now on.... */
            InState(Slow)
                .When<GoSlower>()
                .Then((s,e) => Console.WriteLine( "Told to go slower"))
                .TransitionTo(Stopped);

            /* Whoops - looks like someone added some silly guard clauses - real ones would be meaningful */
            InState(Slow)
                .When<GoStop>()
                .WithGuard((s, e) => true)
                .Then()
                .TransitionTo(Stopped);

            /* A Good thing this Guard condition is in place ! */
            InState(Slow)
                .When<GoStop>()
                .WithGuard((s, e) => false)
                .Then(() =>
                {
                    throw new ApplicationException("This will not be called, as the guard condition is always false");
                });

            InState(Fast)
                .When<GoSlower>()
                .TransitionTo(Slow);

            InState(Fast)
                .When<GoFaster>()
                .Then((state, evnt) => Console.WriteLine("I cannae go nae faster..."));

            InState(Fast)
                .When<GoStop>()
                .TransitionTo(Stopped);
        }
    }
}
