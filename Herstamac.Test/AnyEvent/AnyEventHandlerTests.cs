using System;
using Shouldly;
using Xunit;

namespace Herstamac.Test.DefaultHandlers;

public class AnyEventHandlersTests
{
    readonly MachineDefinition<AnyHandlerState> MachineDefinition;
    readonly IMachineState<AnyHandlerState> MachineState;
    readonly AnyHandlerBuilder MachineBuilder = new();

    public AnyEventHandlersTests()
    {
        MachineDefinition = MachineBuilder.GetMachineDefinition(config =>
        {
            config.Name("AnyHandler");
            config.Logger(x => Console.WriteLine(x));
            config.LogEventWith(x => x?.ToString() ?? string.Empty);
            config.UniqueId.FromProperty(p => p.Id);
        });
        MachineState = MachineDefinition.NewMachineInstance(new AnyHandlerState());

        MachineRunner.Start(MachineDefinition, MachineState);
    }

    [Fact]
    public void InitialConditionsAreCorrect()
    {
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Outer).ShouldBeTrue();
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner).ShouldBeFalse();
    }

    [Fact]
    public void MachineTransitionsToInnerStateAndRunsHandler()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new AnyHandlerBuilder.GoFaster());
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner).ShouldBeTrue();
    }

    [Fact]
    public void DefaultHandlersAreRun()
    {
        // When - GoFaster is encountered.
        MachineRunner.Dispatch(MachineDefinition, MachineState, new AnyHandlerBuilder.GoFaster());

        // Then - the AnyEvent handler on the outer state are executed.
        // Then - the entry events on the Inner state do not cause the AnyEvent() handlers to be executed.
        MachineState.CurrentInternalState.Counter1.ShouldBe(1);
        MachineState.CurrentInternalState.Counter2.ShouldBe(0);

        // Then - GoFaster has caused a transition to Inner state...
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner).ShouldBeTrue();
    }
}
