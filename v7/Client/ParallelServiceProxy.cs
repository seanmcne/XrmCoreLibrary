/*================================================================================================================================

  This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.  

  THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
  INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  

  We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object 
  code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software 
  product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the 
  Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims 
  or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

 =================================================================================================================================*/
namespace Microsoft.Pfe.Xrm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Text;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Discovery;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;

    /// <summary>
    /// Class for executing concurrent IDiscoveryService operations
    /// </summary>
    /// <remarks>
    /// During parallel operations, DiscoveryServiceProxy instances are aligned with individual data partitions
    /// using a thread local class variable to avoid service channel thread-safety issues.
    /// </remarks>
    public class ParallelDiscoveryServiceProxy : ParallelServiceProxy<DiscoveryServiceManager>
    {
        #region Constructor(s)

        public ParallelDiscoveryServiceProxy(DiscoveryServiceManager serviceManager)
            : base(serviceManager) { }

        public ParallelDiscoveryServiceProxy(DiscoveryServiceManager serviceManager, int maxDegreeOfParallelism)
            : base(serviceManager, maxDegreeOfParallelism) { }

        #endregion

        #region Multi-threaded IDiscoveryService Operations

        #region IDiscoveryService.Execute()

        /// <summary>
        /// Performs data parallelism on a keyed collection of type DiscoveryRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection requests to be submitted</param>
        /// <returns>A keyed collection of type DiscoveryResponse containing response to the discovery request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>        
        public IDictionary<string, DiscoveryResponse> Execute(IDictionary<string, DiscoveryRequest> requests)
        {
            return this.Execute(requests, new DiscoveryServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type DiscoveryRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection of requests to be submitted</param>
        /// <param name="options">The configurable options for the parallel DiscoveryServiceProxy requests</param>
        /// <returns>A keyed collection of type DiscoveryResponse containing response to the discovery request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, DiscoveryResponse> Execute(IDictionary<string, DiscoveryRequest> requests, DiscoveryServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, DiscoveryRequest>, KeyValuePair<string, DiscoveryResponse>>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = context.Local.Execute(request.Value);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(new KeyValuePair<string, DiscoveryResponse>(request.Key, response));

                    return context;
                }).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from DiscoveryRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from DiscoveryResponse</typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests)
            where TRequest : DiscoveryRequest
            where TResponse : DiscoveryResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new DiscoveryServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from DiscoveryRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from DiscoveryResponse</typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="options">The configurable options for the parallel DiscoveryServiceProxy requests</param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, DiscoveryServiceProxyOptions options)
            where TRequest : DiscoveryRequest
            where TResponse : DiscoveryResponse
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, TRequest>, KeyValuePair<string, TResponse>>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request.Value);

                    context.Results.Add(new KeyValuePair<string, TResponse>(request.Key, response));

                    return context;
                }).ToDictionary(r => r.Key, r => r.Value);
        }
        
        /// <summary>
        /// Performs data parallelism on a collection of type DiscoveryRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be submitted</param>
        /// <returns>A collection of type DiscoveryResponse containing response to the discovery request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>        
        public IEnumerable<DiscoveryResponse> Execute(IEnumerable<DiscoveryRequest> requests)
        {
            return this.Execute(requests, new DiscoveryServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type DiscoveryRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be submitted</param>
        /// <param name="options">The configurable options for the parallel DiscoveryServiceProxy requests</param>
        /// <returns>A collection of type DiscoveryResponse containing response to the discovery request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<DiscoveryResponse> Execute(IEnumerable<DiscoveryRequest> requests, DiscoveryServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<DiscoveryRequest, DiscoveryResponse>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = context.Local.Execute(request);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(response);

                    return context;
                });
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from DiscoveryRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from DiscoveryResponse</typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests)
            where TRequest : DiscoveryRequest
            where TResponse : DiscoveryResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new DiscoveryServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute IDiscoveryService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from DiscoveryRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from DiscoveryResponse</typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="options">The configurable options for the parallel DiscoveryServiceProxy requests</param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, DiscoveryServiceProxyOptions options)
            where TRequest : DiscoveryRequest
            where TResponse : DiscoveryResponse
        {
            return this.ExecuteOperationWithResponse<TRequest, TResponse>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request);

                    context.Results.Add(response);

                    return context;
                });
        }

        #endregion

        #endregion

        #region Core Parallel Execution Method <TRequest, TResponse>
        
        /// <summary>
        /// Core implementation of the parallel pattern for service operations that should collect responses to each request
        /// </summary>
        /// <typeparam name="TRequest">The type being submitted in the service operation request</typeparam>
        /// <typeparam name="TResponse">The response type collected from each request and returned</typeparam>
        /// <param name="requests">The collection of requests to be submitted</param>
        /// <param name="options">The configurable options for the DiscoveryServiceProxy requests</param>
        /// <param name="operation">The specific operation being executed</param>
        /// <returns>A collection of specified response types from service operation requests</returns>
        /// <remarks>
        /// IMPORTANT!! When defining the core operation, be sure to add responses you wish to collect via proxy.Results.Add(TResponse item);
        /// </remarks>
        private IEnumerable<TResponse> ExecuteOperationWithResponse<TRequest, TResponse>(IEnumerable<TRequest> requests, DiscoveryServiceProxyOptions options,
            Func<TRequest, ParallelLoopState, long, ParallelDiscoveryOperationContext<TResponse>, ParallelDiscoveryOperationContext<TResponse>> operation)
        {
            var responses = new ConcurrentBag<TResponse>();
            
            //Inline method for initializing a new discovery service channel
            Func<ManagedTokenDiscoveryServiceProxy> proxyInit = () =>
                {
                    var proxy = this.ServiceManager.GetProxy();
                    proxy.SetProxyOptions(options);

                    return proxy;
                };            

            using (var threadLocalProxy = new ThreadLocal<ManagedTokenDiscoveryServiceProxy>(proxyInit, true))
            {
                try
                {
                    Parallel.ForEach<TRequest, ParallelDiscoveryOperationContext<TResponse>>(requests,
                        new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism },
                        () => new ParallelDiscoveryOperationContext<TResponse>(threadLocalProxy.Value),
                        operation,
                        (context) =>
                        {
                            Array.ForEach(context.Results.ToArray(), r => responses.Add(r));

                            //Remove temporary reference to ThreadLocal proxy
                            context.Local = null;
                        });
                }
                finally
                {
                    Array.ForEach(threadLocalProxy.Values.ToArray(), p => p.Dispose());
                }
            }

            return responses;
        }

        #endregion
    }

    /// <summary>
    /// Class for executing concurrent IOrganizationService operations
    /// </summary>
    /// <remarks>
    /// During parallel operations, OrganizationServiceProxy instances are aligned with individual data partitions
    /// using a thread local class variable to avoid service channel thread-safety issues.
    /// </remarks>
    public class ParallelOrganizationServiceProxy : ParallelServiceProxy<OrganizationServiceManager>
    {
        #region Constructor(s)

        public ParallelOrganizationServiceProxy(OrganizationServiceManager serviceManager)
            : base(serviceManager) {  }

        public ParallelOrganizationServiceProxy(OrganizationServiceManager serviceManager, int maxDegreeOfParallelism)
            : base(serviceManager, maxDegreeOfParallelism) {  }

        #endregion

        #region Properties

        private bool shouldEnableProxyTypesBehavior { get; set; }

        #endregion

        #region Multi-threaded IOrganizationService Operation Methods

        #region IOrganizationService.Create()

        /// <summary>
        /// Performs data parallelism on a keyed collection of type Entity to execute IOrganizationService.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The keyed collection target entities to be created</param>
        /// <returns>A keyed collection of unique identifiers for each created Entity</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// Only returning generated unique identifier because it's assumed that requesting process will maintain a reference
        /// between key and the Entity instance submitted as the Create target to which the unique identifier can be correlated
        /// </remarks>
        public IDictionary<string, Guid> Create(IDictionary<string, Entity> targets)
        {
            return this.Create(targets, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type Entity to execute IOrganizationService.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The keyed collection target entities to be created</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A keyed collection of unique identifiers for each created Entity</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// Only returning generated unique identifier because it's assumed that requesting process will maintain a reference
        /// between key and the Entity instance submitted as the Create target to which the unique identifier can be correlated
        /// </remarks>
        public IDictionary<string, Guid> Create(IDictionary<string, Entity> targets, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, Entity>, KeyValuePair<string, Guid>>(targets, options,
                (target, loopState, index, context) =>
                {
                    Guid id = context.Local.Create(target.Value); //Hydrate target with response Id

                    //Collect the result from each iteration in this partition
                    context.Results.Add(new KeyValuePair<string, Guid>(target.Key, id));

                    return context;
                }).ToDictionary(t => t.Key, t => t.Value);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type Entity to execute IOrganizationService.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be created</param>
        /// <returns>The collection of created Entity records, hydrated with the response Id</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// By returning the original collection of target entities with Ids, this allows for concurrent creation of multiple entity types
        /// and ability to cross-reference submitted data with the plaftorm generated Id.  Note that subsequent Update requests should
        /// always instantiate a new Entity instance and assign the Id.
        /// </remarks>
        public IEnumerable<Entity> Create(IEnumerable<Entity> targets)
        {
            return this.Create(targets, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type Entity to execute IOrganizationService.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be created</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>The collection of created Entity records, hydrated with the response Id</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// By returning the original collection of target entities with Ids, this allows for concurrent creation of multiple entity types
        /// and ability to cross-reference submitted data with the plaftorm generated Id.  Note that subsequent Update requests should
        /// always instantiate a new Entity instance and assign the Id.
        /// </remarks>
        public IEnumerable<Entity> Create(IEnumerable<Entity> targets, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<Entity, Entity>(targets, options,
                (target, loopState, index, context) =>
                {                   
                    target.Id = context.Local.Create(target); //Hydrate target with response Id
                    
                    //Collect the result from each iteration in this partition
                    context.Results.Add(target);

                    return context;
                });
        }

        #endregion

        #region IOrganizationService.Update()

        /// <summary>
        /// Performs data parallelism on a list of type Entity to execute IOrganizationService.Update() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Update(IEnumerable<Entity> targets)
        {
            this.Update(targets, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a list of type Entity to execute IOrganizationService.Update() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Update(IEnumerable<Entity> targets, OrganizationServiceProxyOptions options)
        {
            this.ExecuteOperation<Entity>(targets, options,
                (target, proxy) =>
                {
                    proxy.Update(target);
                });
        }

        #endregion

        #region IOrganizationService.Delete()

        /// <summary>
        /// Performs data parallelism on a list of type EntityReference to execute IOrganizationService.Delete() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Delete(IEnumerable<EntityReference> targets)
        {
            this.Delete(targets, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a list of type EntityReference to execute IOrganizationService.Delete() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be deleted</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Delete(IEnumerable<EntityReference> targets, OrganizationServiceProxyOptions options)
        {
            this.ExecuteOperation<EntityReference>(targets, options,
                (target, proxy) =>
                {
                    proxy.Delete(target.LogicalName, target.Id);
                });
        }

        #endregion

        #region IOrganizationService.Associate()

        /// <summary>
        /// Performs data parallelism on a list of type AssociateRequest to execute IOrganizationService.Associate() requests concurrently
        /// </summary>
        /// <param name="requests">The AssociateRequests defining the entity, relationship, and entities to associate</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Associate(IEnumerable<AssociateRequest> requests)
        {
            this.Associate(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a list of type AssociateRequest to execute IOrganizationService.Associate() requests concurrently
        /// </summary>
        /// <param name="requests">The AssociateRequests defining the entity, relationship, and entities to associate</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Associate(IEnumerable<AssociateRequest> requests, OrganizationServiceProxyOptions options)
        {
            this.ExecuteOperation<AssociateRequest>(requests, options,
                (request, proxy) =>
                {
                    proxy.Associate(request.Target.LogicalName, request.Target.Id, request.Relationship, request.RelatedEntities);
                });
        }

        #endregion

        #region IOrganizationService.Disassociate()

        /// <summary>
        /// Performs data parallelism on a list of type DisassociateRequest to execute IOrganizationService.Disassociate() requests concurrently
        /// </summary>
        /// <param name="requests">The DisassociateRequests defining the entity, relationship, and entities to disassociate</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Disassociate(IEnumerable<DisassociateRequest> requests)
        {
            this.Disassociate(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a list of type DisassociateRequest to execute IOrganizationService.Disassociate() requests concurrently
        /// </summary>
        /// <param name="requests">The DisassociateRequests defining the entity, relationship, and entities to disassociate</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public void Disassociate(IEnumerable<DisassociateRequest> requests, OrganizationServiceProxyOptions options)
        {
            this.ExecuteOperation<DisassociateRequest>(requests, options,
                (request, proxy) =>
                {
                    proxy.Disassociate(request.Target.LogicalName, request.Target.Id, request.Relationship, request.RelatedEntities);
                });
        }

        #endregion

        #region IOrganizationService.Retrieve()

        /// <summary>
        /// Performs data parallelism on a list of type RetrieveRequest to execute IOrganizationService.Retrieve() requests concurrently
        /// </summary>
        /// <param name="requests">The RetrieveRequests defining the entities to be retrieved</param>
        /// <returns>A list of type Entity containing the retrieved entities</returns>
        /// <remarks>
        /// IMPORTANT!! RetrieveMultiple is the favored approach for retrieving multiple entities of the same type
        /// This approach should only be used if trying to retrieve multiple individual records of varying entity types.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests)
        {
            return this.Retrieve(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type RetrieveRequest to execute IOrganizationService.Retrieve() requests concurrently
        /// </summary>
        /// <param name="requests">The RetrieveRequests defining the entities to be retrieved</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A collection of type Entity containing the retrieved entities</returns>
        /// <remarks>
        /// IMPORTANT!! RetrieveMultiple is the favored approach for retrieving multiple entities of the same type
        /// This approach should only be used if trying to retrieve multiple individual records of varying entity types.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<RetrieveRequest, Entity>(requests, options,
                (request, loopState, index, context) =>
                {
                    var entity = context.Local.Retrieve(request.Target.LogicalName, request.Target.Id, request.ColumnSet);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(entity);

                    return context;
                });
        }

        #endregion

        #region IOrganizationService.RetrieveMultiple()

        /// <summary>
        /// Performs data parallelism on a keyed collection of QueryBase values to execute IOrganizationService.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (QueryExpresion or FetchExpression)</param>
        /// <returns>A keyed collection of EntityCollection values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes that only the first page of results is desired (shouldRetrieveAllPages: false)
        /// Assumes default proxy options should be used (options: new OrganizationServiceProxyOptions())
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries)
        {
            return this.RetrieveMultiple(queries, false, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of QueryBase values to execute IOrganizationService.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (QueryExpresion or FetchExpression)</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A keyed collection of EntityCollection values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes that only the first page of results is desired (shouldRetrieveAllPages: false)
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, OrganizationServiceProxyOptions options)
        {
            return this.RetrieveMultiple(queries, false, options);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of QueryBase values to execute IOrganizationService.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (QueryExpresion or FetchExpression)</param>
        /// <param name="shouldRetrieveAllPages">True = iterative requests will be performed to retrieve all pages, otherwise only the first results page will be returned for each query</param>
        /// <returns>A keyed collection of EntityCollection values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes default proxy options should be used (options: new OrganizationServiceProxyOptions()).
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages)
        {
            return this.RetrieveMultiple(queries, shouldRetrieveAllPages, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of QueryBase values to execute IOrganizationService.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (QueryExpresion or FetchExpression)</param>
        /// <param name="shouldRetrieveAllPages">True = iterative requests will be performed to retrieve all pages, otherwise only the first results page will be returned for each query</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A keyed collection of EntityCollection values which represent the results of each query</returns>
        /// <remarks>
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, QueryBase>, KeyValuePair<string, EntityCollection>>(queries, options,
                    (query, loopState, index, context) =>
                    {
                        var result = context.Local.RetrieveMultiple(query.Value, shouldRetrieveAllPages);

                        context.Results.Add(new KeyValuePair<string, EntityCollection>(query.Key, result));

                        return context;
                    }).ToDictionary(r => r.Key, r => r.Value);
        }     

        #endregion

        #region IOrganizationService.Execute()

        /// <summary>
        /// Performs data parallelism on a keyed collection of type OrganizationRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <returns>A keyed collection of type OrganizationResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests)
        {
            return this.Execute(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type OrganizationRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A keyed collection of type OrganizationResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, OrganizationRequest>, KeyValuePair<string, OrganizationResponse>>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = context.Local.Execute(request.Value);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(new KeyValuePair<string, OrganizationResponse>(request.Key, response));

                    return context;
                }).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from OrganiztionRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from OrganizationResponse</typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from OrganiztionRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from OrganizationResponse</typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, OrganizationServiceProxyOptions options)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, TRequest>, KeyValuePair<string, TResponse>>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request.Value);

                    context.Results.Add(new KeyValuePair<string, TResponse>(request.Key, response));

                    return context;
                }).ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type OrganizationRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be executed</param>
        /// <returns>A collection of type OrganizationResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests)
        {
            return this.Execute(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type OrganizationRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A collection of type OrganizationResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests, OrganizationServiceProxyOptions options)
        {
            return this.ExecuteOperationWithResponse<OrganizationRequest, OrganizationResponse>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = context.Local.Execute(request);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(response);

                    return context;
                });
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from OrganiztionRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from OrganizationResponse</typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new OrganizationServiceProxyOptions());
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute IOrganizationService.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from OrganiztionRequest</typeparam>
        /// <typeparam name="TResponse">The response type that derives from OrganizationResponse</typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch AggregateException to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {            
            return this.ExecuteOperationWithResponse<TRequest, TResponse>(requests, options,
                (request, loopState, index, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request);

                    context.Results.Add(response);

                    return context;
                });
        }

        #endregion

        #endregion

        #region Core Parallel Execution Methods <TRequest> & <TRequest, TResponse>

        /// <summary>
        /// Core implementation of the parallel pattern for service operations that do not return a response (i.e. Update/Delete/Associate/Disassociate)
        /// </summary>
        /// <typeparam name="TRequest">The type being submitted in the service operation request</typeparam>
        /// <param name="requests">The collection of requests to be submitted</param>
        /// <param name="options">The configurable options for the OrganizationServiceProxy requests</param>
        /// <param name="operation">The specific operation being executed</param>
        private void ExecuteOperation<TRequest>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options,
            Action<TRequest, ManagedTokenOrganizationServiceProxy> operation)
        {
            //Inline method for initializing a new organization service channel
            Func<ManagedTokenOrganizationServiceProxy> proxyInit = () =>
                {
                    var proxy = this.ServiceManager.GetProxy();
                    proxy.SetProxyOptions(options);

                    return proxy;
                };
            
            using (var threadLocalProxy = new ThreadLocal<ManagedTokenOrganizationServiceProxy>(proxyInit, true))
            {
                try
                {
                    Parallel.ForEach<TRequest>(requests,
                        new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism },
                        (request) =>
                        {
                            operation(request, threadLocalProxy.Value);
                        });
                }
                finally
                {
                    Array.ForEach(threadLocalProxy.Values.ToArray(), p => p.Dispose());
                }
            }
        }

        /// <summary>
        /// Core implementation of the parallel pattern for service operations that should collect responses to each request
        /// </summary>
        /// <typeparam name="TRequest">The type being submitted in the service operation request</typeparam>
        /// <typeparam name="TResponse">The response type collected from each request and returned</typeparam>
        /// <param name="requests">The collection of requests to be submitted</param>
        /// <param name="options">The configurable options for the OrganizationServiceProxy requests</param>
        /// <param name="operation">The specific operation being executed</param>
        /// <returns>A collection of specified response types from service operation requests</returns>
        /// <remarks>
        /// IMPORTANT!! When defining the core operation, be sure to add responses you wish to collect via proxy.Results.Add(TResponse item);
        /// </remarks>
        private IEnumerable<TResponse> ExecuteOperationWithResponse<TRequest, TResponse>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options,
            Func<TRequest, ParallelLoopState, long, ParallelOrganizationOperationContext<TResponse>, ParallelOrganizationOperationContext<TResponse>> operation)
        {
            var responses = new ConcurrentBag<TResponse>();

            //Inline method for initializing a new organization service channel
            Func<ManagedTokenOrganizationServiceProxy> proxyInit = () =>
                {
                    var proxy = this.ServiceManager.GetProxy();
                    proxy.SetProxyOptions(options);

                    return proxy;
                };
            
            using (var threadLocalProxy = new ThreadLocal<ManagedTokenOrganizationServiceProxy>(proxyInit, true))
            {
                try
                {
                    Parallel.ForEach<TRequest, ParallelOrganizationOperationContext<TResponse>>(requests,
                        new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism },
                        () => new ParallelOrganizationOperationContext<TResponse>(threadLocalProxy.Value),
                        operation,
                        (context) =>
                        {                                                                                                                
                            Array.ForEach(context.Results.ToArray(), r => responses.Add(r));

                            //Remove temporary reference to ThreadLocal proxy
                            context.Local = null;
                        });
                }
                finally
                {
                    Array.ForEach(threadLocalProxy.Values.ToArray(), p => p.Dispose());
                }
            }

            return responses;
        }

        #endregion 
    }

    /// <summary>
    /// Base class for executing concurrent requests for common ServiceProxy<TService> operations
    /// </summary>
    /// <typeparam name="T">The type of service manager Discovery or Organization</typeparam>
    public abstract class ParallelServiceProxy<T> : ParallelServiceProxy
        where T : XrmServiceManagerBase
    {
        protected static object syncRoot = new Object();            

        #region Constructor(s)

        private ParallelServiceProxy() { throw new NotImplementedException(); }

        protected ParallelServiceProxy(T serviceManager)
            : this(serviceManager, ParallelServiceProxy.MaxDegreeOfParallelismDefault) { }

        protected ParallelServiceProxy(T serviceManager, int maxDegreeOfParallelism)
        {
            this.ServiceManager = serviceManager;
            this.MaxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        #endregion

        #region Fields

        private int maxDegreeOfParallelism;

        #endregion

        #region Properties
        
        protected T ServiceManager { get; set; }


        /// <summary>
        /// Override the default max degree of concurrency for the ParallelServiceProxy operations
        /// </summary>
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

        #endregion

        #region Methods

        /// <summary>
        /// Ensures a valid max degree of parallelism argument before initiating the parallel process
        /// </summary>
        /// <param name="maxDegree">The max degree of parallelism</param>
        /// <exception cref="ArgumentOutOfRangeException">An exception will be thrown if value is less than -1 or equal to 0.</exception>
        protected void ValidateDegreeOfParallelism(int maxDegree)
        {
            if (maxDegree < -1
                || maxDegree == 0)
                throw new ArgumentOutOfRangeException(string.Format("The provided MaxDegreeOfParallelism={0} is not valid. Argument must be -1 or greater than 0.", maxDegree));
        }

        #endregion
    }

    public abstract class ParallelServiceProxy
    {
        public const int MaxDegreeOfParallelismDefault = -1;
    }
}
