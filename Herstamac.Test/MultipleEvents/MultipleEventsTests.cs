using System;
using Shouldly;
using Xunit;

namespace Herstamac.Test.MultipleEvents;

public class MultipleEventsTests
{
    readonly MachineDefinition<MultipleEventsMachineState> MachineDefinition;
    readonly IMachineState<MultipleEventsMachineState> MachineState;
    readonly MultipleEventsStateMachine MachineBuilder;

    public MultipleEventsTests()
    {
        MachineBuilder = new MultipleEventsStateMachine();
        MachineDefinition = MachineBuilder.GetMachineDefinition(config =>
        {
            config.Name("MultipleEventsTest");
            config.Logger(x => Console.WriteLine(x));
            config.LogEventWith(x => x.ToString());
            config.UniqueId.FromProperty(p => p.Id);
        });
        MachineState = MachineDefinition.NewMachineInstance(new MultipleEventsMachineState());

        MachineRunner.Start(MachineDefinition, MachineState);
    }

    [Fact]
    public void InitialConditionsAreCorrect()
    {
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Sane).ShouldBeTrue();
    }

    [Fact]
    public void TransistionsOnOneTypeOfMessages()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new MultipleEventsStateMachine.TelephoneCall());
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Insane).ShouldBeTrue();
    }
    
    [Fact]      
    public void TransistionsOnTheOtherTypeOfMessages()
    {
        MachineRunner.Dispatch(MachineDefinition, MachineState, new MultipleEventsStateMachine.EmailMessage());
        MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Insane).ShouldBeTrue();
    }
}
