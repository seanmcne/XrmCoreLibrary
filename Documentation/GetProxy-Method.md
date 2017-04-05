#### OrganizationServiceManager.GetProxy() Method

This method returns an instance of ManagedTokenOrganizationServiceProxy which represents a WCF service channel to an Organization.svc endpoint and provides the necessary methods for executing organization requests.  Internally, the method efficiently handles constructing the service channel using the existing configuration and authentication details stored on the OrganizationServiceManager instance.

Callers of this method should be sure to dispose of the proxy instance before it falls out of scope or implement IDisposable if the calling type has a member that references the proxy instance.

{code:c#}
public TProxy GetProxy()
{
    return this.GetProxy<TProxy>();
}
protected T GetProxy<T>()
{
    // Obtain discovery/organization service proxy based on Authentication Type
    switch (this.ServiceManagement.AuthenticationType)
    {

        case AuthenticationProviderType.ActiveDirectory:
            // Invokes ManagedTokenDiscoveryServiceProxy or ManagedTokenOrganizationServiceProxy 
            // (IServiceManagement<TService>, ClientCredentials) constructor.
            return (T)typeof(T)
                .GetConstructor(new Type[]() 
                { 
                    typeof(IServiceManagement<TService>), 
                    typeof(ClientCredentials)
                })
                    .Invoke(new object[]() 
                { 
                    this.ServiceManagement, 
                    this.Credentials.ClientCredentials  
                });

        case AuthenticationProviderType.Federation:
        case AuthenticationProviderType.OnlineFederation:
        case AuthenticationProviderType.LiveId:
            //If we don't already have a token or token is expired, refresh the token
            if (!this.HasToken
                || this.TokenExpired)
            {
                RefreshSecurityToken();
            }

            // Invokes ManagedTokenOrganizationServiceProxy or ManagedTokenDiscoveryServiceProxy 
            // (IServiceManagement<TService>, SecurityTokenResponse) constructor.
            return (T)typeof(T)
                .GetConstructor(new Type[]() 
                { 
                    typeof(IServiceManagement<TService>), 
                    typeof(SecurityTokenResponse) 
                })
                    .Invoke(new object[]() 
                { 
                    this.ServiceManagement, 
                    this.AuthenticatedCredentials.SecurityTokenResponse 
                });
                
        default:                    
            throw new NotSupportedException(string.Format("{0} authentication type is not supported", this.ServiceManagement.AuthenticationType));
    }
}
{code:c#}