using System;
using Herstamac.Test.MultipleEvents;
using Xunit;

namespace Herstamac.Test
{
	public class ConfigurationTests
	{
        readonly MachineDefinition<MultipleEventsMachineState> MachineDefinition;
        readonly IMachineState<MultipleEventsMachineState> MachineState;
        readonly MultipleEventsStateMachine MachineBuilder;

        public ConfigurationTests()
        {
            MachineBuilder = new MultipleEventsStateMachine();

            MachineDefinition = MachineBuilder.GetMachineDefinition((x) =>
			{
                x.Name("Roger");
                x.UniqueId.FromProperty(t => t.Id);
                x.Logger((str) => Console.WriteLine(str));
			});

            MachineState = MachineDefinition.NewMachineInstance(new MultipleEventsMachineState());


        }

        [Fact]
        public void Start()
        {
            MachineRunner.Start(MachineDefinition, MachineState);
        }
    }
}
