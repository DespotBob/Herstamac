using System;

namespace Herstamac
{
    public interface ITransistionBuilderEnd<TInternalState, TEvent> where TEvent : class
    {
        void TransitionTo(InternalState<TInternalState> transitionToState);
    }

    public interface ITransitionBuilderWithTransition<TInternalState, TEvent> where TEvent : class
    {
        ITransitionBuilder<TInternalState, TEvent> WithGuard(Func<TInternalState, TEvent, bool> guard);

        ITransistionBuilderEnd<TInternalState, TEvent> Then();
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent, Action<string>> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action handler);

        void TransitionTo(InternalState<TInternalState> transitionToState);
    }

    public interface ITransitionBuilder<TInternalState, TEvent> where TEvent : class
    {
        ITransistionBuilderEnd<TInternalState, TEvent> Then();
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent, Action<string>> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action handler);
    }

    public interface ITransitionBuilderWithGuard<TInternalState, TEvent> where TEvent : class
    {
        ITransitionBuilder<TInternalState, TEvent> WithGuard(Func<TInternalState, TEvent, bool> guard);

        ITransistionBuilderEnd<TInternalState, TEvent> Then();
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action<TInternalState, TEvent, Action<string>> action);
        ITransistionBuilderEnd<TInternalState, TEvent> Then(Action handler);
    }
}