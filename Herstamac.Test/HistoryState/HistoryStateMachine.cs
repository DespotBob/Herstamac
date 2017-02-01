using System;
using System.Collections.Generic;
using Herstamac.Fluent;

namespace Herstamac.Test.HistoryState
{
    public class HistoryState 
    {
        public int count;
        public int ExitCounter;
    }

    public class HistoryStateMachineBuilder : MachineBuilder<HistoryState>
    {
        public State RunningState = NewState("RunningState");
        public State SteerLeftState = NewState("SteerLeftState");
        public State SteerMiddleState = NewState("SteerMiddleState");
        public State SteerRightState = NewState("SteerRightState");

        public State StoppedState = NewState("StoppedState");

        /// <summary>
        /// Application Specific Business Logic - We will allow any event with this interface to be able to be raised by the user.
        /// </summary>
        public interface UserActionable { };

        public class TurnLeftEvent : UserActionable { }
        public class StopEvent : UserActionable { }
        public class StartEvent  { }

        public HistoryStateMachineBuilder()
        {
            Emit = (str) =>
            {
                events.Add(str);
                Console.WriteLine("Emit: '{0}'", str);
            };

            AddEventInterceptor((evnt) =>
            {
                Console.WriteLine("Rx'd: {0}", evnt.GetType().Name);
                return evnt;
            });

            RegisterState(RunningState);
            RegisterState(SteerLeftState);
            RegisterState(SteerMiddleState);
            RegisterState(SteerRightState);
            RegisterState(StoppedState);

            RegisterParentStateFor(SteerLeftState, () => RunningState);
            RegisterParentStateFor(SteerMiddleState, () => RunningState);
            RegisterParentStateFor(SteerRightState, () => RunningState);

            RegisterHistoryState(RunningState, SteerMiddleState); 

            InState(StoppedState)
                .When<StartEvent>()
                .TransitionTo(RunningState);

            InState(RunningState)
                .OnAnyEvent()
                .Then((s,e)=> {  });

            InState(RunningState)
                .When<StopEvent>()
                .TransitionTo(StoppedState);

            InState(RunningState)
                .OnExit()
                .Then((state, evnt) => state.ExitCounter++);

            InState(RunningState)
                .OnEntry()
                .Then((state, evnt) => state.count++);

            InState(SteerRightState)
                .When<TurnLeftEvent>()
                .TransitionTo(SteerMiddleState);

            InState(SteerMiddleState)
                .When<TurnLeftEvent>()
                .TransitionTo(SteerLeftState); 

            InState(SteerLeftState)
                .When<TurnLeftEvent>()
                .Then( () => Emit("Cannot turn any further!"));

        }

        public List<string> events = new List<string>();

        public Action<string> Emit;
    }
}
