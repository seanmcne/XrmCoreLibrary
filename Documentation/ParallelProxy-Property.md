#### OrganizationServiceManager.ParallelProxy Property

This property references an instance of [ParallelOrganizationServiceProxy](ParallelOrganizationServiceProxy-class.md) class and is available immediately upon instantiation of a new OrganizationServiceManager.  This property should be used for executing parallelized request operations.  

```c#
public ParallelOrganizationServiceProxy ParallelProxy
{
    get
    {
        if (this.parallelProxy == null)
        {
            this.parallelProxy = new ParallelOrganizationServiceProxy(this);
        }

        return this.parallelProxy;
    }
} 
```