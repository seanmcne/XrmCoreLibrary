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
    using System.Linq;
    using System.Text;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;

    public static class DiscoveryServiceProxyExtensions
    {
        /// <summary>
        /// Helper method for assigning common proxy-specific settings such as channel timeout
        /// </summary>
        /// <param name="proxy">The service proxy</param>
        /// <param name="options">The options to configure on the service proxy</param>
        public static void SetProxyOptions(this DiscoveryServiceProxy proxy, DiscoveryServiceProxyOptions options)
        {
            if (!options.Timeout.Equals(TimeSpan.Zero)
                && options.Timeout > TimeSpan.Zero)
                proxy.Timeout = options.Timeout;
        }
    }

    public static class OrganizationServiceProxyExtensions
    {
        /// <summary>
        /// Helper method for assigning common proxy-specific settings for impersonation, early-bound types, and channel timeout
        /// </summary>
        /// <param name="proxy">The service proxy</param>
        /// <param name="options">The options to configure on the service proxy</param>
        public static void SetProxyOptions(this OrganizationServiceProxy proxy, OrganizationServiceProxyOptions options)
        {
            if (!options.CallerId.Equals(Guid.Empty))
                proxy.CallerId = options.CallerId;

            if (options.ShouldEnableProxyTypes)
                proxy.EnableProxyTypes();
            
            if (!options.Timeout.Equals(TimeSpan.Zero)
                && options.Timeout > TimeSpan.Zero)
                proxy.Timeout = options.Timeout;
        }        
        
        /// <summary>
        /// Performs an iterative series of RetrieveMultiple requests in order to obtain all pages of results
        /// </summary>
        /// <param name="proxy">The IOrganizationService proxy connection</param>
        /// <param name="query">The query to be executed</param>
        /// <param name="shouldRetrieveAllPages">True = perform iterative paged query requests, otherwise return first page only</param>
        /// <returns>An EntityCollection containing the results of the query</returns>
        /// <remarks>
        /// Assumes no paged operation to perform. Passes an empty paged operation.
        ///
        /// CRM limits query response to paged result sets of 5,000. This method encapsulates the logic for performing subsequent 
        /// query requests so that all results can be retrieved.
        /// </remarks>
        public static EntityCollection RetrieveMultiple(this OrganizationServiceProxy proxy, QueryBase query, bool shouldRetrieveAllPages)
        {
            return proxy.RetrieveMultiple(query, shouldRetrieveAllPages, (page) => { return; });
        }

        /// <summary>
        /// Performs an iterative series of RetrieveMultiple requests in order to obtain all pages of results
        /// </summary>
        /// <param name="proxy">The IOrganizationService proxy connection</param>
        /// <param name="query">The query to be executed</param>
        /// <param name="shouldRetrieveAllPages">True = perform iterative paged query requests, otherwise return first page only</param>
        /// <param name="pagedOperation">An operation to perform on each page of results as it's retrieved</param>
        /// <returns>An EntityCollection containing the results of the query</returns>
        /// <remarks>
        /// CRM limits query response to paged result sets of 5,000. This method encapsulates the logic for performing subsequent 
        /// query requests so that all results can be retrieved.
        /// </remarks>
        public static EntityCollection RetrieveMultiple(this OrganizationServiceProxy proxy, QueryBase query, bool shouldRetrieveAllPages, Action<EntityCollection> pagedOperation)
        {
            if (query == null)
                throw new ArgumentNullException("query", "Must supply a query for the RetrieveMultiple request");
            if (pagedOperation == null)
                throw new ArgumentNullException("pagedOperation", "Must define an inline method to be invoked on each EntityCollection page");
            
            var qe = query as QueryExpression;

            if (qe != null)
            {
                return proxy.RetrieveMultiple(qe, shouldRetrieveAllPages, pagedOperation);
            }
            else
            {
                var fe = query as FetchExpression;

                if (fe != null)
                {
                    return proxy.RetrieveMultiple(fe, shouldRetrieveAllPages, pagedOperation);
                }
            }

            throw new ArgumentException("This method only handles FetchExpression and QueryExpression types.", "query");
        }

        /// <summary>
        /// Performs an iterative series of RetrieveMultiple requests using QueryExpression in order to obtain all pages of results
        /// </summary>
        /// <param name="proxy">The IOrganizationService proxy connection</param>
        /// <param name="query">The QueryExpression query to be executed</param>
        /// <param name="shouldRetrieveAllPages">True = perform iterative paged query requests, otherwise return first page only</param>
        /// <param name="pagedOperation">An operation to perform on each page of results as it's retrieved</param>
        /// <returns>An EntityCollection containing the results of the query</returns>
        /// <remarks>
        /// CRM limits query response to paged result sets of 5,000. This method encapsulates the logic for performing subsequent 
        /// query requests so that all results can be retrieved.
        /// </remarks>
        private static EntityCollection RetrieveMultiple(this OrganizationServiceProxy proxy, QueryExpression query, bool shouldRetrieveAllPages, Action<EntityCollection> pagedOperation)
        {
            var allResults = new EntityCollection();
            var firstPage = true;

            //Establish first page
            if (query.PageInfo == null)
            {
                query.PageInfo = new PagingInfo()
                {
                    Count = 5000,
                    PageNumber = 1,
                    PagingCookie = null,
                    ReturnTotalRecordCount = false
                };
            }
            else if (query.PageInfo.PageNumber != 1
                    || query.PageInfo.PagingCookie != null)
            {
                //Reset to first page
                query.PageInfo.PageNumber = 1;
                query.PageInfo.PagingCookie = null;
            }

            while (true)
            {
                var page = proxy.RetrieveMultiple(query); //retrieve the page
                
                //Capture the page
                if (firstPage)
                {
                    allResults = page;
                    firstPage = false;
                }
                else
                {                    
                    allResults.Entities.AddRange(page.Entities);
                }

                //Invoke the paged operation
                pagedOperation(page);

                //Setup for next page
                if (shouldRetrieveAllPages
                    && page.MoreRecords)
                {
                    query.PageInfo.PageNumber++;
                    query.PageInfo.PagingCookie = page.PagingCookie;
                }
                else
                {
                    break;
                }
            }

            return allResults;
        }

        /// <summary>
        /// Performs an iterative series of RetrieveMultiple requests using FetchExpression in order to obtain all pages of results
        /// </summary>
        /// <param name="proxy">The IOrganizationService proxy connection</param>
        /// <param name="query">The FetchExpression query to be executed</param>
        /// <param name="shouldRetrieveAllPages">True = perform iterative paged query requests, otherwise return first page only</param>
        /// <param name="pagedOperation">An operation to perform on each page of results as it's retrieved</param>
        /// <returns>An EntityCollection containing the results of the query</returns>
        /// <remarks>
        /// CRM limits query response to paged result sets of 5,000. This method encapsulates the logic for performing subsequent 
        /// query requests so that all results can be retrieved.
        /// </remarks>
        private static EntityCollection RetrieveMultiple(this OrganizationServiceProxy proxy, FetchExpression query, bool shouldRetrieveAllPages, Action<EntityCollection> pagedOperation)
        {            
            var allResults = new EntityCollection();
            var firstPage = true;
            var pageNumber = 1;
            var pageSize = query.GetPageSize();

            //Establish the first page
            query.SetPage(null, pageNumber, pageSize);

            while (true)
            {
                var page = proxy.RetrieveMultiple(query);
                
                if (firstPage)
                {
                    allResults = page;
                    firstPage = false;
                }
                else
                {
                    allResults.Entities.AddRange(page.Entities);
                }

                pagedOperation(page);

                if (shouldRetrieveAllPages
                    && page.MoreRecords)
                {
                    //Get next page
                    pageNumber++;

                    query.SetPage(page.PagingCookie, pageNumber, pageSize);
                }
                else
                {
                    break;
                }
            }

            return allResults;
        }
    }
}
