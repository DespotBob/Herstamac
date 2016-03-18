using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Herstamac.Test.SlowFastStopped
{
     public class BaseState : Herstamac.InternalState<SlowFastStoppedInternalState>
     {
         public BaseState(string name)
            : base(name)
        {
        }
     }

    public class SlowState : BaseState
    {
        public SlowState()
            : base("Slow")
        {
        }
    }

    public class FastState : BaseState
    {
        public FastState()
            : base("Slow")
        {
        }
    }

    public class MovingState : BaseState
    {
        public MovingState()
            : base("Moving")
        {
        }
    }

    public class StoppedState : BaseState
    {
        public StoppedState()
            : base("Stopped")
        {
        }
    }
}
