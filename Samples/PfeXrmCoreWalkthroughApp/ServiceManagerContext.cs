using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Discovery;

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
                                    ? new OrganizationServiceManager(ServiceLocations.OrganizationEndpoint, SamplesConfig.CrmUsername, pw.ToUnsecureString())
                                    : new OrganizationServiceManager(ServiceLocations.OrganizationEndpointViaDiscovery, SamplesConfig.CrmUsername, pw.ToUnsecureString());
                            }
                        }
                    }
                }

                return organizationManager;
            }
        }

        #endregion

        #region Service Channels

        public ParallelOrganizationServiceProxy ParallelProxy
        {
            get
            {
                return this.OrganizationManager.ParallelProxy;
            }
        } 

        public ManagedTokenDiscoveryServiceProxy GetDiscoveryProxy()
        {
            return this.DiscoveryManager.GetProxy();
        }

        public ManagedTokenOrganizationServiceProxy GetOrgServiceProxy()
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
                    return new Uri(String.Format(DiscoveryServiceManager.DiscoveryServiceUriFormat, SamplesConfig.CrmDiscoveryHost));
                }
            }

            public static Uri OrganizationEndpoint
            {
                get
                {
                    return new Uri(String.Format(OrganizationServiceManager.OrganizationServiceInternalUriFormat, SamplesConfig.CrmDiscoveryHost, SamplesConfig.CrmOrganization));
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
