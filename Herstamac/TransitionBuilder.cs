using System;

namespace Herstamac
{
    public class TransitionBuilder<TInternalState, TEvent> : 
        ITransitionBuilder<TInternalState, TEvent>, 
        ITransitionBuilderWithGuard<TInternalState, TEvent>,
        ITransitionBuilderWithTransition<TInternalState, TEvent>,
        ITransistionBuilderEnd<TInternalState,TEvent>
        where TEvent : class
    {
        private readonly BuilderTransitionDefinition<TInternalState> _td;
        private readonly Func<State, BuilderState<TInternalState>>_lookup;

        public TransitionBuilder(BuilderTransitionDefinition<TInternalState> td, Func<State, BuilderState<TInternalState>> lookup)
        {
            _td = td;
            _lookup = lookup;
        }

        public ITransitionBuilder<TInternalState, TEvent> WithGuard(Func<TInternalState, TEvent, bool> guard)
        {
            _td.GuardCondition = (s, e) => guard(s, (TEvent)e);
            return this;
        }

        public ITransistionBuilderEnd<TInternalState, TEvent> Then()
        {
            return this;
        }

        public ITransistionBuilderEnd<TInternalState, TEvent> Then(Action handler)
        {
            _td.Action = (state, obj, log) => handler();
            return this;
        }

        public ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent, Action<string>> action)
        {
            _td.Action = (state, obj, log) => action(state, (TEvent)obj, log);
            return this;
        }

        public ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent> action)
        {
            _td.Action = (state, obj, log) => action(state, (TEvent)obj);
            return this;
        }

        public void TransitionTo(State transitionToState)
        {
            _td._transitionTo = _lookup( transitionToState );
        }
    }
}
