
namespace Herstamac.Test;

using Initial;
using Shouldly;
using Xunit;

public class InitialStateMachineTests
{
    readonly InitialMachineBuilder MachineBuilder;
    readonly MachineDefinition<IState> MachineDefinition;
    readonly IMachineState<IState> MachineState;

    public InitialStateMachineTests()
    {
        MachineBuilder = new InitialMachineBuilder();

        MachineDefinition = MachineBuilder.GetMachineDefinition();
        MachineState = MachineDefinition.NewMachineInstance(new State());
    }

    [Fact]
    public void GivenTheStateMachineHasNotBeenStartedThenTheInitialStateIs()
    {

        // Then - the machine is 
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Outer);

        // Then - there are no emitted events.
        MachineState.CurrentInternalState.Events.Count.ShouldBe(0);
    }

    [Fact]
    public void WhenTheMachineIsStartedThenTheInitialStateIsEntered()
    {
        // When - The machine is started
        MachineRunner.Start(MachineDefinition, MachineState);

        // Then - the machine transitions to the inner state
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner);

        // Then - there are two emitted events.
        MachineState.CurrentInternalState.Events.Count.ShouldBe(2);

        // Then - the first is OuterEntry
        MachineState.CurrentInternalState.Events[0].ShouldBe("Entry - OuterState");
        MachineState.CurrentInternalState.Events[1].ShouldBe("Entry - InnerState");
    }

    [Fact]
    public void WhenTheMachineIsRunningThenSomeoneFeelsLonley()
    {
        // Given - There are no events in the que.
        MachineRunner.Start(MachineDefinition, MachineState);

        // When - Two feeling lonelys occur the machine transitions to Lonley

        MachineRunner.Dispatch(MachineDefinition, MachineState, new InitialMachineBuilder.FeelingLonely());
        MachineRunner.Dispatch(MachineDefinition, MachineState, new InitialMachineBuilder.FeelingLonely());

        // Then - the first is OuterEntry
        MachineState.CurrentInternalState.Events.Count.ShouldBe(9);
        MachineState.CurrentInternalState.Events[0].ShouldBe("Entry - OuterState");
        MachineState.CurrentInternalState.Events[1].ShouldBe("Entry - InnerState");
        MachineState.CurrentInternalState.Events[2].ShouldBe("I Feel an outer Lonesome!");
        MachineState.CurrentInternalState.Events[3].ShouldBe("I Feel an inner Lonesome! - But I should not transition to the Death state!");
        MachineState.CurrentInternalState.Events[4].ShouldBe("Exit - OuterState");
        MachineState.CurrentInternalState.Events[5].ShouldBe("Exit - InnerState");
        MachineState.CurrentInternalState.Events[6].ShouldBe("Enter - LoneState");
        MachineState.CurrentInternalState.Events[7].ShouldBe("I feel lonely - Run code before transitioning to InnerLoneState");
        MachineState.CurrentInternalState.Events[8].ShouldBe("Enter - InnerLoneState");

        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.LoneState);
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.GrandState);
    }
}
