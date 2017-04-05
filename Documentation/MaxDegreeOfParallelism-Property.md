#### ParallelOrganizationServiceProxy.MaxDegreeOfParallelism Property

By default (-1), the max degree of parallelism is constrained only by available system resources.  For constraining the max degree of parallelism applied, the MaxDegreeOfParallelism property can be set on the [ParallelOrganizationServiceProxy](ParallelOrganizationServiceProxy-Class.md) class.  The value set is validate to be -1 or greater than 0 otherwise a System.**ArgumentOutOfRangeException** is thrown.  This property can be set multiple times on the same instance to temporarily constrain parallelism.  

```c#
public int MaxDegreeOfParallelism
{
    get
    {
        return this.maxDegreeOfParallelism;
    }
    set
    {
        ValidateDegreeOfParallelism(value);

        this.maxDegreeOfParallelism = value;
    }
}
```