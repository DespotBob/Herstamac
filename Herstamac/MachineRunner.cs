using System;
using System.Collections.Generic;
using System.Linq;

namespace Herstamac
{
    public static class MachineRunner
    {
        public static void Start<TInternalState>(MachineDefinition<TInternalState> machineDefinition, IMachineState<TInternalState> internalState)
        {
            Dispatch(machineDefinition, internalState, new Events.EntryEvent());
        }

        public static void Dispatch<TInternalState, TEvent>(MachineDefinition<TInternalState> machineDefinition, IMachineState<TInternalState> machineState, TEvent evnt)
        {
            Dispatch(evnt
                , machineState
                , machineState.CurrentState
                , machineDefinition.ParentStates
                , machineDefinition.EventInterceptors
                , machineDefinition.Config
                , true);
        }

        public static bool IsInState<TInternalState>(IMachineState<TInternalState> internalState, MachineDefinition<TInternalState> machineDefinition, InternalState<TInternalState> state)
        {
            return Misc<TInternalState>.FindAllStates(machineDefinition.ParentStates, internalState.CurrentState)
                .Any(currentState => currentState == state);
        }

        private static void Dispatch<TInternalState, TEvent>(TEvent evnt, IMachineState<TInternalState> internalState
           , InternalState<TInternalState> currentState
           , IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> relations
           , IEnumerable<Func<object, object>> eventInterceptors
           , MachineConfiguration<TInternalState> config
           , bool outerTransitionsTakePrecedence)
        {
            var currentStates = Misc<TInternalState>.FindAllStates(relations, currentState);

            if (!outerTransitionsTakePrecedence)
            {
                currentStates.Reverse();
            }

            var nextState = DispatchToStates<TInternalState, TEvent>(evnt, internalState, currentStates, eventInterceptors, config);

            if (nextState != null && nextState != currentStates.Last())
            {
                nextState = TransitionTo( (object) evnt, internalState, relations, nextState, eventInterceptors, false, config);
            }

            if (nextState != null)
            {
                UpdateStateHistories(internalState, relations, nextState);
            }
        }

        private static InternalState<TInternalState> TransitionTo<TInternalState>(
             object evnt
           , IMachineState<TInternalState> machineState
           , IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> relations
           , InternalState<TInternalState> transitionToState
           , IEnumerable<Func<object, object>> eventInterceptors
           , bool exitInnerStatesFirst
           , MachineConfiguration<TInternalState>  config)
        {
            config.Logger(string.Format("SM:{0}:{1} = State: {2} -> {3} on ^{4} = '{5}'"
                , config.Name
                , config.GetUniqueId(machineState.CurrentInternalState)
                , machineState.CurrentState.Name
                , transitionToState.Name
                , evnt.GetType().Name
                , config.LogEvents(evnt)));

            var _currentStates = Misc<TInternalState>.FindAllStates(relations, machineState.CurrentState);
            var _nextStates = Misc<TInternalState>.FindAllStates(relations, transitionToState);

            while (machineState.StateHistory.ContainsKey(transitionToState))
            {
                var nextTransitionToState = machineState.StateHistory[transitionToState];
                _nextStates.Add(nextTransitionToState);
                transitionToState = nextTransitionToState;
            }

            var entryConditionsToRun = _nextStates.Except(_currentStates);
            var exitConditionsToRun = _currentStates.Except(_nextStates);

            if (exitInnerStatesFirst)
            {
                entryConditionsToRun.Reverse();
                exitConditionsToRun.Reverse();
            }

            // Dispatch and do not transition...
            var newState = DispatchToStates(new Events.ExitEvent(), machineState, exitConditionsToRun, eventInterceptors, config);

            machineState.ChangeState(_nextStates.Last());

            // Dispath entry event - transition if neccesary  and do not transition...
            newState = DispatchToStates(new Herstamac.Events.EntryEvent(), machineState, entryConditionsToRun, eventInterceptors, config);
            if (newState != null)
            {
                transitionToState = TransitionTo(evnt, machineState, relations, newState, eventInterceptors, exitInnerStatesFirst, config);
            }

            return transitionToState;
        }

        /// <summary>
        /// Runs through all the current states - and updates the state histories to the current ones.
        /// </summary>
        private static void UpdateStateHistories<TInternalState>(IMachineState<TInternalState> internalState, IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> parentStates, InternalState<TInternalState> nextState)
        {
            foreach (var state in Misc<TInternalState>.FindAllStates(parentStates, nextState))
            {
                var parentState = Misc<TInternalState>.FindParentState(parentStates, state);

                if ((parentState != null) && internalState.StateHistory.ContainsKey(parentState))
                {
                    internalState.StateHistory[parentState] = state;
                }
            }
        }

        private static TEvent ExecuteInterceptorsForEvent<TEvent>(TEvent evnt, IEnumerable<Func<object, object>> eventInterceptors)
        {
            TEvent evntToDispatch = evnt;
            foreach (var interceptor in eventInterceptors)
            {
                evntToDispatch = (TEvent)interceptor(evnt);
                if (evntToDispatch == null)
                {
                    break;
                }
            }
            return evntToDispatch;
        }

        private static InternalState<TInternalState> DispatchToStates<TInternalState, TEvent>(TEvent evnt
            , IMachineState<TInternalState> internalState
            , IEnumerable<InternalState<TInternalState>> statesToDispatchTo
            , IEnumerable<Func<object, object>> eventInterceptors
            , MachineConfiguration<TInternalState> config)
        {
            TEvent evntToDispatch = ExecuteInterceptorsForEvent<TEvent>(evnt, eventInterceptors);

            if (evntToDispatch == null)
            {
                // Do not dispatch a null event!
                return null;
            }
            InternalState<TInternalState> finalTransitionState = null;

            foreach (var currentState in statesToDispatchTo)
            {
                if (currentState._handlers.ContainsKey(evnt.GetType()))
                {
                    var handler = currentState._handlers[evnt.GetType()];

                    Action<string> log = str => config.Logger(string.Format("SM:{0}:{1} = State: {2} - {3}"
                        , config.Name
                        , config.GetUniqueId(internalState.CurrentInternalState)
                        , currentState.Name
                        , str));

                    foreach (var action in handler.TransistionDefinitions)
                    {
                        if (action.GuardCondition(internalState.CurrentInternalState, evnt))
                        {
                            if (action.Action != null)
                            {
                                action.Action(internalState.CurrentInternalState, evnt, log );
                            }

                            if (action.TransitionTo != null)
                            {
                                finalTransitionState = finalTransitionState ?? action.TransitionTo;
                            }
                        }
                    }
                }

                // Place holder...
                // finalTransitionState = DispatchToStateViaReflection(evnt, internalState.CurrentInternalState, currentState) ?? finalTransitionState;
            }

            return finalTransitionState;
        }
    }
}
