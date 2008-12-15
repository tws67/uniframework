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
 * 0.2.1 - Introduction of SharePointDataContext.
 * 0.2.2 - New entity model.
 *         Provider model.
 * 0.2.4 - Dropped SPSite ctor dependency, now using the provider model instead.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using Uniframework.SharePoint.Linq.Providers;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Data context used for LINQ to SharePoint querying.
    /// </summary>
    public class SharePointDataContext : IDisposable
    {
        #region Private members

        /// <summary>
        /// Data provider used for communication with SharePoint.
        /// </summary>
        private ISharePointDataProvider _dataProvider;

        /// <summary>
        /// Dictionary of SharePointListSource objects for entities. The key represents the entity type, the value the SharePointListSource object.
        /// </summary>
        private Dictionary<Type, object> _lists = new Dictionary<Type, object>();

        /// <summary>
        /// Logger object to report information about the query when executing.
        /// </summary>
        private TextWriter _log;

        /// <summary>
        /// Indicates whether the version of the list on the SharePoint server should be matched against the list version as indicated by the metadata on the list items entity type. (Default: true)
        /// </summary>
        private bool? _checkListVersion;

        /// <summary>
        /// Used to enable/disable deferred loading of entities in Lookup(Multi) field references.
        /// </summary>
        private bool _deferredLoadingEnabled = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a data context object using the SharePoint web services to connect to SharePoint.
        /// </summary>
        /// <param name="wsUri">URI to the SharePoint site.</param>
        public SharePointDataContext(Uri wsUri)
        {
            _dataProvider = new WebServicesSharePointDataProvider(wsUri);
        }

        /// <summary>
        /// Create a data context object using the specified SharePoint data provider.
        /// </summary>
        /// <param name="provider">SharePoint data provider.</param>
        public SharePointDataContext(ISharePointDataProvider provider)
        {
            _dataProvider = provider;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        public ISharePointDataProvider DataProvider
        {
            get
            {
                return _dataProvider;
            }
        }

        /// <summary>
        /// Indicates whether or not deferred loading of entities is enabled for Lookup and LookupMulti fields.
        /// </summary>
        public bool DeferredLoadingEnabled
        {
            get { CheckDisposed(); return _deferredLoadingEnabled; }
            set { CheckDisposed(); _deferredLoadingEnabled = value; }
        }

        /// <summary>
        /// Gets or sets a logger object to report information about the query when executing.
        /// </summary>
        public TextWriter Log
        {
            get { CheckDisposed(); return _log; }
            set { CheckDisposed(); _log = value; }
        }

        /// <summary>
        /// Gets or sets whether the actual SharePoint list version should be matched against the list version as indicated by the metadata on the list entity type.
        /// If null, this setting is ignored and the SharePointList's setting is taken.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CheckList")]
        public bool? CheckListVersion
        {
            get { CheckDisposed(); return _checkListVersion; }
            set { CheckDisposed(); _checkListVersion = value; }
        }

        /// <summary>
        /// Gets or sets the network credentials to connect to the SharePoint web services.
        /// </summary>
        public ICredentials Credentials
        {
            get
            {
                CheckDisposed();
                return _dataProvider.Credentials;
            }
            set
            {
                CheckDisposed();
                _dataProvider.Credentials = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Retrieves a list source object for the entity as specified by <typeparamref name="T">T</typeparamref>.
        /// </summary>
        /// <typeparam name="T">Entity type to get a list source object for.</typeparam>
        /// <returns>List source object for the specified entity type.</returns>
        /// <remarks>Implements a singleton pattern on a per-entity type basis.</remarks>
        public SharePointList<T> GetList<T>()
            where T : class
        {
            CheckDisposed();

            Type t = typeof(T);

            if (!_lists.ContainsKey(t))
            {
                //
                // Create new list and add list to the context. Note: we avoid the public ctor with side-effect because of Microsoft.Design CA1806.
                //
                RegisterList<T>(new SharePointList<T>(this, false));
            }

            return (SharePointList<T>)_lists[t];
        }

        /// <summary>
        /// Retrieves a list source object for the entity as specified by <paramref name="listType">listType</paramref>.
        /// </summary>
        /// <param name="listType">Entity type to get a list source object for.</param>
        /// <returns>List source object for the specified entity type.</returns>
        /// <remarks>Implements a singleton pattern on a per-entity type basis.</remarks>
        internal object GetList(Type listType)
        {
            Debug.Assert(listType != null);

            CheckDisposed();

            object list;
            if (!_lists.ContainsKey(listType))
            {
                //
                // Constructor will add list to the context.
                //
                list = Activator.CreateInstance(
                    typeof(SharePointList<>).MakeGenericType(listType),
                    new object[] { this }
                );
            }
            else
            {
                list = _lists[listType];
                Debug.Assert(list.GetType().IsGenericType && list.GetType().GetGenericTypeDefinition() == typeof(SharePointList<>));
            }

            return list;
        }

        /// <summary>
        /// Registers a list source object for the entity as specified by <typeparamref name="T">T</typeparamref>.
        /// </summary>
        /// <param name="list">List source to register.</param>
        /// <typeparam name="T">Entity type to register the corresponding list source object for.</typeparam>
        internal void RegisterList<T>(SharePointList<T> list) where T : class
        {
            Debug.Assert(list != null);

            CheckDisposed();

            Type t = typeof(T);

            //
            // The context can only hold one SharePointList<T> object for each type T.
            //
            if (_lists.ContainsKey(t))
                throw RuntimeErrors.DuplicateSharePointListObject();

            _lists.Add(t, list);
        }

        /// <summary>
        /// Executes the query specified by the expression parameter.
        /// </summary>
        /// <typeparam name="T">Type for the result objects.</typeparam>
        /// <param name="expression">Expression representing the query.</param>
        /// <returns>Query results.</returns>
        public IEnumerator<T> ExecuteQuery<T>(Expression expression)
        {
            CheckDisposed();

            if (expression == null)
                throw new ArgumentNullException("expression");

            CamlQuery query = CamlQuery.Parse(expression, false);

            return query.Execute<T>().GetEnumerator();
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
                    if (_dataProvider != null)
                        _dataProvider.Dispose();
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
