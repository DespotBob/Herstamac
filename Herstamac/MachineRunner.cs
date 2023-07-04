using System;
using System.Collections.Generic;
using System.Linq;

namespace Herstamac
{
    public static class MachineRunner
    {
        public static void Start<TInternalState>(MachineDefinition<TInternalState> machineDefinition
            , IMachineState<TInternalState> internalState)
        {
            Dispatch(machineDefinition, internalState, new Events.EntryEvent());
        }

        public static void Dispatch<TInternalState, TEvent>(MachineDefinition<TInternalState> machineDefinition
            , IMachineState<TInternalState> machineState
            , TEvent evnt)
        {
            var lookUp = Find(machineDefinition);

            Dispatch(evnt
                , machineState
                , machineState.CurrentState
                , machineDefinition.ParentStates 
                , machineDefinition.EventInterceptors
                , machineDefinition.Config
                , true
                , lookUp);
        }

        public static bool IsInState<TInternalState>(IMachineState<TInternalState> internalState, 
            MachineDefinition<TInternalState> machineDefinition,
            State state)
        {
            return Misc<TInternalState>.FindAllStates(machineDefinition.ParentStates, internalState.CurrentState)
                .Any(currentState => currentState.Name == state.Name);
        }


        public static IEnumerable<InternalState<TInternalState>> CurrentStates<TInternalState>( IMachineState<TInternalState> internalState, MachineDefinition<TInternalState> machineDefinition )
        {
            return Misc<TInternalState>.FindAllStates(machineDefinition.ParentStates, internalState.CurrentState);
        }

        /// <summary>
        /// Returns a list of of the Events that the state machine will currently perform an action on.
        /// </summary>
        /// <typeparam name="TInternalState"></typeparam>
        /// <param name="internalState"></param>
        /// <param name="machineDefinition"></param>
        /// <returns>Will not return Events.EntryEvents or exit Events.ExitEvent events</returns>
        public static IEnumerable<Type> CurrentlyActionableEvents<TInternalState>(IMachineState<TInternalState> internalState, MachineDefinition<TInternalState> machineDefinition)
        {
            return Misc<TInternalState>.FindAllStates(machineDefinition.ParentStates, internalState.CurrentState)
                .SelectMany(x => x.Handlers)
                .Select(x => x.Key)
                .Where(x => x != typeof(Events.EntryEvent))
                .Where(x => x != typeof(Events.ExitEvent))
                .Distinct()
                .ToList();
        }

        private static Func<string, InternalState<TInternalState>> Find<TInternalState>
            ( MachineDefinition<TInternalState> machineDefinition )
        {
            return (name) => machineDefinition.LookupRegisteredState(name);
        }

        private static void Dispatch<TInternalState, TEvent>(TEvent evnt
           , IMachineState<TInternalState> internalState
           , InternalState<TInternalState> currentState
           , IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> relations
           , IEnumerable<Func<object, object>> eventInterceptors
           , MachineConfiguration<TInternalState> config
           , bool outerTransitionsTakePrecedence
           , Func<string, InternalState<TInternalState>> lookup)
        {
            var currentStates = Misc<TInternalState>.FindAllStates(relations, currentState);

            if (!outerTransitionsTakePrecedence)
            {
                currentStates.Reverse();
            }

            var nextState = DispatchToStates(evnt, internalState, currentStates, eventInterceptors, config, lookup);

            if (nextState != null && nextState != currentStates.Last())
            {
                nextState = TransitionTo((object) evnt, internalState, relations, nextState, eventInterceptors, false, config, lookup);
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
           , MachineConfiguration<TInternalState> config
           , Func<string, InternalState<TInternalState>> lookup)
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
                transitionToState = nextTransitionToState;
                _nextStates.Add(nextTransitionToState);
            }

            var entryConditionsToRun = _nextStates.Except(_currentStates);
            var exitConditionsToRun = _currentStates.Except(_nextStates);

            if (exitInnerStatesFirst)
            {
                entryConditionsToRun.Reverse();
                exitConditionsToRun.Reverse();
            }

            // Dispatch and do not transition...
            var newState = DispatchToStates(new Events.ExitEvent(), machineState, exitConditionsToRun, eventInterceptors, config, lookup);

            machineState.ChangeState(_nextStates.Last());

            // Dispath entry event - transition if neccesary  and do not transition...
            newState = DispatchToStates(new Events.EntryEvent(), machineState, entryConditionsToRun, eventInterceptors, config, lookup);
            if (newState != null)
            {
                transitionToState = TransitionTo(evnt, machineState, relations, newState, eventInterceptors, exitInnerStatesFirst, config, lookup);
            }

            return transitionToState;
        }

        /// <summary>
        /// Runs through all the current states - and updates the state histories to the current ones.
        /// </summary>
        private static void UpdateStateHistories<TInternalState>(IMachineState<TInternalState> internalState, 
            IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> parentStates,
            InternalState<TInternalState> nextState)
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
            , MachineConfiguration<TInternalState> config
            , Func<string, InternalState<TInternalState>> lookup)
        {
            TEvent evntToDispatch = ExecuteInterceptorsForEvent(evnt, eventInterceptors);
            
            if (evntToDispatch == null)
            {
                // Do not dispatch a null event!
                return null;
            }

            InternalState<TInternalState> finalTransitionState = null;
            Type eventType = evnt.GetType();

            foreach (var currentState in statesToDispatchTo)
            {
                if (currentState.Handlers.ContainsKey(eventType))
                {
                    finalTransitionState = Execute(evnt, internalState, config, finalTransitionState, currentState, lookup);
                }
                else if(eventType != typeof(Events.EntryEvent) & eventType != typeof(Events.ExitEvent))
                {
                    if (currentState.Handlers.ContainsKey(typeof(Events.DefaultEvent)))
                    {
                        finalTransitionState = Execute(new Events.DefaultEvent(), internalState, config, finalTransitionState, currentState, lookup);
                    }
                }

                if (eventType != typeof(Events.EntryEvent) & eventType != typeof(Events.ExitEvent))
                {
                    // Always run an AnyEvent() - 
                    if (currentState.Handlers.ContainsKey(typeof(Events.AnyEvent)))
                    {
                        finalTransitionState = Execute(new Events.AnyEvent(), internalState, config, finalTransitionState, currentState, lookup);
                    }
                }
            }

            return finalTransitionState;
        }

        private static InternalState<TInternalState> Execute<TInternalState, TEvent>(TEvent evnt, 
            IMachineState<TInternalState> internalState, 
            MachineConfiguration<TInternalState> config,
            InternalState<TInternalState> finalTransitionState,
            InternalState<TInternalState> currentState,
            Func<string, InternalState<TInternalState>> lookup)
        {
            var handler = currentState.Handlers[evnt.GetType()];

            void log(string str) => config.Logger($"SM:{config.Name}:{config.GetUniqueId(internalState.CurrentInternalState)} = State: {currentState.Name} - {str}");

            foreach (var action in handler.TransistionDefinitions)
            {
                if (action.GuardCondition(internalState.CurrentInternalState, evnt))
                {
                    action.Action?.Invoke(internalState.CurrentInternalState, evnt, log);

                    if (action.TransitionTo != null)
                    {
                        finalTransitionState ??= lookup( action.TransitionTo );
                    }
                }
            }

            return finalTransitionState;
        } 
    }
}
