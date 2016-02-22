using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herstamac
{
    public interface IState<TInternalState>
    {
        string Name { get; }
        Dictionary<Type, State<TInternalState>.ListOfHandlers> Handlers {get;}
    }

    [DebuggerDisplay("State = {Name}")]
    public class State<TInternalState> : IState<TInternalState>
    {
        public class ListOfHandlers
        {
            public List<TransitionDefinition<TInternalState>> TransistionDefinitions = new List<TransitionDefinition<TInternalState>>();
        }

        private string _name;

        public Dictionary<Type, ListOfHandlers> _handlers = new Dictionary<Type, ListOfHandlers>();

        public State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "A state must have a name.");
            }

            _name = name;
        }

        public string Name {  get { return _name; }}

        public Dictionary<Type, ListOfHandlers> Handlers { get { return _handlers; } }

        public void Handles(Events.EntryEvent evnt)
        {
        }

        public void Handles(Events.ExitEvent evnt)
        {
        }

        internal TransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent,TAsHandler>()
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent,TAsHandler>((s, e) => true);
        }

        internal TransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent>()
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent>((s, e) => true);
        }

        internal TransitionDefinition<TInternalState> AddTransitionDefinitionToState<TEvent>(Func<TInternalState, TEvent, bool> guard)
               where TEvent : class
        {
            return AddTransitionDefinitionToState<TEvent,TEvent>(guard);
        }

        internal TransitionDefinition<TInternalState> AddTransitionDefinitionToState<T1,TAsEvent>(Func<TInternalState, TAsEvent, bool> guard)
              where T1 : class
        {
            if (_handlers.Keys.Contains(typeof(T1)) == false)
            {
                _handlers.Add(typeof(T1), new ListOfHandlers());
            }

            var h = _handlers[typeof(T1)];

            var transDefinition = new TransitionDefinition<TInternalState>((s, o) => guard(s, (TAsEvent)o), null, null);

            h.TransistionDefinitions.Add(transDefinition);

            return transDefinition;
        }

    }
}
