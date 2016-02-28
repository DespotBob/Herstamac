﻿using System;

namespace Herstamac
{
    public class TransitionDefinition<TInternalState>
    {
        public Func<object, bool> _typeGuardCondition;

        public Func<TInternalState, object, bool> _guardCondition;
        public Action<TInternalState, object, Action<string> > _action;
        public State<TInternalState> _transitionTo;

        public TransitionDefinition( Func<object, bool> typeGuardCondition, Func<TInternalState, object, bool> guardCondition, Action<TInternalState, object, Action<string>> action, State<TInternalState> transitionToState)
        {
            if (typeGuardCondition == null)
            {
                throw new Exception("typeGuardCondition");
            }

            if (guardCondition == null)
            {
                throw new Exception( "guardCondition" );
            }

            _typeGuardCondition = typeGuardCondition;
            _guardCondition = guardCondition;
            _action = action;
            _transitionTo = transitionToState;
        }

        public Func<TInternalState, object, bool> GuardCondition { get { return _guardCondition; } set { _guardCondition = value; } }
        public Action<TInternalState, object, Action<string> > Action { get { return _action; } set { _action = value; } }
        public State<TInternalState> TransitionTo { get { return _transitionTo; } }
    }
}
