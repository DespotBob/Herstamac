using System;
using System.Linq.Expressions;

namespace Herstamac
{
    /// <summary>
    /// Specify the source of a unique ID, that identifies instances of this statemachine.
    /// </summary>
    /// <typeparam name="TInternalState"></typeparam>
    public interface IUniqueIdConfigure<TInternalState>
    {
        /// <summary>
        /// Use a function to create your own unique id.
        /// </summary>
        /// <param name="UniqueIdentifier"></param>
        void From(Func<TInternalState, string> UniqueIdentifier);

        /// <summary>
        /// Get the unique Id from a property in the State.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        void FromProperty<TProperty>(Expression<Func<TInternalState, TProperty>> property);

        /// <summary>
        /// Use the given Id/
        /// </summary>
        /// <param name="uniqueId"></param>
        void UniqueId(string uniqueId);
    }

    public interface IMachineConfigure<TInternalState>
    {
        void Name(string name);

        /// <summary>
        /// Help tell instances of the same state machine apart with a Unique Id.
        /// </summary>
        IUniqueIdConfigure<TInternalState> UniqueId { get; }

        void Logger(Action<string> logger);
        void LogEventWith(Func<object, string> eventSerialiser);
    }
}
