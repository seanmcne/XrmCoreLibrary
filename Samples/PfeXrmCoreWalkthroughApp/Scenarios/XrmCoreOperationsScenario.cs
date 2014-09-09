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
using System.ServiceModel;
using System.Text;

using Microsoft.Xrm.Sdk;

namespace Microsoft.Pfe.Xrm.Samples
{
    public class XrmCoreOperationsScenario : ScenarioBase
    {
        /// <summary>
        /// Demonstrates submitting parallelized requests to Organization.svc using the Xrm.Core ParallelProxy
        /// </summary>
        /// <param name="requestCount"></param>
        /// <remarks>
        /// This class also leverages the Xrm.Core XrmServiceManager classes as implemented in <see cref="ServiceManagerContext"/>
        /// </remarks>
        public XrmCoreOperationsScenario(int requestCount)
            : base("XrmCoreOperationsScenario", requestCount) { }

        #region Scenario Implementations

        /// <summary>
        /// Submits a list of account create requests via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step1CreateAccounts
        {
            get
            {
                return () =>
                    {
                        var targets = new List<Entity>();
                        targets.AddRange(this.Data.AccountCreateTargets);

                        try
                        {
                            //Issue create requests parallelized
                            targets = ServiceManagerContext.Current.ParallelProxy.Create(this.Data.AccountCreateTargets).ToList();
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }

                        this.Data.AccountCreateTargets.Clear();
                        this.Data.AccountCreateTargets.AddRange(targets);
                    };
            }
        }

        /// <summary>
        /// Submits a list of contact and opportunity create requests via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step2CreateRelatedData
        {
            get
            {
                return () =>
                    {
                        var targets = new List<Entity>();
                        targets.AddRange(this.Data.ContactCreateTargets);
                        targets.AddRange(this.Data.OpportunityCreateTargets);

                        try
                        {
                            //Issue create requests parallelized
                            targets = ServiceManagerContext.Current.ParallelProxy.Create(targets).ToList();
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }

                        this.Data.ContactCreateTargets.Clear();
                        this.Data.ContactCreateTargets.AddRange(targets.Where(t => t.LogicalName.Equals("contact", StringComparison.OrdinalIgnoreCase)));
                        this.Data.OpportunityCreateTargets.Clear();
                        this.Data.OpportunityCreateTargets.AddRange(targets.Where(t => t.LogicalName.Equals("opportunity", StringComparison.OrdinalIgnoreCase)));
                    };
            }
        }

        /// <summary>
        /// Submits a list of account and opportunity update requests via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step3UpdateAccountsAndOpportunities
        {
            get
            {
                return () =>
                    {
                        var targets = new List<Entity>();
                        targets.AddRange(this.Data.AccountUpdateTargets);
                        targets.AddRange(this.Data.OpportunityUpdateTargets);

                        try
                        {
                            //Issue update requests parallelized
                            ServiceManagerContext.Current.ParallelProxy.Update(targets);
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }
                    };
            }
        }

        /// <summary>
        /// Submits a list of execute win and deactivate requests via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step4ExecuteWinAndDeactivateRequests
        {
            get
            {
                return () =>
                    {
                        var requests = new List<OrganizationRequest>();
                        requests.AddRange(this.Data.WinRequests);
                        requests.AddRange(this.Data.SetStateRequests);

                        try
                        {
                            //Issue execute requests parallelized
                            ServiceManagerContext.Current.ParallelProxy.Execute(requests);
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }
                    };
            }
        }

        /// <summary>
        /// Submits a list of queries via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step5RetrieveMultipleEntities
        {
            get
            {
                return () =>
                    {
                        var results = ServiceManagerContext.Current.ParallelProxy.RetrieveMultiple(this.Data.RetrieveMultipleQueries, true);

                        EntityCollection accounts = results.Where(r => r.Key.Equals("accounts", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                        EntityCollection contacts = results.Where(r => r.Key.Equals("contacts", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                        EntityCollection opps = results.Where(r => r.Key.Equals("opportunities", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                    };
            }
        }

        /// <summary>
        /// Submits a list of delete requests via Xrm.Core ParallelProxy instance
        /// </summary>
        protected override Action Step6DeleteData
        {
            get
            {
                return () =>
                    {
                        try
                        {
                            //If targeting CRM 2011, deadlocks may occur when parallelizing deletes that require updates to activitypartybase

                            //Deadlock victim example: 
                            //  UPDATE [ActivityPartyBase] 
                            //  SET [IsPartyDeleted] = 1 
                            //  WHERE ([PartyId] = '0a747997-349e-e311-a8cf-00155d011f0a' AND [PartyObjectTypeCode] = 3)

                            //This behavior is not exhibited in CRM 2013

                            //ServiceManagerContext.Current.ParallelProxy.MaxDegreeOfParallelism = 1;
                            ServiceManagerContext.Current.ParallelProxy.Delete(this.Data.DeleteTargets);
                            //ServiceManagerContext.Current.ParallelProxy.MaxDegreeOfParallelism = ParallelServiceProxy.MaxDegreeOfParallelismDefault;
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }
                    };
            }
        } 

        #endregion
    }
}
