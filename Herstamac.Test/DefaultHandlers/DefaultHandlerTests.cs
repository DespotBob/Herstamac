using Shouldly;
using System;
using Xunit;

namespace Herstamac.Test.DefaultHandlers;

public class DefaultHandlersTests
{
    MachineDefinition<DefaultHandlerState> MachineDefinition;
    IMachineState<DefaultHandlerState> MachineState;
    DefaultHandlerBuilder MachineBuilder = new DefaultHandlerBuilder();


    public DefaultHandlersTests()
    {

        MachineDefinition = MachineBuilder.GetMachineDefinition(config =>
        {
            config.Name("DefaultHandler");
            config.Logger(x => Console.WriteLine(x));
            config.LogEventWith(x => x.ToString());
            config.UniqueId.FromProperty(p => p.Id);
        });
        MachineState = MachineDefinition.NewMachineInstance(new DefaultHandlerState());

        MachineRunner.Start(MachineDefinition, MachineState);
    }

    [Fact]
    public void InitialConditionsAreCorrect()
    {
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner).ShouldBeTrue();
    }

    [Fact]
    public void DefaultHandlerIsNotRun()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new DefaultHandlerBuilder.GoFaster());
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner).ShouldBeTrue();
    }

    [Fact]
    public void DefaultHandlersAreRun()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new DefaultHandlerBuilder.GoStop());
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Error).ShouldBeTrue();
        MachineState.CurrentInternalState.Counter.ShouldBe(1);
    }
}