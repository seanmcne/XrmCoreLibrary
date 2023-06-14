namespace Microsoft.Pfe.Xrm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Factory class used to create <see cref="Uri"/> instances targeting Dynamics CRM endpoints
    /// </summary>
    public static class XrmServiceUriFactory
    {
        public const string OrganizationServicePath = @"/XRMServices/2011/Organization.svc";
        public const string OrganizationDataServicePath = @"/XRMServices/2011/OrganizationData.svc";
        public const string OrganizationWebServicePath = @"/XRMServices/2011/Organization.svc/Web";
        public const string OrganizationServiceOnlineNAUriFormat = "https://{0}.api.crm.dynamics.com";
        public const string OrganizationServiceOnlineEMEAUriFormat = "https://{0}.api.crm4.dynamics.com";
        public const string OrganizationServiceOnlineAPACUriFormat = "https://{0}.api.crm5.dynamics.com";

        /// <summary>
        /// Creates an Organization.svc <see cref="Uri"/> instance based on the specified location
        /// </summary>
        /// <param name="location">The scheme, host, port # (if applicable), and organization representing location of Organization.svc</param>
        /// <returns>A new instance of <see cref="Uri"/> for the Organization.svc</returns>
        /// <remarks>
        /// EXAMPLES: https://hostname:5555/organization, https://organization.hostname:5555, https://hostname/organization, https://organization.hostname
        /// The organization name, if detected in the <see cref="Uri"/> path, is preserved in the resulting <see cref="Uri"/>
        /// The Organization.svc endpoint path is not necessary as this will be appended to all <see cref="Uri"/>'s based on the type specified
        /// </remarks>       
        public static Uri CreateOrganizationServiceUri(string location)
        {
            return XrmServiceUriFactory.CreateServiceUri(location, XrmServiceType.Organization);
        }

        /// <summary>
        /// Creates an OrganizationData.svc <see cref="Uri"/> instance based on the specified location
        /// </summary>
        /// <param name="location">The scheme, host, port # (if applicable), and organization representing location of Organization.svc</param>
        /// <returns>A new instance of <see cref="Uri"/> for the OrganizationData.svc</returns>
        /// <remarks>
        /// EXAMPLES: https://hostname:5555/organization, https://organization.hostname:5555, https://hostname/organization, https://organization.hostname
        /// The organization name, if detected in the <see cref="Uri"/> path, is preserved in the resulting <see cref="Uri"/>
        /// The OrganizationData.svc endpoint path is not necessary as this will be appended to all <see cref="Uri"/>'s based on the type specified
        /// </remarks>  
        public static Uri CreateOrganizationDataServiceUri(string location)
        {
            return XrmServiceUriFactory.CreateServiceUri(location, XrmServiceType.OrganizationData);
        }

        /// <summary>
        /// Creates an Organization.svc/Web <see cref="Uri"/> instance based on the specified location
        /// </summary>
        /// <param name="location">The scheme, host, port # (if applicable), and organization representing location of Organization.svc</param>
        /// <returns>A new instance of <see cref="Uri"/> for the Organization.svc/Web</returns>
        /// <remarks>
        /// EXAMPLES: https://hostname:5555/organization, https://organization.hostname:5555, https://hostname/organization, https://organization.hostname
        /// The organization name, if detected in the <see cref="Uri"/> path, is preserved in the resulting <see cref="Uri"/>
        /// The Organization.svc/Web endpoint path is not necessary as this will be appended to all <see cref="Uri"/>'s based on the type specified
        /// </remarks>
        public static Uri CreateOrganizationWebServiceUri(string location)
        {
            return XrmServiceUriFactory.CreateServiceUri(location, XrmServiceType.OrganizationWeb);
        }

        /// <summary>
        /// Creates an Organization.svc <see cref="Uri"/> instance targeting CRM Online for the specified organization/region pair
        /// </summary>
        /// <param name="organizationName">The organization name</param>
        /// <param name="region">The region where the organization is located</param>
        /// <returns>A new instance of <see cref="Uri"/> for the Organization.svc</returns>
        public static Uri CreateOnlineOrganizationServiceUri(string organizationName, CrmOnlineRegion region = CrmOnlineRegion.NA)
        {
            string location = XrmServiceUriFactory.CreateOnlineOrganizationServiceLocation(organizationName, region);

            return XrmServiceUriFactory.CreateOrganizationServiceUri(location);
        }

        /// <summary>
        /// Creates an OrganizationData.svc <see cref="Uri"/> instance targeting CRM Online for the specified organization/region pair
        /// </summary>
        /// <param name="organizationName">The organization name</param>
        /// <param name="region">The region where the organization is located</param>
        /// <returns>A new instance of <see cref="Uri"/> for the OrganizationData.svc</returns>
        public static Uri CreateOnlineOrganizationDataServiceUri(string organizationName, CrmOnlineRegion region = CrmOnlineRegion.NA)
        {
            string location = XrmServiceUriFactory.CreateOnlineOrganizationServiceLocation(organizationName, region);

            return XrmServiceUriFactory.CreateOrganizationDataServiceUri(location);
        }

        /// <summary>
        /// Creates an Organization.svc/Web <see cref="Uri"/> instance targeting CRM Online for the specified organization/region pair
        /// </summary>
        /// <param name="organizationName">The organization name</param>
        /// <param name="region">The region where the organization is located</param>
        /// <returns>A new instance of <see cref="Uri"/> for the Organization.svc/Web</returns>
        public static Uri CreateOnlineOrganizationWebServiceUri(string organizationName, CrmOnlineRegion region = CrmOnlineRegion.NA)
        {
            string location = XrmServiceUriFactory.CreateOnlineOrganizationServiceLocation(organizationName, region);

            return XrmServiceUriFactory.CreateOrganizationWebServiceUri(location);
        }

        /// <summary>
        /// Creates a <see cref="Uri"/> that targets the specified location and XRM service endpoint type 
        /// </summary>
        /// <param name="location">The location of the Dynamics CRM endpoint - This should include scheme, host, port, and organization name (if applicable)</param>
        /// <param name="serviceType">The Dynamics CRM endpoint type</param>
        /// <returns>A <see cref="Uri"/> targeting the specified Dynamics CRM service endpoint</returns>
        /// <remarks>
        /// EXAMPLES: https://hostname:4443/organization, https://hostname/organization, https://organization.hostname, https://hostname
        /// For Organization.svc types, the organization name, if detected in the <see cref="Uri"/> path, is preserved in the resulting <see cref="Uri"/>
        /// The Dynamics CRM service endpoint path is not necessary as this will be appended to all <see cref="Uri"/>'s based on the type specified
        /// </remarks>
        private static Uri CreateServiceUri(string location, XrmServiceType serviceType)
        {
            Uri providedUri = null;

            //Try to create an absolute Uri from the provided string location
            if (Uri.TryCreate(location, UriKind.Absolute, out providedUri))
            {
                //Get the root Uri including scheme + delimeter and authority
                string providedAuthority = providedUri.GetLeftPart(UriPartial.Authority);

                var uriBuilder = new UriBuilder(providedAuthority);
                var pathBuilder = new StringBuilder(128);

                //If we detect something other than XRMServices in the second path segment, assume it's the organization name and preserve it for non-discovery locations. 
                if (providedUri.Segments.Length > 1
                    && !providedUri.Segments[1].Equals(@"XRMServices/", StringComparison.OrdinalIgnoreCase))
                {
                    pathBuilder.Append(providedUri.Segments[0]);
                    pathBuilder.Append(providedUri.Segments[1].TrimEnd('/'));                    
                }

                //Append path to the XRM service endpoint location based on endpoint type specified
                switch (serviceType)
                {
                    case XrmServiceType.Organization:
                        pathBuilder.Append(XrmServiceUriFactory.OrganizationServicePath);
                        break;

                    case XrmServiceType.OrganizationData:
                        pathBuilder.Append(XrmServiceUriFactory.OrganizationDataServicePath);
                        break;

                    case XrmServiceType.OrganizationWeb:
                        pathBuilder.Append(XrmServiceUriFactory.OrganizationWebServicePath);
                        break;
                }

                uriBuilder.Path = pathBuilder.ToString();

                return uriBuilder.Uri;
            }

            return null;
        }

        /// <summary>
        /// Creates a string representing the CRM Online Organization.svc location based on the specified region
        /// </summary>
        /// <param name="organizationName">The organization name being targeted</param>
        /// <param name="region">The applicable CRM Online region</param>
        /// <returns>The formatted Organization.svc location for the organization/region pair</returns>
        private static string CreateOnlineOrganizationServiceLocation(string organizationName, CrmOnlineRegion region)
        {
            switch (region)
            {
                case CrmOnlineRegion.NA:
                default:
                    return String.Format(XrmServiceUriFactory.OrganizationServiceOnlineNAUriFormat, organizationName);
                case CrmOnlineRegion.EMEA:
                    return String.Format(XrmServiceUriFactory.OrganizationServiceOnlineEMEAUriFormat, organizationName);
                case CrmOnlineRegion.APAC:
                    return String.Format(XrmServiceUriFactory.OrganizationServiceOnlineAPACUriFormat, organizationName);
            }
        }
    }
}
