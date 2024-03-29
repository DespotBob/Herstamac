﻿using System;
using Herstamac.Fluent;
using Shouldly;
using Xunit;

namespace Herstamac.Test
{
    public class SwitchUp { }
    public class SwitchDown  { }

    public class GivenAStateMachineWithTwoState
    {
        MachineBuilder<OnOffInternalState> machineBuilder = new OnOffStateMachineBuilder();
        MachineDefinition<OnOffInternalState> MachineDefintion;

        UnitTestState On = new UnitTestState("On");
        UnitTestState Off = new UnitTestState("Off");

        int executed = 0;
        IMachineState<OnOffInternalState> MachineState;

        
        public GivenAStateMachineWithTwoState()
        {
            Console.WriteLine("Starting...");

            machineBuilder.RegisterState(On);
            machineBuilder.RegisterState(Off);

            machineBuilder.InState(On)
                .OnEntry()
                .Then((s, e) => this.executed = 13);

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .Then((state, evnt) =>
                {
                    state.counter += 3;
                    Console.WriteLine("Rx'd Switchup!");
                });

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard((state, x) => false )
                .Then((state, x) => { throw new Exception("The guard should prevent this from being thrown!"); } );

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard((state,x) => true)
                .Then((state,x) => Console.WriteLine("Rx'd Switchup! - with a Guard Condition"));

            machineBuilder.InState(On)
                .When<SwitchUp>()
                .WithGuard( (state,x) => true)
                .Then()
                .TransitionTo(Off);

            machineBuilder.InState(Off)
                .When<SwitchDown>()
                .TransitionTo(On);

            MachineDefintion = machineBuilder.GetMachineDefinition();
            MachineState     = MachineDefintion.NewMachineInstance(new OnOffInternalState());
            

            MachineRunner.Start(MachineDefintion, MachineState);

            MachineRunner.IsInState( MachineState,  MachineDefintion, On).ShouldBeTrue();
        }

        [Fact]
        public void WhenTheStateMachineIsStarted()
        {
            // Then the entry conditions of the initial state are run!
            this.executed.ShouldBe(13);
        }

        [Fact]
        public void WhenTheCurrentStateIsQueried()
        {
            // Then - The first state registered is the current state.
            MachineRunner.IsInState(MachineState, MachineDefintion, On).ShouldBeTrue();
        }

        [Fact]
        public void GivenTheMachineIsInTheOffPositionWhenItReceivesASwitchDownItTransitionsToOn ()
        {
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchDown());
            MachineRunner.IsInState(MachineState, MachineDefintion, On).ShouldBeTrue();
        }

        [Fact]
        public void WhenAStateTransitionOccursTheNewStateHasItsEntryConditionCalled()
        {
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchUp());
            MachineRunner.Dispatch(MachineDefintion, MachineState, new SwitchDown());

            MachineState.CurrentInternalState.counter.ShouldBe(3);
            MachineRunner.IsInState(MachineState, MachineDefintion, On).ShouldBeTrue();
        }
    }
}
