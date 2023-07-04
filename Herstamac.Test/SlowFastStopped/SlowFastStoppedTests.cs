using System;
using Herstamac.Test.SlowFastStopped;
using Shouldly;
using Xunit;

namespace Herstamac.Test;

public class SlowFastStoppedTests
{
    readonly SlowFastStoppedStateMachineBuilder machine = new();
    readonly MachineDefinition<SlowFastStoppedInternalState> MachineDefinition;
    readonly IMachineState<SlowFastStoppedInternalState> MachineState;

    public SlowFastStoppedTests()
    {
        machine.AddEventInterceptor((evnt) =>
        {
            Console.WriteLine("Rx'd Event: {0}", evnt.GetType().Name);
            return evnt;
        });

        machine.AddEventInterceptor((evnt) =>
        {
            Console.WriteLine("Rx'd Event2: {0}", evnt.GetType().Name);
            return evnt;
        });

        MachineDefinition = machine.GetMachineDefinition( config => 
        {
            /* Name the Statemachine */
            config.Name("FastSlow");

            /* Define where the logging information is going! */
            config.Logger(x => Console.WriteLine(x));

            /* Define a function that will be used to serialise an event to a string */
            config.LogEventWith(x => x.ToString());

            /* Hmmm - Every state machine needs a unique Id - Get this one from here, otherwise it's a GUID! */
            config.UniqueId.FromProperty(p => p.Id);
        });

        MachineState = MachineDefinition.NewMachineInstance(new SlowFastStoppedInternalState());
    }

    [Fact]
    public void WhenTheCurrentStateIsQueried()
    {
        // Then - The first state registered is the current state.
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped).ShouldBeTrue();
    }

    [Fact]
    public void WhenTheMachineTransitionsToANestedState()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());

        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow).ShouldBeTrue();
    }

    [Fact]
    public void WhenTheTwoGoFasterareReceviedThenResultingStateIsFast()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());

        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Fast).ShouldBeTrue();
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow).ShouldBeFalse();
    }

    [Fact]
    public void WhenTheTwoGoFasterAndAStoppedAreReceviedThenResultingStateIsStopped()
    {

        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());

        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped).ShouldBeTrue();
    }

    [Fact]
    public void GoFastLogicTest96()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow).ShouldBeTrue();

        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped).ShouldBeTrue();
    }

    [Fact]
    public void GoFastLogicTest97()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow).ShouldBeTrue();

        MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());
        MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped).ShouldBeTrue();
    }
}
