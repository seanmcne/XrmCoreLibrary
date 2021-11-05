using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Pfe.Xrm.Diagnostics
{
	public partial class XrmCoreEventSource : EventSource
	{
        [Event(XrmCoreEventSourceEventIds.ThrottleEventGeneric,
           Message = "ThrottleEvent: {0}",
           Level = EventLevel.Warning,
           Keywords = Keywords.Parallel,
           Task = Tasks.ExceptionHandler,
           Opcode = EventOpcode.Info)]
        internal void ThrottleEventGeneric(string throttleDetails)
        {
            this.WriteEvent(XrmCoreEventSourceEventIds.ThrottleEventGeneric, throttleDetails);
        }

        [Event(XrmCoreEventSourceEventIds.ParallelCoreOperationCompleted,
           Message = "{0} ParallelCoreOperationCompleted",
           Level = EventLevel.Verbose,
           Keywords = Keywords.Parallel,
           Task = Tasks.OrganizationRequest,
           Opcode = EventOpcode.Info)]
        internal void ParallelCoreOperationCompleted(string details)
        {
            this.WriteEvent(XrmCoreEventSourceEventIds.ParallelCoreOperationCompleted, details);
        }

        [Event(XrmCoreEventSourceEventIds.ParallelCoreOperationStart,
           Message = "{0} ParallelCoreOperationStart",
           Level = EventLevel.Verbose,
           Keywords = Keywords.Parallel,
           Task = Tasks.OrganizationRequest,
           Opcode = EventOpcode.Info)]
        internal void ParallelCoreOperationStart(string details)
        {
            this.WriteEvent(XrmCoreEventSourceEventIds.ParallelCoreOperationStart, details);
        }
    }
}
