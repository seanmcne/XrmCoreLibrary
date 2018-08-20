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
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Microsoft.Pfe.Xrm.Samples
{
    /// <summary>
    /// Demonstrates submitting parallelized requests to Organization.svc via .NET Task Parallel Library (TPL)
    /// </summary>
    public class ParallelOperationsScenario : ScenarioBase
    {
        public ParallelOperationsScenario(int requestCount)
            : base("ParallelOperationsScenario", requestCount)
        {
            InitializeOrganizationServiceEndpoint();
        }

        private object syncRoot = new Object();

        private IServiceManagement<IOrganizationService> ServiceManagement { get; set; }
        private AuthenticationCredentials Credentials { get; set; }

        #region Scenario Implementations

        /// <summary>
        /// Submits a list of account create requests via .NET TPL
        /// </summary>
        protected override Action Step1CreateAccounts
        {
            get
            {
                return () =>
                {
                    var targets = new List<Entity>();
                    targets.AddRange(this.Data.AccountCreateTargets);

                    this.Data.AccountCreateTargets.Clear();

                    try
                    {
                        //Issue create requests parallelized
                        Parallel.ForEach<Entity, LocalProxy<Entity>>(targets,
                            () =>
                            {
                                return new LocalProxy<Entity>()
                                {
                                    Channel = this.GetProxy()
                                };
                            },
                            (entity, loopState, index, proxy) =>
                            {
                                entity.Id = proxy.Channel.Create(entity);

                                proxy.Results.Add(entity);

                                return proxy;
                            },
                            (proxy) =>
                            {
                                lock (syncRoot)
                                {
                                    this.Data.AccountCreateTargets.AddRange(proxy.Results);
                                }

                                if (proxy != null)
                                {
                                    proxy.Dispose();
                                    proxy = null;
                                }
                            });
                    }
                    catch (AggregateException ae)
                    {
                        HandleAggregateExceptions(ae);
                    }
                };
            }
        }

        /// <summary>
        /// Submits a list of contact and opportunity create requests via .NET TPL
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

                    this.Data.ContactCreateTargets.Clear();
                    this.Data.OpportunityCreateTargets.Clear();

                    try
                    {
                        //Issue create requests parallelized
                        Parallel.ForEach<Entity, LocalProxy<Entity>>(targets,
                            () =>
                            {
                                return new LocalProxy<Entity>()
                                {
                                    Channel = this.GetProxy()
                                };
                            },
                            (entity, loopState, index, proxy) =>
                            {
                                entity.Id = proxy.Channel.Create(entity);

                                proxy.Results.Add(entity);

                                return proxy;
                            },
                            (proxy) =>
                            {
                                lock (syncRoot)
                                {
                                    this.Data.ContactCreateTargets.AddRange(proxy.Results.Where(c => c.LogicalName.Equals("contact", StringComparison.OrdinalIgnoreCase)));
                                    this.Data.OpportunityCreateTargets.AddRange(proxy.Results.Where(c => c.LogicalName.Equals("opportunity", StringComparison.OrdinalIgnoreCase)));
                                }

                                if (proxy != null)
                                {
                                    proxy.Dispose();
                                    proxy = null;
                                }
                            });
                    }
                    catch (AggregateException ae)
                    {
                        HandleAggregateExceptions(ae);
                    }
                };
            }
        }

        /// <summary>
        /// Submits a list of account and opportunity update requests via .NET TPL
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
                        Parallel.ForEach<Entity, OrganizationServiceProxy>(targets,
                            () =>
                            {
                                return this.GetProxy();
                            },
                            (entity, loopState, index, proxy) =>
                            {
                                proxy.Update(entity);

                                return proxy;
                            },
                            (proxy) =>
                            {
                                if (proxy != null)
                                {
                                    proxy.Dispose();
                                    proxy = null;
                                }
                            });
                    }
                    catch (AggregateException ae)
                    {
                        HandleAggregateExceptions(ae);
                    }
                };
            }
        }

        /// <summary>
        /// Submits a list of execute win and deactivate requests via .NET TPL
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
                        Parallel.ForEach<OrganizationRequest, OrganizationServiceProxy>(requests,
                            () =>
                            {
                                return this.GetProxy();
                            },
                            (entity, loopState, index, proxy) =>
                            {
                                proxy.Execute(entity);

                                return proxy;
                            },
                            (proxy) =>
                            {
                                if (proxy != null)
                                {
                                    proxy.Dispose();
                                    proxy = null;
                                }
                            });
                    }
                    catch (AggregateException ae)
                    {
                        HandleAggregateExceptions(ae);
                    }
                };
            }
        }

        /// <summary>
        /// Submits a list of queries via .NET TPL
        /// </summary>
        protected override Action Step5RetrieveMultipleEntities
        {
            get
            {
                return () =>
                    {
                        var combinedResults = new List<KeyValuePair<string, EntityCollection>>();

                        try
                        {
                            //Bear with the contrived example contianing nested generics so that we can mimic the Dictionary<TKey, TValue> approach we incorporate into PFE Core Lib parallel RetrieveMultiples
                            Parallel.ForEach<KeyValuePair<string, QueryBase>, LocalProxy<KeyValuePair<string, EntityCollection>>>(this.Data.RetrieveMultipleQueries,
                                () =>
                                {
                                    return new LocalProxy<KeyValuePair<string, EntityCollection>>()
                                    {
                                        Channel = this.GetProxy()
                                    };
                                },
                                (q, loopState, index, proxy) =>
                                {
                                    var allResults = new EntityCollection();
                                    var firstPage = true;

                                    var qe = q.Value as QueryExpression;

                                    if (qe != null)
                                    {

                                        if (qe.PageInfo == null)
                                        {
                                            qe.PageInfo = new PagingInfo()
                                            {
                                                PageNumber = 1,
                                                PagingCookie = null,
                                                ReturnTotalRecordCount = false
                                            };
                                        }
                                        else if (qe.PageInfo.PageNumber != 1
                                                || qe.PageInfo.PagingCookie != null)
                                        {
                                            qe.PageInfo.PageNumber = 1;
                                            qe.PageInfo.PagingCookie = null;
                                        }

                                        while (true)
                                        {
                                            var page = proxy.Channel.RetrieveMultiple(qe); //retrieve the page

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

                                            //Setup for next page
                                            if (page.MoreRecords)
                                            {
                                                qe.PageInfo.PageNumber++;
                                                qe.PageInfo.PagingCookie = page.PagingCookie;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //Skip paging requests for FetchExpression and just get the first page of results for sake of brevity
                                        allResults = proxy.Channel.RetrieveMultiple(q.Value);
                                    }

                                    //Add allResults to local results for partition
                                    proxy.Results.Add(new KeyValuePair<string, EntityCollection>(q.Key, allResults));

                                    return proxy;
                                },
                                (proxy) =>
                                {
                                    lock (syncRoot)
                                    {
                                        combinedResults.AddRange(proxy.Results);
                                    }

                                    if (proxy != null)
                                    {
                                        proxy.Dispose();
                                        proxy = null;
                                    }
                                });
                        }
                        catch (AggregateException ae)
                        {
                            HandleAggregateExceptions(ae);
                        }

                        //PFE Core Lib returns the results as a Dictionary<TKey, TValue> using same key associated to the query that generated the results.
                        var combinedDictionary = combinedResults.ToDictionary(r => r.Key, r => r.Value);

                        EntityCollection accounts = combinedDictionary.Where(r => r.Key.Equals("account", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                        EntityCollection contacts = combinedDictionary.Where(r => r.Key.Equals("contact", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                        EntityCollection opps = combinedDictionary.Where(r => r.Key.Equals("opportunity", StringComparison.OrdinalIgnoreCase)).Select(r => r.Value).FirstOrDefault();
                    };
            }
        }

        /// <summary>
        /// Submits a list of delete requests via .NET TPL
        /// </summary>
        protected override Action Step6DeleteData
        {
            get
            {
                return () =>
                {
                    try
                    {
                        var options = new ParallelOptions()
                        {
                            //If targeting CRM 2011, deadlocks may occur when parallelizing deletes that require updates to activitypartybase

                            //Deadlock victim example: 
                            //  UPDATE [ActivityPartyBase] 
                            //  SET [IsPartyDeleted] = 1 
                            //  WHERE ([PartyId] = '0a747997-349e-e311-a8cf-00155d011f0a' AND [PartyObjectTypeCode] = 3)

                            //This behavior is not exhibited in CRM 2013

                            //MaxDegreeOfParallelism = 1
                        };

                        //Issue delete requests parallelized
                        Parallel.ForEach<EntityReference, OrganizationServiceProxy>(this.Data.DeleteTargets,
                            options,
                            () =>
                            {
                                return this.GetProxy();
                            },
                            (entity, loopState, index, proxy) =>
                            {
                                proxy.Delete(entity.LogicalName, entity.Id);

                                return proxy;
                            },
                            (proxy) =>
                            {
                                if (proxy != null)
                                {
                                    proxy.Dispose();
                                    proxy = null;
                                }
                            });
                    }
                    catch (AggregateException ae)
                    {
                        HandleAggregateExceptions(ae);
                    }
                };
            }
        } 
        #endregion

        #region Private Methods

        /// <summary>
        /// Setup the service configuration and credentials necessary for communication with Organization.svc endpoint
        /// </summary>
        private void InitializeOrganizationServiceEndpoint()
        {
            this.ServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(String.Format("{0}/XRMServices/2011/Organization.svc", SamplesConfig.CrmOrganizationHost)));

            //this.ServiceManagement = SamplesConfig.CrmShouldDiscover
            //    ? ServiceConfigurationFactory.CreateManagement<IOrganizationService>(ServiceManagerContext.ServiceLocations.OrganizationEndpointViaDiscovery) //Just use Xrm.Core discovery manager to keep things consistent
            //    : ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(String.Format("{0}/XRMServices/2011/Organization.svc", SamplesConfig.CrmOrganizationHost)));

            if (this.ServiceManagement.AuthenticationType == AuthenticationProviderType.ActiveDirectory)
            {
                using (var pw = SamplesConfig.GetCrmDecryptedPassword())
                {
                    this.Credentials = new AuthenticationCredentials()
                    {
                        ClientCredentials =
                        {
                            Windows =
                            {
                                ClientCredential = new NetworkCredential(SamplesConfig.CrmUsername, pw.ToUnsecureString())
                            }
                        }
                    };
                }
            }
            else
            {
                using (var pw = SamplesConfig.GetCrmDecryptedPassword())
                {
                    this.Credentials = this.ServiceManagement.Authenticate(
                        new AuthenticationCredentials()
                        {
                            ClientCredentials =
                            {
                                UserName =
                                {
                                    UserName = SamplesConfig.CrmUsername,
                                    Password = pw.ToUnsecureString()
                                }
                            }, 
                            UserPrincipalName = this.ServiceManagement.AuthenticationType == AuthenticationProviderType.OnlineFederation
                                ? SamplesConfig.CrmUsername
                                : null
                        });
                }
            }
        }

        /// <summary>
        /// Allocate a new service channel to organization.svc
        /// </summary>
        /// <returns>An instance of OrganizationServiceProxy represnting a service channel</returns>
        private OrganizationServiceProxy GetProxy()
        {
            if (this.ServiceManagement.AuthenticationType == AuthenticationProviderType.ActiveDirectory)
            {
                return new OrganizationServiceProxy(this.ServiceManagement, this.Credentials.ClientCredentials);
            }

            return new OrganizationServiceProxy(this.ServiceManagement, this.Credentials.SecurityTokenResponse);
        } 

        #endregion

        /// <summary>
        /// An object to pass a service channel and collection of results for operations within a parallelized partition
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        private class LocalProxy<TResponse> : IDisposable
        {
            public OrganizationServiceProxy Channel { get; set; }
            private List<TResponse> results;
            public List<TResponse> Results
            {
                get
                {
                    if (this.results == null)
                    {
                        this.results = new List<TResponse>();
                    }

                    return this.results;
                }
            }

            public void Dispose()
            {
                if (this.Channel != null)
                {
                    this.Channel.Dispose();
                }
            }
        }
    }
}
