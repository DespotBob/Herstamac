using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test
{
    using Initial;

    [TestClass]
    public class InitialStateMachineTests
    {
        InitialMachineBuilder MachineBuilder;
        MachineDefinition<IState> MachineDefinition;
        IMachineState<IState> MachineState;

        [TestInitialize]
        public void GivenANewlyInitialisedSM()
        {
            MachineBuilder = new InitialMachineBuilder();

            MachineDefinition = MachineBuilder.GetMachineDefinition();
            MachineState      = MachineDefinition.NewMachineInstance(new State());
        }

        [TestMethod]
        public void GivenTheStateMachineHasNotBeenStartedThenTheInitialStateIs()
        {
           
            // Then - the machine is 
            MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Outer);

            // Then - there are no emitted events.
            Assert.AreEqual(0, MachineState.CurrentInternalState.Events.Count);
        }

        [TestMethod]
        public void WhenTheMachineIsStartedThenTheInitialStateIsEntered()
        {
            // When - The machine is started
            MachineRunner.Start(MachineDefinition, MachineState);

            // Then - the machine transitions to the inner state
            MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner);

            // Then - there are two emitted events.
            Assert.AreEqual(2, MachineState.CurrentInternalState.Events.Count);
        
            // Then - the first is OuterEntry
            Assert.AreEqual("Entry - OuterState", MachineState.CurrentInternalState.Events[0]);
            Assert.AreEqual("Entry - InnerState", MachineState.CurrentInternalState.Events[1]);
        }

        [TestMethod]
        public void WhenTheMachineIsRunningThenSomeoneFeelsLonley()
        {          
            // Given - There are no events in the que.
            MachineRunner.Start(MachineDefinition, MachineState);

            // When - Two feeling lonelys occur the machine transitions to Lonley

            MachineRunner.Dispatch(MachineDefinition, MachineState, new InitialMachineBuilder.FeelingLonely());
            MachineRunner.Dispatch(MachineDefinition, MachineState, new InitialMachineBuilder.FeelingLonely());

            // Then - the first is OuterEntry
            Assert.AreEqual(9, MachineState.CurrentInternalState.Events.Count);
            Assert.AreEqual("Entry - OuterState", MachineState.CurrentInternalState.Events[0]);
            Assert.AreEqual("Entry - InnerState", MachineState.CurrentInternalState.Events[1]);
            Assert.AreEqual("I Feel an outer Lonesome!", MachineState.CurrentInternalState.Events[2]);
            Assert.AreEqual("I Feel an inner Lonesome! - But I should not transition to the Death state!", MachineState.CurrentInternalState.Events[3]);
            Assert.AreEqual("Exit - OuterState", MachineState.CurrentInternalState.Events[4]);
            Assert.AreEqual("Exit - InnerState", MachineState.CurrentInternalState.Events[5]);
            Assert.AreEqual("Enter - LoneState", MachineState.CurrentInternalState.Events[6]);
            Assert.AreEqual("I feel lonely - Run code before transitioning to InnerLoneState", MachineState.CurrentInternalState.Events[7]);
            Assert.AreEqual("Enter - InnerLoneState", MachineState.CurrentInternalState.Events[8]);

            MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.LoneState);
            MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.GrandState);
        }
    }
}