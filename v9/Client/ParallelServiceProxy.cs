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
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Pfe.Xrm.Diagnostics;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    
    /// <summary>
    /// Class for executing concurrent <see cref="IOrganizationService"/> operations
    /// </summary>
    /// <remarks>
    /// During parallel operations, <see cref="OrganizationServiceProxy"/> instances are aligned with individual data partitions
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

        #endregion

        #region Multi-threaded IOrganizationService Operation Methods

        #region IOrganizationService.Create()

        /// <summary>
        /// Performs data parallelism on a keyed collection of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The keyed collection target entities to be created</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>A keyed collection of unique identifiers for each created <see cref="Entity"/></returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// Only returning generated unique identifier because it's assumed that requesting process will maintain a reference
        /// between key and the <see cref="Entity"/> instance submitted as the Create target to which the unique identifier can be correlated
        /// </remarks>
        public IDictionary<string, Guid> Create(IDictionary<string, Entity> targets, Action<KeyValuePair<string, Entity>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.Create(targets, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The keyed collection target entities to be created</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>A keyed collection of unique identifiers for each created <see cref="Entity"/></returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// Only returning generated unique identifier because it's assumed that requesting process will maintain a reference
        /// between key and the <see cref="Entity"/> instance submitted as the Create target to which the unique identifier can be correlated
        /// </remarks>
        public IDictionary<string, Guid> Create(IDictionary<string, Entity> targets, OrganizationServiceProxyOptions options, Action<KeyValuePair<string, Entity>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, Entity>, KeyValuePair<string, Guid>>(targets, options,
                (target, context) =>
                {
                    Guid id = context.Local.Create(target.Value); //Hydrate target with response Id

                    //Collect the result from each iteration in this partition
                    context.Results.Add(new KeyValuePair<string, Guid>(target.Key, id));
                },
                errorHandler)
                .ToDictionary(t => t.Key, t => t.Value);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be created</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>The collection of created <see cref="Entity"/> records, hydrated with the response Id</returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// By returning the original collection of target entities with Ids, this allows for concurrent creation of multiple entity types
        /// and ability to cross-reference submitted data with the plaftorm generated Id.  Note that subsequent Update requests should
        /// always instantiate a new <see cref="Entity"/> instance and assign the Id.
        /// </remarks>
        public IEnumerable<Entity> Create(IEnumerable<Entity> targets, Action<Entity, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.Create(targets, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Create() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be created</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>The collection of created <see cref="Entity"/> records, hydrated with the response Id</returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        /// <remarks>
        /// By returning the original collection of target entities with Ids, this allows for concurrent creation of multiple entity types
        /// and ability to cross-reference submitted data with the plaftorm generated Id.  Note that subsequent Update requests should
        /// always instantiate a new <see cref="Entity"/> instance and assign the Id.
        /// </remarks>
        public IEnumerable<Entity> Create(IEnumerable<Entity> targets, OrganizationServiceProxyOptions options, Action<Entity, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<Entity, Entity>(targets, options,
                (target, context) =>
                {                   
                    target.Id = context.Local.Create(target); //Hydrate target with response Id
                    
                    //Collect the result from each iteration in this partition
                    context.Results.Add(target);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.Update()

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Update() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Update(IEnumerable<Entity> targets, Action<Entity, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.Update(targets, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="Entity"/> to execute <see cref="IOrganizationService"/>.Update() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Update(IEnumerable<Entity> targets, OrganizationServiceProxyOptions options, Action<Entity, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.ExecuteOperation<Entity>(targets, options,
                (target, proxy) =>
                {
                    proxy.Update(target);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.Delete()

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="EntityReference"/> to execute <see cref="IOrganizationService"/>.Delete() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be updated</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Delete(IEnumerable<EntityReference> targets, Action<EntityReference, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.Delete(targets, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="EntityReference"/> to execute <see cref="IOrganizationService"/>.Delete() requests concurrently
        /// </summary>
        /// <param name="targets">The target entities to be deleted</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Delete(IEnumerable<EntityReference> targets, OrganizationServiceProxyOptions options, Action<EntityReference, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.ExecuteOperation<EntityReference>(targets, options,
                (target, proxy) =>
                {
                    proxy.Delete(target.LogicalName, target.Id);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.Associate()

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="AssociateRequest"/> to execute <see cref="IOrganizationService"/>.Associate() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="AssociateRequest"/> collection defining the entity, relationship, and entities to associate</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Associate(IEnumerable<AssociateRequest> requests, Action<AssociateRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.Associate(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="AssociateRequest"/> to execute <see cref="IOrganizationService"/>.Associate() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="AssociateRequest"/> collection defining the entity, relationship, and entities to associate</param>
        /// <param name="options">The configurable options for the parallel OrganizationServiceProxy requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Associate(IEnumerable<AssociateRequest> requests, OrganizationServiceProxyOptions options, Action<AssociateRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.ExecuteOperation<AssociateRequest>(requests, options,
                (request, proxy) =>
                {
                    proxy.Associate(request.Target.LogicalName, request.Target.Id, request.Relationship, request.RelatedEntities);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.Disassociate()

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="DisassociateRequest"/> to execute <see cref="IOrganizationService"/>.Disassociate() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="DisassociateRequest"/> collection defining the entity, relationship, and entities to disassociate</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Disassociate(IEnumerable<DisassociateRequest> requests, Action<DisassociateRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {         
            this.Disassociate(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a list of type <see cref="DisassociateRequest"/> to execute <see cref="IOrganizationService"/>.Disassociate() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="DisassociateRequest"/> collection defining the entity, relationship, and entities to disassociate</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public void Disassociate(IEnumerable<DisassociateRequest> requests, OrganizationServiceProxyOptions options, Action<DisassociateRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            this.ExecuteOperation<DisassociateRequest>(requests, options,
                (request, proxy) =>
                {
                    proxy.Disassociate(request.Target.LogicalName, request.Target.Id, request.Relationship, request.RelatedEntities);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.Retrieve()

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="RetrieveRequest"/> to execute <see cref="IOrganizationService"/>.Retrieve() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="RetrieveRequest"/> collection defining the entities to be retrieved</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A collection of type <see cref="Entity"/> containing the retrieved entities</returns>
        /// <remarks>
        /// IMPORTANT!! RetrieveMultiple is the favored approach for retrieving multiple entities of the same type
        /// This approach should only be used if trying to retrieve multiple individual records of varying entity types.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests, Action<RetrieveRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.Retrieve(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="RetrieveRequest"/> to execute <see cref="IOrganizationService"/>.Retrieve() requests concurrently
        /// </summary>
        /// <param name="requests">The <see cref="RetrieveRequest"/> collection defining the entities to be retrieved</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A collection of type <see cref="Entity"/> containing the retrieved entities</returns>
        /// <remarks>
        /// IMPORTANT!! RetrieveMultiple is the favored approach for retrieving multiple entities of the same type
        /// This approach should only be used if trying to retrieve multiple individual records of varying entity types.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<Entity> Retrieve(IEnumerable<RetrieveRequest> requests, OrganizationServiceProxyOptions options, Action<RetrieveRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<RetrieveRequest, Entity>(requests, options,
                (request, context) =>
                {
                    var entity = context.Local.Retrieve(request.Target.LogicalName, request.Target.Id, request.ColumnSet);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(entity);
                },
                errorHandler);
        }

        #endregion

        #region IOrganizationService.RetrieveMultiple()

        /// <summary>
        /// Performs data parallelism on a keyed collection of <see cref="QueryBase"/> values to execute <see cref="IOrganizationService"/>.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (<see cref="QueryExpresion"/> or <see cref="FetchExpression"/>)</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of <see cref="EntityCollection"/> values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes that only the first page of results is desired (shouldRetrieveAllPages: false)
        /// Assumes default proxy options should be used (options: new <see cref="OrganizationServiceProxyOptions"/>()).
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, Action<KeyValuePair<string, QueryBase>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.RetrieveMultiple(queries, false, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of <see cref="QueryBase"/> values to execute <see cref="IOrganizationService"/>.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (<see cref="QueryExpresion"/> or <see cref="FetchExpression"/>)</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of <see cref="EntityCollection"/> values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes that only the first page of results is desired (shouldRetrieveAllPages: false)
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, OrganizationServiceProxyOptions options, Action<KeyValuePair<string, QueryBase>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.RetrieveMultiple(queries, false, options, errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of <see cref="QueryBase"/> values to execute <see cref="IOrganizationService"/>.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (<see cref="QueryExpresion"/> or <see cref="FetchExpression"/>)</param>
        /// <param name="shouldRetrieveAllPages">True = iterative requests will be performed to retrieve all pages, otherwise only the first results page will be returned for each query</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of <see cref="EntityCollection"/> values which represent the results of each query</returns>
        /// <remarks>
        /// Assumes default proxy options should be used (options: new <see cref="OrganizationServiceProxyOptions"/>()).
        /// 
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages, Action<KeyValuePair<string, QueryBase>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.RetrieveMultiple(queries, shouldRetrieveAllPages, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of <see cref="QueryBase"/> values to execute <see cref="IOrganizationService"/>.RetrieveMultiple() requests concurrently
        /// </summary>
        /// <param name="queries">The keyed collection of queries (<see cref="QueryExpresion"/> or <see cref="FetchExpression"/>)</param>
        /// <param name="shouldRetrieveAllPages">True = iterative requests will be performed to retrieve all pages, otherwise only the first results page will be returned for each query</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>A keyed collection of <see cref="EntityCollection"/> values which represent the results of each query</returns>
        /// <remarks>
        /// IMPORTANT!! This approach should only be used if multiple queries for varying entity types are required or the result set can't be expressed in a single query. In the latter case, 
        /// leverage NoLock=true where possible to reduce database contention.
        /// </remarks>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, EntityCollection> RetrieveMultiple(IDictionary<string, QueryBase> queries, bool shouldRetrieveAllPages, OrganizationServiceProxyOptions options, Action<KeyValuePair<string, QueryBase>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, QueryBase>, KeyValuePair<string, EntityCollection>>(queries, options,
                    (query, context) =>
                    {
                        var result = context.Local.RetrieveMultiple(query.Value, shouldRetrieveAllPages);

                        context.Results.Add(new KeyValuePair<string, EntityCollection>(query.Key, result));
                    },
                    errorHandler)
                    .ToDictionary(r => r.Key, r => r.Value);
        }     

        #endregion

        #region IOrganizationService.Execute()

        /// <summary>
        /// Performs data parallelism on a keyed collection of type <see cref="OrganizationRequest"/> to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of type <see cref="OrganizationResponse"/> containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests, Action<KeyValuePair<string, OrganizationRequest>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.Execute(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type <see cref="OrganizationRequest"/> to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of type <see cref="OrganizationResponse"/> containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, OrganizationResponse> Execute(IDictionary<string, OrganizationRequest> requests, OrganizationServiceProxyOptions options, Action<KeyValuePair<string, OrganizationRequest>, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, OrganizationRequest>, KeyValuePair<string, OrganizationResponse>>(requests, options,
                (request, context) =>
                {
                    var response = context.Local.Execute(request.Value);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(new KeyValuePair<string, OrganizationResponse>(request.Key, response));
                },
                errorHandler)
                .ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from <see cref="OrganiztionRequest"/></typeparam>
        /// <typeparam name="TResponse">The response type that derives from <see cref="OrganizationResponse"/></typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, Action<KeyValuePair<string, TRequest>, FaultException<OrganizationServiceFault>> errorHandler = null)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a keyed collection of type TRequest to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from <see cref="OrganiztionRequest"/></typeparam>
        /// <typeparam name="TResponse">The response type that derives from <see cref="OrganizationResponse"/></typeparam>
        /// <param name="requests">The keyed collection of requests to be executed</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A keyed collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IDictionary<string, TResponse> Execute<TRequest, TResponse>(IDictionary<string, TRequest> requests, OrganizationServiceProxyOptions options, Action<KeyValuePair<string, TRequest>, FaultException<OrganizationServiceFault>> errorHandler = null)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.ExecuteOperationWithResponse<KeyValuePair<string, TRequest>, KeyValuePair<string, TResponse>>(requests, options,
                (request, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request.Value);

                    context.Results.Add(new KeyValuePair<string, TResponse>(request.Key, response));
                },
                errorHandler)
                .ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="OrganizationRequest"/> to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>A collection of type <see cref="OrganizationResponse"/> containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests, Action<OrganizationRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.Execute(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type <see cref="OrganizationRequest"/> to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>        
        /// <returns>A collection of type <see cref="OrganizationResponse"/> containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<OrganizationResponse> Execute(IEnumerable<OrganizationRequest> requests, OrganizationServiceProxyOptions options, Action<OrganizationRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
        {
            return this.ExecuteOperationWithResponse<OrganizationRequest, OrganizationResponse>(requests, options,
                (request, context) =>
                {
                    var response = context.Local.Execute(request);

                    //Collect the result from each iteration in this partition
                    context.Results.Add(response);
                },
                errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from <see cref="OrganiztionRequest"/></typeparam>
        /// <typeparam name="TResponse">The response type that derives from <see cref="OrganizationResponse"/></typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, Action<TRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.Execute<TRequest, TResponse>(requests, new OrganizationServiceProxyOptions(), errorHandler);
        }

        /// <summary>
        /// Performs data parallelism on a collection of type TRequest to execute <see cref="IOrganizationService"/>.Execute() requests concurrently
        /// </summary>
        /// <typeparam name="TRequest">The request type that derives from <see cref="OrganiztionRequest"/></typeparam>
        /// <typeparam name="TResponse">The response type that derives from <see cref="OrganizationResponse"/></typeparam>
        /// <param name="requests">The requests to be executed</param>
        /// <param name="options">The configurable options for the parallel <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="errorHandler">An optional error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A collection of type TResponse containing the responses to each executed request</returns>
        /// <exception cref="AggregateException">Callers should catch <see cref="AggregateException"/> to handle exceptions raised by individual requests</exception>
        public IEnumerable<TResponse> Execute<TRequest, TResponse>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options, Action<TRequest, FaultException<OrganizationServiceFault>> errorHandler = null)
            where TRequest : OrganizationRequest
            where TResponse : OrganizationResponse
        {
            return this.ExecuteOperationWithResponse<TRequest, TResponse>(requests, options,
                (request, context) =>
                {
                    var response = (TResponse)context.Local.Execute(request);

                    context.Results.Add(response);
                },
                errorHandler);
        }

        #endregion

        #endregion

        #region Core Parallel Execution Methods <TRequest> & <TRequest, TResponse>

        /// <summary>
        /// Core implementation of the parallel pattern for service operations that do not return a response (i.e. Update/Delete/Associate/Disassociate)
        /// </summary>
        /// <typeparam name="TRequest">The type being submitted in the service operation request</typeparam>
        /// <param name="requests">The collection of requests to be submitted</param>
        /// <param name="options">The configurable options for the <see cref="OrganizationServiceProxy"/> requests</param>
        /// <param name="operation">The specific operation being executed</param>
        /// <param name="errorHandler">The error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        private void ExecuteOperation<TRequest>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options,
            Action<TRequest, CrmServiceClient> operation, Action<TRequest, FaultException<OrganizationServiceFault>> errorHandler)
        {
            var allFailures = new ConcurrentBag<ParallelOrganizationOperationFailure<TRequest>>();
            
            // Inline method for initializing a new organization service channel
            Func<CrmServiceClient> proxyInit = () =>
                {
                    var proxy = this.ServiceManager.GetProxy();
                    proxy.SetProxyOptions(options);

                    return proxy;
                };
            
            using (var threadLocalProxy = new ThreadLocal<CrmServiceClient>(proxyInit, true))
            {
                try
                {
                    Parallel.ForEach<TRequest, ParallelOrganizationOperationContext<TRequest, bool>>(requests,
                        new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism },
                        () => {
                            return new ParallelOrganizationOperationContext<TRequest, bool>(); 
                        },
                        (request, loopState, index, context) =>
                        {
                            try
                            {
                                XrmCoreEventSource.Log.ParallelCoreOperationStart("ExecuteOperation"); 
                                operation(request, threadLocalProxy.Value);
                                XrmCoreEventSource.Log.ParallelCoreOperationCompleted("ExecuteOperation"); 
                            }
                            catch (FaultException<OrganizationServiceFault> fault)
                            {
                                XrmCoreEventSource.Log.LogError(fault.ToErrorMessageString().ToString());
                                // Track faults locally
                                if (errorHandler != null)
                                {
                                    context.Failures.Add(new ParallelOrganizationOperationFailure<TRequest>(request, fault));
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            catch (Exception exception) 
                                when (exception is System.TimeoutException || exception is System.Net.WebException)
                            {
                                XrmCoreEventSource.Log.LogError(exception.ToErrorMessageString().ToString());

                                var errorDetails = new ErrorDetailCollection();

                                foreach (KeyValuePair<string, object> dataElement in exception.Data)
                                {
                                    errorDetails.Add(dataElement);
                                }

                                var orgFaultMock = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault()
                                {
                                    ErrorCode = exception.HResult,
                                    ErrorDetails = errorDetails
                                });

                                // Track faults locally
                                if (errorHandler != null)
                                {
                                    context.Failures.Add(new ParallelOrganizationOperationFailure<TRequest>(request, orgFaultMock));
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            return context;
                        },
                        (context) =>
                        {
                            // Join faults together
                            Array.ForEach(context.Failures.ToArray(), f => allFailures.Add(f));
                        });
                }
                finally
                {
                    Array.ForEach(threadLocalProxy.Values.ToArray(), p => p.Dispose());
                }
            }

            // Handle faults
            if (errorHandler != null)
            {
                foreach (var failure in allFailures)
                {
                    errorHandler(failure.Request, failure.Exception);                    
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
        /// <param name="coreOperation">The specific operation being executed</param>
        /// <param name="errorHandler">The error handling operation. Handler will be passed the request that failed along with the corresponding <see cref="FaultException{OrganizationServiceFault}"/></param>
        /// <returns>A collection of specified response types from service operation requests</returns>
        /// <remarks>
        /// IMPORTANT!! When defining the core operation, be sure to add responses you wish to collect via proxy.Results.Add(TResponse item);
        /// </remarks>
        private IEnumerable<TResponse> ExecuteOperationWithResponse<TRequest, TResponse>(IEnumerable<TRequest> requests, OrganizationServiceProxyOptions options,
            Action<TRequest, ParallelOrganizationOperationContext<TRequest, TResponse>> coreOperation, Action<TRequest, FaultException<OrganizationServiceFault>> errorHandler)
        {
            var allResponses = new ConcurrentBag<TResponse>();
            var allFailures = new ConcurrentBag<ParallelOrganizationOperationFailure<TRequest>>();

            // Inline method for initializing a new organization service channel
            Func<CrmServiceClient> proxyInit = () =>
                {
                    var proxy = this.ServiceManager.GetProxy();
                    proxy.SetProxyOptions(options);

                    return proxy;
                };
            
            using (var threadLocalProxy = new ThreadLocal<CrmServiceClient>(proxyInit, true))
            {
                try
                {
                    Parallel.ForEach<TRequest, ParallelOrganizationOperationContext<TRequest, TResponse>>(
                        requests,
                        new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism },
                        () => {
                            return new ParallelOrganizationOperationContext<TRequest, TResponse>(threadLocalProxy.Value);
                        },
                        (request, loopState, index, context) =>
                        {
                            try
                            {
                                XrmCoreEventSource.Log.ParallelCoreOperationStart("ExecuteOperationWithResponse");
                                coreOperation(request, context);
                                XrmCoreEventSource.Log.ParallelCoreOperationCompleted("ExecuteOperationWithResponse");
                            }
                            catch (FaultException<OrganizationServiceFault> fault)
                            {
                                XrmCoreEventSource.Log.LogError(fault.ToErrorMessageString().ToString());
                                // Track faults locally
                                if (errorHandler != null)
                                {
                                    context.Failures.Add(new ParallelOrganizationOperationFailure<TRequest>(request, fault));
                                }
                                else
                                {
                                    throw;
                                }
                            }
                            catch (Exception exception)
                                when (exception is System.TimeoutException || exception is System.Net.WebException)
                            {
                                XrmCoreEventSource.Log.LogError(exception.ToErrorMessageString().ToString()); 
                                var errorDetails = new ErrorDetailCollection();

                                foreach (KeyValuePair<string, object> dataElement in exception.Data)
                                {
                                    errorDetails.Add(dataElement);
                                }

                                var orgFaultMock = new FaultException<OrganizationServiceFault>(new OrganizationServiceFault()
                                {
                                    ErrorCode = exception.HResult,
                                    ErrorDetails = errorDetails
                                });

                                // Track faults locally
                                if (errorHandler != null)
                                {
                                    context.Failures.Add(new ParallelOrganizationOperationFailure<TRequest>(request, orgFaultMock));
                                }
                                else
                                {
                                    throw;
                                }
                            }

                            return context;
                        },
                        (context) =>
                        {                                                                                                                
                            // Join results and faults together
                            Array.ForEach(context.Results.ToArray(), r => allResponses.Add(r));
                            Array.ForEach(context.Failures.ToArray(), f => allFailures.Add(f));                            

                            // Remove temporary reference to ThreadLocal proxy
                            context.Local = null;
                        });
                }
                finally
                {
                    Array.ForEach(threadLocalProxy.Values.ToArray(), p => p.Dispose());
                }
            }

            // Handle faults
            if (errorHandler != null)
            {
                foreach(var failure in allFailures)
                {
                    errorHandler(failure.Request, failure.Exception);                  
                }
            }

            return allResponses;
        }

        #endregion 
    }

    /// <summary>
    /// Base class for executing concurrent requests for common <see cref="ServiceProxy{TService}"/> operations
    /// </summary>
    /// <typeparam name="T">The type of service manager Organization</typeparam>
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

        protected ParallelServiceProxy(T serviceManager, int maxDegreeOfParallelism, int ThrottleRetryCountOverride, TimeSpan ThrottleRetryDelayOverride)
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

        public bool IsCrmServiceClientReady
        {
            get
            {
                if (ServiceManager != null &&
                    ServiceManager is OrganizationServiceManager )
                {

                    if ((ServiceManager as OrganizationServiceManager).IsCrmServiceClient)
                    {
                        return (ServiceManager as OrganizationServiceManager).IsCrmServiceClientReady;
                    }
                }
                return false; 
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
