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
        public const int ParallelOperationFailure = 2; 

        public const int ServiceConfigurationInitialized = 400;
        public const int ProxyChannelOpened = 410;

        public const int SecurityTokenRequested = 500;
        public const int SecurityTokenRequestFailure = 501;
        public const int SecurityTokenRefreshRequired = 510;
        public const int SecurityTokenRefreshFailure = 511;

        public const int ServiceClientCloneRequested = 601;

        public const int ParallelCoreOperationStart = 650;
        public const int ParallelCoreOperationCompleted = 660;
        public const int ParallelProxyInit = 670; 


        public const int ThrottleEventGeneric = 700;
        public const int ThrottleRetryAfter = 701;

        public const int GenericInformational = 1000; 
        public const int GenericWarning = 2000;
        public const int GenericFailure = 3000;
    }
}
