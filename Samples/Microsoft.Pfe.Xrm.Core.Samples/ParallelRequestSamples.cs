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
    class ParallelRequestSamples
    {
        public ParallelRequestSamples(Uri serverUri, string username, string password)
        {
            this.Manager = new OrganizationServiceManager(serverUri, username, password);            
        }

        /// <summary>
        /// Reusable instance of OrganizationServiceManager
        /// </summary>
        OrganizationServiceManager Manager { get; set; }


        /// <summary>
        /// Demonstrates parallelized submission of multiple create requests
        /// </summary>
        /// <param name="targets">The list of target entities to create in parallel</param>
        /// <returns>The list of targets created with the assigned unique identifier</returns>
        public List<Entity> ParallelCreate(List<Entity> targets)
        {
            try
            {
                targets = this.Manager.ParallelProxy.Create(targets).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            targets.ForEach(r =>
                {
                    Console.WriteLine("Created {0} with id = {1}", r.LogicalName, r.Id);
                });

            return targets;
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple create requests with service proxy options
        /// 1. CallerId = The unique identifier of a systemuser being impersonated for the parallelized requests
        /// 2. ShouldEnableProxyTypes = Adds ProxyTypesBehavior to each OrganizationServiceProxy binding. Used for early-bound programming approach
        /// 3. Timeout = Increase the default 2 minute timeout on the channel to 5 minutes.
        /// </summary>
        /// <param name="targets">The list of target entities to create in parallel</param>
        /// <param name="callerId">The systemuser who should be impersonated for the parallelized requests</param>
        /// <returns>The list of targets created with the assigned unique identifier</returns>
        public List<Entity> ParallelCreateWithOptions(List<Entity> targets, Guid callerId)
        {
            var options = new OrganizationServiceProxyOptions()
            {
                CallerId = callerId,
                ShouldEnableProxyTypes = true,
                Timeout = new TimeSpan(0, 5, 0)
            };

            try
            {
                targets = this.Manager.ParallelProxy.Create(targets, options).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            targets.ForEach(r =>
            {
                Console.WriteLine("Created {0} with id = {1}", r.LogicalName, r.Id);
            });

            return targets;
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple update requests
        /// </summary>
        /// <param name="targets">The list of target entities to update in parallel</param>
        public void ParallelUpdate(List<Entity> targets)
        {
            try
            {
                this.Manager.ParallelProxy.Update(targets);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple delete requests
        /// </summary>
        /// <param name="targets">The list of target entities to delete in parallel</param>
        public void ParallelDelete(List<EntityReference> targets)
        {
            try
            {
                this.Manager.ParallelProxy.Delete(targets);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple associate requests
        /// </summary>
        /// <param name="requests">The list of associate requests to submit in parallel</param>
        public void ParallelAssociate(List<AssociateRequest> requests)
        {
            try
            {
                this.Manager.ParallelProxy.Associate(requests);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple disassociate requests
        /// </summary>
        /// <param name="requests">The list of disassociate requests to submit in parallel</param>
        public void ParallelDisassociate(List<DisassociateRequest> requests)
        {
            try
            {
                this.Manager.ParallelProxy.Disassociate(requests);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple retrieve requests
        /// </summary>
        /// <param name="requests">The list of retrieve requests to submit in parallel</param>
        /// <returns>The list of retrieved entities</returns>
        public List<Entity> ParallelRetrieve(List<RetrieveRequest> requests)
        {
            List<Entity> responses = null;

            try
            {
                responses = this.Manager.ParallelProxy.Retrieve(requests).ToList();
            }
            catch(AggregateException ae)
            {
                // Handle exceptions
            }

            responses.ForEach(r =>
                {
                    Console.WriteLine("Retrieved {0} with id = {1}", r.LogicalName, r.Id);
                });

            return responses;
        }
    }
}
