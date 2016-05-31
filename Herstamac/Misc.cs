using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herstamac
{
    public static class Misc<TInternalState>
    {
        public static List<InternalState<TInternalState>> FindAllStates(
            IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> relations,
            InternalState<TInternalState> childState)
        {
            if (childState == null)
            {
                throw new ArgumentNullException("childState", "When trying to find a parent state the childState cannot be null!");
            }

            var _nextStates = new Stack<InternalState<TInternalState>>();

            // Find the list of states that represents every state that surround the state we are trying to transition to

            InternalState<TInternalState> currentState = childState;
            _nextStates.Push(childState);

            while (relations.ContainsKey(currentState))
            {
                var t = relations[currentState];
                _nextStates.Push(t);
                currentState = t;
            }

            return _nextStates.ToList();
        }

        public static InternalState<TInternalState> FindParentState(
            IReadOnlyDictionary<InternalState<TInternalState>, InternalState<TInternalState>> relations,
            InternalState<TInternalState> childState )
        {
            if (relations.ContainsKey(childState))
            {
                return relations[childState];
            }
            else
            {
                return null; 
            }
        }
    }
}
