namespace Herstamac
{
    public class EventHandledResponse<TInternalState>
    {
        private string _name;

        private State<TInternalState> _transitionTo;

        private EventHandledResponse() { }

        private EventHandledResponse(string name) { _name = name; }

        private EventHandledResponse(State<TInternalState> transitionTo) {
            _transitionTo = transitionTo;
        }

        public static EventHandledResponse<TInternalState> Nothing = new EventHandledResponse<TInternalState>("Nothing");

        public static EventHandledResponse<TInternalState> Swallow = new EventHandledResponse<TInternalState>("Swallow");

        public static EventHandledResponse<TInternalState> Transition(State<TInternalState> state)
        {
            return new EventHandledResponse<TInternalState>(state);
        }

        public bool IsTransition { get { return _transitionTo != null; } }

        public State<TInternalState> NextState { get { return _transitionTo; }  }
    }

    public class Transition 
    {
    
    }
}
