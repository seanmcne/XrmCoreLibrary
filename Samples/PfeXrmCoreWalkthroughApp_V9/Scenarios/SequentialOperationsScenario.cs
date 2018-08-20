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

                            foreach (var q in this.Data.RetrieveMultipleQueries)
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
                                    allResults = proxy.RetrieveMultiple(q.Value);
                                }

                                switch (allResults.EntityName.ToLower())
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
                            }
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
        /// <returns>A instance of OrganizationServiceProxy representing a service channel</returns>
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
