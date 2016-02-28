using System;
using System.Linq.Expressions;

namespace Herstamac
{
    public class MachineConfigure<TInternalState> : IMachineConfigure<TInternalState>, IUniqueIdConfigure<TInternalState>
    {
        MachineConfiguration<TInternalState> config = new MachineConfiguration<TInternalState>();

        public MachineConfiguration<TInternalState> Results
        {
            get
            {
                return config;
            }
        }

        void IUniqueIdConfigure<TInternalState>.From(Func<TInternalState, string> func)
        {
            config.GetUniqueId = func;
        }

        public void FromProperty<TProperty>(Expression<Func<TInternalState, TProperty>> property)
        {
            var t = property.Compile();
            config.GetUniqueId = (prop) => t(prop).ToString();
        }

        public void Logger(Action<string> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            config.Logger = logger;
        }

        public void Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            config.Name = name;
        }

        void IUniqueIdConfigure<TInternalState>.UniqueId(string uniqueId)
        {
            config.GetUniqueId = (x) => uniqueId;
        }

        public void LogEventWith(Func<object, string> eventSerialiser)
        {
            if (eventSerialiser == null)
            {
                throw new ArgumentNullException("eventSerialiser");
            }

            config.LogEvents = eventSerialiser;
        }

        IUniqueIdConfigure<TInternalState> IMachineConfigure<TInternalState>.UniqueId
        {
            get
            {
                return this;
            }
        }
    }



}
