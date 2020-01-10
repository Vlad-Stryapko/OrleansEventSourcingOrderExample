# OrleansEventSourcingOrderExample
Example of how it's possible to not correctly populate state of Orleans' Journaled Grain

Run the silo, run the client a couple of times. Then stop the silo, start it again and run the client once more. Silo's logs would contain the default state (zero) even though all the events are persisted. Removing the comment from OnActivateAsync fixes the issue.

Responsible line:

https://github.com/Vlad-Stryapko/OrleansEventSourcingOrderExample/blob/master/OrleansEsPoc/Grains/TestGrain.cs#L15
