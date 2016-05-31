using System;
using Herstamac.Test.MultipleEvents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test
{
	[TestClass]
	public class ConfigurationTests
	{
        MachineDefinition<MultipleEventsMachineState> MachineDefinition;
        IMachineState<MultipleEventsMachineState> MachineState;
        MultipleEventsStateMachine MachineBuilder;

        [TestInitialize]
        public void Init()
        {
            MachineBuilder = new MultipleEventsStateMachine();

            MachineDefinition = MachineBuilder.GetMachineDefinition((x) =>
			{
                x.Name("Roger");
                x.UniqueId.FromProperty(t => t.Id);
                x.Logger((str) => Console.WriteLine(str));
			});

            MachineState = MachineDefinition.NewMachineInstance(new MultipleEventsMachineState());

            MachineRunner.Start(MachineDefinition, MachineState);
        }
    }
}
