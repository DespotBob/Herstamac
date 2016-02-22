using System;

namespace Herstamac
{
    public class StateBuilder<TInternalState>
    {
        private State<TInternalState> _state;

        public StateBuilder(State<TInternalState> state)
        {
            _state = state;
        }

        public void When<TEvent>( Action<TransitionBuilderWithTransition<TInternalState,TEvent>> builder )
            where TEvent : class
        {
            var transDefinition = _state.AddTransitionDefinitionToState<TEvent>();

            var localbBuilder = new TransitionBuilderWithTransition<TInternalState, TEvent>(transDefinition);

            builder(localbBuilder);
        }

        public void When<TEvent1, TEvent2>(Action<TransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
        }

        private void AddTransitionAsObject<TEvent1>(Action<TransitionBuilderWithTransition<TInternalState, object>> builder) where TEvent1 : class
        {
            var td = _state.AddTransitionDefinitionToState<TEvent1, object>();
            var localbBuilder = new TransitionBuilderWithTransition<TInternalState, object>(td);
            builder(localbBuilder);
        }

        public void When<TEvent1, TEvent2, TEvent3>(Action<TransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
            where TEvent3 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
            AddTransitionAsObject<TEvent3>(builder);
        }

        public void When<TEvent1, TEvent2, TEvent3, TEvent4>(Action<TransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
            where TEvent3 : class
            where TEvent4 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
            AddTransitionAsObject<TEvent3>(builder);
            AddTransitionAsObject<TEvent4>(builder);
        }

        public void When<TEvent1, TEvent2, TEvent3, TEvent4,TEvent5>(Action<TransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
            where TEvent3 : class
            where TEvent4 : class
            where TEvent5 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
            AddTransitionAsObject<TEvent3>(builder);
            AddTransitionAsObject<TEvent4>(builder);
            AddTransitionAsObject<TEvent5>(builder);
        }


        public TransitionBuilderWithTransition<TInternalState, TEvent> When<TEvent>()
            where TEvent : class
        {
            var transDefinition = _state.AddTransitionDefinitionToState<TEvent>();
            return new TransitionBuilderWithTransition<TInternalState, TEvent>(transDefinition);
        }

        public TransitionBuilderWithGuard<TInternalState, Events.ExitEvent> OnExit()
        {
            var transDefinition = _state.AddTransitionDefinitionToState<Events.ExitEvent>();
            return new TransitionBuilderWithGuard<TInternalState, Events.ExitEvent>(transDefinition);
        }

        public TransitionBuilderWithGuard<TInternalState, Events.EntryEvent> OnEntry()
        {
            var transDefinition = _state.AddTransitionDefinitionToState<Events.EntryEvent>();
            return new TransitionBuilderWithGuard<TInternalState, Events.EntryEvent>(transDefinition);
        }
    }
}
