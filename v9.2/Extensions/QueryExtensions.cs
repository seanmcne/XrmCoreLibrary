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
        internal const int DefaultPageSize = 5000;
        
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
            return fe.GetPageSize(DefaultPageSize);
        }

        /// <summary>
        /// Gets the page size specified in the 'count' attribute of a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="defaultPageSize">The default page size to return if not found in the FetchXML query</param>
        /// <returns>The fetch count as an integer value</returns>
        public static int GetPageSize(this FetchExpression fe, int defaultPageSize)
        {
            XElement fetchXml = fe.ToXml();
            
            return fetchXml.GetFetchXmlPageSize(defaultPageSize);
        }

        /// <summary>
        /// Gets the page size specified in the 'count' attribute of a FetchXML query
        /// </summary>
        /// <param name="fetchXml">The XElement representing the fetch query</param>
        /// <param name="defaultPageSize">The default page size to return if not found in the FetchXML query</param>
        /// <returns>The fetch count as an integer value</returns>
        public static int GetFetchXmlPageSize(this XElement fetchXml, int defaultPageSize)
        {
            int pageSize = defaultPageSize;
            XAttribute countAttribute = fetchXml.Attribute("count");

            if (countAttribute != null)
            {
                Int32.TryParse(countAttribute.Value, out pageSize);
            }

            return pageSize;
        }

        /// <summary>
        /// Gets the top count specified in the 'top' attribute of a FetchXML query
        /// </summary>
        /// <param name="fetchXml">The XElement representing the fetch query</param>
        /// <returns>The fetch top count as an integer value, returns '0' if top count not specified</returns>
        public static int GetFetchXmlTopCount(this XElement fetchXml)
        {
            int topCount = 0;
            XAttribute topAttribute = fetchXml.Attribute("top");

            if (topAttribute != null)
            {
                Int32.TryParse(topAttribute.Value, out topCount);
            }

            return topCount;
        }

        /// <summary>
        /// Gets the page number specificed in the 'page' attribute of a FetchXML query
        /// </summary>
        /// <param name="fetchXml">The XElement representing the fetch query</param>
        /// <returns>The fetch page number as an integer value, returns '1' if page not specified</returns>
        public static int GetFetchXmlPageNumber(this XElement fetchXml)
        {
            int pageNumber = 1;
            XAttribute pageAttribute = fetchXml.Attribute("page");

            if (pageAttribute != null)
            {
                Int32.TryParse(pageAttribute.Value, out pageNumber);
            }

            return pageNumber;
        }

        /// <summary>
        /// Gets the paging cookie specified in the 'paging-cookie' attribute of a FetchXML query
        /// </summary>
        /// <param name="fetchXml">The XElement representing the fetch query</param>
        /// <returns>The fetch paging cookie string value, returns emtpty string if not specified</returns>
        public static string GetFetchXmlPageCookie(this XElement fetchXml)
        {
            XAttribute cookieAttribute = fetchXml.Attribute("paging-cookie");

            if (cookieAttribute != null)
            {
                return cookieAttribute.Value;
            }

            return String.Empty;
        }

        /// <summary>
        /// Sets the paging info in a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="pagingCookie">The paging cookie string to set in the FetchXML query</param>
        /// <param name="pageNumber">The page number to set in the FetchXML query</param>
        /// <remarks>
        /// If top count is greater than 0, skips page setup and assumes query should only return TOP(X) results
        /// </remarks>
        public static void SetPage(this FetchExpression fe, string pagingCookie, int pageNumber)
        {
            XElement fetchXml = fe.ToXml();
            int count = fetchXml.GetFetchXmlPageSize(DefaultPageSize);
            
            fetchXml.SetFetchXmlPage(pagingCookie, pageNumber, count);

            fe.Query = fetchXml.ToString();
        }

        /// <summary>
        /// Sets the paging info in a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <param name="pagingCookie">The paging cookie string to set in the FetchXML query</param>
        /// <param name="pageNumber">The page number to set in the FetchXML query</param>
        /// <param name="count">The page size (count) to set in the FetchXML query</param>
        ///  <remarks>
        /// If top count is greater than 0, skips page setup and assumes query should only return TOP(X) results
        /// </remarks>
        public static void SetPage(this FetchExpression fe, string pagingCookie, int pageNumber, int count)
        {
            XElement fetchXml = fe.ToXml();
            fetchXml.SetFetchXmlPage(pagingCookie, pageNumber, count);

            fe.Query = fetchXml.ToString();
        }

        /// <summary>
        /// Sets the paging info in a FetchXML query
        /// </summary>
        /// <param name="fetchXml">The XElement representing the FetchXML query</param>
        /// <param name="pagingCookie">The paging cookie to set in the FetchXML query</param>
        /// <param name="pageNumber">The page number to set in the FetchXML query</param>
        /// <param name="count">The page size (count) to set in the FetchXML query</param>
        /// <remarks>
        /// If top count is greater than 0, skips page setup and assumes query should only return TOP(X) results
        /// </remarks>
        public static void SetFetchXmlPage(this XElement fetchXml, string pagingCookie, int pageNumber, int count)
        {
            if (fetchXml.GetFetchXmlTopCount() <= 0)
            {
                if (!String.IsNullOrWhiteSpace(pagingCookie))
                {
                    fetchXml.SetAttributeValue("paging-cookie", pagingCookie);
                }

                fetchXml.SetAttributeValue("page", pageNumber);
                fetchXml.SetAttributeValue("count", count);
            }
        }


        /// <summary>
        /// Copies the details from one EntityCollection page to another
        /// </summary>
        /// <param name="target">The copy target</param>
        /// <param name="source">The copy source</param>
        public static void CopyFrom(this EntityCollection target, EntityCollection source)
        {
            if (source != null)
            {
                target.EntityName = source.EntityName;
                target.MinActiveRowVersion = source.MinActiveRowVersion;
                target.MoreRecords = source.MoreRecords;
                target.PagingCookie = source.PagingCookie;
                target.TotalRecordCount = source.TotalRecordCount;
                target.TotalRecordCountLimitExceeded = source.TotalRecordCountLimitExceeded;
            }
        }

        /// <summary>
        /// Extracts the page number from the paging cookie returned with the EntityCollection result
        /// </summary>
        /// <param name="results">The current result</param>
        /// <returns>The page attribute of the paging cookie as an integer, otherwise '1' if cookie is null or empty</returns>
        public static int PageNumber(this EntityCollection results)
        {
            int pageNumber = 1;

            if (!String.IsNullOrWhiteSpace(results.PagingCookie))
            {
                XElement cookie = XElement.Parse(results.PagingCookie, LoadOptions.PreserveWhitespace);

                if (cookie != null)
                {
                    XAttribute page = cookie.Attribute("page");

                    if (page != null)
                    {
                        Int32.TryParse(page.Value, out pageNumber);                       
                    }
                }
            }

            return pageNumber;
        }
    }
}
