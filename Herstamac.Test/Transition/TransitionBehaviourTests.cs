using Herstamac.Lifts;
using Shouldly;
using Xunit;

namespace Herstamac.Test.Transition;

public class TransitionBehaviourTests
{
    MachineDefinition<TransistionState> MachineDefinition;
    IMachineState<TransistionState> InternalState;

    TransitionMachineBuilder MachineBuilder;

    Lift<TransistionState> Lift;

    public TransitionBehaviourTests()
    {
        MachineBuilder = new TransitionMachineBuilder();
        MachineDefinition = MachineBuilder.GetMachineDefinition( configure =>
        {
            configure.Logger((x) => Console.WriteLine(x));
        });
        InternalState = MachineDefinition.NewMachineInstance(new TransistionState());

        Lift = new Lift<TransistionState>(InternalState, MachineDefinition);

        // Given - The Machine is started
        MachineRunner.Start(MachineDefinition, InternalState);
    }

    [Fact]
    public void Test1()
    {
        // Given - The starting conditions

        // When - At start

        // Then - The machine is in the expected state.
        Lift.IsInState(MachineBuilder.Apple).ShouldBeTrue();
    }

    [Fact]
    public void TransistionsOnString()
    {
        // Given - The starting conditions

        // When - At start
        Lift.Dispatch("Here");

        // Then - The machine has transitioned to banna.
        Lift.IsInState(MachineBuilder.Banna).ShouldBeTrue();
    }

    // Future Functionality
    //[Fact]
    //public void HandlesAnExceptionGracefully()
    //{
    //    // Given - The starting conditions

    //    // When - At start
    //    Lift.Dispatch("Here");
    //    Lift.Dispatch("Fault!");

    //    // Then - The machine is in the expected state.
    //    Lift.IsInState(MachineBuilder.Banna).ShouldBeTrue();
    //}
}
