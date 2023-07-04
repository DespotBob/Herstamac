using System;

namespace Herstamac
{
    public class TransistionDefinition<TInternalState>
    {
        public TransistionDefinition(Func<object, bool>? typeGuardCondition
            , Func<TInternalState, object, bool>?guardCondition
            , Action<TInternalState, object, Action<string>>? action
            , string transitionToState)
        {
            TypeGuardCondition = typeGuardCondition;
            GuardCondition = guardCondition;
            Action = action;
            TransitionTo = transitionToState;
        }

        public Func<object, bool>? TypeGuardCondition { get; }
        public Func<TInternalState, object, bool>? GuardCondition { get; }
        public Action<TInternalState, object, Action<string>>? Action { get; }
        public string TransitionTo { get; }
    }
}
