#### ParallelServiceProxy Classes

Provides abstracted parallelized processing of CRM service operations. These classes leverage the XrmServiceManager classes to efficiently allocate service channels aligned to dispatched threads. This ensures your CRM service connections remain thread-safe and maximize throughput. But, why should you have to worry about all of that? We couldn't agree more. You supply a collection of requests, we handle the multi-threading and return you a collection of responses (when appropriate). 'Nuff said.

There are two types of ParallelServiceProxy classes to handle multi-threaded operations targeting the Discovery.svc and Organization.svc and options classes that provide flexibility in setting proxy channel behavior:

* [ParallelDiscoveryServiceProxy Class](ParallelDiscoveryServiceProxy-Class.md)
* [ParallelOrganizationServiceProxy Class](ParallelOrganizationServiceProxy-Class.md)
* [DiscoveryServiceProxyOptions Class](DiscoveryServiceProxyOptions-Class.md)
* [OrganizationServiceProxyOptions Class](OrganizationServiceProxyOptions-Class.md)
If you're using the XrmServiceManager classes, then each manager instance has a corresponding **ParallelProxy** property that can be used for issuing parallelized requests to their respective endpoint.