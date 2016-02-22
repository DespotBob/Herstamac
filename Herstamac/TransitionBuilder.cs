namespace Herstamac
{
    public class TransitionBuilder<TInternalState, TEvent>
        where TEvent : class
    {
        TransitionDefinition<TInternalState> _td;

        public TransitionBuilder(TransitionDefinition<TInternalState> td)
        {
            _td = td;
        }

        public TransitionBuilder<TInternalState, TEvent> TransitionTo(State<TInternalState> transitionToState)
        {
            _td._transitionTo = transitionToState;

            return this;
        }
    }
}
