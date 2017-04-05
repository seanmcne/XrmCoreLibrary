#### ParallelDiscoveryServiceProxy.Execute() Method

The Execute method accepts a list of discovery requests which can be a mix of discovery request types.  A method overload accepts an instance of [DiscoveryServiceProxyOptions](DiscoveryServiceProxyOptions-Class) to control proxy channel behaviors.  Each Execute method returns a list of the discovery response returned from each request.  

There is also a set of generic Execute methods where you can specify a request/response type pair. Using these methods allows you to create a more specifically typed list of requests and provides a list of responses directly cast to the specified type. Unless submitting a mix of DiscoveryRequest types, the generic methods are preferred.

Parallel Discovery requests can also be performed using a keyed collection of requests.  These methods return a keyed collection of responses with the key being used to correlate the originating request to the response.

The Execute method accepts any type of request in the **Microsoft.Xrm.Sdk.Discovery** namespace.  Parallelizing discovery requests is especially useful when needing to lookup multiple systemuser's by their external identifier or if needing to retrieve detail about a subset of tenants (organizations).

##### Method Overloads

{code:c#}
public IEnumerable<DiscoveryResponse> Execute(IEnumerable<DiscoveryRequest> requests)
public IEnumerable<DiscoveryResponse> Execute(IEnumerable<DiscoveryRequest> requests, DiscoveryServiceProxyOptions options)
public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests)
    where TRequest : DiscoveryRequest
    where TResponse : DiscoveryResponse
public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, DiscoveryServiceProxyOptions options)
    where TRequest : DiscoveryRequest
    where TResponse : DiscoveryResponse
public IDictionary<string, DiscoveryResponse> Execute(IDictionary<string, DiscoveryRequest> requests)
public IDictionary<string, DiscoveryResponse> Execute(IDictionary<string, DiscoveryRequest> requests, DiscoveryServiceProxyOptions options)
public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests)
    where TRequest : DiscoveryRequest
    where TResponse : DiscoveryResponse
public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, DiscoveryServiceProxyOptions options)
    where TRequest : DiscoveryRequest
    where TResponse : DiscoveryResponse
{code:c#}

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<DiscoveryServiceFault>>**.

##### Usage Examples

Each example represents a method of a sample class that contains the following property

{code:c#}
DiscoveryServiceManager Manager { get; set; }
{code:c#}

1. +How to Execute discovery requests in parallel+
Demonstrates parallelized execution of multiple discovery requests.  Note, this scenario would only be applicable if needing to discover a set of organizations not representative of all organizations that the authenticated user belongs to in the deployment.  Otherwise, a single **RetrieveOrganizationsRequest** could be executed in lieu of these parallelized individual organization discovery requests.

{code:c#}
public List<DiscoveryResponse> ParallelExecuteDiscoveryRequests(string[]() organizationNames)
{            
    List<DiscoveryResponse> responses = null;
    var requests = new List<DiscoveryRequest>();

    Array.ForEach(organizationNames, name =>
        {
            var request = new RetrieveOrganizationRequest()
            {
                UniqueName = name,
                AccessType = EndpointAccessType.Default,
                Release = OrganizationRelease.Current
            };

            requests.Add(request);
        });

    try
    {
        responses = this.Manager.ParallelProxy.Execute(requests).ToList();
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return responses;
}
{code:c#}

2. +How to Execute retrieve user by UPN in parallel+
Demonstrates parallelized execution of multiple RetrieveUserByExternalIdRequest.  This example uses the IDictionary<TKey, TValue> method to demonstrate how a key (UPN) can be used to correlate the discovery request to its response.

{code:c#}
public void ParallelExecuteRetrieveByUpnRequests(Guid organizationId, string[]() upns)
{
    var requests = new Dictionary<string, RetrieveUserIdByExternalIdRequest>();

    Array.ForEach(upns, upn =>
    {
        var request = new  RetrieveUserIdByExternalIdRequest()
        {
            ExternalId = String.Format("C:{0}", upn),
            OrganizationId = organizationId
        };

        requests.Add(upn, request);
    });

    try
    {
        var responses = this.Manager.ParallelProxy.Execute<RetrieveUserIdByExternalIdRequest, RetrieveUserIdByExternalIdResponse>(requests);

        foreach (var response in responses)
        {
            Console.WriteLine("Retrieved user UPN={0}, systemuserid={1}", 
                response.Key, 
                response.Value.UserId);
        }
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }
}
{code:c#}

3. +How to use the optional exception handler delegate+
Demonstrates a parallelized execution of multiple discovery requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

{code:c#}
public Dictionary<string, RetrieveUserIdByExternalIdResponse> ParallelExecuteRequestsWithExceptionHandler(Dictionary<string, RetrieveUserIdByExternalIdRequest> requests)
{
    int errorCount = 0;
    var responses = new Dictionary<string, RetrieveUserIdByExternalIdResponse>();

    try
    {
        responses = this.Manager.ParallelProxy.Execute<RetrieveUserIdByExternalIdRequest, RetrieveUserIdByExternalIdResponse>(requests,
            (request, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during discovery of external user with Id={0}: {1}", request.Value.ExternalId, ex.Detail.Message);
                errorCount++;
            })
            .ToDictionary(t => t.Key, t => t.Value);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel discovery requests.", errorCount);

    return responses;
}
{code:c#}