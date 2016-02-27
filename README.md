# Herstamac

A Statemachine engine for .NET, with a fluent api for constructing a StateMachine definition.

<h3> Example - A Machine Builder</h3>

A MachineBuilder is a class that is used to produce a StateMachineDefinition.

    public class SlowFastStoppedStateMachineBuilder : MachineBuilder<SlowFastStoppedInternalState>
    {
        /* Create some states to use in our state machine - The names are the most important thing really. There here inside this builder - just cos */
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
                .Then((state, @event, log ) =>
                    log("Log a string with specii ")
                });

            /* In the stopped state, when a GoFasterEvent arrives - transition to the slow state */
            InState(Stopped)
                .When<GoFaster>()
                .TransitionTo(Slow);

            InState(Slow)
                .OnExit()
                .Then((state, @event) => 
				{
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

            /* Whoops - looks like someone added some silly guard clauses - real ones would be meaningful */
            InState(Slow)
                .When<GoStop>()
                .WithGuard( (s,e) => true )
                .Then()
                .TransitionTo(Stopped);
                
            /* A Good thing this Guard condition is in place ! */
            InState(Slow)
                .When<GoStop>()
                .WithGuard((s, e) => false)
                .Then(() => 
				{ 
					throw new ApplicationException("This will not be called, as the guard condition is always false"); 
				});

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
    
<h3>Example 2 - Using a Machine builder to get a StateMachine running.</h3>

The MachineRunner is the engine that drives a state machine - it need two things:
<ul>
    <li>A MachineState</li>
    <li>A MachineDefintion</li>
</ul>
        
Luckily, these can be generated using a StateMachineBuilder.

        
        SlowFastStoppedStateMachineBuilder machine = new SlowFastStoppedStateMachineBuilder();
        
        /* Now use the machineBuilder to produce a MachineDefinition */
        MachineDefinition<SlowFastStoppedInternalState> MachineDefinition =  machine.GetMachineDefinition( config => 
            {
                /* Name the Statemachine */
                config.Name("FastSlow");   
                
                /* Define where the logging information is going! */
                config.Logger(x => Console.WriteLine(x));
                
                /* Define a function that will be used to serialise an event to a string */
                config.LogEventWith(x => x.ToString());
                
                /* Hmmm - Every state machine needs a unique Id - Get this one from here, otherwise it's a GUID! */
                config.UniqueId.FromProperty(p => p.Id);
            });

The MachineBuilder is also used to produce a MachineState

        //
        MachineState = machine.NewMachineState(new SlowFastStoppedInternalState());
        
Now we have all three things - Let's jam them into a MachineRunner, and dispatch an event into the state machine.

         MachineRunner.Dispatch(MachineDefinition, MachineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
    
    
<h3>Example - Log output </h3>

	SM:FastSlow - Registered state: 'Stopped', with 3 Handlers
	SM:FastSlow - Registered state: 'Slow', with 4 Handlers
	SM:FastSlow - Registered state: 'Fast', with 3 Handlers
	SM:FastSlow - Registered state: 'Stopped', with 3 Handlers
	Rx'd Event: GoFaster
	Rx'd Event2: GoFaster
	SM:FastSlow:bdd369d3-7ea0-41ab-96fb-2aa895ceec69 = State: Stopped -> Slow on ^GoFaster = 'Herstamac.Test.SlowFastStopped.SlowFastStoppedStateMachineBuilder+GoFaster'
	Rx'd Event: ExitEvent
	Rx'd Event2: ExitEvent
	Exiting Stopped!
	Rx'd Event: EntryEvent
	Rx'd Event2: EntryEvent
	Rx'd Event: GoStop
	Rx'd Event2: GoStop
	SM:FastSlow:bdd369d3-7ea0-41ab-96fb-2aa895ceec69 = State: Slow -> Stopped on ^GoStop = 'Herstamac.Test.SlowFastStopped.SlowFastStoppedStateMachineBuilder+GoStop'
	Rx'd Event: ExitEvent
	Rx'd Event2: ExitEvent
	Entering Slow!
	Rx'd Event: EntryEvent
	Rx'd Event2: EntryEvent
	SM:FastSlow:bdd369d3-7ea0-41ab-96fb-2aa895ceec69 = State: Stopped - Entered log in the stopped state!

	<em>Note:</em> A Guid has been used to identfify this statemachine in the logs, because no unique identifier was specified. If you don't like Guids, specify something else to be used in is place.

<h3>Using Event Interceptor</h3>

Invent Interceptors can be added to a statemachine. These are run before any event is dispatched to the statemachine.

        SlowFastStoppedStateMachineBuilder machine = new SlowFastStoppedStateMachineBuilder();
        
		/* Demonstrate a Simple Event Interceptor - This one just logs the event */
        machine.AddEventInterceptor((evnt) =>
        {
            Console.WriteLine("Rx'd Event: {0}", evnt.GetType().Name);
            return evnt;
        });

		/* Returning null causes the event to be discarded before use. */
		machine.AddEventInterceptor((evnt) =>
        {
            if( /* business logic */ true ) 
			{
				return null;
			} 
			else
			{
				return evnt;
			}
        });

		/* Returning a completely different event is just as acceptable.. */
		machine.AddEventInterceptor((evnt) =>
        {
            return new JimBobBoab();
        });


