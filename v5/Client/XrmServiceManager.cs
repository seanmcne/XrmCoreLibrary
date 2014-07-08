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
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;

    using Microsoft.Crm.Services.Utility;
    using Microsoft.Xrm;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Discovery;

    /// <summary>
    /// Wrapper class for managing the service configuration of the Discovery.svc endpoint
    /// </summary>
    public class DiscoveryServiceManager : XrmServiceManager<IDiscoveryService, ManagedTokenDiscoveryServiceProxy>
    {
        public const string DiscoveryServiceUriFormat = "http://{0}/XRMServices/2011/Discovery.svc";
        public const string DiscoveryServiceSecureUriFormat = "https://{0}/XRMServices/2011/Discovery.svc";
        public static Uri DiscoveryServiceOnlineO365NAUri = new Uri("https://disco.crm.dynamics.com/XRMServices/2011/Discovery.svc");
        public static Uri DiscoveryServiceOnlineLiveNAUri = new Uri("https://dev.crm.dynamics.com/XRMServices/2011/Discovery.svc");
        public static Uri DiscoveryServiceOnlineO365EMEAUri = new Uri("https://disco.crm4.dynamics.com/XRMServices/2011/Discovery.svc");
        public static Uri DiscoveryServiceOnlineLiveEMEAUri = new Uri("https://dev.crm4.dynamics.com/XRMServices/2011/Discovery.svc");
        public static Uri DiscoveryServiceOnlineO365APACUri = new Uri("https://disco.crm5.dynamics.com/XRMServices/2011/Discovery.svc");
        public static Uri DiscoveryServiceOnlineLiveAPACUri = new Uri("https://dev.crm5.dynamics.com/XRMServices/2011/Discovery.svc");

        #region Constructor(s)

        /// <summary>
        /// Establishes an IDiscoveryService configuration at Uri location using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        public DiscoveryServiceManager(Uri serviceUri, AuthenticationCredentials credentials)
            : base(serviceUri, credentials) { }

        /// <summary>
        /// Establishes an IDiscoveryService configuration at Uri location using supplied identity details
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="domain">Optional parameter for specifying the domain (when known)</param>
        /// <param name="homeRealm">Optional parameter for specifying the federated home realm location (when known)</param>
        public DiscoveryServiceManager(Uri serviceUri, string username, string password, string domain = null, Uri homeRealm = null)
            : base(serviceUri, username, password, domain, homeRealm) { }

        /// <summary>
        /// Manages an established IDiscoveryService configuration using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        public DiscoveryServiceManager(IServiceManagement<IDiscoveryService> serviceManagement, AuthenticationCredentials credentials)
            : base(serviceManagement, credentials) { }

        /// <summary>
        /// Manages an established IDiscoveryService configuration using DefaultNetworkCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>  
        /// <remarks>
        /// This approach authenticates using DefaultNetworkCredentials (AD) since no credentials are supplied
        /// </remarks>
        public DiscoveryServiceManager(IServiceManagement<IDiscoveryService> serviceManagement)
            : base(serviceManagement) { }

        #endregion

        #region Properties

        private ParallelDiscoveryServiceProxy parallelProxy;
        public ParallelDiscoveryServiceProxy ParallelProxy
        {
            get
            {
                if (this.parallelProxy == null)
                {
                    this.parallelProxy = new ParallelDiscoveryServiceProxy(this);
                }

                return this.parallelProxy;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// DEPRECATED v6.0.1702.1
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        [System.Obsolete]
        public ThreadLocalDiscoveryServiceProxy<TResult> GetThreadLocalProxy<TResult>()
        {
            return this.GetProxy<ThreadLocalDiscoveryServiceProxy<TResult>>();
        } 

        #endregion
    }
    
    /// <summary>
    /// Wrapper class for managing the service configuration of the Organization.svc endpoint
    /// </summary>
    public class OrganizationServiceManager : XrmServiceManager<IOrganizationService, ManagedTokenOrganizationServiceProxy>
    {
        public const string OrganizationServiceInternalUriFormat = "http://{0}/{1}/XRMServices/2011/Organization.svc";
        public const string OrganizationServiceInternalSecureUriFormat = "https://{0}/{1}/XRMServices/2011/Organization.svc";
        public const string OrganizationServiceExtneralUriFormat = "https://{0}.{1}/XRMServices/2011/Organization.svc";
        public const string OrganizationServiceOnlineNAUriFormat = "https://{0}.api.crm.dynamics.com/XRMServices/2011/Organization.svc";
        public const string OrganizationServiceOnlineEMEAUriFormat = "https://{0}.api.crm4.dynamics.com/XRMServices/2011/Organization.svc";
        public const string OrganizationServiceOnlineAPACUriFormat = "https://{0}.api.crm5.dynamics.com/XRMServices/2011/Organization.svc";
                
        public const string OrganizationServiceSilverlightInternalUriFormat = "http://{0}/{1}/XRMServices/2011/Organization.svc/web";
        public const string OrganizationServiceSilverlightInternalSecureUriFormat = "https://{0}/{1}/XRMServices/2011/Organization.svc/web";
        public const string OrganizationServiceSilverlightExternalUriFormat = "https://{0}.{1}/XRMServices/2011/Organization.svc/web";
        public const string OrganizationServiceSilverlightOnlineNAUriFormat = "https://{0}.api.crm.dynamics.com/XRMServices/2011/Organization.svc/web";
        public const string OrganizationServiceSilverlightOnlineEMEAUriFormat = "https://{0}.api.crm4.dynamics.com/XRMServices/2011/Organization.svc/web";
        public const string OrganizationServiceSilverlightOnlineAPACUriFormat = "https://{0}.api.crm5.dynamics.com/XRMServices/2011/Organization.svc/web";

        #region Constructor(s)

        /// <summary>
        /// Establishes an IOrganizationService configuration at Uri location using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        public OrganizationServiceManager(Uri serviceUri, AuthenticationCredentials credentials)
            : base(serviceUri, credentials) { }

        /// <summary>
        /// Establishes an IOrganizationService configuration at Uri location using supplied identity details
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="domain">Optional parameter for specifying the domain (when known)</param>
        /// <param name="homeRealm">Optional parameter for specifying the federated home realm location (when known)</param>
        public OrganizationServiceManager(Uri serviceUri, string username, string password, string domain = null, Uri homeRealm = null)
            : base(serviceUri, username, password, domain, homeRealm) { }

        /// <summary>
        /// Manages an established IOrganizationService configuration using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        public OrganizationServiceManager(IServiceManagement<IOrganizationService> serviceManagement, AuthenticationCredentials credentials)
            : base(serviceManagement, credentials) { }

        /// <summary>
        /// Manages an established IOrganizationService configuration using DefaultNetworkCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>  
        /// <remarks>
        /// This approach authenticates using DefaultNetworkCredentials (AD) since no credentials are supplied
        /// </remarks>
        public OrganizationServiceManager(IServiceManagement<IOrganizationService> serviceManagement)
            : base(serviceManagement) { }

        /// <summary>
        /// Manages an established IOrganizationService configuration using DefaultNetworkCredentials
        /// </summary>
        /// <param name="serviceUri">Uri to use for establishing IServiceManagement</param>  
        /// <remarks>
        /// This approach authenticates using DefaultNetworkCredentials (AD only) since no credentials are supplied
        /// </remarks>
        public OrganizationServiceManager(Uri serviceUri)
            : base(ServiceConfigurationFactory.CreateManagement<IOrganizationService>(serviceUri))
        { }

        #endregion

        #region Fields
        
        private ParallelOrganizationServiceProxy parallelProxy;

        #endregion

        #region Properties

        public ParallelOrganizationServiceProxy ParallelProxy
        {
            get
            {
                if (this.parallelProxy == null)
                {
                    this.parallelProxy = new ParallelOrganizationServiceProxy(this);
                }

                return this.parallelProxy;
            }
        } 

        #endregion

        #region Methods

        /// <summary>
        /// DEPRECATED v6.0.1702.1
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        [System.Obsolete]
        public ThreadLocalOrganizationServiceProxy<TResult> GetThreadLocalProxy<TResult>()
        {
            return this.GetProxy<ThreadLocalOrganizationServiceProxy<TResult>>();
        } 

        #endregion
    }

    /// <summary>
    /// Partially implemented class that contains Uri details for OData endpoint. Still evaluating scenarios where to expand this class.
    /// </summary>
    public class OrganizationDataServiceManager
    {
        public const string OrganizationDataServiceInternalUriFormat = "http://{0}/{1}/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationDataServiceInternalSecureUriFormat = "https://{0}/{1}/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationDataServiceExternalUriFormat = "https://{0}.{1}/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationDataServiceOnlineNAUriFormat = "https://{0}.api.crm.dynamics.com/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationDataServiceOnlineEMEAUriFormat = "https://{0}.api.crm4.dynamics.com/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationDataServiceOnlineAPACUriFormat = "https://{0}.api.crm5.dynamics.com/XRMServices/2011/OrganizationData.svc";
    }

    /// <summary>
    /// Generic class for establishing and managing a ServiceConfiguration for Dynamics CRM 2011 WCF endpoints
    /// </summary>
    /// <typeparam name="TService">Set IDiscoveryService or IOrganizationService type to request respective service proxy instances.</typeparam>
    /// <typeparam name="TProxy">Set a proxy return type to either DiscoveryServiceProxy or OrganizationServiceProxy type based on TService type.</typeparam>
    /// <remarks>
    /// Provides a means to reuse thread-safe service configurations and security tokens to open multiple client service proxies (channels)
    /// </remarks>
    public abstract class XrmServiceManager<TService, TProxy> : XrmServiceManagerBase
        where TService : class
        where TProxy : ServiceProxy<TService>
    {
        #region Constructor(s)

        /// <summary>
        /// Default constructor
        /// </summary>
        private XrmServiceManager()
        {
            throw new NotImplementedException("Default constructor not implemented");
        }

        #region Uri Constructor(s)

        /// <summary>
        /// Establishes a service configuration at Uri location using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        protected XrmServiceManager(Uri serviceUri, AuthenticationCredentials credentials)
        {            
            this.ServiceUri = serviceUri;
            this.ServiceManagement = ServiceConfigurationFactory.CreateManagement<TService>(serviceUri);            

            Authenticate(credentials);
        }

        /// <summary>
        /// Establishes a service configuration of type TService at Uri location using supplied identity details
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="domain">Optional parameter for specifying the domain (when known)</param>
        /// <param name="homeRealm">Optional parameter for specifying the federated home realm location (when known)</param>
        protected XrmServiceManager(Uri serviceUri, string username, string password, string domain = null, Uri homeRealm = null)            
        {
            this.ServiceUri = serviceUri;
            this.ServiceManagement = ServiceConfigurationFactory.CreateManagement<TService>(serviceUri);

            Authenticate(username, password, domain, homeRealm);            
        }

        #endregion

        #region IServiceManagement<TService> Constructor(s)

        /// <summary>
        /// Manages an established service configuration using supplied AuthenticationCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>
        /// <param name="credentials">The auth credentials</param>
        /// <remarks>
        /// AuthenticationCredentials can represent AD ClientCredentials, Claims ClientCredentials, or Cross-realm Claims ClientCredentials
        /// The authCredentials may already contain a SecurityTokenResponse
        /// For cross-realm (federated) scenarios it can contain a HomeRealm Uri by itself, or also include a SecurityTokenResponse from the federated realm
        /// </remarks>
        protected XrmServiceManager(IServiceManagement<TService> serviceManagement, AuthenticationCredentials credentials)
        {                        
            this.ServiceUri = serviceManagement.CurrentServiceEndpoint.Address.Uri;
            this.ServiceManagement = serviceManagement;

            this.Credentials = credentials != null
                ? credentials
                : this.DefaultCredentials;

            RequestSecurityToken();
        }

        /// <summary>
        /// Manages an established service configuration using DefaultNetworkCredentials
        /// </summary>
        /// <param name="serviceManagement">The established service configuration management object</param>  
        /// <remarks>
        /// This approach authenticates using default network credentials (AD) since no credentials are supplied
        /// </remarks>
        protected XrmServiceManager(IServiceManagement<TService> serviceManagement)
            : this(serviceManagement, null) { }

        #endregion

        #endregion

        #region Fields

        private AuthenticationCredentials defaultCredentials;

        #endregion

        #region Properties

        #region Private

        private Uri ServiceUri { get; set; }
        private IServiceManagement<TService> ServiceManagement { get; set; }
        private AuthenticationCredentials Credentials { get; set; }        
        
        private AuthenticationCredentials DefaultCredentials
        {       
            get
            {
                if (this.defaultCredentials == null)
                {
                    this.defaultCredentials = new AuthenticationCredentials()
                    {
                        ClientCredentials = new ClientCredentials()
                        {
                            Windows =
                            {
                                ClientCredential = CredentialCache.DefaultNetworkCredentials
                            }
                        }
                    };
                }

                return this.defaultCredentials;
            }
        }

        /// <summary>
        /// Is TService an IOrganizationService type
        /// </summary>
        private bool IsOrganizationService
        {
            get
            {
                return typeof(TService).Equals(typeof(IOrganizationService));
            }
        }

        /// <summary>
        /// If AuthCredentials has a SecurityTokenResponse, then true. Otherwise, false;
        /// </summary>
        private bool HasToken
        {
            get
            {
                if (this.Credentials != null
                    && this.Credentials.SecurityTokenResponse != null)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// If security token is nearing expiration (15 minutes) or expired, then true. Otherwise, false
        /// </summary>
        private bool TokenExpired
        {
            get
            {                
                if (this.HasToken
                    && this.Credentials.SecurityTokenResponse.Response.Lifetime.Expires <= DateTime.UtcNow.AddMinutes(15))
                    return true;

                return false;
            }
        }

        #endregion

        /// <summary>
        /// The AuthenticationProviderType of the targeted endpoint
        /// </summary>
        protected AuthenticationProviderType AuthenticationType
        {
            get
            {
                return this.ServiceManagement.AuthenticationType;
            }
        }
        
        #endregion

        #region Methods

        #region Private        

        /// <summary>
        /// Handles identity authentication when the authentication provider is known
        /// </summary>
        /// <param name="credentials">The credentials representing the identity to authenticate</param>
        private void Authenticate(AuthenticationCredentials credentials)
        {
            this.Credentials = credentials;

            RequestSecurityToken();
        }

        /// <summary>
        /// Handles identity authentication based on the service endpoint's authentication provider type
        /// </summary>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="domain">Optional parameter for specifying the domain (when known)</param>
        /// <param name="homeRealmUri">Optional parameter for specifying the federated home realm location (when known)</param>
        /// <remarks>
        /// Invokes helper methods for each authentication type that in turn perform the request for a security token from the auth provider
        /// </remarks>
        private void Authenticate(string username, string password, string domain = null, Uri homeRealmUri = null)
        {
            switch (this.AuthenticationType)
            {
                case AuthenticationProviderType.ActiveDirectory:
                    this.AuthenticateCredentials(new ClientCredentials() { Windows = { ClientCredential = new NetworkCredential(username, password, domain) } });
                    return;

                case AuthenticationProviderType.Federation:
                case AuthenticationProviderType.OnlineFederation:
                    this.AuthenticateFederatedRealmCredentials(new ClientCredentials() { UserName = { UserName = username, Password = password } }, homeRealmUri);
                    return;

                case AuthenticationProviderType.LiveId:
                    this.AuthenticateLiveIdCredentials(new ClientCredentials() { UserName = { UserName = username, Password = password } });
                    return;

                default:                                       
                    throw new NotSupportedException(string.Format("{0} authentication type is not supported", this.ServiceManagement.AuthenticationType));
            }
        }

        /// <summary>
        /// Prepare and authenticate an OSDP/Office365 Authentication Credentials using UPN for SSO
        /// </summary>
        /// <param name="userPrincipalName">The user principal name (UPN)</param>
        private void AuthenticateSingleSignOnCredentials(string userPrincipalName)
        {
            this.Credentials = new AuthenticationCredentials()
            {
                UserPrincipalName = userPrincipalName
            };

            RequestSecurityToken();
        }

        /// <summary>
        /// Prepare and authenticate client credentials
        /// </summary>
        /// <param name="clientCredentials">The client credentials</param>
        private void AuthenticateCredentials(ClientCredentials clientCredentials)
        {
            this.Credentials = new AuthenticationCredentials()
            {
                ClientCredentials = clientCredentials
            };

            RequestSecurityToken();
        }

        /// <summary>
        /// Prepare and authenticate client credentials from a federated realm
        /// </summary>
        /// <param name="clientCredentials">The client credentials</param>
        /// <param name="HomeRealmUri">The federated home realm location</param>
        private void AuthenticateFederatedRealmCredentials(ClientCredentials clientCredentials, Uri HomeRealmUri)
        {
            this.Credentials = new AuthenticationCredentials()
            {
                ClientCredentials = clientCredentials,
                HomeRealm = HomeRealmUri
            };

            RequestSecurityToken();
        }

        /// <summary>
        /// Prepare and authenticate client credentials and supporting device credentials for LiveID scenario
        /// </summary>
        /// <param name="clientCredentials">The client credentials (Microsoft Account)</param>
        /// <remarks>Implicitly registers device credentials using deviceidmanager.cs helper</remarks>
        private void AuthenticateLiveIdCredentials(ClientCredentials clientCredentials)
        {
            //Attempt to call .LoadOrRegisterDevice using IssuerEndpoint to load existing and/or persist to file.
            var deviceCredentials = this.ServiceManagement.IssuerEndpoints.ContainsKey("Username")
                ? DeviceIdManager.LoadOrRegisterDevice(this.ServiceManagement.IssuerEndpoints["Username"].IssuerAddress.Uri)
                : DeviceIdManager.LoadOrRegisterDevice();

            AuthenticateLiveIdCredentials(clientCredentials, deviceCredentials);
        }

        /// <summary>
        /// Prepare and authenticate client credentials and supporting device credentials for LiveID scenario
        /// </summary>
        /// <param name="clientCredentials">The client credentials</param>
        /// <param name="deviceCredentials">The supporting device credentials</param>
        private void AuthenticateLiveIdCredentials(ClientCredentials clientCredentials, ClientCredentials deviceCredentials)
        {
            this.Credentials = new AuthenticationCredentials()
            {
                ClientCredentials = clientCredentials,
                SupportingCredentials = new AuthenticationCredentials() { ClientCredentials = deviceCredentials }
            };

            RequestSecurityToken();
        }

        /// <summary>
        /// Request a security token from the identity provider using the supplied credentials
        /// </summary>
        /// <remarks>
        /// Invokes IServiceManagement<TService>.Authenticate() method to perform claims request
        /// Updates the stored credentials with the authentication response that includes the security token
        /// 
        /// Only performs this action if endpoint indicates a non-AD authentication provider scenario
        /// IServiceManagement<TService>.Authenticate() handles multiple scenarios:
        /// 1: Gets security token in current realm for AuthenticationCredentials.ClientCredentials.UserName
        /// 2: Gets security token in current realm by authenticating cross-realm for AuthenticationCredentials.ClientCredentials.UserName using AuthenticationCredentials.HomeRealm
        /// 3: Gets securtiy token in current realm using the cross-realm AuthenticationCredentials.SecurityTokenResponse from AuthenticationCredentials.HomeRealm
        /// </remarks>
        private void RequestSecurityToken()
        {
            //Obtain a security token only for non-AD scenarios
            if (this.AuthenticationType != AuthenticationProviderType.ActiveDirectory
                && this.Credentials != null)
            {
                this.Credentials = this.ServiceManagement.Authenticate(this.Credentials);
            }
        }

        #endregion

        protected T GetProxy<T>()
        {
            // Obtain discovery/organization service proxy based on Authentication Type
            switch (this.ServiceManagement.AuthenticationType)
            {

                case AuthenticationProviderType.ActiveDirectory:
                    // Invokes ManagedTokenDiscoveryServiceProxy or ManagedTokenOrganizationServiceProxy 
                    // (IServiceManagement<TService>, ClientCredentials) constructor.
                    return (T)typeof(T)
                        .GetConstructor(new Type[] 
                        { 
                            typeof(IServiceManagement<TService>), 
                            typeof(ClientCredentials)
                        })
                            .Invoke(new object[] 
                        { 
                            this.ServiceManagement, 
                            this.Credentials.ClientCredentials  
                        });

                case AuthenticationProviderType.Federation:
                case AuthenticationProviderType.OnlineFederation:
                case AuthenticationProviderType.LiveId:
                    //If we don't already have a token or token is expired, get a new token response
                    if (!this.HasToken
                        || this.TokenExpired)
                    {
                        RequestSecurityToken();
                    }

                    // Invokes ManagedTokenOrganizationServiceProxy or ManagedTokenDiscoveryServiceProxy 
                    // (IServiceManagement<TService>, SecurityTokenResponse) constructor.
                    return (T)typeof(T)
                        .GetConstructor(new Type[] 
                        { 
                            typeof(IServiceManagement<TService>), 
                            typeof(SecurityTokenResponse) 
                        })
                            .Invoke(new object[] 
                        { 
                            this.ServiceManagement, 
                            this.Credentials.SecurityTokenResponse 
                        });
                
                default:                    
                    throw new NotSupportedException(string.Format("{0} authentication type is not supported", this.ServiceManagement.AuthenticationType));
            }
        }
        
        /// <summary>
        /// Sets up a new proxy connection of type TProxy using IServiceManagement<TService>
        /// </summary>
        /// <returns>An instance of a managed token TProxy 
        /// i.e. DiscoveryServiceProxy or OrganizationServiceProxy</returns>
        /// <remarks>
        /// The proxy represents a client service channel to a service endpoint. 
        /// Proxy connections should be disposed of properly before they fall out of scope to free up the allocated service channel.
        /// </remarks>
        public TProxy GetProxy()
        {
            return this.GetProxy<TProxy>();
        }

        #endregion
    }

    /// <summary>
    /// Base class for XrmServiceManager
    /// </summary>
    public abstract class XrmServiceManagerBase { }
}