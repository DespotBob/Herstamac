using System;
using System.Collections.Generic;

namespace Herstamac.Test.Transition
{
    public class TransitionBehaviour
    {
        private readonly List<string> _events = new List<String>();

        public List<string> Events
        {
            get
            {
                return _events;
            }
        }
    }

    public class GoFaster { }
}
