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
        /// <param name="organizationNames">The array of organizations to discover by name</param>
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
        /// <param name="upns">The array of user principal names (UPN)</param>
        public void ParallelExecuteRetrieveByUpnRequests(Guid organizationId, string[] upns)
        {
            var requests = new Dictionary<string, RetrieveUserIdByExternalIdRequest>();

            Array.ForEach(upns, upn =>
            {
                var request = new  RetrieveUserIdByExternalIdRequest()
                {
                    ExternalId = String.Format("C:{0}", upn),
                    OrganizationId = organizationId
                };

                requests.Add(upn, request);
            });

            try
            {
                var responses = this.Manager.ParallelProxy.Execute<RetrieveUserIdByExternalIdRequest, RetrieveUserIdByExternalIdResponse>(requests);

                foreach (var response in responses)
                {
                    Console.WriteLine("Retrieved user UPN={0}, systemuserid={1}", 
                        response.Key, 
                        response.Value.UserId);
                }
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }
    }
}
