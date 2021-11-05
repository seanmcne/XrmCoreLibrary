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
    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Crm.Sdk.Messages;

    public static class BatchRequestExtensions
    {
        public const int maxBatchSize = 1000;
        public const bool continueSettingDefault = true;
        public const bool returnSettingDefault = true;


        /// <summary>
        /// Converts a collection of type <see cref="Entity"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="CreateRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="CreateRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <returns>A keyed collection of <see cref="ExecuteMultipleRequest"/>s representing the request batches</returns>
        /// <remarks>
        /// Uses default settings of ContinueOnError = True, ReturnResposnes = True
        /// </remarks>
        public static IDictionary<string, ExecuteMultipleRequest> AsCreateBatches(this IEnumerable<Entity> entities, int batchSize)
        {
            return entities.AsCreateBatches(batchSize, continueSettingDefault, returnSettingDefault);
        }

        /// <summary>
        /// Converts a collection of type <see cref="Entity"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="CreateRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="CreateRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <param name="continueOnError">True if each <see cref="ExecuteMultipleRequest"/> should continue processing when an <see cref="OrganizationServiceFault"/> is encountered</param>
        /// <param name="returnResponses">True if an <see cref="ExecuteMultipleResponse"/> should be returned after processing is complete</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        public static IDictionary<string, ExecuteMultipleRequest> AsCreateBatches(this IEnumerable<Entity> entities, int batchSize, bool continueOnError, bool returnResponses)
        {
            var requests = new List<CreateRequest>(entities.Count());

            foreach (Entity entity in entities)
            {
                var request = new CreateRequest()
                {
                    Target = entity
                };

                requests.Add(request);
            }

            return requests.AsBatches(batchSize, continueOnError, returnResponses);
        }

        /// <summary>
        /// Converts a collection of type <see cref="Entity"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="UpdateRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="UpdateRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <returns>A keyed collection of <see cref="ExecuteMultipleRequest"/>s representing the request batches</returns>
        /// <remarks>
        /// Uses default settings of ContinueOnError = True, ReturnResposnes = True
        /// </remarks>
        public static IDictionary<string, ExecuteMultipleRequest> AsUpdateBatches(this IEnumerable<Entity> entities, int batchSize)
        {
            return entities.AsUpdateBatches(batchSize, continueSettingDefault, returnSettingDefault);
        }

        /// <summary>
        /// Converts a collection of type <see cref="Entity"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="UpdateRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="UpdateRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <param name="continueOnError">True if each <see cref="ExecuteMultipleRequest"/> should continue processing when an <see cref="OrganizationServiceFault"/> is encountered</param>
        /// <param name="returnResponses">True if an <see cref="ExecuteMultipleResponse"/> should be returned after processing is complete</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        public static IDictionary<string, ExecuteMultipleRequest> AsUpdateBatches(this IEnumerable<Entity> entities, int batchSize, bool continueOnError, bool returnResponses)
        {
            var requests = new List<UpdateRequest>(entities.Count());

            foreach (Entity entity in entities)
            {
                var request = new UpdateRequest()
                {
                    Target = entity
                };

                requests.Add(request);
            }

            return requests.AsBatches(batchSize, continueOnError, returnResponses);
        }

        /// <summary>
        /// Converts a collection of type <see cref="EntityReference"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="DeleteRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="DeleteRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <returns>A keyed collection of <see cref="ExecuteMultipleRequest"/>s representing the request batches</returns>
        /// <remarks>
        /// Uses default settings of ContinueOnError = True, ReturnResposnes = True
        /// </remarks>
        public static IDictionary<string, ExecuteMultipleRequest> AsDeleteBatches(this IEnumerable<EntityReference> entityReferences, int batchSize)
        {
            return entityReferences.AsDeleteBatches(batchSize, continueSettingDefault, returnSettingDefault);   
        }

        /// <summary>
        /// Converts a collection of type <see cref="Entity"/> to <see cref="ExecuteMultipleRequest"/> batches of <see cref="DeleteRequest"/>
        /// </summary>
        /// <param name="requests">The collection of entities to partition into batches as <see cref="DeleteRequest"/></param>
        /// <param name="batchSize">The size of each batch</param>
        /// <param name="continueOnError">True if each <see cref="ExecuteMultipleRequest"/> should continue processing when an <see cref="OrganizationServiceFault"/> is encountered</param>
        /// <param name="returnResponses">True if an <see cref="ExecuteMultipleResponse"/> should be returned after processing is complete</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        public static IDictionary<string, ExecuteMultipleRequest> AsDeleteBatches(this IEnumerable<EntityReference> entityReferences, int batchSize, bool continueOnError, bool returnResponses)
        {
            var requests = new List<DeleteRequest>(entityReferences.Count());

            foreach (EntityReference entityRef in entityReferences)
            {
                var request = new DeleteRequest()
                {
                    Target = entityRef
                };

                requests.Add(request);
            }

            return requests.AsBatches(batchSize, continueOnError, returnResponses);
        }

        /// <summary>
        /// Converts a collection of type <see cref="OrganizationRequest"/> to <see cref="ExecuteMultipleRequest"/> batches
        /// </summary>
        /// <typeparam name="T">The typeof<see cref="OrganizationRequest"/></typeparam>
        /// <param name="requests">The collection of requests to partition into batches</param>
        /// <param name="batchSize">The size of each batch</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        /// <remarks>
        /// Uses default settings of ContinueOnError = True, ReturnResposnes = True
        /// </remarks>
        public static IDictionary<string, ExecuteMultipleRequest> AsBatches<T>(this IEnumerable<T> requests, int batchSize)
            where T : OrganizationRequest
        {
            return requests.AsBatches(batchSize, continueSettingDefault, returnSettingDefault);
        }

        /// <summary>
        /// Converts a collection of type <see cref="OrganizationRequest"/> to <see cref="ExecuteMultipleRequest"/> batches
        /// </summary>
        /// <typeparam name="T">The typeof<see cref="OrganizationRequest"/></typeparam>
        /// <param name="requests">The collection of requests to partition into batches</param>
        /// <param name="batchSize">The size of each batch</param>
        /// <param name="continueOnError">True if each <see cref="ExecuteMultipleRequest"/> should continue processing when an <see cref="OrganizationServiceFault"/> is encountered</param>
        /// <param name="returnResponses">True if an <see cref="ExecuteMultipleResponse"/> should be returned after processing is complete</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        public static IDictionary<string, ExecuteMultipleRequest> AsBatches<T>(this IEnumerable<T> requests, int batchSize, bool continueOnError, bool returnResponses)
            where T : OrganizationRequest
        {
            return requests.AsBatches(batchSize, new ExecuteMultipleSettings() { ContinueOnError = continueOnError, ReturnResponses = returnResponses });
        }

        /// <summary>
        /// Converts a collection of type <see cref="OrganizationRequest"/> to <see cref="ExecuteMultipleRequest"/> batches
        /// </summary>
        /// <typeparam name="T">The typeof<see cref="OrganizationRequest"/></typeparam>
        /// <param name="requests">The collection of requests to partition into batches</param>
        /// <param name="batchSize">The size of each batch</param>
        /// <param name="batchSettings">The desired settings</param>
        /// <returns>A keyed collection of type <see cref="ExecuteMultipleRequest"/> representing the request batches</returns>
        public static IDictionary<string, ExecuteMultipleRequest> AsBatches<T>(this IEnumerable<T> requests, int batchSize, ExecuteMultipleSettings batchSettings)
            where T : OrganizationRequest
        {
            if (batchSize <= 0)
                throw new ArgumentException("Batch size must be greater than 0", "batchSize");
            if (batchSize > maxBatchSize)
                throw new ArgumentException(String.Format("Batch size of {0} exceeds max batch size of 1000", batchSize), "batchSize");
            if (batchSettings == null)
                throw new ArgumentNullException("batchSettings");

            // Index each request
            var indexedRequests = requests.Select((r, i) => new { Index = i, Value = r });

            // Partition the indexed requests by batch size 
            var partitions = indexedRequests.GroupBy(ir => ir.Index / batchSize);

            // Convert each partition to an ExecuteMultilpleRequest batch
            IEnumerable<ExecuteMultipleRequest> batches = partitions.Select(p => p.Select(ir => ir.Value).AsBatch(batchSettings));

            // Index each batch
            var indexedBatches = batches.Select((b, i) => new { Index = i, Value = b });

            // Return indexed batches as dictionary
            return indexedBatches.ToDictionary(ib => ib.Index.ToString(), ib => ib.Value);
        }

        /// <summary>
        /// Converts a collection of type <see cref="OrganizationRequest"/> to a single <see cref="ExecuteMultipleRequest"/> instance  
        /// </summary>
        /// <typeparam name="T">The typeof<see cref="OrganizationRequest"/></typeparam>
        /// <param name="requests">The collection of requests representing the batch</param>
        /// <param name="batchSettings">The desired settings</param>
        /// <returns>A single <see cref="ExecuteMultipleRequest"/> instance</returns>
        public static ExecuteMultipleRequest AsBatch<T>(this IEnumerable<T> requests, ExecuteMultipleSettings batchSettings)
            where T : OrganizationRequest
        {
            var batch = new OrganizationRequestCollection();
            batch.AddRange(requests);

            return new ExecuteMultipleRequest()
            {
                Requests = batch,
                Settings = batchSettings
            };
        }
    }
}
