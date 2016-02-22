using System;

namespace Herstamac.Test.SlowFastStopped
{
    public class GoFaster : Event { }
    public class GoSlower : Event { }
    public class Stop : Event { }

    public class SlowFastStoppedStateMachineBuilder : MachineBuilder<SlowFastStoppedInternalState>
    {
        public State<SlowFastStoppedInternalState> Moving = NewState("Moving");
        public State<SlowFastStoppedInternalState> Slow = NewState("Slow");
        public State<SlowFastStoppedInternalState> Fast = NewState("Fast");
        public State<SlowFastStoppedInternalState> Stopped = NewState("Stopped"); 

        public class GoFaster : Event { }
        public class GoSlower : Event { }
        public class GoStop : Event { }

        public SlowFastStoppedStateMachineBuilder()
        {
            RegisterState(Stopped);
            RegisterState(Slow);
            RegisterState(Fast);
            RegisterState(Stopped);

            InState(Stopped)
                .OnEntry()
                .Then((s, e) =>
                {
                    Console.WriteLine("Entering Stoppped!");
                });

            InState(Stopped)
                .OnExit()
                .Then((s, e) => {
                    Console.WriteLine("Exiting Stopped!");
                });

            InState(Stopped)
                .When<GoFaster>()
                .TransitionTo(Slow);

            InState(Slow)
                .OnExit()
                .Then((s, e) => {
                    Console.WriteLine("Entering Slow!");
                });

            InState(Slow)
                .When<GoFaster>()
                .TransitionTo(Fast);

            InState(Slow)
                .When<GoSlower>()
                .Then((s,e) => Console.WriteLine( "Told to go slower"))
                .TransitionTo(Stopped);

            InState(Slow)
                .When<GoStop>()
                .TransitionTo(Stopped);

            InState(Fast)
                .When<GoSlower>()
                .TransitionTo(Slow);

            InState(Fast)
                .When<GoFaster>()
                .Then((state, evnt) => Console.WriteLine("I cannae go nae faster..."));

            InState(Fast)
                .When<GoStop>()
                .TransitionTo(Stopped);

            RegisterParentStateFor(Slow, () => Moving);
            RegisterParentStateFor(Fast, () => Moving);

            AddTransitionLog((x) => Console.WriteLine(x));
        }
    }
}
