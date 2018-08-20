// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================
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
    /// Wrapper class for <see cref="DiscoveryServiceProxy"/> to support auto refresh security token
    /// </summary>
    public sealed class ManagedTokenDiscoveryServiceProxy : DiscoveryServiceProxy
    {
        private AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService> _proxyManager;

        public ManagedTokenDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement, ClientCredentials credentials)
            : base(serviceManagement, credentials)
        {
            this._proxyManager = new AutoRefreshSecurityToken<DiscoveryServiceProxy, IDiscoveryService>(this);
        }

        /// <summary>
        /// Constructor with an authenticated security token that will renew using on the supplied credentials
        /// </summary>
        /// <param name="serviceManagement">The service configuration</param>
        /// <param name="securityTokenResponse">The existing security token response from prior authentication request</param>
        /// <param name="credentials">The credentails of the identity being used for authentication</param>
        /// <remarks>
        /// The credentials are required for proxy to perform subsequent authentice requests to obtain new valid tokens
        /// </remarks>
        public ManagedTokenDiscoveryServiceProxy(IServiceManagement<IDiscoveryService> serviceManagement,
            SecurityTokenResponse securityTokenResponse, ClientCredentials credentials)
            : this(serviceManagement, credentials)
        {            
            this.SecurityTokenResponse = securityTokenResponse;
        }

        protected override void AuthenticateCore()
        {
            this._proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            this._proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }
    }

    /// <summary>
    /// Wrapper class for <see cref="OrganizationServiceProxy"/> to support auto refresh security token
    /// </summary>
    public sealed class ManagedTokenOrganizationServiceProxy : OrganizationServiceProxy
    {
        private AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService> _proxyManager;

        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement, ClientCredentials credentials)
            : base(serviceManagement, credentials)
        {
            this._proxyManager = new AutoRefreshSecurityToken<OrganizationServiceProxy, IOrganizationService>(this);
        }

        /// <summary>
        /// Constructor with an authenticated security token that will renew using on the supplied credentials
        /// </summary>
        /// <param name="serviceManagement">The service configuration</param>
        /// <param name="securityTokenResponse">The existing security token response from prior authentication request</param>
        /// <param name="credentials">The credentails of the identity being used for authentication</param>
        /// <remarks>
        /// The credentials are required for proxy to perform subsequent authentice requests to obtain new valid tokens
        /// </remarks>
        public ManagedTokenOrganizationServiceProxy(IServiceManagement<IOrganizationService> serviceManagement,
            SecurityTokenResponse securityTokenResponse, ClientCredentials credentials)
            : this(serviceManagement, credentials)
        {            
            this.SecurityTokenResponse = securityTokenResponse;
        }

        protected override void AuthenticateCore()
        {
            this._proxyManager.PrepareCredentials();
            base.AuthenticateCore();
        }

        protected override void ValidateAuthentication()
        {
            this._proxyManager.RenewTokenIfRequired();
            base.ValidateAuthentication();
        }
    }
}
