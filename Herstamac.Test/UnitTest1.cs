using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test
{
    public class SwitchUp : Event { }
    public class SwitchDown : Event { }

    [TestClass]
    public class GivenAStateMachineWithTwoState
    {
        OnOffStateMachine machine = new OnOffStateMachine();

        UnitTestState On = new UnitTestState("On");
        UnitTestState Off = new UnitTestState("Off");

        int executed = 0;

        [TestInitialize]
        public void Setup()
        {
            Console.WriteLine("Starting...");

            machine.RegisterState(On);
            machine.RegisterState(Off);

            machine.InState(On)
                .OnEntry()
                .Then((s, e) => this.executed = 13);

            machine.InState(On)
                .When<SwitchUp>()
                .Then((state, evnt) =>
                {
                    state.counter += 3;
                    Console.WriteLine("Rx'd Switchup!");
                });

            machine.InState(On)
                .When<SwitchUp>()
                .WithGuard((state,x) => true)
                .Then((state,x) => Console.WriteLine("Rx'd Switchup! - with a Guard Condition"));

            machine.InState(On)
                .When<SwitchUp>()
                .WithGuard( (state,x) => true)
                .Then()
                .TransitionTo(Off);

            machine.InState(Off)
                .When<SwitchDown>()
                .TransitionTo(On);

            machine.Start();

            Assert.IsTrue(machine.IsInState(On));
        }

        [TestMethod]
        public void WhenTheStateMachineIsStarted()
        {
            machine.Start();

            // Then the entry conditions of the initial state are run!
            Assert.AreEqual(13, this.executed);
        }

        [TestMethod]
        public void WhenTheCurrentStateIsQueried()
        {
            // Then - The first state registered is the current state.
            Assert.AreEqual(On, machine.CurrentState);
        }

        [TestMethod]
        public void GivenTheMachineIsInTheOffPositionWhenItReceivesASwitchDownItTransitionsToOn ()
        {
            machine.Handle(new SwitchDown());
            Assert.AreEqual(On, machine.CurrentState);
        }

        [TestMethod]
        public void WhenAStateTransitionOccursTheOldStateHasItsExitConditionCalled()
        {
            int x = 0;

            Off.ExitEventHandler = (evnt, state) => { 
                x++;
                return Herstamac.EventHandledResponse<OnOffInternalState>.Nothing; 
            };

            // Given 
            machine.Handle(new SwitchUp());
            Assert.AreEqual(Off, machine.CurrentState);

            // When
            machine.Handle(new SwitchDown());

            // Then 
            Assert.AreEqual(On, machine.CurrentState);
            Assert.AreEqual(1, x);
        }

        [TestMethod]
        public void WhenAStateTransitionOccursTheNewStateHasItsEntryConditionCalled()
        {
            machine.Handle(new SwitchUp());
            machine.Handle(new SwitchDown());

            Assert.AreEqual(3, machine._state.CurrentInternalState.counter);
            Assert.IsTrue(machine.IsInState(On));
        }
    }
}
