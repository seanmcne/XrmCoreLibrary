using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Pfe.Xrm.Diagnostics
{
    public partial class XrmCoreEventSource
    {
        [Event(XrmCoreEventSourceEventIds.ServiceClientCloneRequested,
            Message = "Cloned Connection SessionTrackingId: {0}",
            Level = EventLevel.Verbose,
            Keywords = Keywords.Authentication,
            Task = Tasks.SecurityToken,
            Opcode = EventOpcode.Info)]
        internal void ServiceClientCloneRequested(string sessionTrackingId)
        {
            this.WriteEvent(XrmCoreEventSourceEventIds.ServiceClientCloneRequested, sessionTrackingId);
        }
    }
}
