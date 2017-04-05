#### OrganizationServiceManager.ParallelProxy Property

This property references an instance of [ParallelOrganizationServiceProxy](ParallelOrganizationServiceProxy-class) class and is available immediately upon instantiation of a new OrganizationServiceManager.  This property should be used for executing parallelized request operations.  

{code:c#}
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
{code:c#}