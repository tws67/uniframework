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
 * 0.2.2 - Introduction of provider model.
 */

#region Namespace imports

using System;
using System.Data;
using System.Net;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Interface for SharePoint data providers.
    /// </summary>
    public interface ISharePointDataProvider : IDisposable
    {
        /// <summary>
        /// Gets or sets the network credentials to connect to SharePoint.
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// Gets the friendly name of the provider.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Retrieves the list version for the specified list.
        /// </summary>
        /// <param name="list">List to get the version for.</param>
        /// <returns>Current list version.</returns>
        int GetListVersion(string list);

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="list">List to query.</param>
        /// <returns>Query results.</returns>
        DataTable ExecuteQuery(string list, QueryInfo query);
    }
}
