#### DiscoveryServiceManager.ParallelProxy Property

This property references an instance of [ParallelDiscoveryServiceProxy](ParallelDiscoveryServiceProxy-class) class and is available immediately upon instantiation of a new DiscoveryServiceManager.  This property should be used for executing parallelized request operations.  

```c#
public ParallelDiscoveryServiceProxy ParallelProxy
{
    get
    {
        if (this.parallelProxy == null)
        {
            this.parallelProxy = new ParallelDiscoveryServiceProxy(this);
        }

        return this.parallelProxy;
    }
} 
```