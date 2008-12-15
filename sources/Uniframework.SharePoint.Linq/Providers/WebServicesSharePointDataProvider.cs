/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

/*
 * Version history:
 *
 * 0.2.2 - Provider model.
 */

#region Namespace imports

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Net;
using System.Xml;
using System.Web.Services.Protocols;

#endregion

namespace Uniframework.SharePoint.Linq.Providers
{
    /// <summary>
    /// SharePoint data provider using the SharePoint web services.
    /// </summary>
    internal class WebServicesSharePointDataProvider : ISharePointDataProvider
    {
        #region Private members

        /// <summary>
        /// Web services proxy object to connect to SharePoint.
        /// </summary>
        private Lists _wsProxy;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a data context object using the SharePoint web services to connect to SharePoint.
        /// </summary>
        /// <param name="wsUri">URI to the SharePoint site.</param>
        public WebServicesSharePointDataProvider(Uri wsUri)
        {
            if (wsUri == null)
                throw new ArgumentNullException("wsUri");

            _wsProxy = new Lists();
            _wsProxy.Url = wsUri.AbsoluteUri.TrimEnd('/') + "/_vti_bin/lists.asmx";
            _wsProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the network credentials to connect to the SharePoint web services.
        /// </summary>
        public ICredentials Credentials
        {
            get
            {
                CheckDisposed();
                return _wsProxy.Credentials;
            }
            set
            {
                CheckDisposed();
                _wsProxy.Credentials = value;
            }
        }

        /// <summary>
        /// Gets the friendly name of the provider.
        /// </summary>
        public string Name
        {
            get
            {
                CheckDisposed();
                return "web services";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves the list version for the specified list.
        /// </summary>
        /// <param name="list">List to get the version for.</param>
        /// <returns>Current list version.</returns>
        public int GetListVersion(string list)
        {
            CheckDisposed();
            XmlNode lst = _wsProxy.GetList(list);
            return int.Parse(lst.Attributes["Version"].Value, CultureInfo.InvariantCulture.NumberFormat);
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="list">List to query.</param>
        /// <returns>Query results.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public DataTable ExecuteQuery(string list, QueryInfo query)
        {
            CheckDisposed();

            //
            // Construct the query ready for consumption by the SharePoint web services, including a <Query> root element.
            //
            XmlDocument doc = new XmlDocument();
            XmlNode q = doc.CreateElement("Query");
            if (query.Where != null && query.Where.ChildNodes.Count != 0)
                q.AppendChild(doc.ImportNode(query.Where, true));
            if (query.Order != null)
                q.AppendChild(doc.ImportNode(query.Order, true));
            if (query.Grouping != null)
                q.AppendChild(doc.ImportNode(query.Grouping, true));

            //
            // Retrieve the results of the query via a web service call, using the projection and a row limit (if set).
            //
            uint top = 0;
            if (query.Top != null)
            {
                top = (uint)query.Top.Value;
                if (top == 0) //FIX: don't roundtrip to server if no results are requested.
                    return new DataTable();
            }

            //
            // Store results in a DataSet for easy iteration.
            //
            DataSet results = new DataSet();

            //
            // Get data; server-side result paging could occur.
            //
            XmlNode res;
            string nextPage = null;
            bool page = false;
            do
            {
                //
                // Set query options.
                //
                XmlNode queryOptions = doc.CreateElement("QueryOptions");
                XmlElement inclMand = doc.CreateElement("IncludeMandatoryColumns");
                inclMand.InnerText = "FALSE";
                queryOptions.AppendChild(inclMand);
                XmlElement utcDate = doc.CreateElement("DateInUtc");
                utcDate.InnerText = "TRUE";
                queryOptions.AppendChild(utcDate);

                if (nextPage != null)
                {
                    XmlElement paging = Paging(doc, nextPage);
                    queryOptions.AppendChild(paging);
                }

                //
                // Make web service call.
                //
                try
                {
                    res = _wsProxy.GetListItems(list, null, q, query.Projection, query.Top == null ? null : top.ToString(CultureInfo.InvariantCulture.NumberFormat), queryOptions, null);
                }
                catch (SoapException ex)
                {
                    throw RuntimeErrors.ConnectionExceptionWs(_wsProxy.Url, ex);
                }

                //
                // Merge results.
                //
                DataSet ds = new DataSet();
                ds.Locale = CultureInfo.InvariantCulture;
                ds.ReadXml(new StringReader(res.OuterXml));
                results.Merge(ds);

                //
                // If no results found, break.
                //
                if (ds.Tables["row"] == null)
                    break;

                //
                // Avoid paging when a row limit has been set (Take query operator).
                //
                if (query.Top != null && results.Tables["row"].Rows.Count >= query.Top)
                    break;

                //
                // Check for paging.
                //
                nextPage = res["rs:data"].GetAttribute("ListItemCollectionPositionNext");
                page = !string.IsNullOrEmpty(nextPage);
            } while (page);

            //
            // Make sure results are available. If not, return nothing.
            //
            DataTable src = results.Tables["row"];
            if (src == null)
                return new DataTable();

            //
            // Determine columns. Trim ows_ prefix.
            //
            DataTable tbl = new DataTable();
            tbl.Locale = CultureInfo.InvariantCulture;
            foreach (DataColumn col in src.Columns)
                tbl.Columns.Add(col.ColumnName.Substring("ows_".Length));

            //
            // Store data in DataTable.
            //
            foreach (DataRow srcRow in src.Rows)
            {
                DataRow row = tbl.NewRow();
                foreach (DataColumn col in src.Columns)
                    row[col.ColumnName.Substring("ows_".Length)] = srcRow[col];
                tbl.Rows.Add(row);
            }

            return tbl;
        }

        #endregion

        #region Dispose pattern implementation

        /// <summary>
        /// Track whether the Dispose method has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases all resources used by the data source object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Helper method to support resource disposal.
        /// </summary>
        /// <param name="disposing">Indicates the source of the dispose call.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_wsProxy != null)
                        _wsProxy.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Helper method to check for object disposal.
        /// </summary>
        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(null);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Helper method to create a Paging CAML element.
        /// </summary>
        /// <param name="doc">Root document to create the CAML element from.</param>
        /// <param name="nextPage">Next page for paging.</param>
        /// <returns>Paging CAML element.</returns>
        private static XmlElement Paging(XmlDocument doc, string nextPage)
        {
            XmlElement paging = doc.CreateElement("Paging");
            XmlAttribute licpna = doc.CreateAttribute("ListItemCollectionPositionNext");
            licpna.Value = nextPage;
            paging.Attributes.Append(licpna);
            return paging;
        }

        #endregion
    }
}
