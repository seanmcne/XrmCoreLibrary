#### ParallelOrganizationServiceProxy.RetrieveMultiple() Method

The RetrieveMultiple method accepts a keyed collection of queries (QueryExpression or FetchExpression) that will be used to retrieve multiple entity records.  Method overloads allow for retrieving all pages (RetrieveMultiple extension method) and accept an instance of [OrganizationServiceProxyOptions](OrganizationServiceProxyOptions-Class) to control proxy channel behaviors.  Each method returns a keyed collection of the EntityCollections from the executed queries.  The purpose of using keyed collections is that the key can be used to correlate the originating query to its EntityCollection result set.

##### Method Overloads

```c#
public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries)
public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, OrganizationServiceProxyOptions options)
public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages)
public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages, OrganizationServiceProxyOptions options)
```

Each of the above methods also provides an optional exception handling parameter of type **Action<TRequest, FaultException<OrganizationServiceFault>>**.

##### Usage Examples

Each example represents a method of a sample class that contains the following property

```c#
OrganizationServiceManager Manager { get; set; }
```

1. +How to execute RetrieveMultiple queries in parallel+
Demonstrates parallelized submission of multiple queries

```c#
public IDictionary<string, EntityCollection> ParallelRetrieveMultiple(IDictionary<string, QueryBase> queries)
{
    IDictionary<string, EntityCollection> results = null;
            
    try
    {
        results = this.Manager.ParallelProxy.RetrieveMultiple(queries);

        foreach (var result in results)
        {
            Console.WriteLine("Query with key={0} for {1} returned {2} records.", 
                result.Key, 
                result.Value.EntityName, 
                result.Value.Entities.Count);
        }
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return results;
}
```

2. +How to execute RetrieveMultiple queries returning all pages in parallel+
Demonstrates parallelized submission of multiple queries where all pages of results are retrieved for each query

```c#
public IDictionary<string, EntityCollection> ParallelRetrieveMultipleAllPages(IDictionary<string, QueryBase> queries)
{
    IDictionary<string, EntityCollection> results = null;

    try
    {
        results = this.Manager.ParallelProxy.RetrieveMultiple(queries, true);

        foreach (var result in results)
        {
            Console.WriteLine("Query with key={0} for {1} returned {2} records.",
                result.Key,
                result.Value.EntityName,
                result.Value.Entities.Count);
        }
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return results;
}
```

3. How to execute many RetrieveMultiple query types in parallel+
Demonstrates parallelized submission of multiple queries. Queries can be mix of QueryExpression and FetchXML

```c#
public IDictionary<string, EntityCollection> ParallelRetrieveMultipleFull()
{
    IDictionary<string, EntityCollection> results = null;
    var queries = new Dictionary<string, QueryBase>();

    var accountQuery = new QueryExpression("account");
    accountQuery.ColumnSet.AddColumns("name", "address1_city", "primarycontactid");
    accountQuery.Criteria.AddCondition(new ConditionExpression("name", ConditionOperator.BeginsWith, "C"));
    accountQuery.NoLock = true;
    queries.Add("accounts", accountQuery);

    var contactQuery = new QueryExpression("contact");
    contactQuery.ColumnSet.AddColumns("firstname", "lastname", "parentcustomerid");
    contactQuery.Criteria.AddCondition(new ConditionExpression("statecode", ConditionOperator.Equal, 1));
    contactQuery.NoLock = true;
    queries.Add("contacts", contactQuery);

    var oppQuery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                        <entity name='opportunity'>
                            <attribute name='name' />
                            <attribute name='estimatedvalue' />
                            <attribute name='customerid' />
                            <filter type='and'>
                                <condition attribute='estimatedvalue' operator='gt' value='1000' />
                            </filter>
                        </entity> 
                        </fetch>";
    queries.Add("opportunities", new FetchExpression(oppQuery));

    try
    {
        results = this.Manager.ParallelProxy.RetrieveMultiple(queries, true);

        foreach (var result in results)
        {
            Console.WriteLine("Query with key={0} for {1} returned {2} records.",
                result.Key,
                result.Value.EntityName,
                result.Value.Entities.Count);
        }
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    return results;
} 
```

4. +How to use the optional exception handler delegate+
Demonstrates a parallelized submission of multiple queries with an optional delegate exception handling function. The delegate is provided the original request and the fault exception encountered. It is executed on the calling thread after all parallel operations are complete.

```c#
public IDictionary<string, EntityCollection> ParallelRetrieveMultipleWithExceptionHandler(IDictionary<string, QueryBase> queries)
{
    int errorCount = 0;
    IDictionary<string, EntityCollection> results = null;

    try
    {
        results = this.Manager.ParallelProxy.RetrieveMultiple(queries, true,
            (query, ex) =>
            {
                System.Diagnostics.Debug.WriteLine("Error encountered during query with key={0}: {1}", query.Key, ex.Detail.Message);
                errorCount++;
            });
    }
    catch (AggregateException ae)
    {
        // Handle exceptions
    }

    Console.WriteLine("{0} errors encountered during parallel queries.", errorCount);

    return results;
}
```