using Herstamac.Lifts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Herstamac.Test.Transition
{
    [TestClass]
    public class TransitionBehaviourTests
    {
        MachineDefinition<TransistionState> MachineDefinition;
        IMachineState<TransistionState> InternalState;

        TransitionMachineBuilder MachineBuilder;

        Lift<TransistionState> Lift;

        [TestInitialize]
        public void GivenANewlyInitialisedSM()
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

        [TestMethod]
        public void Test1()
        {
            // Given - The starting conditions

            // When - At start

            // Then - The machine is in the expected state.
            Assert.IsTrue(Lift.IsInState(MachineBuilder.Apple));
        }

        [TestMethod]
        public void TransistionsOnString()
        {
            // Given - The starting conditions

            // When - At start
            Lift.Dispatch("Here");

            // Then - The machine has transitioned to banna.
            Assert.IsTrue(Lift.IsInState(MachineBuilder.Banna));
        }

        [TestMethod]
        public void HandlesAnExceptionGracefully()
        {
            // Given - The starting conditions

            // When - At start
            Lift.Dispatch("Here");
            Lift.Dispatch("Fault!");

            // Then - The machine is in the expected state.
            Assert.IsTrue(Lift.IsInState(MachineBuilder.Banna));
        }
    }
}
