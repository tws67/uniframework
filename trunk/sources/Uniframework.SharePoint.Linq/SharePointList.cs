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
 * 0.2.1 - Introduction of SharePointList<T>.
 * 0.2.2 - New entity model.
 * 0.2.3 - Orcas Beta 2 changes.
 *         Lazy loading improvement for multiple ids.
 */

#region Namespace imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Data source object for querying of a SharePoint list as specified by <typeparamref name="T">T</typeparamref>.
    /// </summary>
    /// <typeparam name="T">Entity type for the underlying SharePoint list.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class SharePointList<T> : IOrderedQueryable<T>
        where T : class
    {
        #region Private members

        /// <summary>
        /// Data context object used to connect to SharePoint.
        /// </summary>
        private SharePointDataContext _context;

        /// <summary>
        /// Cache of entities retrieved using a *ById method.
        /// </summary>
        private Dictionary<int, T> cache = new Dictionary<int, T>();

        #endregion

        #region Constructors

        /// <summary>
        /// Create a list source object for querying of a SharePoint list.
        /// </summary>
        /// <param name="context">Data context object used to connect to SharePoint.</param>
        public SharePointList(SharePointDataContext context) : this(context, true)
        {
        }

        /// <summary>
        /// Create a list source object for querying of a SharePoint list.
        /// </summary>
        /// <param name="context">Data context object used to connect to SharePoint.</param>
        /// <param name="autoRegister">Indicates whether or not the list should be registered with the context.</param>
        internal SharePointList(SharePointDataContext context, bool autoRegister)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            if (autoRegister)
                _context.RegisterList(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SharePoint data context object used to connect to this list.
        /// </summary>
        public SharePointDataContext Context
        {
            get
            {
                return _context;
            }
        }

        /// <summary>
        /// Gets the type of the elements held in the list data source object.
        /// </summary>
        public Type ElementType
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Gets the expression tree representation of the list data source object.
        /// </summary>
        public Expression Expression
        {
            get
            {
                return Expression.Constant(this);
            }
        }

        /// <summary>
        /// Gets the query provider for LINQ support.
        /// </summary>
        public IQueryProvider Provider
        {
            get
            {
                return SharePointListQueryProvider.GetInstance(_context);
            }
        }

        /// <summary>
        /// Gets or sets whether the actual SharePoint list version should be matched against the list version as indicated by the metadata on the list entity type.
        /// If null, this setting is ignored and the entity type's ListAttribute setting is taken.
        /// </summary>
        /// <remarks>This setting can be overridden on the SharePointDataContext level.</remarks>
        public bool? CheckVersion
        {
            get;
            set;
        }

        #endregion

        #region IOrderedQueryable<T> implementation

        /// <summary>
        /// Gets all entity objects from the SharePoint list.
        /// </summary>
        /// <returns>All entity objects from the SharePoint list.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _context.ExecuteQuery<T>(this.Expression);
        }

        /// <summary>
        /// Gets all entity objects from the SharePoint list.
        /// </summary>
        /// <returns>All entity objects from the SharePoint list.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _context.ExecuteQuery<T>(this.Expression);
        }

        #endregion

        #region Support for entity retrieval by key value

        /// <summary>
        /// Retrieves an entity by the given ID (primary key field).
        /// </summary>
        /// <param name="id">ID of the entity to retrieve.</param>
        /// <returns>Entity object with the given ID; null if not found.</returns>
        public T GetEntityById(int id)
        {
            //
            // Look in cache first.
            //
            if (cache.ContainsKey(id))
                return cache[id];

            //
            // Find primary key field and property.
            //
            FieldAttribute pkField;
            PropertyInfo pkProp;
            Helpers.FindPrimaryKey(typeof(T), out pkField, out pkProp, true);

            //
            // Build a manual query representing this.Where(e => e.ID = id) with ID property being variable, based on pkProp.
            //
            ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
            MemberExpression pk = Expression.Property(parameter, pkProp);
            BinaryExpression byID = Expression.Equal(pk, Expression.Constant(id, typeof(int)));
            Expression<Func<T, bool>> filter = Expression.Lambda<Func<T, bool>>(byID, parameter);

            //
            // Return the result if found, null otherwise.
            // Remark: AsEnumerable() is required because SingleOrDefault isn't directly supported in LINQ to SharePoint at the moment.
            //
            T result = Queryable.Where<T>(this, filter).AsEnumerable().SingleOrDefault();

            //
            // Cache the result and return it.
            //
            cache[id] = result;
            return result;
        }

        /// <summary>
        /// Retrieves a list of entities by the given set of IDs (primary key field).
        /// </summary>
        /// <param name="ids">IDs of the entities to retrieve.</param>
        /// <returns>List of entity objects with the given IDs; null if not found.</returns>
        public IList<T> GetEntitiesById(int[] ids)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            //
            // Entities to be loaded because these are missing in the cache.
            //
            List<int> missing = new List<int>();

            //
            // Find missing entities to build load list.
            //
            foreach (int id in ids)
                if (!cache.ContainsKey(id))
                    missing.Add(id);

            //
            // Need to load missing entities?
            //
            if (missing.Count > 0)
            {
                //
                // Find primary key field and property.
                //
                FieldAttribute pkField;
                PropertyInfo pkProp;
                Helpers.FindPrimaryKey(typeof(T), out pkField, out pkProp, true);

                //
                // Build a manual query representing this.Where(e => e.ID = missing[0] || e.ID = missing[1] || ...) with ID property being variable, based on pkProp.
                //
                ParameterExpression parameter = Expression.Parameter(typeof(T), "e");
                MemberExpression pk = Expression.Property(parameter, pkProp);

                //
                // Get equality checks for all of the ids.
                //
                Queue<BinaryExpression> byIDs = new Queue<BinaryExpression>();
                foreach (int id in missing)
                    byIDs.Enqueue(Expression.Equal(pk, Expression.Constant(id, typeof(int))));
                
                //
                // Try to balance the tree.
                //
                while (byIDs.Count > 1)
                    byIDs.Enqueue(Expression.Or(byIDs.Dequeue(), byIDs.Dequeue()));

                //
                // Get the filter based on the expression node.
                //
                Expression<Func<T, bool>> filter = Expression.Lambda<Func<T, bool>>(byIDs.Dequeue(), parameter);

                //
                // Put loaded entities in the cache.
                //
                foreach (T item in Queryable.Where<T>(this, filter))
                {
                    int id = (int)pkProp.GetValue(item, null);
                    cache[id] = item;
                }
            }

            //
            // Result list based on cached entities. Keeps the ordering of the original ids parameter.
            //
            List<T> lst = new List<T>(ids.Length);
            foreach (int id in ids)
                lst.Add(cache[id]); //TODO: what if an entity is missing for some reason?

            return lst;
        }

        #endregion

        #region Internal access to entity cache

        /// <summary>
        /// Retrieve an entity object from the cache.
        /// </summary>
        /// <param name="id">Primary key value to get the entity object for.</param>
        /// <returns>Entity object corresponding to the specified primary key if present; otherwise null.</returns>
        internal T FromCache(int id)
        {
            if (cache.ContainsKey(id))
                return cache[id];
            else
                return null;
        }

        /// <summary>
        /// Adds the specified entity with the specified primary key value to the cache.
        /// </summary>
        /// <param name="id">Primary key value.</param>
        /// <param name="item">Entity object to add to the cache.</param>
        internal void ToCache(int id, T item)
        {
            Debug.Assert(item != null);

            //
            // Remove duplicates.
            //
            if (cache.ContainsKey(id))
                cache.Remove(id);

            cache.Add(id, item);
        }

        #endregion
    }
}
