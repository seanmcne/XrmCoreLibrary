#### ParallelDiscoveryServiceProxy Class

The ParallelDiscoveryServiceProxy class provides a set of methods syntactically similar to IDiscoveryService, but with the ability to submit the requests in parallel using multiple threads.  Each method implementation leverages .NET Task Parallel Library (TPL) to partition and dispatch the work.  

There is no need to instantiate this class directly because the [DiscoveryServiceManager](DiscoveryServiceManager-Class.md) class contains a [ParallelProxy](Discovery-ParallelProxy-Property.md) property referencing an instance of this class.  It is immediately available on every instance of DiscoveryServiceManager and should be used for executing parallelized request operations.

**Namespace**: Microsoft.Pfe.Xrm
**Assembly**: Microsoft.Pfe.Xrm.Core.dll

##### Properties

* [MaxDegreeOfParallelism](Discovery-MaxDegreeOfParallelism-Property.md)

##### Methods

* [Execute](Discovery-Execute-Method.md)
Each method provides an overload that accepts an instance of [DiscoveryServiceProxyOptions](DiscoveryServiceProxyOptions-Class.md) class.

##### Exception Handling

If you wish to ensure that all operations are processed in the event a **FaultException<DiscoveryServiceFault>** is encountered, each method provides an optional parameter for you to specify a delegate exception handling Action.  When supplying this argument, FaultExceptions are caught and collected during execution of the underlying service operations.  Once all parallel operations have completed, the collected faults are then passed to the exception handling delegate sequentially.  Recommended ways to use the delegate function are to log fault events, help in determining faulted request counts, and queuing failed requests for retry.

For all other exception scenarios, each method call on the ParallelDiscoveryServiceProxy class should be guarded by catching type of System.**AggregateException** which is thrown by the underlying TPL data parallelization methods if any unhandled exceptions are encountered.  This exception type will provide a collection of exceptions encountered while executing the requests.  

_**NOTE:** The default behavior of TPL when it encounters an unhandled exception is to complete processing existing iterations, but no new iterations are processed.  This means that not all of your requests may be processed if an unhandled exception is encountered. Reference the section Variations > Exception Handling: [https://msdn.microsoft.com/library/ff963552.aspx](https___msdn.microsoft.com_library_ff963552.aspx)_

Typical exception types you might otherwise encounter in the AggregateException include CommunicationException and authentication/security-related exceptions.  These types generally indicate a condition that affects all service channels.  Thus, allowing these exception types to interrupt remaining parallel iterations is by design.