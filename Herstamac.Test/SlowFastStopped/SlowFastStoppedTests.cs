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
                /* Name the Statemachine */
                config.Name("FastSlow");

                /* Define where the logging information is going! */
                config.Logger(x => Console.WriteLine(x));

                /* Define a function that will be used to serialise an event to a string */
                config.LogEventWith(x => x.ToString());

                /* Hmmm - Every state machine needs a unique Id - Get this one from here, otherwise it's a GUID! */
                config.UniqueId.FromProperty(p => p.Id);
            });

            MachineState = MachineDefinition.NewMachineState(new SlowFastStoppedInternalState());
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
