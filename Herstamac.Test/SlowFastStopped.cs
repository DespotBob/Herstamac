using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Herstamac.Test
{
    using SlowFastStopped;
    
    [TestClass]
    public class SlowFastStoppedTests
    {
        SlowFastStoppedStateMachineBuilder machine = new SlowFastStoppedStateMachineBuilder();
        MachineDefinition<SlowFastStoppedInternalState> MachineDefinition;
        IMachineState<SlowFastStoppedInternalState> MachineState;

        [TestInitialize]
        public void GivenANewlyInitialisedSM()
        {
            machine.AddEventInterceptor((evnt) =>
            {
                Console.WriteLine("Rx'd Event: {0}", evnt.GetType().Name);
                return evnt;
            });

            machine.AddEventInterceptor((evnt) =>
            {
                Console.WriteLine("Rx'd Event2: {0}", evnt.GetType().Name);
                return evnt;
            });

            MachineDefinition = machine.GetMachineDefinition( config => 
            {
                config.Name("FastSlow");
                config.Logger(x => Console.WriteLine(x));
                config.LogEventWith(x => x.ToString());
                config.UniqueId.FromProperty(p => p.Id);
            });

            MachineState = machine.NewMachineState(new SlowFastStoppedInternalState());
        }

        [TestMethod]
        public void WhenTheCurrentStateIsQueried()
        {
            // Then - The first state registered is the current state.
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
        }

        [TestMethod]
        public void WhenTheMachineTransitionsToANestedState()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());

            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow));
        }

        [TestMethod]
        public void WhenTheTwoGoFasterareReceviedThenResultingStateIsFast()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());

            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Fast));
            Assert.IsFalse(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow));
        }

        [TestMethod]
        public void WhenTheTwoGoFasterAndAStoppedAreReceviedThenResultingStateIsStopped()
        {

            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());

            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
        }

        [TestMethod]
        public void GoFastLogicTest96()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow));

            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
        }

        [TestMethod]
        public void GoFastLogicTest97()
        {
            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Slow));

            MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoStop());
            Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));

        }
    }
}
