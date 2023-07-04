//using System;


//namespace Herstamac.Lifts
//{
//    public static class Lifts
//    {
//        public static IDisposable AsObservable<TInternalState, TEvent>(
//            this IObservable<TEvent> t
//            , MachineDefinition<TInternalState> machineDefinition
//            , IMachineState<TInternalState> internalState)
//        {
//            return t.Subscribe(x => MachineRunner.Dispatch(machineDefinition, internalState, x));
//        }
//    }
//}
