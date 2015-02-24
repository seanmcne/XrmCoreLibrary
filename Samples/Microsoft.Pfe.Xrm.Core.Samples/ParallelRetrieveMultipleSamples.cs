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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

namespace Microsoft.Pfe.Xrm.Samples
{
    class ParallelRetrieveMultipleSamples
    {
        public ParallelRetrieveMultipleSamples(Uri serverUri, string username, string password)
        {
            this.Manager = new OrganizationServiceManager(serverUri, username, password);
        }

        /// <summary>
        /// Reusable instance of OrganizationServiceManager
        /// </summary>
        OrganizationServiceManager Manager { get; set; }


        /// <summary>
        /// Demonstrates parallelized submission of multiple queries
        /// </summary>
        /// <param name="queries">The keyed collection of queries to retrieve</param>
        /// <returns>The keyed collection of query result collections</returns>
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

        /// <summary>
        /// Demonstrates parallelized submission of multiple queries where all pages of results are retrieved for each query
        /// </summary>
        /// <param name="queries">The keyed collection of queries to retrieve</param>
        /// <returns>The keyed collection of query result collections</returns>
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

        /// <summary>
        /// Demonstrates parallelized submission of multiple queries. Queries can be mix of QueryExpression and FetchXML
        /// </summary>
        /// <returns>The keyed collection of query result collections</returns>
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

        /// <summary>
        /// Demonstrates parallelized submission of multiple queries with the optional exception handler delegate
        /// </summary>
        /// <param name="queries">The keyed collection of queries to retrieve</param>
        /// <returns>The keyed collection of query result collections</returns>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
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
    }
}
