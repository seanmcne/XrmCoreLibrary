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
                this.Manager.ParallelProxy.Execute(requests).ToList();
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
        public List<RetrieveTeamPrivilegesResponse> ParallelGenericExecuteRequests(Guid[] teamIds)
        {
            List<RetrieveTeamPrivilegesResponse> responses = null;
            var requests = new List<RetrieveTeamPrivilegesRequest>();

            Array.ForEach(teamIds, id =>
                {
                    var request = new RetrieveTeamPrivilegesRequest()
                    {
                        TeamId = id
                    };

                    requests.Add(request);
                });

            try
            {
                responses = this.Manager.ParallelProxy.Execute<RetrieveTeamPrivilegesRequest, RetrieveTeamPrivilegesResponse>(requests).ToList();
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
        /// <param name="requests">The list of ExecuteMultipleRequests to execute in parallel</param>
        /// <returns>The list of ExecuteMultipleResponses (assuming that a response was indicated in the requests)</returns>
        public List<ExecuteMultipleResponse> ParallelExecuteMultiple(List<ExecuteMultipleRequest> requests)
        {
            List<ExecuteMultipleResponse> responses = null;

            try
            {
                responses = this.Manager.ParallelProxy.Execute<ExecuteMultipleRequest, ExecuteMultipleResponse>(requests).ToList();
            }
            catch(AggregateException ae)
            {
                // Handle exceptions
            }

            return responses;
        }
    }
}
