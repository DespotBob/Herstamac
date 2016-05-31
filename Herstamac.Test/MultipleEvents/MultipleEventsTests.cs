using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test.MultipleEvents
{
    [TestClass]
    public class MultipleEventsTests
    {
        MachineDefinition<MultipleEventsMachineState> MachineDefinition;
        IMachineState<MultipleEventsMachineState> MachineState;
        MultipleEventsStateMachine MachineBuilder;

        [TestInitialize]
        public void Init()
        {
            MachineBuilder = new MultipleEventsStateMachine();
            MachineDefinition = MachineBuilder.GetMachineDefinition(config =>
            {
                config.Name("MultipleEventsTest");
                config.Logger(x => Console.WriteLine(x));
                config.LogEventWith(x => x.ToString());
                config.UniqueId.FromProperty(p => p.Id);
            });
            MachineState = MachineDefinition.NewMachineState(new MultipleEventsMachineState());

            MachineRunner.Start(MachineDefinition, MachineState);
        }

        [TestMethod]
        public void InitialConditionsAreCorrect()
        {
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Sane));
        }

        [TestMethod]
        public void TransistionsOnOneTypeOfMessages()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new MultipleEventsStateMachine.TelephoneCall());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Insane));
        }

        [TestMethod]
        public void TransistionsOnTheOtherTypeOfMessages()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new MultipleEventsStateMachine.EmailMessage());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, MachineBuilder.Insane));
        }
    }
}
