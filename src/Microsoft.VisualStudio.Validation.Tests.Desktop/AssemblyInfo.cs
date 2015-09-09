using Xunit;

// Suppress xunit parallelizing tests since we manipulate statics (TraceListeners)
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
