using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Herstamac
{
    [DebuggerDisplay("State = {Name}")]
    public class State : IState
    {
        public State(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}
