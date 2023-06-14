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
    using System.Text;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;

    /// <summary>
    /// Class that contains configurable options for <see cref="OrganizationServiceProxy"/> requests
    /// </summary>
    public class OrganizationServiceProxyOptions : ServiceProxyOptions
    {
        [Obsolete("When creating the original service client object, pass the proxy types in to enable proxy types", true)]
        public bool ShouldEnableProxyTypes { get; set; }
        public Guid CallerId { get; set; }
    }
    
    /// <summary>
    /// Base class that contains configurable options for <see cref="ServiceProxy{T}"/> requests
    /// </summary>
    public class ServiceProxyOptions
    {                
        public static TimeSpan DefaultProxyTimeout = new TimeSpan(0,2,0);
        
        /// <summary>
        /// Construct a <see cref="ServiceProxyOptions"/> with default Timeout
        /// </summary>
        public ServiceProxyOptions()
        {        
            this.Timeout = ServiceProxyOptions.DefaultProxyTimeout;
        }

        /// <summary>
        /// The timeout for the <see cref="ServiceProxy{T}"/> channel
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
}