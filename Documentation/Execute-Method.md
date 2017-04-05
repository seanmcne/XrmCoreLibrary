#### ParallelOrganizationServiceProxy.Execute() Method

The Execute method accepts a list of organization requests which can be a mix of organization request types. A method overload accepts an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class.md) to control proxy channel behaviors. Each Execute method returns a list of the organization response returned from each request. 

There is also a set of generic Execute methods where you can specify a request/response type pair. Using these methods allows you to create a more specifically typed list of requests and provides a list of responses directly cast to the specified type. Unless submitting a mix of OrganizationRequest types, the generic methods are preferred.

Parallel Organization requests can also be performed using a keyed collection of requests.  These methods return a keyed collection of responses with the key being used to correlate the originating request to the response.

The Execute method accepts any type of request in the **Microsoft.Xrm.Sdk.Messages** or **Microsoft.Crm.Sdk.Messages** namespaces. Also, this is the method you should use to parallelize **ExecuteMultipleRequests**.

##### Method Overloads

```c#
public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests){}
public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests, OrganizationServiceProxyOptions options){}
public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests)
    where TRequest : OrganizationRequest
    where TResponse : OrganizationResponse
	{}
public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options)
    where TRequest : OrganizationRequest
    where TResponse : OrganizationResponse
	{}
public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests){}
public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests, OrganizationServiceProxyOptions options){}
public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests)
    where TRequest : OrganizationRequest
    where TResponse : OrganizationResponse
	{}
public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, OrganizationServiceProxyOptions options)
    where TRequest : OrganizationRequest
    where TResponse : OrganizationResponse
	{}
```

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each sample represents a method of a sample class that contains the following property

```c#
OrganizationServiceManager Manager { get; set; }
```

1. +Parallel Execute SetStateRequest Sample+
Demonstrates parallelized execution of multiple set state requests

```c#
public void ParallelExecuteSetStateRequests(Guid[]() accountIds)
{
    var requests = new List<OrganizationRequest>();

    Array.ForEach(accountIds, id =>
        {
            var request = new SetStateRequest()
            {
                EntityMoniker = new EntityReference("account", id),
                State = new OptionSetValue(1),
                Status = new OptionSetValue(2)
            };

            requests.Add(request);
        });

    try
    {
        this.Manager.ParallelProxy.Execute(requests);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }
}
```

2. +Parallel Generic Execute Requests Sample+
Demonstrates parallelized execution of multiple team privilege requests using generic method

```c#
public IDictionary<Guid, RetrieveTeamPrivilegesResponse> ParallelGenericExecuteRequests(Guid[]() teamIds)
{
    IDictionary<Guid, RetrieveTeamPrivilegesResponse> responses = null;
    var requests = new Dictionary<string, RetrieveTeamPrivilegesRequest>();

    Array.ForEach(teamIds, id =>
        {
            var request = new RetrieveTeamPrivilegesRequest()
            {
                TeamId = id
            };

            requests.Add(id.ToString(), request);
        });

    try
    {
        responses = this.Manager.ParallelProxy.Execute<RetrieveTeamPrivilegesRequest, RetrieveTeamPrivilegesResponse>(requests)
            .ToDictionary(r => Guid.Parse(r.Key), r => r.Value);

        foreach (var response in responses)
        {
            Console.WriteLine("Retrieves {0} privileges for team with id={1}", 
                response.Value.RolePrivileges.Length, 
                response.Key);
        }
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return responses;
}
```

3. +Parallel ExecuteMultipleRequest Sample+
Demonstrates how ExecuteMultipleRequests should be parallelized via generic Execute method. 

```c#
public IDictionary<string, ExecuteMultipleResponse> ParallelExecuteMultiple(IDictionary<string, ExecuteMultipleRequest> requests)
{
    IDictionary<string, ExecuteMultipleResponse> responses = null;

    try
    {
        responses = this.Manager.ParallelProxy.Execute<ExecuteMultipleRequest, ExecuteMultipleResponse>(requests);

        foreach (var response in responses)
        {
            Console.WriteLine("{0} responses and {1} errors for ExecuteMultipleRequest with key={2}",
                response.Value.Responses.Count(r => r.Response != null),
                response.Value.Responses.Count(r => r.Fault != null),
                response.Key);
        }
    }
    catch(AggregateException ae)
    {
        // Handle exceptions
    }

    return responses;
}
```

4. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple execute requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

```c#
public void ParallelExecuteRequestsWithExceptionHandler(List<AssignRequest> requests)
{
    int errorCount = 0;
            
    try
    {
        this.Manager.ParallelProxy.Execute<AssignRequest, AssignResponse>(requests,
            (request, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered assigning entity with Id={0} to user with Id={1}: {2}", 
                    request.Target.Id, 
                    request.Assignee.Id, 
                    ex.Detail.Message);
                errorCount++;
            });
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during execute of parallel assign requests.", errorCount);
}
```