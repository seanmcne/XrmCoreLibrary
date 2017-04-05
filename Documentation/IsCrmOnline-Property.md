This property is a helper in identifying whether the current service manager instance targets CRM Online or not based upon the targeted endpoint's authentication provider type.

{code:c#}
/// <summary>
/// True if targeted endpoint's authentication provider type is LiveId or OnlineFederation, otherwise False
/// </summary>
public bool IsCrmOnline
{
    get
    {
        return this.AuthenticationType == AuthenticationProviderType.LiveId
            || this.AuthenticationType == AuthenticationProviderType.OnlineFederation;
    }
}
{code:c#}