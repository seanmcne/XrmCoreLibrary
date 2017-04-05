#### DiscoveryServiceManager Class

The DiscoveryServiceManager class purpose is to store endpoint metadata and issued security token (if applicable). An instance of this class should be reused to construct multiple service channels (DiscoveryServiceProxy) as may be necessary for parallelized operations.  In addition to providing a simplified and consistent pattern for connecting to the Discovery.svc, this class also contain a [ParallelProxy](Discovery-ParallelProxy-Property) property which references an instance of [ParallelDiscoveryServiceProxy](ParallelDiscoveryServiceProxy-Class).  Once a new manager class is instantiated, this property is immediately available to begin executing parallelized request operations.

Discovery is not required for connecting to Organization.svc endpoints, but may prove useful for multi-tenant deployments as well as deployments where Organization.svc endpoint location and/or authentication strategy may change.

**Namespace**: Microsoft.Pfe.Xrm
**Assembly**: Microsoft.Pfe.Xrm.Core.dll

##### Properties

* [AuthenticationType](Discovery-AuthenticationType-Property)
* [IsCrmOnline](Discovery-IsCrmOnline-Property)
* [ParallelProxy](Discovery-ParallelProxy-Property)
* [ServiceUri](Discovery-ServiceUri-Property)

##### Methods

* [GetProxy](Discovery-GetProxy-Method)

##### Usage Examples

1. +Basic Connection to CRM Discovery.svc+
Demonstrates a basic connection to Discovery.svc using a username and password

{code:c#}
var serverUri = XrmServiceUriFactory.CreateDiscoveryServiceUri("https://mycrmserver:5555");
var discoManager = new DiscoveryServiceManager(serverUri, "username", "password");

using (var discoProxy = discoManager.GetProxy())
{
    //Do discovery requests...
}
{code:c#}

2. +Basic Connection to CRM Online Discovery.svc+
Demonstrates an online-federated connection to Discovery.svc using a userprincipalname and password

{code:c#}
var discoManager = new DiscoveryServiceManager(XrmServiceUriFactory.DiscoveryServiceOnlineO365NAUri, "username@mydomain.onmicrosoft.com", "password");

using (var discoProxy = discoManager.GetProxy())
{
    //Do discovery requests...
}
{code:c#}

The DiscoveryServiceManager class supports all of the same authentication strategies as the OrganizationServieManager class.  For examples of other possible scenarios, reference the [OrganizationServiceManager Class](OrganizationServiceManager-Class) documentation.