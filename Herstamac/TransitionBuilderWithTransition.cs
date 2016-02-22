namespace Herstamac
{
    public class TransitionBuilderWithTransition<TInternalState, TEvent> : TransitionBuilderWithGuard<TInternalState, TEvent>
           where TEvent : class
    {
        public TransitionBuilderWithTransition(TransitionDefinition<TInternalState> td) : base(td)
        {
        }

        public TransitionBuilder<TInternalState, TEvent> TransitionTo(State<TInternalState> transitionToState)
        {
            return new TransitionBuilder<TInternalState, TEvent>(TD).TransitionTo(transitionToState);
        }
    }
}
