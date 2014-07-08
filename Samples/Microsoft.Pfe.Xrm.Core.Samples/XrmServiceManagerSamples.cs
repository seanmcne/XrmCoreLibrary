using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Microsoft.Pfe.Xrm.Samples
{
    public class XrmServiceManagerSamples
    {
        /// <summary>
        /// Demonstrates a basic Active Directory connection to Organization.svc using a username, password, and domain
        /// </summary>
        /// <remarks>
        /// OrganizationServiceManager stores endpoint metadata. Instance can be reused to construct multiple organization service channels (OrganizationServiceProxy)
        /// </remarks>        
        public static void BasicADConnectionToCrm()
        {
            var serverUri = new Uri(String.Format(OrganizationServiceManager.OrganizationServiceInternalSecureUriFormat, "mycrmserver:5555", "myorganization"));
            var manager = new OrganizationServiceManager(serverUri, "username", "password", "mydomain");

            using (var proxy = manager.GetProxy())
            {
                //Do organization requests...
            }
        }

        /// <summary>
        /// Demonstrates a basic claims-based connection to Organization.svc using a username and password
        /// </summary>
        /// <remarks>
        /// OrganizationServiceManager stores endpoint metadata and security token. Instance can be reused to construct multiple organization service channels (OrganizationServiceProxy)
        /// </remarks>   
        public static void BasicClaimsConnectionToCrm()
        {
            var serverUri = new Uri(String.Format(OrganizationServiceManager.OrganizationServiceInternalSecureUriFormat, "mycrmserver:5555", "myorganization"));
            var manager = new OrganizationServiceManager(serverUri, "username", "password");

            using (var proxy = manager.GetProxy())
            {
                //Do organization requests...
            }
        }

        /// <summary>
        /// Demonstrates a claims-based, cross-realm connection to Organization.svc using a username, password, and alternate realm
        /// </summary>
        /// <remarks>
        /// Authentication will be handled by federated realm's identity provider. Issued token will be converted to current realm token that CRM will accept.
        /// OrganizationServiceManager stores endpoint metadata and security token. Instance can be reused to construct multiple organization service channels (OrganizationServiceProxy)
        /// </remarks>   
        public static void BasicCrossRealmConnectionToCrm()
        {
            var serverUri = new Uri(String.Format(OrganizationServiceManager.OrganizationServiceInternalSecureUriFormat, "mycrmserver:5555", "myorganization"));
            var manager = new OrganizationServiceManager(serverUri, "username", "password", homeRealm: new Uri("https://myhomerealm.com"));

            using (var proxy = manager.GetProxy())
            {
                //Do organization requests...
            }
        }

        /// <summary>
        /// Demonstrates a claims-based connection to Organization.svc using a pre-authenticated instance of AuthenticationCredentials
        /// </summary>
        /// <remarks>
        /// OrganizationServiceManager stores endpoint metadata and security token. Instance can be reused to construct multiple organization service channels (OrganizationServiceProxy)
        /// </remarks>  
        public static void BasicPreAuthConnectionToCrm(AuthenticationCredentials preAuthCredentials)
        {
            var serverUri = new Uri(String.Format(OrganizationServiceManager.OrganizationServiceInternalSecureUriFormat, "mycrmserver:5555", "myorganization"));
            var manager = new OrganizationServiceManager(serverUri, preAuthCredentials);

            using (var proxy = manager.GetProxy())
            {
                //Do organization requests...
            }
        }

        /// <summary>
        /// Demonstrates an online-federated connection to Organization.svc using a userprincipalname and password
        /// </summary>
        /// <remarks>
        /// OrganizationServiceManager stores endpoint metadata and security token. Instance can be reused to construct multiple organization service channels (OrganizationServiceProxy)
        /// </remarks>
        public static void BasicConnectionToCrmOnline()
        {
            var serverUri = new Uri(String.Format(OrganizationServiceManager.OrganizationServiceOnlineNAUriFormat, "myorganization"));
            var manager = new OrganizationServiceManager(serverUri, "username@mydomain.onmicrosoft.com", "password");

            using (var proxy = manager.GetProxy())
            {
                //Do organization requests...
            }
        }

        /// <summary>
        /// Demonstrates a basic connection to Discovery.svc using a username and password
        /// </summary>
        /// <remarks>
        /// DiscoveryServiceManager stores endpoint metadata and security token (if necessary). Instance can be reused to 
        /// construct multiple discovery service channels (DiscoveryServiceProxy)
        /// </remarks>
        public static void BasicCrmDiscovery()
        {
            var serverUri = new Uri(String.Format(DiscoveryServiceManager.DiscoveryServiceSecureUriFormat, "mycrmserver:5555"));
            var discoManager = new DiscoveryServiceManager(serverUri, "username", "password");

            using (var discoProxy = discoManager.GetProxy())
            {
                //Do discovery requests...
            }
        }
    }
}
