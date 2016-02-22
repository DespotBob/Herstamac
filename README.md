# Herstamac

A Fluent Statemachine engine for .NET

Example - A Machine Builder.

A builder is a class that is used to produce a StateMachineDefinition.

    public class SlowFastStoppedStateMachineBuilder : MachineBuilder<SlowFastStoppedInternalState>
    {
        /* Create some states to use in our state machine - The names are the most important thing really. */
        public State<SlowFastStoppedInternalState> Moving = NewState("Moving");
        public State<SlowFastStoppedInternalState> Slow = NewState("Slow");
        public State<SlowFastStoppedInternalState> Fast = NewState("Fast");
        public State<SlowFastStoppedInternalState> Stopped = NewState("Stopped"); 

        /* Events - Can be any class, and can be located anywhere, here there inside the builder - just cos.. */
        public class GoFaster : Event { }
        public class GoSlower : Event { }
        public class GoStop : Event { }
    
        public SlowFastStoppedStateMachineBuilder()
        {
            /* Register the states before use.. */
            RegisterState(Stopped);
            RegisterState(Slow);
            RegisterState(Fast);
            RegisterState(Stopped);
            
            /* Define a state Hierachy....*/
            RegisterParentStateFor(Slow, () => Moving);
            RegisterParentStateFor(Fast, () => Moving);

            /* Every time the StateMachine transitions - log it using this method */
            AddTransitionLog((x) => Console.WriteLine(x));

            /* Use a style of fluent api to define your behaviour */
            
            /* Some code that runs when the Stopped state is entered */
            InState(Stopped)
                .OnEntry()
                .Then((state, @event) =>
                {
                    Console.WriteLine("Entering Stoppped!");
                });

            /* Some code that runs when the Stopped state is exited */
            InState(Stopped)
                .OnExit()
                .Then((state, @event) =>
                    Console.WriteLine("Exiting Stopped!");
                });

            /* In the stopped state, when a GoFasterEvent arrives - transition to the slow state */
            InState(Stopped)
                .When<GoFaster>()
                .TransitionTo(Slow);

            InState(Slow)
                .OnExit()
                .Then((state, @event) => {
                    Console.WriteLine("Exiting Slow!");
                });

            InState(Slow)
                .When<GoFaster>()
                .TransitionTo(Fast);

            /* Right - just going to use (s,e) for state and @event from now on.... */
            InState(Slow)
                .When<GoSlower>()
                .Then((s,e) => Console.WriteLine( "Told to go slower"))
                .TransitionTo(Stopped);

            /* Whoops - looks like someone added some silly guard clauses - real ones 
            InState(Slow)
                .When<GoStop>()
                .WithGuard( (s,e) => true )
                .Then()
                .TransitionTo(Stopped);
                
            /* A Good thing this Guard condition is in place ! */
            InState(Slow)
                .When<GoStop>()
                .WithGuard((s, e) => false)
                .Then(() => { throw new ApplicationException("This will not be called, as the guard condition is always false"); } );

            InState(Fast)
                .When<GoSlower>()
                .TransitionTo(Slow);

            InState(Fast)
                .When<GoFaster>()
                .Then((state, evnt) => Console.WriteLine("I cannae go nae faster..."));

            InState(Fast)
                .When<GoStop>()
                .TransitionTo(Stopped);
        }
    }
    
Example 2 - Using a Machine builder to get a StateMachine running.

The MachineRunner is the engine that drives a state machine - it need two things:
    * A MachineState
    * A MachineDefintion
        
Luckily, these can be generated using a StateMachineBuilder.
    
        SlowFastStoppedStateMachineBuilder machine = new SlowFastStoppedStateMachineBuilder();
        
        machine.AddEventInterceptor((evnt) =>
        {
            /* Demonstrate a Simple Event Interceptor - This one just logs the event */
            Console.WriteLine("Rx'd Event: {0}", evnt.GetType().Name);
            return evnt;
        });

        /* Now use the machineBuilder to produce a MachineDefinition */
        MachineDefinition<SlowFastStoppedInternalState> MachineDefinition =  machine.GetMachineDefinition( config => 
            {
                /* Name the Statemachine */
                config.Name("FastSlow");   
                
                /* Define where the logging information is going! */
                config.Logger(x => Console.WriteLine(x));
                
                /* Define a function that will be used to serialise an event to a string */
                config.LogEventWith(x => x.ToString());
                
                /* Hmmm - Every state machine needs a unique Id - Get this one from here */
                config.UniqueId.FromProperty(p => p.Id);
            });

        /* The MachineBuilder is also used to produce a MachineState.
        MachineState = machine.NewMachineState(new SlowFastStoppedInternalState());
        
        /* Now we have all three things - Let's jam them into a MachineRunner.
         MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
    
    
