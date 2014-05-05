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
    using System.ServiceModel.Description;
    using System.Text;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Discovery;

    /// <summary>
    /// A DiscoveryServiceProxy that implements IThreadLocalResults<TResponse> for collecting partitioned results in parallel operations
    /// </summary>
    /// <typeparam name="TResponse">The expected response type to collect</typeparam>
    public sealed class ThreadLocalDiscoveryServiceProxy<TResponse> : DiscoveryServiceProxy, IThreadLocalResults<TResponse>
    {
        public ThreadLocalDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement, ClientCredentials credentials)
            : base(serviceManagement, credentials) { }

        public ThreadLocalDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement, SecurityTokenResponse securityTokenResponse)
            : base(serviceManagement, securityTokenResponse) { }

        #region IThreadLocalResults<TResponse> Members

        private IList<TResponse> results;
        public IList<TResponse> Results
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

        #endregion
    }

    /// <summary>
    /// An OrganizationServiceProxy that implements IThreadLocalResults<TResponse> for collecting partitioned results in parallel operations
    /// </summary>
    /// <typeparam name="TResponse">The expected response type to collect</typeparam>
    public sealed class ThreadLocalOrganizationServiceProxy<TResponse> : OrganizationServiceProxy, IThreadLocalResults<TResponse>
    {
        public ThreadLocalOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement, ClientCredentials credentials)
            : base(serviceManagement, credentials) { }

        public ThreadLocalOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement, SecurityTokenResponse securityTokenResponse)
            : base(serviceManagement, securityTokenResponse) { }

        #region IThreadLocalResults<TResponse> Members

        private IList<TResponse> results;
        public IList<TResponse> Results
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

        #endregion
    }
}
