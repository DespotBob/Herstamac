using System;
using Herstamac.Fluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using static Herstamac.Test.HistoryState.HistoryStateMachineBuilder;

namespace Herstamac.Test.HistoryState
{
    [TestClass]
    public class UnitTest1
    {
        MachineDefinition<HistoryState> MachineDefinition;
        IMachineState<HistoryState> InternalState;
        HistoryStateMachineBuilder MachineBuilder;

        [TestInitialize]
        public void GivenANewlyInitialisedSM()
        {
            MachineBuilder = new HistoryStateMachineBuilder();
            MachineDefinition = MachineBuilder.GetMachineDefinition();
            InternalState = MachineDefinition.NewMachineInstance(new HistoryState());

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
            GivenTheMachineIsStarted();

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

        private void GivenTheMachineIsStarted()
        {
            // When - A Start event is issued
            MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

            // THEN - 
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.RunningState));
            Assert.IsTrue(MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerMiddleState));
        }

        [TestMethod]
        public void WhenACallersAsksForAllPossibleEventsThatCouldCauseStateToChange()
        {
            GivenTheMachineIsStarted();

            // When - A called request all the events that can trigger some from of action.
            var eventTypes = MachineRunner.CurrentlyActionableEvents(InternalState, MachineDefinition);

            // Then - The expected types of the events are returned...
            Assert.IsTrue(eventTypes.Contains(typeof(TurnLeftEvent)));
            Assert.IsTrue(eventTypes.Contains(typeof(StopEvent)));
            Assert.IsTrue(eventTypes.Contains(typeof(Events.AnyEvent)));

            // Then - we do not expected these events to be returned.
            Assert.IsFalse(eventTypes.Contains(typeof(Events.EntryEvent)));
            Assert.IsFalse(eventTypes.Contains(typeof(Events.ExitEvent)));

            Assert.AreEqual(3, eventTypes.Count());
        }

        [TestMethod]
        public void TestingSmallBitsOftheBusinessLogic()
        {
            GivenTheMachineIsStarted();

            Assert.IsFalse(typeof(HistoryStateMachineBuilder.UserActionable).IsAssignableFrom(typeof(HistoryStateMachineBuilder.StartEvent)));
            Assert.IsTrue(typeof(HistoryStateMachineBuilder.UserActionable).IsAssignableFrom(typeof(HistoryStateMachineBuilder.StopEvent)));
        }
    }
}
