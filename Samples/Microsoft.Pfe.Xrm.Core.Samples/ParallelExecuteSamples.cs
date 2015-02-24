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
    class ParallelExecuteSamples
    {
        public ParallelExecuteSamples(Uri serverUri, string username, string password)
        {
            this.Manager = new OrganizationServiceManager(serverUri, username, password);
        }

        /// <summary>
        /// Reusable instance of OrganizationServiceManager
        /// </summary>
        OrganizationServiceManager Manager { get; set; }


        /// <summary>
        /// Demonstrates parallelized execution of multiple set state requests
        /// </summary>
        /// <param name="accountIds">The list of accounts to deactivate</param>
        public void ParallelExecuteSetStateRequests(Guid[] accountIds)
        {
            var requests = new List<OrganizationRequest>();

            Array.ForEach(accountIds, id =>
                {
                    var request = new SetStateRequest()
                    {
                        EntityMoniker = new EntityReference("account", id),
                        State = new OptionSetValue(1),
                        Status = new OptionSetValue(2)
                    };

                    requests.Add(request);
                });

            try
            {
                this.Manager.ParallelProxy.Execute(requests);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }

        /// <summary>
        /// Demonstrates parallelized execution of multiple team privilege requests using generic method
        /// </summary>
        /// <param name="teamIds">The list of teams who's privileges that should be retrieved</param>
        /// <returns>The list of team privileges</returns>
        public IDictionary<Guid, RetrieveTeamPrivilegesResponse> ParallelGenericExecuteRequests(Guid[] teamIds)
        {
            IDictionary<Guid, RetrieveTeamPrivilegesResponse> responses = null;
            var requests = new Dictionary<string, RetrieveTeamPrivilegesRequest>();

            Array.ForEach(teamIds, id =>
                {
                    var request = new RetrieveTeamPrivilegesRequest()
                    {
                        TeamId = id
                    };

                    requests.Add(id.ToString(), request);
                });

            try
            {
                responses = this.Manager.ParallelProxy.Execute<RetrieveTeamPrivilegesRequest, RetrieveTeamPrivilegesResponse>(requests)
                    .ToDictionary(r => Guid.Parse(r.Key), r => r.Value);

                foreach (var response in responses)
                {
                    Console.WriteLine("Retrieves {0} privileges for team with id={1}", 
                        response.Value.RolePrivileges.Length, 
                        response.Key);
                }
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            return responses;
        }


        /// <summary>
        /// Demonstrates how ExecuteMultipleRequests should be parallelized via generic Execute method
        /// </summary>
        /// <param name="requests">The keyed collection of ExecuteMultipleRequests to execute in parallel</param>
        /// <returns>The keyed collection of ExecuteMultipleResponses (assuming that a response was indicated in the requests)</returns>
        public IDictionary<string, ExecuteMultipleResponse> ParallelExecuteMultiple(IDictionary<string, ExecuteMultipleRequest> requests)
        {
            IDictionary<string, ExecuteMultipleResponse> responses = null;

            try
            {
                responses = this.Manager.ParallelProxy.Execute<ExecuteMultipleRequest, ExecuteMultipleResponse>(requests);

                foreach (var response in responses)
                {
                    Console.WriteLine("{0} responses and {1} errors for ExecuteMultipleRequest with key={2}",
                        response.Value.Responses.Count(r => r.Response != null),
                        response.Value.Responses.Count(r => r.Fault != null),
                        response.Key);
                }
            }
            catch(AggregateException ae)
            {
                // Handle exceptions
            }

            return responses;
        }

        /// <summary>
        /// Demonstrates parallelized execution of multiple requests with optional exception handler delegate
        /// </summary>
        /// <param name="requests">The list of requests</param>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public void ParallelExecuteRequestsWithExceptionHandler(List<AssignRequest> requests)
        {
            int errorCount = 0;
            
            try
            {
                this.Manager.ParallelProxy.Execute<AssignRequest, AssignResponse>(requests,
                    (request, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered assigning entity with Id={0} to user with Id={1}: {2}", 
                            request.Target.Id, 
                            request.Assignee.Id, 
                            ex.Detail.Message);
                        errorCount++;
                    });
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during execute of parallel assign requests.", errorCount);
        }
    }
}
