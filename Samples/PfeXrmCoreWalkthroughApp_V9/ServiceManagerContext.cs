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

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Tooling.Connector;

namespace Microsoft.Pfe.Xrm.Samples
{
    /// <summary>
    /// A singleton representation XrmServiceManagers targeting the CRM environment
    /// </summary>
    public class ServiceManagerContext
    {
        private static volatile ServiceManagerContext current;
        private static volatile Object syncRoot = new Object();

        private ServiceManagerContext() { }

        public static ServiceManagerContext Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                        {
                            current = new ServiceManagerContext();
                        }
                    }
                }

                return current;
            }
        }

        #region Service Configurations

        private DiscoveryServiceManager discoveryManager;
        private DiscoveryServiceManager DiscoveryManager
        {
            get
            {
                if (discoveryManager == null)
                {
                    lock (syncRoot)
                    {
                        if (discoveryManager == null)
                        {
                            using (var pw = SamplesConfig.GetCrmDecryptedPassword())
                            {
                                discoveryManager = new DiscoveryServiceManager(ServiceLocations.DiscoveryEndpoint, SamplesConfig.CrmUsername, pw.ToUnsecureString());
                            }
                        }
                    }
                }

                return discoveryManager;
            }
        }

        private OrganizationServiceManager organizationManager;
        private OrganizationServiceManager OrganizationManager
        {
            get
            {
                if (organizationManager == null)
                {
                    lock (syncRoot)
                    {
                        if (organizationManager == null)
                        {
                            using (var pw = SamplesConfig.GetCrmDecryptedPassword())
                            {
                                organizationManager = SamplesConfig.CrmShouldDiscover
                                    ? new OrganizationServiceManager(ServiceLocations.OrganizationEndpointViaDiscovery, SamplesConfig.CrmUsername, pw.ToUnsecureString())
                                    : new OrganizationServiceManager(ServiceLocations.OrganizationEndpoint, SamplesConfig.CrmUsername, pw.ToUnsecureString());
                            }
                        }
                    }
                }

                return organizationManager;
            }
        }


        private OrganizationServiceManager organizationManagerClient;
        private OrganizationServiceManager OrganizationManagerClient
        {
            get
            {
                if (organizationManagerClient == null)
                {
                    lock (syncRoot)
                    {
                        if (organizationManagerClient == null)
                        {
                            using (var pw = SamplesConfig.GetCrmDecryptedPassword())
                            {
                                string connectionstring = $"AuthType=OAuth;url={SamplesConfig.CrmOrganizationHost};UserId={SamplesConfig.CrmUsername};password={pw.ToUnsecureString()};RequireNewInstance=true;SkipDiscovery=true;clientid=2ad88395-b77d-4561-9441-d0e40824f9bc;redirecturi=app://5d3e90d6-aa8e-48a8-8f2c-58b45cc67315";// connection string 
                                organizationManagerClient = new OrganizationServiceManager(new CrmServiceClient(connectionstring));
                            }
                        }
                    }
                }

                return organizationManagerClient;
            }
        }

        #endregion

        #region Service Channels

        public ParallelOrganizationServiceProxy ParallelProxy
        {
            get
            {
                return this.OrganizationManagerClient.ParallelProxy;
            }
        } 

        public ManagedTokenDiscoveryServiceProxy GetDiscoveryProxy()
        {
            return this.DiscoveryManager.GetProxy();
        }

        public CrmServiceClient GetOrgServiceProxy()
        {
            return this.OrganizationManager.GetProxy();
        }

        #endregion

        #region Service Locations

        public static class ServiceLocations
        {
            public static Uri DiscoveryEndpoint
            {
                get
                {
                    return XrmServiceUriFactory.CreateDiscoveryServiceUri(SamplesConfig.CrmDiscoveryHost);
                }
            }

            public static Uri OrganizationEndpoint
            {
                get
                {
                    return XrmServiceUriFactory.CreateOrganizationServiceUri(SamplesConfig.CrmOrganizationHost);
                }
            }

            private static Uri organizationEndpointViaDiscovery;
            public static Uri OrganizationEndpointViaDiscovery
            {
                get
                {
                    if (organizationEndpointViaDiscovery == null)
                    {
                        lock (syncRoot)
                        {
                            if (organizationEndpointViaDiscovery == null)
                            {
                                using (var proxy = ServiceManagerContext.Current.DiscoveryManager.GetProxy())
                                {
                                    var request = new RetrieveOrganizationRequest()
                                    {
                                        UniqueName = SamplesConfig.CrmOrganization,
                                        Release = OrganizationRelease.Current
                                    };

                                    var response = proxy.Execute(request);
                                    var endpoint = ((RetrieveOrganizationResponse)response).Detail.Endpoints[EndpointType.OrganizationService];

                                    organizationEndpointViaDiscovery = new Uri(endpoint);
                                }
                            }
                        }
                    }

                    return organizationEndpointViaDiscovery;
                }
            }
        } 

        #endregion
    }
}
