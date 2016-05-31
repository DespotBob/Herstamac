using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test
{
    public class SwitchUp { }
    public class SwitchDown  { }

    [TestClass]
    public class GivenAStateMachineWithTwoState
    {
        MachineBuilder<OnOffInternalState> machineBuilder = new OnOffStateMachineBuilder();
        MachineDefinition<OnOffInternalState> MachineDefintion;

        UnitTestState On = new UnitTestState("On");
        UnitTestState Off = new UnitTestState("Off");

        int executed = 0;
        IMachineState<OnOffInternalState> MachineState;

        [TestInitialize]
        public void Setup()
        {
            Console.WriteLine("Starting...");

            machineBuilder.RegisterState(On);
            machineBuilder.RegisterState(Off);

            machineBuilder.InState(On)
                .OnEntry()
                .Then((s, e) => this.executed = 13);

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .Then((state, evnt) =>
                {
                    state.counter += 3;
                    Console.WriteLine("Rx'd Switchup!");
                });

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard((state, x) => false )
                .Then((state, x) => { throw new Exception("The guard should prevent this from being thrown!"); } );

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard((state,x) => true)
                .Then((state,x) => Console.WriteLine("Rx'd Switchup! - with a Guard Condition"));

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard( (state,x) => true)
                .Then()
                .TransitionTo(Off);

            machineBuilder.InState(Off)
                .When<SwitchDown>()
                .TransitionTo(On);

            MachineDefintion = machineBuilder.GetMachineDefinition();
            MachineState     = MachineDefintion.NewMachineState(new OnOffInternalState());
            

            MachineRunner.Start(MachineDefintion, MachineState);

            Assert.IsTrue(MachineRunner.IsInState( MachineState,  MachineDefintion, On));
        }

        [TestMethod]
        public void WhenTheStateMachineIsStarted()
        {
            // Then the entry conditions of the initial state are run!
            Assert.AreEqual(13, this.executed);
        }

        [TestMethod]
        public void WhenTheCurrentStateIsQueried()
        {
            // Then - The first state registered is the current state.
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefintion, On));
        }

        [TestMethod]
        public void GivenTheMachineIsInTheOffPositionWhenItReceivesASwitchDownItTransitionsToOn ()
        {
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchDown());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefintion, On));
        }

        [TestMethod]
        public void WhenAStateTransitionOccursTheNewStateHasItsEntryConditionCalled()
        {
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchUp());
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchDown());

            Assert.AreEqual(3, MachineState.CurrentInternalState.counter);
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefintion, On));
        }
    }
}
