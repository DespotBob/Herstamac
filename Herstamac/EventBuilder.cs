using System;

namespace Herstamac
{
    public class TransitionBuilderWithGuard<TInternalState, TEvent>
        where TEvent : class
    {
        protected TransitionDefinition<TInternalState> TD;
        
        public TransitionBuilderWithGuard(TransitionDefinition<TInternalState> td)
        {
            TD = td;
        }

        public TransitionBuilderWithGuard<TInternalState, TEvent> WithGuard(Func<TInternalState, TEvent, bool> guard)
        {
            TD.GuardCondition = (s,e) => guard( s, (TEvent) e);
            return this;
        }

        public TransitionBuilder<TInternalState, TEvent> Then()
        {
            return new TransitionBuilder<TInternalState, TEvent>(TD);
        }

        public TransitionBuilder<TInternalState, TEvent> Then(Action handler)
        {
            TD.Action = (state, obj) => handler();
            return new TransitionBuilder<TInternalState, TEvent>(TD);
        }

        public TransitionBuilder<TInternalState, TEvent> Then(Action<TInternalState, TEvent> action)
        {
            TD.Action = (state, obj) => action(state, (TEvent)obj);
            return new TransitionBuilder<TInternalState, TEvent>(TD);
        }
    }
}
