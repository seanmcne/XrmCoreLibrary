using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;

namespace Microsoft.Pfe.Xrm.Diagnostics
{
    internal static class DiagnosticExtensionMethods
    {
        private const string InnerPrefix = "Inner ";
        private const string StackTraceHeader = "Stack Trace: ";

        /// <summary>
        /// Appends exception message and stack trace using StringBuilder, recurses inner exceptions
        /// </summary>
        /// <param name="ex">The current exception</param>
        /// <param name="builder">The current message builder</param>
        /// <param name="isInnerException">True = current exception is an inner exception</param>
        /// <returns>The provided StringBuilder instance. If arg is null, a new instance is created.</returns>
        public static StringBuilder ToErrorMessageString(this Exception ex, StringBuilder builder = null, bool isInnerException = false)
        {
            if (builder == null)
                builder = new StringBuilder();

            if (isInnerException)
            {
                builder.Append(InnerPrefix);
            }

            // Handle aggregate exception scenario
            AggregateException ae = ex as AggregateException;
            if (ae != null)
            {
                builder.AppendFormat("AggregateException: {0}", ae.Message);

                ae.Handle(e =>
                {
                    // Append message for each exception in the aggregateexception					
                    e.ToErrorMessageString(builder);

                    // Assume handled here...
                    return true;
                });

                return builder;
            }

            // Build using fault exception details
            FaultException<OrganizationServiceFault> orgFault = ex as FaultException<OrganizationServiceFault>;
            if (orgFault != null)
            {
                builder.AppendFormat("FaultException<OrganizationServiceFault>: {0}", ex.Message).AppendLine();

                return orgFault.Detail.ToFaultMessageString(builder);
            }

            // Build for all other exception types
            builder.AppendFormat("{0}: {1}", ex.GetType().Name, ex.Message).AppendLine();
            builder.AppendLine(StackTraceHeader);
            builder.AppendLine(ex.StackTrace);

            // Recurse building inner exception message string
            if (ex.InnerException != null)
            {
                ex.InnerException.ToErrorMessageString(builder, true);
            }

            return builder;
        }

        /// <summary>
        /// Appends fault details using StringBuilder, recurses inner faults
        /// </summary>
        /// <param name="fault">The current fault encountered</param>
        /// <param name="builder">The current message builder</param>
        /// <param name="isInnerFault">True = current fault is an inner fault</param>
        /// <returns>The provided StringBuilder instance. If arg is null, a new instance is created.</returns>
        public static StringBuilder ToFaultMessageString(this OrganizationServiceFault fault, StringBuilder builder = null, bool isInnerFault = false)
        {
            if (builder == null)
                builder = new StringBuilder();

            if (isInnerFault)
            {
                builder.Append(InnerPrefix);
            }

            // Append current fault
            builder.AppendFormat("OrganizationServiceFault: {0}", fault.Message).AppendLine();
            builder.AppendLine(StackTraceHeader);
            builder.AppendLine(fault.TraceText);

            // Recurse building the inner fault message string
            if (fault.InnerFault != null)
            {
                fault.InnerFault.ToFaultMessageString(builder, true);
            }

            return builder;
        }
    }
}

