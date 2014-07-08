using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;

namespace Microsoft.Pfe.Xrm.Samples
{
    class ParallelDiscoverySamples
    {
        public ParallelDiscoverySamples(Uri serverUri, string username, string password)
        {
            this.Manager = new DiscoveryServiceManager(serverUri, username, password);
        }

        /// <summary>
        /// Reusable instance of DiscoveryServiceManager
        /// </summary>
        DiscoveryServiceManager Manager { get; set; }


        /// <summary>
        /// Demonstrates parallelized execution of multiple discovery requests
        /// </summary>
        /// <param name="organizationNames">The list of organizations to discover by name</param>
        public List<DiscoveryResponse> ParallelExecuteDiscoveryRequests(string[] organizationNames)
        {            
            List<DiscoveryResponse> responses = null;
            var requests = new List<DiscoveryRequest>();

            Array.ForEach(organizationNames, name =>
                {
                    var request = new RetrieveOrganizationRequest()
                    {
                        UniqueName = name,
                        AccessType = EndpointAccessType.Default,
                        Release = OrganizationRelease.Current
                    };

                    requests.Add(request);
                });

            try
            {
                responses = this.Manager.ParallelProxy.Execute(requests).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            return responses;
        }

        /// <summary>
        /// Demonstrates parallelized execution of multiple retrieve systemuser by externalID (UPN)requests
        /// </summary>
        /// <param name="upns">The list of user principal names (UPN)</param>
        /// <remarks>Available for CRM Online only</remarks>
        public List<DiscoveryResponse> ParallelExecuteRetrieveByUpnRequests(Guid organizationId, string[] upns)
        {
            List<DiscoveryResponse> responses = null;
            var requests = new List<DiscoveryRequest>();

            Array.ForEach(upns, upn =>
            {
                var request = new  RetrieveUserIdByExternalIdRequest()
                {
                    ExternalId = String.Format("C:{0}", upn),
                    OrganizationId = organizationId
                };

                requests.Add(request);
            });

            try
            {
                responses = this.Manager.ParallelProxy.Execute(requests).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            return responses;
        }
    }
}
