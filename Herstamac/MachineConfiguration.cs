using System;

namespace Herstamac
{

    public class MachineConfiguration<TInternalState>
    {
        internal Func<object, string> LogEvents = (x) => string.Empty;
        internal Action<string> Logger = (x) => { };
        internal Func<TInternalState, string> GetUniqueId = (x) => "XXXXXX";
        internal string Name { get; set; }
    }
}
