namespace Herstamac
{
    //public class EventHandledResponse<TInternalState>
    //{
    //    private string _name;

    //    private State<TInternalState> _transitionTo;

    //    private EventHandledResponse() { }

    //    private EventHandledResponse(string name) { _name = name; }

    //    private EventHandledResponse(State<TInternalState> transitionTo) {
    //        _transitionTo = transitionTo;
    //    }

    //    public static EventHandledResponse<TInternalState> Nothing = new EventHandledResponse<TInternalState>("Nothing");

    //    public static EventHandledResponse<TInternalState> Swallow = new EventHandledResponse<TInternalState>("Swallow");

    //    public static EventHandledResponse<TInternalState> Transition(State<TInternalState> state)
    //    {
    //        return new EventHandledResponse<TInternalState>(state);
    //    }

    //    public bool IsTransition { get { return _transitionTo != null; } }

    //    public State<TInternalState> NextState { get { return _transitionTo; }  }
    //}


    //private static State<TInternalState> DispatchToStateViaReflection<TInternalState, T>(T evnt, TInternalState internalState, State<TInternalState> currentState)
    //{
    //    var methods = currentState
    //        .GetType()
    //        .GetProperties()
    //        .Where(x => x.PropertyType == typeof(Func<T, TInternalState, Herstamac.EventHandledResponse<TInternalState>>))
    //        .ToList();

    //    foreach (var method in methods)
    //    {
    //        Func<T, TInternalState, Herstamac.EventHandledResponse<TInternalState>> func = method.GetValue(currentState) as Func<T, TInternalState, EventHandledResponse<TInternalState>>;

    //        if (func == null)
    //        {
    //            // Skip - this...
    //            continue;
    //        }

    //        var result = func(evnt, internalState);

    //        if (result.IsTransition)
    //        {
    //            return result.NextState;
    //        }
    //    }
    //    return null;
    //}

}
