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
        /// <param name="targets">The keyed collection of target entities to create in parallel</param>
        /// <returns>The keyed collection of the generated unique identifiers</returns>
        public IDictionary<string, Guid> ParallelCreate(IDictionary<string, Entity> targets)
        {
            var responses = new Dictionary<string, Guid>();

            try
            {
                responses = this.Manager.ParallelProxy.Create(targets)
                    .ToDictionary(t => t.Key, t => t.Value);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            foreach (var response in responses)
            {
                Console.WriteLine("Created {0} with id={1} for key={2}",
                    targets[response.Key].LogicalName,
                    response.Value,
                    response.Key);
            }

            return responses;
        }

        /// <summary>
        /// Demonstrates parallelized submission of multiple create requests as a list
        /// </summary>
        /// <param name="targets">The collection of target entities to create in parallel</param>
        /// <returns>The provided list of target entities, hydrated with the generated unique identifiers</returns>
        public List<Entity> ParallelCreateWithEntityList(List<Entity> targets)
        {
            try
            {
                targets = this.Manager.ParallelProxy.Create(targets).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            targets.ForEach(t =>
            {
                Console.WriteLine("Created {0} with id={1}", t.LogicalName, t.Id);
            });

            return targets;
        }

        /// <summary>
        /// Demonstrates parallelized submission of multiple create requests with the optional exception handler delegate
        /// </summary>
        /// <param name="targets">The keyed collection of target entities to create in parallel</param>
        /// <returns>The keyed collection of the generated unique identifiers</returns>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public IDictionary<string, Guid> ParallelCreateWithExceptionHandler(IDictionary<string, Entity> targets)
        {
            int errorCount = 0;
            var responses = new Dictionary<string, Guid>();

            try
            {
                responses = this.Manager.ParallelProxy.Create(targets,
                    (target, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during create of entity with key={0}: {1}", target.Key, ex.Detail.Message);
                        errorCount++;
                    })
                    .ToDictionary(t => t.Key, t => t.Value);
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel create.", errorCount);

            return responses;
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple create requests with service proxy options
        /// 1. CallerId = The unique identifier of a systemuser being impersonated for the parallelized requests
        /// 2. ShouldEnableProxyTypes = Adds ProxyTypesBehavior to each OrganizationServiceProxy binding. Used for early-bound programming approach
        /// 3. Timeout = Increase the default 2 minute timeout on the channel to 5 minutes.
        /// </summary>
        /// <param name="targets">The list of target entities to create in parallel</param>
        /// <param name="callerId">The systemuser who should be impersonated for the parallelized requests</param>
        /// <returns>The collection of targets created with the assigned unique identifier</returns>
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

            targets.ForEach(t =>
            {
                Console.WriteLine("Created {0} with id={1}", t.LogicalName, t.Id);
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
        /// Demonstrates a parallelized submission of multiple update requests with the optional exception handler delegate
        /// </summary>
        /// <param name="targets">The list of target entities to update in parallel</param>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public void ParallelUpdateWithExceptionHandler(List<Entity> targets)
        {
            int errorCount = 0;

            try
            {
                this.Manager.ParallelProxy.Update(targets,
                    (target, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during update of entity with Id={0}: {1}", target.Id, ex.Detail.Message);
                        errorCount++;
                    });
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel update.", errorCount);
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
        /// Demonstrates a parallelized submission of multiple delete requests with the optional exception handler delegate
        /// </summary>
        /// <param name="targets">The list of target entities to delete in parallel</param>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public void ParallelDeleteWithExceptionHandler(List<EntityReference> targets)
        {
            int errorCount = 0;

            try
            {
                this.Manager.ParallelProxy.Delete(targets,
                    (target, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during delete of entity with Id={0}: {1}", target.Id, ex.Detail.Message);
                        errorCount++;
                    });
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel delete.", errorCount);
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
        /// Demonstrates a parallelized submission of multiple associate requests with optional exception handler delegate
        /// </summary>
        /// <param name="requests">The list of associate requests to submit in parallel</param>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public void ParallelAssociateWithExceptionHandler(List<AssociateRequest> requests)
        {
            int errorCount = 0;

            try
            {
                this.Manager.ParallelProxy.Associate(requests,
                    (request, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during associate of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                        errorCount++;
                    });
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel associate.", errorCount);
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
        /// Demonstrates a parallelized submission of multiple disassociate requests with optional exception handler delegate
        /// </summary>
        /// <param name="requests">The list of disassociate requests to submit in parallel</param>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public void ParallelDisassociateWithExceptionHandler(List<DisassociateRequest> requests)
        {
            int errorCount = 0;

            try
            {
                this.Manager.ParallelProxy.Disassociate(requests,
                    (request, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during disassociate of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                        errorCount++;
                    });
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel disassociate.", errorCount);
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
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            responses.ForEach(r =>
                {
                    Console.WriteLine("Retrieved {0} with id = {1}", r.LogicalName, r.Id);
                });

            return responses;
        }

        /// <summary>
        /// Demonstrates a parallelized submission of multiple retrieve requests with optional exception handler delegate
        /// </summary>
        /// <param name="requests">The list of retrieve requests to submit in parallel</param>
        /// <returns>The list of retrieved entities</returns>
        /// <remarks>
        /// The exception handler delegate is provided the request type and the fault exception encountered. This delegate function is executed on the
        /// calling thread after all parallel operations are complete
        /// </remarks>
        public List<Entity> ParallelRetrieveWithExceptionHandler(List<RetrieveRequest> requests)
        {
            int errorCount = 0;
            List<Entity> responses = null;

            try
            {
                responses = this.Manager.ParallelProxy.Retrieve(requests,
                    (request, ex) =>
                    {
                        System.Diagnostics.Debug.WriteLine("Error encountered during retrieve of entity with Id={0}: {1}", request.Target.Id, ex.Detail.Message);
                        errorCount++;
                    }).ToList();
            }
            catch (AggregateException ae)
            {
                // Handle exceptions
            }

            Console.WriteLine("{0} errors encountered during parallel retrieves.", errorCount);

            return responses;
        }
    }
}
