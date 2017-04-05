#### DiscoveryServiceProxyOptions Class

This class provides the ability to set additional options on the service proxy instances that are created within parallelized request operations of the [ParallelDiscoveryServiceProxy ](ParallelDiscoveryServiceProxy-Class) class.  

**Namespace**: Microsoft.Pfe.Xrm
**Assembly**: Microsoft.Pfe.Xrm.Core.dll

##### Properties

* **Timeout** -Set this property to the desired timeout duration, overriding the default 2-minute request timeout on each channel.  

```c#
public class DiscoveryServiceProxyOptions : ServiceProxyOptions { }

public class ServiceProxyOptions
{
    public TimeSpan Timeout { get; set; }
}
```

##### Usage Examples

1. +How to use DiscoveryServiceProxyOptions when making parallel requests+
Demonstrates a parallelized submission of multiple discovery requests with service proxy options specifying an increase of the channel timeout to 5 minutes.

```c#
public IEnumerable<DiscoveryResponse> ParallelExecuteWithOptions(List<DiscoveryRequest> requests)
{
    IEnumerable<DiscoveryResponse> responses = null;
            
    var options = new DiscoveryServiceProxyOptions()
    {
        Timeout = new TimeSpan(0, 5, 0)
    };

    try
    {
        responses = this.Manager.ParallelProxy.Execute(requests, options);
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return responses;
}
```