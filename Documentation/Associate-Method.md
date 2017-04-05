#### ParallelOrganizationServiceProxy.Associate() Method

The Associate method accepts a list of associate requests. A method overload accepts an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class.md) to control proxy channel behaviors.  Nothing is returned by either method.

##### Method Overloads

```c#
public void Associate(IEnumerable<AssociateRequest> requests)
public void Associate(IEnumerable<AssociateRequest> requests, OrganizationServiceProxyOptions options)
```

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each example represents a method of a sample class that contains the following property

```c#
OrganizationServiceManager Manager { get; set; }
``` 

1. +How to Associate entities in parallel+
Demonstrates a parallelized submission of multiple associate requests

``` C#
public void ParallelAssociate(List<AssociateRequest> requests)
{
    try
    {
        this.Manager.ParallelProxy.Associate(requests);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }
}
```

2. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple associate requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

```c#
public void ParallelAssociateWithExceptionHandler(List<AssociateRequest> requests)
{
    int errorCount = 0;

    try
    {
        this.Manager.ParallelProxy.Associate(requests,
            (request, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during associate of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                errorCount++;
            });
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel associate.", errorCount);
}
```
