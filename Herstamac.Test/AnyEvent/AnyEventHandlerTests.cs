using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test.DefaultHandlers
{
    [TestClass]
    public class AnyEventHandlersTests
    {
        MachineDefinition<AnyHandlerState> MachineDefinition;
        IMachineState<AnyHandlerState> MachineState;
        AnyHandlerBuilder MachineBuilder = new AnyHandlerBuilder();

        [TestInitialize]
        public void Init()
        {
            MachineDefinition = MachineBuilder.GetMachineDefinition(config =>
            {
                config.Name("AnyHandler");
                config.Logger(x => Console.WriteLine(x));
                config.LogEventWith(x => x.ToString());
                config.UniqueId.FromProperty(p => p.Id);
            });
            MachineState = MachineBuilder.NewMachineState(new AnyHandlerState());

            MachineRunner.Start(MachineDefinition, MachineState);
        }

        [TestMethod]
        public void InitialConditionsAreCorrect()
        {
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Outer));
            Assert.IsFalse(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner));
        }

        [TestMethod]
        public void MachineTransitionsToInnerStateAndRunsHandler()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new AnyHandlerBuilder.GoFaster() );
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner));
        }

        [TestMethod]
        public void DefaultHandlersAreRun()
        {
            // When - GoFaster is encountered.
            MachineRunner.Dispatch(MachineDefinition, MachineState, new AnyHandlerBuilder.GoFaster());

            // Then - the AnyEvent handler on the outer state are executed.
            // Then - the entry events on the Inner state do not cause the AnyEvent() handlers to be executed.
            Assert.AreEqual(1, MachineState.CurrentInternalState.Counter1);
            Assert.AreEqual(0, MachineState.CurrentInternalState.Counter2);

            // Then - GoFaster has caused a transition to Inner state...
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner));
        }
    }
}
