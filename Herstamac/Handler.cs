using System;
using System.Collections.Generic;

namespace Herstamac
{
    public class Handler<TMessage, TReturns>
    {
        private readonly Dictionary<Type, Func<TMessage, TReturns>> _dictionary = new Dictionary<Type, Func<TMessage, TReturns>>();

        public void Add<T>(Func<T, TReturns> handler) where T : TMessage
        {
            _dictionary.Add(typeof(T), x => handler((T)x));
        }

        public TReturns Handle(TMessage command)
        {
            Func<TMessage, TReturns> handler;

            if (!_dictionary.TryGetValue(command.GetType(), out handler))
            {
                throw new Exception("cannot map " + handler.GetType());
            }

            return handler(command);
        }
    }
}
