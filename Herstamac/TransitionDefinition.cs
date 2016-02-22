using System;

namespace Herstamac
{
    public class TransitionDefinition<TInternalState>
    {
        public Func<TInternalState, object, bool> _guardCondition;
        public Action<TInternalState, object> _action;
        public State<TInternalState> _transitionTo;

        public TransitionDefinition() : this((s, o) => true, null, null)
        {
        }

        public TransitionDefinition(Func<TInternalState, object, bool> guardCondition, Action<TInternalState, object> action, State<TInternalState> transitionToState)
        {
            if (guardCondition == null)
            {
                throw new Exception( "guardCondition" );
            }

            _guardCondition = guardCondition;
            _action = action;
            _transitionTo = transitionToState;
        }

        public Func<TInternalState, object, bool> GuardCondition { get { return _guardCondition; } set { _guardCondition = value; } }
        public Action<TInternalState, object> Action { get { return _action; } set { _action = value; } }
        public State<TInternalState> TransitionTo { get { return _transitionTo; } }
    }
}
