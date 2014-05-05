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

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;

    public static class QueryExtensions
    {       
        /// <summary>
        /// Get an representation of the FetchXML query as a XmlDocument
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <returns>An XmlDocument representing the query</returns>
        public static XmlDocument GetXml(this FetchExpression fe)
        {
            var doc = new XmlDocument();

            using (var xmlReader = XmlTextReader.Create(new StringReader(fe.Query)))
            {
                doc.Load(xmlReader);
            }

            return doc;
        }
        
        /// <summary>
        /// Gets the page size specified in the 'count' attribute of a FetchXML query
        /// </summary>
        /// <param name="fe">The expression of a FetchXML query</param>
        /// <returns>The fetch count as an integer value</returns>
        public static int GetPageSize(this FetchExpression fe)
        {
            var defaultPageSize = 5000; //TODO: Should we not include a default page size in case the setting is overriden?
            var doc = fe.GetXml();

            var attributes = doc.DocumentElement.Attributes;
            var countAttribute = (XmlAttribute)attributes.GetNamedItem("count");

            if (countAttribute != null
                && !String.IsNullOrEmpty(countAttribute.Value))
                return Int32.Parse(countAttribute.Value);
            
            return defaultPageSize;
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
            var doc = fe.GetXml();

            var attributes = doc.DocumentElement.Attributes;

            if (!String.IsNullOrEmpty(pagingCookie))
            {
                var cookieAttribute = doc.CreateAttribute("paging-cookie");
                cookieAttribute.Value = pagingCookie;
                attributes.Append(cookieAttribute);
            }

            var pageAttribute = doc.CreateAttribute("page");
            pageAttribute.Value = pageNumber.ToString();
            attributes.Append(pageAttribute);

            var countAttribute = doc.CreateAttribute("count");
            countAttribute.Value = count.ToString();
            attributes.Append(countAttribute);

            var stringBuilder = new StringBuilder(1024);

            using (var xmlWriter = XmlTextWriter.Create(stringBuilder))
            {
                doc.WriteTo(xmlWriter);
            }

            fe.Query = stringBuilder.ToString();
        }
    }
}
