using System;
using System.Collections.Generic;

namespace Herstamac.Test.Initial
{
    public class State : Herstamac.Test.Initial.IState 
    {
        private readonly List<string> _events = new List<String>();

        public List<string> Events
        {
            get
            {
                return _events;
            }
        }
    }

    public class GoFaster { }

    public class InitialMachineBuilder : Herstamac.MachineBuilder<IState>
    {
        public Herstamac.State GrandState = NewState("GrandState");

        public Herstamac.State Outer = NewState("Outer");
        public Herstamac.State Inner = NewState("Inner");

        public Herstamac.State LoneState = new Herstamac.State("LoneState");
        public Herstamac.State InnerLoneState = new Herstamac.State("InnerLoneState");

        public Herstamac.State DeathState = new Herstamac.State("DeathState");

        public class FeelingLonely { }

        public InitialMachineBuilder()
        {
            Emit = (state, str) =>
            {
                state.Events.Add(str);
                System.Console.WriteLine("Emit: '{0}'", str);
            };

            AddEventInterceptor((evnt) =>
            {
                Console.WriteLine("Rx'd: {0}", evnt.GetType().Name);
                return evnt;
            });

            RegisterState(GrandState);
            RegisterState(Outer);
            RegisterState(Inner);

            RegisterState(LoneState);
            RegisterState(InnerLoneState);

            RegisterState(DeathState);

            // Outer
            InState(Outer)
                .OnEntry()
                .Then((state, evnt) => Emit(state, "Entry - OuterState"))
                .TransitionTo(Inner);

            InState(Outer)
                .OnExit()
                .Then((state, evnt) => Emit(state, "Exit - OuterState"));

            InState(Outer)
                .When<FeelingLonely>()
                .Then((state, lonely) => Emit(state, "I Feel an outer Lonesome!"))
                .TransitionTo(LoneState);

            // Inner
            InState(Inner)
                .OnEntry()
                .Then((state,evnt) => Emit(state, "Entry - InnerState"));

            InState(Inner)
                .OnExit()
                .Then((state, evnt) => Emit(state, "Exit - InnerState"));

            InState(Inner)
                .When<FeelingLonely>()
                .Then((state, lonely) => Emit(state, "I Feel an inner Lonesome! - But I should not transition to the Death state!"))
                .TransitionTo(DeathState);  // Test that this does not happen!

            // LoneState
            InState(LoneState)
                .OnEntry()
                .Then((state, lonely) => Emit(state, "Enter - LoneState"));

            InState(LoneState)
                .OnExit()
                .Then((state, lonely) => Emit(state, "Exit - LoneState"));

            InState(LoneState)
                .When<FeelingLonely>()
                .Then((state, lonely) => Emit(state, "I feel lonely - Run code before transitioning to InnerLoneState"))
                .TransitionTo(InnerLoneState);

            // Inner Lone State
            InState(InnerLoneState)
                .OnEntry()
                .Then( (state, evnt) =>  Emit(state, "Enter - InnerLoneState"));


            // Death State
            InState(DeathState)
                .OnEntry()
                .Then(() => { throw new ApplicationException("Arrhhed! I die!!!!"); });
               
            RegisterParentStateFor(Inner, () => Outer);
            RegisterParentStateFor(Outer, () => GrandState);
            RegisterParentStateFor(LoneState, () => GrandState);
            RegisterParentStateFor(InnerLoneState, () => LoneState);
        }



        public Action<IState, string> Emit;
    }
}
