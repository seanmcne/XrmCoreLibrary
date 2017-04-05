#### OrganizationServiceProxyOptions Class

This class provides the ability to set additional options on the service proxy instances that are created within parallelized request operations of the [ParallelOrganizationServiceProxy ](ParallelOrganizationServiceProxy-Class.md) class.  

**Namespace**: Microsoft.Pfe.Xrm
**Assembly**: Microsoft.Pfe.Xrm.Core.dll

##### Properties

* **ShouldEnableProxyTypes** - Setting this property to **true** will add the ProxyTypesBehavior to each proxy instance. This should be set if using early-bound programming model.
* **CallerId** - Set this property to the unique identifier of system user that should be impersonated on each proxy channel.  Standard pre-requisite permissions apply.
* **Timeout** - Set this property to the desired timeout duration, overriding the default 2-minute request timeout on each channel.  Especially useful for long-running request types such as ExecuteMultipleRequest.

```c#
public class OrganizationServiceProxyOptions : ServiceProxyOptions
{
    public bool ShouldEnableProxyTypes { get; set; }
    public Guid CallerId { get; set; }
}

public class ServiceProxyOptions
{
    public TimeSpan Timeout { get; set; }
}
```

##### Usage Examples

1. +How to use OrganizationServiceProxyOptions when making parallel requests+
Demonstrates a parallelized submission of multiple create requests with service proxy options specifying a callerId to impersonate, that the ProxyTypesBehavior should be enabled, and increases the channel timeout to 5 minutes.

```c#
public List<Entity> ParallelCreateWithOptions(List<Entity> targets, Guid callerId)
{
    var options = new OrganizationServiceProxyOptions()
    {
        CallerId = callerId,
        ShouldEnableProxyTypes = true,
        Timeout = new TimeSpan(0, 5, 0)
    };

    try
    {
        targets = this.Manager.ParallelProxy.Create(targets, options).ToList();
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    targets.ForEach(r =>
    {
        Console.WriteLine("Created {0} with id={1}", r.LogicalName, r.Id);
    });

    return targets;
}
```