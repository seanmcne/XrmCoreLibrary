#### ParallelOrganizationServiceProxy.Delete() Method

The Delete method accepts a list of entities representing the targets of each delete request. A method overload accepts an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class) to control proxy channel behaviors.  Nothing is returned by either method.

##### Method Overloads

{code:c#}
public void Delete(IEnumerable<EntityReference> targets)
public void Delete(IEnumerable<EntityReference> targets, OrganizationServiceProxyOptions options)
{code:c#}

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each example represents a method of a sample class that contains the following property

{code:c#}
OrganizationServiceManager Manager { get; set; }
{code:c#}

1. +How to Delete entities in parallel+
Demonstrates a parallelized submission of multiple delete requests

{code:c#}
public void ParallelDelete(List<EntityReference> targets)
{
    try
    {
        this.Manager.ParallelProxy.Delete(targets);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }
}
{code:c#}

2. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple delete requests with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

{code:c#}
public void ParallelDeleteWithExceptionHandler(List<EntityReference> targets)
{
    int errorCount = 0;
            
    try
    {
        this.Manager.ParallelProxy.Delete(targets,
            (target, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during delete of entity with Id={0}: {1}", target.Id, ex.Detail.Message);
                errorCount++;
            });
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel delete.", errorCount);
}
{code:c#}