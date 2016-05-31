using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Herstamac;

namespace Herstamac.Test.DefaultHandlers
{
    [TestClass]
    public class DefaultHandlersTests
    {
        MachineDefinition<DefaultHandlerState> MachineDefinition;
        IMachineState<DefaultHandlerState> MachineState;
        DefaultHandlerBuilder MachineBuilder = new DefaultHandlerBuilder();

        [TestInitialize]
        public void Init()
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

        [TestMethod]
        public void InitialConditionsAreCorrect()
        {
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner));
        }

        [TestMethod]
        public void DefaultHandlerIsNotRun()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new DefaultHandlerBuilder.GoFaster() );
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Inner));
        }

        [TestMethod]
        public void DefaultHandlersAreRun()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new DefaultHandlerBuilder.GoStop());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Error));
            Assert.AreEqual(1, MachineState.CurrentInternalState.Counter);
        }
    }
}
