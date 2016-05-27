using System;

namespace Herstamac
{
    public class StateBuilder<TInternalState> 
    {
        private InternalState<TInternalState> _state;
        private readonly Func<State, InternalState<TInternalState>> _lookup;

        public StateBuilder(InternalState<TInternalState> state, Func<State, InternalState<TInternalState>> lookup)
        {
            _state = state;
            _lookup = lookup;
        }

        public void When<TEvent>( Action<ITransitionBuilderWithTransition<TInternalState,TEvent>> builder )
            where TEvent : class
        {
            var transDefinition = _state.AddTransitionDefinitionToState<TEvent>();
            var localbBuilder = new TransitionBuilder<TInternalState, TEvent>(transDefinition, _lookup);
            builder(localbBuilder);
        }

        public void When<TEvent1, TEvent2>(Action<ITransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
        }

        private void AddTransitionAsObject<TEvent1>(Action<ITransitionBuilderWithTransition<TInternalState, object>> builder) where TEvent1 : class
        {
            var td = _state.AddTransitionDefinitionToState<TEvent1, object>();
            var localbBuilder = new TransitionBuilder<TInternalState, object>(td, _lookup);
            builder(localbBuilder);
        }

        public void When<TEvent1, TEvent2, TEvent3>(Action<ITransitionBuilderWithTransition<TInternalState, object>> builder)
            where TEvent1 : class
            where TEvent2 : class
            where TEvent3 : class
        {
            AddTransitionAsObject<TEvent1>(builder);
            AddTransitionAsObject<TEvent2>(builder);
            AddTransitionAsObject<TEvent3>(builder);
        }

        public void When<TEvent1, TEvent2, TEvent3, TEvent4>(Action<ITransitionBuilderWithTransition<TInternalState, object>> builder)
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

        public void When<TEvent1, TEvent2, TEvent3, TEvent4,TEvent5>(Action<ITransitionBuilderWithTransition<TInternalState, object>> builder)
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

        public ITransitionBuilderWithTransition<TInternalState, TEvent> When<TEvent>()
            where TEvent : class
        {
            var transDefinition = _state.AddTransitionDefinitionToState<TEvent>();
            return new TransitionBuilder<TInternalState, TEvent>(transDefinition, _lookup);
        }

        public ITransitionBuilderWithGuard<TInternalState, Events.ExitEvent> OnExit()
        {
            var transDefinition = _state.AddTransitionDefinitionToState<Events.ExitEvent>();
            return new TransitionBuilder<TInternalState, Events.ExitEvent>(transDefinition, _lookup);
        }

        public ITransitionBuilderWithGuard<TInternalState, Events.EntryEvent> OnEntry()
        {
            var transDefinition = _state.AddTransitionDefinitionToState<Events.EntryEvent>();
            return new TransitionBuilder<TInternalState, Events.EntryEvent>(transDefinition, _lookup);
        }

        public ITransitionBuilderWithGuard<TInternalState, Events.DefaultEvent> OnDefaultEvent()
        {
            var transDefinition = _state.AddTransitionDefinitionToState<Events.DefaultEvent>();
            return new TransitionBuilder<TInternalState, Events.DefaultEvent>(transDefinition, _lookup);
        }
    }
}
