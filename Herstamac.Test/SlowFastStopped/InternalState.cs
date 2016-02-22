using System;

namespace Herstamac.Test.SlowFastStopped
{
    public class SlowFastStoppedInternalState : Herstamac.MachineState<object> 
    {
        private Guid _guid = Guid.NewGuid();

        public Guid Id {  get { return _guid; } }
    }
}
