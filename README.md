# Herstamac

A Hierachical Statemachine engine for .NET, with: 

* A fluent api for building a StateMachine definition.
* An api fpr building a StateMachine using a classical object model. (Removed: will be added again later....)


### Installation 

	Install-Package Herstamac

### Example - A Machine Builder

A MachineBuilder is a class that is used to produce a StateMachineDefinition.

		public class SlowFastStoppedStateMachineBuilder : MachineBuilder<SlowFastStoppedInternalState>
		{
			/* Create some states to use in our state machine - The names are the most important thing really. There here inside this builder - just cos */
			public State Moving = NewState("Moving");
			public State Slow = NewState("Slow");
			public State Fast = NewState("Fast");
			public State Stopped = NewState("Stopped");

			/* Events - Can be any class, and can be located anywhere, here there inside the builder - just cos.. */
			public class GoFaster { }
			public class GoSlower  { }
			public class GoStop { }

			public SlowFastStoppedStateMachineBuilder()
			{
				/* Register the states before use.. */
				RegisterState(Stopped);
				RegisterState(Slow);
				RegisterState(Fast);
				RegisterState(Moving);

				/* Define a state Hierachy....*/
				RegisterParentStateFor(Slow, () => Moving);
				RegisterParentStateFor(Fast, () => Moving);

				/* Use a style of fluent api to define your behaviour */

				/* Some code that runs when the Stopped state is entered */
				InState(Stopped)
					.OnEntry()
					.Then((state, @event, log) =>
					{
						log("Entered log in the stopped state!");
					});

				/* Some code that runs when the Stopped state is exited */
				InState(Stopped)
					.OnExit()
					.Then((state, @event, log) => {
						log("Log something - This can be identified easily in the logs..");
					});

				/* In the stopped state, when a GoFasterEvent arrives - transition to the slow state */
				InState(Stopped)
					.When<GoFaster>()
					.TransitionTo(Slow);

				InState(Slow)
					.OnExit()
					.Then((state, @event) => {
						Console.WriteLine("Entering Slow!");
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
					.WithGuard((s, e) => true)
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
    
### Example 2 - Using a Machine builder to get a StateMachine running.

The MachineRunner is the engine that drives a state machine - it need two things:

* A MachineState
* A MachineDefintion
        
Luckily, these can be generated using a StateMachineBuilder.

        var machine = new SlowFastStoppedStateMachineBuilder();
		
		var machineDefinition =  machine.GetMachineDefinition();
		var machineState       = machineDefinition.NewMachineInstance(new SlowFastStoppedInternalState());
        
You can use the config action when calling GetMachineDefinition():

        /* Now use the machineBuilder to produce a MachineDefinition */
        var machineDefinition =  machine.GetMachineDefinition( config => 
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

Now we have all three things - Let's jam them into a MachineRunner, and dispatch an event into the state machine.

        MachineRunner.Dispatch(machineDefinition, machineState, new SlowFastStoppedStateMachineBuilder.GoFaster());
        Assert.IsTrue(MachineRunner.IsInState(MachineState, MachineDefinition, machine.Stopped));
    
<h3>Example - Log output </h3>

	
	SM:FastSlow - Registered state: 'Stopped', with 3 Handlers
	SM:FastSlow - Registered state: 'Slow', with 4 Handlers
	SM:FastSlow - Registered state: 'Fast', with 3 Handlers
	SM:FastSlow - Registered state: 'Stopped', with 3 Handlers
	Rx'd Event: GoFaster
	Rx'd Event2: GoFaster
	SM:FastSlow:827d132b-b371-4793-a1fd-df649cb34014 = State: Stopped -> Slow on ^GoFaster = 'Herstamac.Test.SlowFastStopped.SlowFastStoppedStateMachineBuilder+GoFaster'
	Rx'd Event: ExitEvent
	Rx'd Event2: ExitEvent
	SM:FastSlow:827d132b-b371-4793-a1fd-df649cb34014 = State: Stopped - Log something - This can be identified easily in the logs..
	Rx'd Event: EntryEvent
	Rx'd Event2: EntryEvent
	Rx'd Event: GoStop
	Rx'd Event2: GoStop
	SM:FastSlow:827d132b-b371-4793-a1fd-df649cb34014 = State: Slow -> Stopped on ^GoStop = 'Herstamac.Test.SlowFastStopped.SlowFastStoppedStateMachineBuilder+GoStop'
	Rx'd Event: ExitEvent
	Rx'd Event2: ExitEvent
	Entering Slow!
	Rx'd Event: EntryEvent
	Rx'd Event2: EntryEvent
	SM:FastSlow:827d132b-b371-4793-a1fd-df649cb34014 = State: Stopped - Entered log in the stopped state!

**>Note:** A Guid has been used to identfify this statemachine in the logs, because no unique identifier was specified. If you don't like Guids, specify something else to be used in is place.</em>

### Using Event Interceptors

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


### Using Default Event handlers

Event handlers can be added to any state:

        InState(Inner)
            .OnDefaultEvent()
            .Then((s,e) => s.Counter++ )
            .TransitionTo(Error);

These handlers will be executed if no other event is run.

<h3>Using Any Event handlers</h3>

Sometimes, you might want a handler to be run, when ANY event is received by a state. 

	InState(Outer)
		.OnAnyEvent()
		.Then((s,e) => s.Counter1++);

These events are:
* Executed after every other event handler.
* Are not execeuted on Entry and Exit events.

**Note:** The first transition wins.

