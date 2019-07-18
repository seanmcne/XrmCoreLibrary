using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Pfe.Xrm.Diagnostics
{
    public class XrmCoreEventSourceEventIds
    {
        public const int Failure = 1;

        public const int ServiceConfigurationInitialized = 400;
        public const int ProxyChannelOpened = 410;

        public const int SecurityTokenRequested = 500;
        public const int SecurityTokenRequestFailure = 501;
        public const int SecurityTokenRefreshRequired = 510;
        public const int SecurityTokenRefreshFailure = 511;

    }
}
