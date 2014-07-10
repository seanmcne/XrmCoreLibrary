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
