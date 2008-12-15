/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Event arguments to indicate a connection with SharePoint being made.
    /// </summary>
    public class ConnectingEventArgs : EventArgs
    {
        public ConnectingEventArgs(Uri url)
            : base()
        {
            Url = url;
        }

        /// <summary>
        /// Url connecting to.
        /// </summary>
        public Uri Url { get; set; }
    }

    /// <summary>
    /// Event arguments to indicate a connection to SharePoint has been established.
    /// </summary>
    public class ConnectedEventArgs : EventArgs
    {
        public ConnectedEventArgs()
            : base()
        {
            Succeeded = true;
        }

        public ConnectedEventArgs(Exception ex)
            : base()
        {
            Succeeded = false;
            Exception = ex;
        }

        /// <summary>
        /// Indicates whether the connection has succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// In case the connection didn't succeed, this property contains the corresponding connection exception.
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Event arguments to indicate a schema load is being carried out.
    /// </summary>
    public class LoadingSchemaEventArgs : EventArgs
    {
        public LoadingSchemaEventArgs(string list)
            : base()
        {
            List = list;
        }

        /// <summary>
        /// List the schema is being loaded for.
        /// </summary>
        public string List { get; set; }
    }

    /// <summary>
    /// Event arguments to indicate a schema has been loaded.
    /// </summary>
    public class LoadedSchemaEventArgs : EventArgs
    {
        public LoadedSchemaEventArgs()
            : base()
        {
            Succeeded = true;
        }

        public LoadedSchemaEventArgs(Exception ex)
            : base()
        {
            Succeeded = false;
            Exception = ex;
        }

        /// <summary>
        /// Indicates whether the schema load operation has succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// In case the schema load didn't succeed, this property contains the corresponding load failure exception.
        /// </summary>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Event arguments to indicate a schema export is being carried out.
    /// </summary>
    public class ExportingSchemaEventArgs : EventArgs
    {
        public ExportingSchemaEventArgs(string listName, Guid list, int version)
            : base()
        {
            List = listName;
            Identifier = list;
            Version = version;
        }

        /// <summary>
        /// List the schema is being exported for.
        /// </summary>
        public string List { get; set; }

        /// <summary>
        /// Unique identifier of the list.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Version number of the list.
        /// </summary>
        public int Version { get; set; }
    }

    /// <summary>
    /// Event arguments to indicate a schema has been exported.
    /// </summary>
    public class ExportedSchemaEventArgs : EventArgs
    {
        public ExportedSchemaEventArgs(int propertyCount)
            : base()
        {
            Succeeded = true;
            PropertyCount = propertyCount;
        }

        public ExportedSchemaEventArgs(Exception ex)
            : base()
        {
            Succeeded = false;
            Exception = ex;
        }

        /// <summary>
        /// Indicates whether the schema export operation has succeeded.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// In case the schema export didn't succeed, this property contains the corresponding export failure exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Number of properties that have been exported from the schema.
        /// </summary>
        public int PropertyCount { get; set; }
    }
}
