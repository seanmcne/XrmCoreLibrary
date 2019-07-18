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

    using Microsoft.Xrm;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;

    using Microsoft.Pfe.Xrm.Diagnostics;
    using Microsoft.Xrm.Tooling.Connector;
    using Microsoft.Xrm.Tooling.Connector.Model;
    using System.Reflection;
    using System.Data;
    using System.Diagnostics;


    /// <summary>
    /// Wrapper class for managing the service configuration of the Dynamics CRM Organization.svc endpoint
    /// </summary>
    public class OrganizationServiceManager : XrmServiceManager<IOrganizationService, CrmServiceClient>
    {
        #region Constructor(s)

        /// <summary>
        /// Setup base / Preinitialized XRM client. 
        /// </summary>
        /// <param name="cli"></param>
        public OrganizationServiceManager(CrmServiceClient crmServiceClientObject) 
            : base(crmServiceClientObject) { }

        /// <summary>
        /// Establishes an <see cref="IOrganizationService"/> configuration at Uri location using supplied identity details
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="applicationId">Authorized AppId</param>
        /// <param name="redirectUri">Redirect uri for the appId provided Redirect</param>
        public OrganizationServiceManager(Uri serviceUri, string username, string password, string applicationId = null, Uri redirectUri = null)
            : base(serviceUri, username, password, applicationId, redirectUri) { }

        /// <summary>
        /// Establishes an <see cref="IOrganizationService"/> configuration at Uri location using supplied identity details
        /// </summary>
        /// <param name="serviceUri">The service endpoint location</param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        public OrganizationServiceManager(Uri serviceUri, string username, string password)
            : base(serviceUri, username, password) { }

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
    }

    /// <summary>
    /// Generic class for establishing and managing a service configuration for Dynamics CRM endpoints
    /// </summary>
    /// <typeparam name="TService">Set <see cref="IOrganizationService"/> type to request respective service proxy instances.</typeparam>
    /// <typeparam name="TProxy">Set a proxy return type to <see cref="OrganizationServiceProxy"/> type based on TService type.</typeparam>
    /// <remarks>
    /// Provides a means to reuse thread-safe service configurations and security tokens to open multiple client service proxies (channels)
    /// </remarks>
    public abstract class XrmServiceManager<TService, TProxy> : XrmServiceManagerBase
        where TService : class
        where TProxy : class //ServiceProxy<TService>
    {
        #region Constructor(s)

        /// <summary>
        /// Default constructor
        /// </summary>
        private XrmServiceManager()
        {
            throw new NotImplementedException("Default constructor not implemented");
        }

        protected XrmServiceManager(CrmServiceClient serviceClient)
        {
            if (serviceClient != null && serviceClient.IsReady == true)
            {
                this.ServiceClient = serviceClient;
            }
            else
            {
                XrmCoreEventSource.Log.LogFailureLine($"The provided serviceClient is 'Not Ready' or null.  The reason provided by CrmServiceClient is: {ServiceClient.LastCrmError}");

                throw new Exception($"The provided serviceClient is 'Not Ready' or null.  The reason provided by CrmServiceClient is: {ServiceClient.LastCrmError}", ServiceClient.LastCrmException);
            }
        }


        #region Uri Constructor(s)

        /// <summary>
        /// Establishes a service configuration of type TService at <see cref="Uri"/> location using supplied identity details
        /// </summary>
        /// <param name="environmentUri">The service endpoint location (ie: https://environment.crm.dynamics.com/) </param>
        /// <param name="username">The username of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        /// <param name="applicationId">The password of the identity to authenticate</param>
        /// <param name="password">The password of the identity to authenticate</param>
        protected XrmServiceManager(Uri environmentUri, string username, string password, string applicationId= null, Uri redirectUri = null)            
        {
            if(applicationId == null || redirectUri == null)
            {
                applicationId = "2ad88395-b77d-4561-9441-d0e40824f9bc";
                redirectUri = new Uri("app://5d3e90d6-aa8e-48a8-8f2c-58b45cc67315"); 
            }

            var connectionstring = $"Username={username};Password={password};Url={environmentUri.AbsoluteUri};AuthType=OAuth;ClientId={applicationId};redirecturi={redirectUri};SkipDiscovery=True";
            CrmServiceClient svcClient;
            try
            {
                this.ServiceClient = new CrmServiceClient(connectionstring);
            }
            catch (InvalidOperationException e)
            {
                //retry one more time in the event of an invalid operation exception
                this.ServiceClient = new CrmServiceClient(connectionstring);
            }
        }

        #endregion

        #endregion

        #region Properties

        #region Private

        /// <summary>
        /// Copy of the CrmService Client that has been initialized
        /// </summary>
        private CrmServiceClient ServiceClient { get; set; }
        
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

        #endregion

        /// <summary>
        /// Current endpoint address
        /// </summary>
        public Uri ServiceUri {
            get {
                return this.ServiceClient.CrmConnectOrgUriActual;
            }
        }

        /// <summary>
        /// The <see cref="AuthenticationProviderType"/> of the targeted endpoint
        /// </summary>
        public Microsoft.Xrm.Tooling.Connector.AuthenticationType AuthenticationType
        {
            get
            {
                return this.ServiceClient.ActiveAuthenticationType;
            }
        }

        /// <summary>
        /// True if targeted endpoint's authentication provider type is LiveId or OnlineFederation, otherwise False
        /// </summary>
        public bool IsCrmOnline
        {
            get
            {
                return this.AuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.ExternalTokenManagement
                    || this.AuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.Certificate
                    || this.AuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth;
            }
        }

        public bool IsCrmServiceClient
        {
            get { return ServiceClient != null; }
        }
        public FileVersionInfo AdalVersion
        {
            get;
            private set;
        }

        public bool IsCrmServiceClientReady
        {
            get
            { if (ServiceClient != null) return ServiceClient.IsReady; else return false; }
        }
        #endregion

        #region Methods

        protected T GetProxy<T>()
        {
            if (this.ServiceClient.ActiveAuthenticationType != Microsoft.Xrm.Tooling.Connector.AuthenticationType.ExternalTokenManagement)
            {
                if (AdalVersion == null)
                {
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo("Microsoft.IdentityModel.Clients.ActiveDirectory.dll");
                    AdalVersion = fvi;
                }

                if (AdalVersion != null
                    && (AdalVersion.FileMajorPart != 2))
                {
                    XrmCoreEventSource.Log.LogError($"ADAL Version {AdalVersion.FileVersion} is not matching the expected version of 2.x. Certain functions may not work as expected if you're not using the AuthOverrideHook.");
                }
                else if (AdalVersion != null
                        && (AdalVersion.FileMajorPart == 2 && (AdalVersion.FileMinorPart != 22 || AdalVersion.FileBuildPart != 302111727)))
                {
                    XrmCoreEventSource.Log.LogInformation($"ADAL Version {AdalVersion.FileVersion} is not matching the expected version of 2.x or specifically, 2.22.302111727.");
                }
            }
           
            if ( this.IsCrmServiceClient )
            {
                if(this.ServiceClient.IsReady)
                {
                    if(this.ServiceClient.ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.OAuth
                        || this.ServiceClient.ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.Certificate 
                        || this.ServiceClient.ActiveAuthenticationType == Microsoft.Xrm.Tooling.Connector.AuthenticationType.ExternalTokenManagement)
                    {
                        var svcClientClone = ((T)typeof(T).GetMethod("Clone", new Type[0]).Invoke(ServiceClient, null));

                        PropertyInfo propSessionId = svcClientClone.GetType().GetProperty("SessionTrackingId", BindingFlags.Public | BindingFlags.Instance);

                        if (propSessionId != null && propSessionId.CanWrite)
                        {
                            var sessionTrackingGuid = Guid.NewGuid();
                            XrmCoreEventSource.Log.ServiceClientCloneRequested(sessionTrackingGuid.ToString());
                            propSessionId.SetValue(svcClientClone, sessionTrackingGuid, null);
                        }
                        else
                        {
                            XrmCoreEventSource.Log.ServiceClientCloneRequested("null");
                        }
                        return svcClientClone;
                    }
                    else
                    {
                        XrmCoreEventSource.Log.LogFailureLine($"You must have successfully created a connection to CRM using OAuth or Certificate Auth before it can be cloned.");
                        throw new Exception("You must have successfully created a connection to CRM using OAuth or Certificate Auth before it can be cloned.");
                    }
                }
            }
            XrmCoreEventSource.Log.LogFailureLine($"CrmServiceClient is 'Not Ready'.  The only reason we can find is: {ServiceClient.LastCrmError}");
            throw new Exception($"CrmServiceClient is 'Not Ready'.  The only reason we can find is: {ServiceClient.LastCrmError}", ServiceClient.LastCrmException);
        }
        
        /// <summary>
        /// Sets up a new proxy connection of type TProxy using <see cref="TService"/>
        /// </summary>
        /// <returns>An instance of a managed token TProxy 
        /// i.e. <see cref="CrmServiceClient"/></returns>
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