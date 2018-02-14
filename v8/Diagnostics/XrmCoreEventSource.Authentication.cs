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
		[Event(XrmCoreEventSourceEventIds.SecurityTokenRequested,
			Message = "Security token requested for identity {0}",
			Level = EventLevel.Informational,
			Keywords = Keywords.Authentication,
			Task = Tasks.SecurityToken,
			Opcode = EventOpcode.Info)]
		internal void SecurityTokenRequested(string username)
		{
			this.WriteEvent(XrmCoreEventSourceEventIds.SecurityTokenRequested, username);
		}

		[Event(XrmCoreEventSourceEventIds.SecurityTokenRequestFailure,
			Message = "Failure requesting security token for identity {0}: {1}",
			Level = EventLevel.Error,
			Keywords = Keywords.Authentication,
			Task = Tasks.SecurityToken,
			Opcode = EventOpcode.Info)]
		internal void SecurityTokenRequestFailure(string username, string message)
		{
			this.WriteEvent(XrmCoreEventSourceEventIds.SecurityTokenRequestFailure, username, message);
		}

		[Event(XrmCoreEventSourceEventIds.SecurityTokenRefreshRequired,
			Message = "Refresh required for security token. Expires={0}",
			Level = EventLevel.Informational,
			Keywords = Keywords.Authentication,
			Task = Tasks.SecurityToken,
			Opcode = EventOpcode.Info)]
		internal void SecurityTokenRefreshRequired(string expiration)
		{
			this.WriteEvent(XrmCoreEventSourceEventIds.SecurityTokenRefreshRequired, expiration);
		}

		[Event(XrmCoreEventSourceEventIds.SecurityTokenRefreshFailure,
			Message = "Failure refreshing security token that expires {0}: {1}",
			Level = EventLevel.Error,
			Keywords = Keywords.Authentication,
			Task = Tasks.SecurityToken,
			Opcode = EventOpcode.Info)]
		internal void SecurityTokenRefreshFailure(string expiration, string message)
		{
			this.WriteEvent(XrmCoreEventSourceEventIds.SecurityTokenRefreshFailure, expiration, message);
		}

	}
}
