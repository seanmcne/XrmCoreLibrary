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
    using System.ServiceModel;    
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Xrm.Sdk;

 
    /// <summary>
    /// Represents an <see cref="IOrganizationService"/> operation failure during parallel execution
    /// </summary>
    /// <typeparam name="TRequest">The originating request type</typeparam>
    internal class ParallelOrganizationOperationFailure<TRequest> : ParallelOperationFailure<TRequest, OrganizationServiceFault>
    {
        public ParallelOrganizationOperationFailure(TRequest request, FaultException<OrganizationServiceFault> fault)
            : base(request, fault) { }        
    }
    
    /// <summary>
    /// Represents a service operation failure during parallel execution
    /// </summary>
    /// <typeparam name="TRequest">The originating request type</typeparam>
    /// <typeparam name="TFault">The fault type representing the failure event</typeparam>
    internal class ParallelOperationFailure<TRequest, TFault> : IParallelOperationFailure
        where TFault : BaseServiceFault
    {
        public ParallelOperationFailure(TRequest request, FaultException<TFault> exception)
        {
            this.Request = request;
            this.Exception = exception;
        }
        
        public TRequest Request { get; set; }
        public FaultException<TFault> Exception { get; set; }
    }

    public interface IParallelOperationFailure {  }
}
