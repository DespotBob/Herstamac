using System;

namespace Herstamac.Fluent;

public class BuilderTransitionDefinition<TInternalState>
{
    public Func<object, bool> _typeGuardCondition;

    public Func<TInternalState, object, bool> _guardCondition;
    public Action<TInternalState, object, Action<string>>? _action;
    public BuilderState<TInternalState>? _transitionTo;

    public BuilderTransitionDefinition(Func<object, bool> typeGuardCondition, Func<TInternalState, object, bool> guardCondition, Action<TInternalState, object, Action<string>>? action, BuilderState<TInternalState>? transitionToState)
    {
        _typeGuardCondition = typeGuardCondition ?? throw new Exception(nameof(guardCondition));
        _guardCondition = guardCondition ?? throw new Exception(nameof(guardCondition));
        _action = action;
        _transitionTo = transitionToState;
    }

    public Func<object, bool> TypeGuardCondition { get { return _typeGuardCondition; } }
    public Func<TInternalState, object, bool> GuardCondition { get { return _guardCondition; } set { _guardCondition = value; } }
    public Action<TInternalState, object, Action<string>>? Action { get { return _action; } set { _action = value; } }
    public BuilderState<TInternalState>? TransitionTo { get { return _transitionTo; } }
}
