#### XrmServiceManager Classes

Provides a simplified and efficient connection pattern to CRM services regardless of the authentication method. Similar to the helper classes provided in the SDK, we took a slightly different approach where we isolated the process of authenticating and establishing client service channels from the need to supply credentials and CRM organization details. You provide the credentials, we get you connected.  

XrmServiceManager classes also contains multiple Uri formatting string constants to assist with constructing the location of the CRM service endpoint.  These are not required, but may be helpful if you know the deployment type that will be targeted (e.g. Active Directory, Internal/External Claims, or Online).  The code samples provided for each class demonstrate how to use these format strings for different authentication scenarios.

There are two primary XrmServiceManager classes that simplify connecting to CRM service endpoints:

* [DiscoveryServiceManager Class](DiscoveryServiceManager-Class)
* [OrganizationServiceManager Class](OrganizationServiceManager-Class)