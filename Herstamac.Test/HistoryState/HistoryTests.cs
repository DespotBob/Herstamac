using System;
using System.Linq;
using static Herstamac.Test.HistoryState.HistoryStateMachineBuilder;
using Xunit;
using Shouldly;

namespace Herstamac.Test.HistoryState;

public class UnitTest1
{
    readonly MachineDefinition<HistoryState> MachineDefinition;
    readonly IMachineState<HistoryState> InternalState;
    readonly HistoryStateMachineBuilder MachineBuilder;

    
    public UnitTest1()
    {
        MachineBuilder = new HistoryStateMachineBuilder();
        MachineDefinition = MachineBuilder.GetMachineDefinition();
        InternalState = MachineDefinition.NewMachineInstance(new HistoryState());

        // Given - The Machine is started
        MachineRunner.Start(MachineDefinition, InternalState);
    }

    [Fact]
    public void WhenStartedTheStateMachineIsInTheStoppedState()
    {
        // Then - the correct state is entered.
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.StoppedState).ShouldBeTrue();
    }

    [Fact]
    public void WhenTheMachineStartedAndAStartEventIsIssued()
    {
        // When - The machine is Started AND the start event is issued
        MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

        // Then - the correct state is entered.
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.RunningState).ShouldBeTrue();
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerMiddleState).ShouldBeTrue();
    }

    [Fact]
    public void WhenAMachineIsInAStateWithAHistoryAndItRentersThatState()
    {
        GivenTheMachineIsStarted();

        // WHEN - 
        MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.TurnLeftEvent());

        // THEN - 
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerLeftState).ShouldBeTrue();

        // WHEN - 
        MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StopEvent());

        // THEN - 
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.StoppedState).ShouldBeTrue();

        // WHEN - 
        MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

        // THEN - 
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerLeftState).ShouldBeTrue();
    }

    private void GivenTheMachineIsStarted()
    {
        // When - A Start event is issued
        MachineRunner.Dispatch(MachineDefinition, InternalState, new HistoryStateMachineBuilder.StartEvent());

        // THEN - 
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.RunningState).ShouldBeTrue();
        MachineRunner.IsInState(InternalState, MachineDefinition, MachineBuilder.SteerMiddleState).ShouldBeTrue();
    }

    [Fact]
    public void WhenACallersAsksForAllPossibleEventsThatCouldCauseStateToChange()
    {
        GivenTheMachineIsStarted();

        // When - A called request all the events that can trigger some from of action.
        var eventTypes = MachineRunner.CurrentlyActionableEvents(InternalState, MachineDefinition);

        // Then - The expected types of the events are returned...
        eventTypes.ShouldContain(typeof(TurnLeftEvent));
        eventTypes.ShouldContain(typeof(StopEvent));
        eventTypes.ShouldContain(typeof(Events.AnyEvent));

        // Then - we do not expected these events to be returned.
        eventTypes.ShouldNotContain(typeof(Events.EntryEvent));
        eventTypes.ShouldNotContain(typeof(Events.ExitEvent));

        eventTypes.Count().ShouldBe(3);
    }

    [Fact]
    public void TestingSmallBitsOftheBusinessLogic()
    {
        GivenTheMachineIsStarted();

        typeof(UserActionable).IsAssignableFrom(typeof(StartEvent)).ShouldBeFalse();
        typeof(UserActionable).IsAssignableFrom(typeof(StopEvent)).ShouldBeTrue();
    }
}
