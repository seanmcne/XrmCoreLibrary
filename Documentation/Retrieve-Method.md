#### ParallelOrganizationServiceProxy.Retrieve() Method

The Retrieve method accepts a list of retrieve requests representing individual entities to retrieve. A method overload accepts an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class.md) to control proxy channel behaviors.  Each  method returns a list of the retrieved entities.

_**IMPORTANT**: Use the Retrieve method only under special circumstances when +multiple entity types+ must be retrieved by their unique identifier.  Otherwise for better performance, issue a single query with a condition expression specifying a **ConditionOperartor.In** for an array of System.**Guid**'s._

##### Method Overloads 

```c#
public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests);
public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests, OrganizationServiceProxyOptions options);
```

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each example represents a method of a sample class that contains the following property

```c#
OrganizationServiceManager Manager { get; set; }
```

1. +How to Retrieve entities in parallel+
Demonstrates a parallelized submission of multiple retrieve requests

```c#
public List<Entity> ParallelRetrieve(List<RetrieveRequest> requests)
{
    List<Entity> responses = null;

    try
    {
        responses = this.Manager.ParallelProxy.Retrieve(requests).ToList();
    }
    catch(AggregateException ae)
    {
        // Handle exceptions
    }

    responses.ForEach(r =>
        {
            Console.WriteLine("Retrieved {0} with id = {1}", r.LogicalName, r.Id);
        });

    return responses;
}
```

2. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple retrieve requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

```c#
public List<Entity> ParallelRetrieveWithExceptionHandler(List<RetrieveRequest> requests)
{
    int errorCount = 0;
    List<Entity> responses = null;

    try
    {
        responses = this.Manager.ParallelProxy.Retrieve(requests,
            (request, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during retrieve of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                errorCount++;
            }).ToList();
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel retrieves.", errorCount);

    return responses;
}
```