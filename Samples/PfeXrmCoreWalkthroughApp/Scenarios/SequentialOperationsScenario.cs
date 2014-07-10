using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace Microsoft.Pfe.Xrm.Samples
{
    /// <summary>
    /// Demonstrates submitting requests to Organization.svc sequentially
    /// </summary>
    public class SequentialOperationsScenario : ScenarioBase
    {
        public SequentialOperationsScenario(int requestCount)
            : base("SequentialOperationsScenario", requestCount)
        {
            InitializeOrganizationServiceEndpoint();
        }

        private IServiceManagement<IOrganizationService> ServiceManagement { get; set; }
        private AuthenticationCredentials Credentials { get; set; }

        /// <summary>
        /// Submits a list of account create requests sequentially over a new service channel
        /// </summary>
        protected override Action Step1CreateAccounts
        {
            get
            {
                return () =>
                    {
                        using (var proxy = this.GetProxy())
                        {
                            //Issue account create requests sequentially
                            this.Data.AccountCreateTargets.ForEach(a =>
                                {
                                    try
                                    {
                                        a.Id = proxy.Create(a);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });
                        }
                    };
            }
        }

        /// <summary>
        /// Submits lists of contact and opportunity create requests sequentially over a new service channel
        /// </summary>
        protected override Action Step2CreateRelatedData
        {
            get
            {
                return () =>
                    {
                        using (var proxy = this.GetProxy())
                        {
                            //Issue contact create requests sequentially
                            this.Data.ContactCreateTargets.ForEach(c =>
                                {
                                    try
                                    {
                                        c.Id = proxy.Create(c);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });

                            //Issue opportunity create requests sequentially
                            this.Data.OpportunityCreateTargets.ForEach(o =>
                                {
                                    try
                                    {
                                        o.Id = proxy.Create(o);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });
                        }
                    };

            }
        }

        /// <summary>
        /// Submits lists of account and opportunity update requests sequentially over a new service channel
        /// </summary>
        protected override Action Step3UpdateAccountsAndOpportunities
        {
            get
            {
                return () =>
                    {
                        using (var proxy = this.GetProxy())
                        {
                            //Issue account update requests sequentially
                            this.Data.AccountUpdateTargets.ForEach(a =>
                                {
                                    try
                                    {
                                        proxy.Update(a);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });

                            //Issue opportunity update requests sequentially
                            this.Data.OpportunityUpdateTargets.ForEach(o =>
                                {
                                    try
                                    {
                                        proxy.Update(o);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });
                        }
                    };
            }
        }

        /// <summary>
        /// Submits lists of win and deactivate requests sequentially over a new service channel
        /// </summary>
        protected override Action Step4ExecuteWinAndDeactivateRequests
        {
            get
            {
                return () =>
                    {
                        using (var proxy = this.GetProxy())
                        {
                            //Issue win requests sequentially
                            this.Data.WinRequests.ForEach(w =>
                                {
                                    try
                                    {
                                        proxy.Execute(w);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });

                            //Issue deactivate requests sequentially
                            this.Data.SetStateRequests.ForEach(s =>
                                {
                                    try
                                    {
                                        proxy.Execute(s);
                                    }
                                    catch (FaultException<OrganizationServiceFault> fault)
                                    {
                                        HandleFaultException(fault);
                                    }
                                });
                        }
                    };
            }
        }

        /// <summary>
        /// Submits a list of queries sequentially over a new service channel
        /// </summary>
        protected override Action Step5RetrieveMultipleEntities
        {
            get
            {
                return () =>
                    {
                        using (var proxy = this.GetProxy())
                        {
                            EntityCollection accounts;
                            EntityCollection contacts;
                            EntityCollection opportunities;

                            this.Data.RetrieveMultipleQueries.ForEach(q =>
                                {
                                    var allResults = new EntityCollection();
                                    var firstPage = true;

                                    var qe = q as QueryExpression;

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
                                            var page = proxy.RetrieveMultiple(qe); //retrieve the page

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
                                        allResults = proxy.RetrieveMultiple(q);
                                    }
                                    
                                    switch(allResults.EntityName.ToLower())
                                    {
                                        case "account":
                                            accounts = allResults;
                                            break;
                                        case "contact":
                                            contacts = allResults;
                                            break;
                                        case "opportunity":
                                            opportunities = allResults;
                                            break;
                                    }
                                });
                        }
                    };
            }
        }

        /// <summary>
        /// Submits a list of delete requests sequentially over a new service channel
        /// </summary>
        protected override Action Step6DeleteData
        {
            get
            {
                return () =>
                {
                    using (var proxy = this.GetProxy())
                    {
                        //Issue delete requests sequentially
                        this.Data.DeleteTargets.ForEach(d =>
                            {
                                try
                                {
                                    proxy.Delete(d.LogicalName, d.Id);
                                }
                                catch (FaultException<OrganizationServiceFault> fault)
                                {
                                    HandleFaultException(fault);
                                }
                            });
                    }
                };
            }
        }

        #region Private Methods

        /// <summary>
        /// Setup the service configuration and credentials necessary for communication with Organization.svc endpoint
        /// </summary>
        private void InitializeOrganizationServiceEndpoint()
        {
            this.ServiceManagement = SamplesConfig.CrmShouldDiscover
                ? ServiceConfigurationFactory.CreateManagement<IOrganizationService>(ServiceManagerContext.ServiceLocations.OrganizationEndpointViaDiscovery) //Just use Xrm.Core discovery manager to keep things consistent
                : ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(String.Format("http://{0}/{1}/XRMServices/2011/Organization.svc", SamplesConfig.CrmDiscoveryHost, SamplesConfig.CrmOrganization)));

            //For purposes of this exercise, only handle on-prem auth scenarios (AD or federated/claims).
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
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Allocate a new service channel to organization.svc
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// For purposes of this exercise, only handle on-prem auth scenarios (AD or federated/claims).
        /// </remarks>
        private OrganizationServiceProxy GetProxy()
        {
            if (this.ServiceManagement.AuthenticationType == AuthenticationProviderType.ActiveDirectory)
            {
                return new OrganizationServiceProxy(this.ServiceManagement, this.Credentials.ClientCredentials);
            }

            return new OrganizationServiceProxy(this.ServiceManagement, this.Credentials.SecurityTokenResponse);
        } 

        #endregion
    }
}
