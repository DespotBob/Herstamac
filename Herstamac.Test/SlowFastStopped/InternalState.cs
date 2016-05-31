using System;

namespace Herstamac.Test.SlowFastStopped
{
    public class SlowFastStoppedInternalState 
    {
        private Guid _guid = Guid.NewGuid();

        public Guid Id {  get { return _guid; } }
    }
}
