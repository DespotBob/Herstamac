using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test.HistoryState
{
    [TestClass]
    public class UnitTest1
    {
        MachineDefinition<HistoryState> MachineDefinition;
        IMachineState<HistoryState>      InternalState;
        HistoryStateMachineBuilder       MachineBuilder;

        [TestInitialize]
        public void GivenANewlyInitialisedSM()
        {
            MachineBuilder = new HistoryStateMachineBuilder();
            MachineDefinition = MachineBuilder.GetMachineDefinition();
            InternalState = MachineBuilder.NewMachineState(new HistoryState());

            // Given - The Machine is started
            MachineRunner.Start(MachineDefinition, InternalState);
        }

        [TestMethod]
        public void WhenStartedTheStateMachineIsInTheStoppedState()
        {
            // Then - the correct state is entered.
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.StoppedState));
        }

        [TestMethod]
        public void WhenTheMachineStartedAndAStartEventIsIssued()
        {
            // When - The machine is Started AND the start event is issued
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

            // Then - the correct state is entered.
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.RunningState));
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerMiddleState));
        }

        [TestMethod]
        public void WhenAMachineIsInAStateWithAHistoryAndItRentersThatState()
        {
            // When - A Start event is issued
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

            // THEN - 
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.RunningState));
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerMiddleState));

            // WHEN - 
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.TurnLeftEvent());

            // THEN - 
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerLeftState));
            
            // WHEN - 
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StopEvent());

            // THEN - 
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.StoppedState));

            // WHEN - 
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

            // THEN - 
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerLeftState));
        }
    }
}
