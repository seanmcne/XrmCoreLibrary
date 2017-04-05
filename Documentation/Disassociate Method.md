#### ParallelOrganizationServiceProxy.Disassociate() Method

The Disassociate method accepts a list of disassociate requests. A method overload accepts an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class.md) to control proxy channel behaviors.  Nothing is returned by either method.

##### Method Overloads 

```c#
public void Disassociate(IEnumerable<DisassociateRequest> requests);
public void Disassociate(IEnumerable<DisassociateRequest> requests, OrganizationServiceProxyOptions options);
```

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each sample represents a method of a sample class that contains the following property

```c#
OrganizationServiceManager Manager { get; set; }
```

1. +How to Disassociate entities in parallel+
Demonstrates a parallelized submission of multiple disassociate requests

```c#
public void ParallelDisassociate(List<DisassociateRequest> requests)
{
    try
    {
        this.Manager.ParallelProxy.Disassociate(requests);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }
}
```

2. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple disassociate requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

```c#
public void ParallelDisassociateWithExceptionHandler(List<DisassociateRequest> requests)
{
    int errorCount = 0;
            
    try
    {
        this.Manager.ParallelProxy.Disassociate(requests,
            (request, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during disassociate of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                errorCount++;
            });
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel disassociate.", errorCount);
}
```
