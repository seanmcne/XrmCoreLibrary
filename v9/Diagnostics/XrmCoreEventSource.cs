using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Pfe.Xrm.Diagnostics
{
	[EventSource(Name = "Microsoft-Dynamics365-XrmCore")]
	public partial class XrmCoreEventSource : EventSource
	{
		private XrmCoreEventSource() :base(true) { }
		private static readonly Lazy<XrmCoreEventSource> _instance = new Lazy<XrmCoreEventSource>(() => new XrmCoreEventSource(), true);
		public static XrmCoreEventSource Log { get { return _instance.Value; } }

		public class Keywords
		{
			public const EventKeywords Authentication = (EventKeywords)1;
			public const EventKeywords Service = (EventKeywords)2;
			public const EventKeywords Parallel = (EventKeywords)4;
			public const EventKeywords Query = (EventKeywords)8;
			public const EventKeywords Cryptography = (EventKeywords)16;
		}

		public class Tasks
		{
			public const EventTask Configuration = (EventTask)1;
			public const EventTask Channel = (EventTask)2;
			public const EventTask SecurityToken = (EventTask)3;
			public const EventTask DiscoveryRequest = (EventTask)4;
			public const EventTask OrganizationRequest = (EventTask)5;
			public const EventTask Create = (EventTask)6;
			public const EventTask Update = (EventTask)7;
			public const EventTask Delete = (EventTask)8;
			public const EventTask Retrieve = (EventTask)9;
			public const EventTask RetrieveMultiple = (EventTask)10;
			public const EventTask Associate = (EventTask)11;
			public const EventTask Disassociate = (EventTask)12;
			public const EventTask Execute = (EventTask)13;
			public const EventTask ExceptionHandler = (EventTask)14;
        }

		[Event(XrmCoreEventSourceEventIds.Failure, 
			Message = "Crital Failure: {0}", 
			Level = EventLevel.Critical)]
		internal void LogFailureLine(string message)
		{
			WriteEvent(XrmCoreEventSourceEventIds.Failure, message);
		}

        [Event(XrmCoreEventSourceEventIds.GenericFailure,
            Message = "Request Failure encountered: {0}",
            Level = EventLevel.Error)]
        public void LogError(string Message)
        {
            WriteEvent(XrmCoreEventSourceEventIds.GenericFailure, Message);
        }

        [Event(XrmCoreEventSourceEventIds.GenericWarning,
            Message = "{0}",
            Level = EventLevel.Warning,
            Opcode = EventOpcode.Info)]
        public void LogWarning(string Message)
        {
            WriteEvent(XrmCoreEventSourceEventIds.GenericWarning, Message);
        }

        [Event(XrmCoreEventSourceEventIds.GenericInformational,
            Message = "{0}",
            Level = EventLevel.Informational, 
            Opcode = EventOpcode.Info)]
        public void LogInformation(string Message)
        {
            WriteEvent(XrmCoreEventSourceEventIds.GenericInformational, Message);
        }
    }
}
