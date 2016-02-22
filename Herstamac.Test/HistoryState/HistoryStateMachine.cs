using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herstamac.Test.HistoryState
{
    public class HistoryState 
    {
        public int count;
    }

    public class HistoryStateMachineBuilder : Herstamac.MachineBuilder<HistoryState>
    {
        public Herstamac.State<HistoryState> RunningState = NewState("RunningState");
        public Herstamac.State<HistoryState> SteerLeftState = NewState("SteerLeftState");
        public Herstamac.State<HistoryState> SteerMiddleState = NewState("SteerMiddleState");
        public Herstamac.State<HistoryState> SteerRightState = NewState("SteerRightState");

        public Herstamac.State<HistoryState> StoppedState = NewState("StoppedState");

        public class TurnLeftEvent : Event { }
        public class StopEvent : Event { }
        public class StartEvent : Event { }

        public HistoryStateMachineBuilder()
        {
            Emit = (str) =>
            {
                events.Add(str);
                System.Console.WriteLine("Emit: '{0}'", str);
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
                .When<StopEvent>()
                .TransitionTo(StoppedState);

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
