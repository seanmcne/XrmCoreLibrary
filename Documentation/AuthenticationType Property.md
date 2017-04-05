This property provides the underlying authentication provider type of the endpoint currently targeted by the service manager instance.

{code:c#}
/// <summary>
/// The <see cref="AuthenticationProviderType"/> of the targeted endpoint
/// </summary>
public AuthenticationProviderType AuthenticationType
{
    get
    {
        return this.ServiceManagement.AuthenticationType;
    }
}
{code:c#}