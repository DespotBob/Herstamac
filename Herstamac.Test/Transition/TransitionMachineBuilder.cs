using Herstamac.Fluent;
using System;

namespace Herstamac.Test.Transition
{
    public class TransitionMachineBuilder : MachineBuilder<TransistionState>
    {
        public State Outerstate = NewState("Outerstate");

        public State Apple = NewState("Apple");
        public State Banna = NewState("Banna");

        public class FeelingLonely { }

        public TransitionMachineBuilder()
        {
            RegisterState(Outerstate);
            RegisterState(Apple);
            RegisterState(Banna);

            RegisterParentStateFor(Apple, () => Outerstate);
            RegisterParentStateFor(Banna, () => Apple);

            // Outer
            InState(Outerstate)
                .OnEntry()
                .Then()
                .TransitionTo(Apple);

            InState(Apple)
                .When<string>()
                .Then()
                .TransitionTo(Banna);

            InState(Banna)
                .When<string>()
                .WithGuard( (s,e)=> e == "Fault!")
                .Then(() => { throw new ArgumentNullException(); });
        }
    }
}
