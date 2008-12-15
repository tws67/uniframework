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
 * 0.2.4 - Moved to separate assembly.
 */

#region Namespace imports

using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

#endregion

namespace Uniframework.SharePoint.Linq.Providers
{
    /// <summary>
    /// SharePoint data provider using the SharePoint Object Model.
    /// </summary>
    /// <remarks>In case of name change, make sure to propagate change to EntityGenerator.cs.</remarks>
    [CLSCompliant(false)]
    public class ObjectModelSharePointDataProvider : ISharePointDataProvider
    {
        /// <summary>
        /// SharePoint site object to connect to SharePoint.
        /// </summary>
        private SPSite _site;

        /// <summary>
        /// Create a data context object using the SharePoint object model to connect to SharePoint.
        /// </summary>
        /// <param name="site">SharePoint site object.</param>
        public ObjectModelSharePointDataProvider(SPSite site)
        {
            if (site == null)
                throw new ArgumentNullException("site");

            _site = site;
        }

        /// <summary>
        /// Not supported for this data provider.
        /// </summary>
        public ICredentials Credentials
        {
            get
            {
                CheckDisposed();
                throw new NotSupportedException(Errors.NoNetworkCredentialsForSom);
            }
            set
            {
                CheckDisposed();
                throw new NotSupportedException(Errors.NoNetworkCredentialsForSom);
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
                return "object model";
            }
        }

        /// <summary>
        /// Retrieves the list version for the specified list.
        /// </summary>
        /// <param name="list">List to get the version for.</param>
        /// <returns>Current list version.</returns>
        public int GetListVersion(string list)
        {
            CheckDisposed();
            return _site.RootWeb.Lists[list].Version;
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="list">List to query.</param>
        /// <param name="query">Query to execute.</param>
        /// <returns>Query results.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1306:SetLocaleForDataTypes")]
        public DataTable ExecuteQuery(string list, QueryInfo query)
        {
            CheckDisposed();

            //
            // Construct the query ready for consumption by the SharePoint Object Model, without <Query> root element.
            //
            StringBuilder queryBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            using (XmlWriter xw = XmlWriter.Create(queryBuilder, settings))
            {
                if (query.Where != null && query.Where.ChildNodes.Count != 0)
                    query.Where.WriteTo(xw);
                if (query.Order != null)
                    query.Order.WriteTo(xw);
                if (query.Grouping != null)
                    query.Grouping.WriteTo(xw);
                xw.Flush();
            }

            //
            // Make the SharePoint SPQuery object.
            //
            SPQuery q = new SPQuery();
            q.Query = queryBuilder.ToString();
            q.IncludeMandatoryColumns = false;
            q.DatesInUtc = true;

            //
            // Include projection fields if a projection clause has been parsed.
            //
            if (query.Projection != null)
                q.ViewFields = query.Projection.InnerXml;

            //
            // In case a row limit (Take) was found, set the limit on the query.
            //
            if (query.Top != null)
            {
                uint top = (uint)query.Top.Value;
                if (top == 0)
                    return new DataTable();
                else
                    q.RowLimit = top;
            }

            //
            // Execute the query via the SPList object.
            //
            SPListItemCollection items;
            SPList lst = _site.RootWeb.Lists[list];
            try
            {
                items = lst.GetItems(q);
            }
            catch (Exception ex)
            {
                throw ConnectionExceptionSp(_site.Url, ex);
            }

            DataTable tbl = new DataTable();
            tbl.Locale = CultureInfo.InvariantCulture;

            Debug.Assert(query.Projection != null);

            //
            // Set projection columns.
            //
            foreach (XmlNode node in query.Projection.ChildNodes)
                tbl.Columns.Add(XmlConvert.DecodeName(node.Attributes["Name"].Value));

            //
            // Retrieve data rows.
            //
            foreach (SPListItem item in items)
            {
                DataRow row = tbl.NewRow();
                foreach (DataColumn col in tbl.Columns)
                {
                    object o = item[col.ColumnName];
                    if (o is DateTime)
                        o = SPUtility.CreateISO8601DateTimeFromSystemDateTime((DateTime)o);
                    row[col] = o;
                }
                tbl.Rows.Add(row);
            }

            return tbl;
        }

        public static Exception ConnectionExceptionSp(string url, Exception innerException)
        {
            return new SharePointConnectionException(String.Format(CultureInfo.InvariantCulture, Errors.ConnectionExceptionSp, url), innerException);
        }

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
                    if (_site != null)
                        _site.Dispose();
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
    }
}
