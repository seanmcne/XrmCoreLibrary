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
    using System.IO;
    using System.Linq;    
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public static class QueryExtensions
    {
        /// <summary>
        /// Parses a FetchXML query as an instance of System.Xml.Linq.XElement
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <returns>A System.Xml.Linq.XElement representing the query</returns>
        public static XElement ToXml(this FetchExpression fe)
        {
            return XElement.Parse(fe.Query, LoadOptions.PreserveWhitespace);
        }

        /// <summary>
        /// Gets the page size specified in the 'count' attribute of a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <returns>The fetch count as an integer value</returns>
        /// <remarks>
        /// Returns default page size of 5000
        /// </remarks>
        public static int GetPageSize(this FetchExpression fe)
        {
            return fe.GetPageSize(5000);
        }

        /// <summary>
        /// Gets the page size specified in the 'count' attribute of a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="defaultPageSize">The default page size to return if not found in the FetchXML query</param>
        /// <returns>The fetch count as an integer value</returns>
        public static int GetPageSize(this FetchExpression fe, int defaultPageSize)
        {
            var pageSize = defaultPageSize;
            var fetchXml = fe.ToXml();

            var countAttribute = fetchXml.Attribute("count");

            if (countAttribute != null)
            {
                Int32.TryParse(countAttribute.Value, out pageSize);
            }

            return pageSize;
        }

        /// <summary>
        /// Sets the paging info in a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="pagingCookie">The paging cookie string to set in the FetchXML query</param>
        /// <param name="pageNumber">The page number to set in the FetchXML query</param>
        /// <remarks>
        /// Sets page size (count) based on what is defined in current FetchXML query. Defaults to 5,000.
        /// </remarks>
        public static void SetPage(this FetchExpression fe, string pagingCookie, int pageNumber)
        {
            fe.SetPage(pagingCookie, pageNumber, fe.GetPageSize());
        }

        /// <summary>
        /// Sets the paging info in a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="pagingCookie">The paging cookie string to set in the FetchXML query</param>
        /// <param name="pageNumber">The page number to set in the FetchXML query</param>
        /// <param name="count">The page size (count) to set in the FetchXML query</param>
        public static void SetPage(this FetchExpression fe, string pagingCookie, int pageNumber, int count)
        {
            var fetchXml = fe.ToXml();

            if (!String.IsNullOrWhiteSpace(pagingCookie))
            {
                fetchXml.SetAttributeValue("paging-cookie", pagingCookie);                                
            }

            fetchXml.SetAttributeValue("page", pageNumber);
            fetchXml.SetAttributeValue("count", count);

            fe.Query = fetchXml.ToString();
        }
    }
}
