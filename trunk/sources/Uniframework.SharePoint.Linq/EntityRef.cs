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
 * 0.2.2 - Introduction of EntityRef<T>; replaces lazy loading thunk functionality.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Entity reference. Used to enable deferred loading.
    /// </summary>
    /// <typeparam name="T">Type of the referenced entity.</typeparam>
    public struct EntityRef<T>
        where T : class
    {
        #region Private members

        /// <summary>
        /// List to retrieve the entity from.
        /// </summary>
        private SharePointList<T> _list;

        /// <summary>
        /// Id of the entity referenced to. Allows deferred loading.
        /// </summary>
        private int _id;

        /// <summary>
        /// Entity referenced to. Will be null if not loaded yet.
        /// </summary>
        private T _entity;

        /// <summary>
        /// Indicates whether or not the entity has been loaded already.
        /// </summary>
        private bool _loaded;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an entity reference for deferred loading.
        /// </summary>
        /// <param name="context">Context used to load the entity.</param>
        /// <param name="id">Primary key of the entity to be loaded.</param>
        internal EntityRef(SharePointDataContext context, int id)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _list = context.GetList<T>();
            _id = id;
            _entity = default(T);
            _loaded = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the referenced entity. If the entity isn't loaded already, deferred loading will be done.
        /// </summary>
        public T Entity
        {
            get
            {
                //
                // Deferred loading.
                //
                if (!_loaded)
                    this.Load();

                //
                // Return result.
                //
                return _entity;
            }

            set
            {
                _entity = value;
                _loaded = true;
            }
        }

        /// <summary>
        /// Indicates whether or not the referenced entity has been loaded already.
        /// </summary>
        public bool HasLoadedOrAssignedValue
        {
            get
            {
                return _loaded;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the entity being referred to.
        /// </summary>
        internal void Load()
        {
            _entity = _list.GetEntityById(_id);
            _loaded = true;
        }

        #endregion

        #region Equals and equality operators

        /// <summary>
        /// Checks for equality with a given a object instance.
        /// </summary>
        /// <param name="obj">Object to check for equality.</param>
        /// <returns>True if both objects represent the same EntityRef; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            //
            // Instances compared to null aren't equal.
            //
            if (obj == null)
                return false;

            //
            // Same reference?
            //
            if (object.ReferenceEquals(this, obj))
                return true;

            //
            // Can only compare to another EntityRef instance.
            //
            if (!(obj is EntityRef<T>))
                return false;
            EntityRef<T> e = (EntityRef<T>)obj;

            //
            // Compare both instances.
            //
            return object.ReferenceEquals(e._list, _list)
                && e._id == _id
                && e._loaded == _loaded;
                
                /*
                   don't need this: if an entity with the same id is loaded through the same list, the instances will match
                   && object.ReferenceEquals(e._entity, _entity)
                 */
        }

        /// <summary>
        /// Returns the hash code for the EntityRef.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode() ^ _loaded.GetHashCode() ^ (_list != null ? _list.GetHashCode() : 0) ^ (_entity != null ? _entity.GetHashCode() : 0);
        }

        /// <summary>
        /// Checks for equality between two EntityRefs.
        /// </summary>
        /// <param name="entityRef1">First EntityRef to compare.</param>
        /// <param name="entityRef2">Second EntityRef to compare.</param>
        /// <returns>true if both EntityRefs are equal; otherwise, false.</returns>
        public static bool operator ==(EntityRef<T> entityRef1, EntityRef<T> entityRef2)
        {
            if (entityRef1 == null && entityRef2 == null)
                return true;
            else if (entityRef1 == null || entityRef2 == null)
                return false;
            else
                return entityRef1.Equals(entityRef2);
        }

        /// <summary>
        /// Checks for inequality between two EntityRefs.
        /// </summary>
        /// <param name="entityRef1">First EntityRef to compare.</param>
        /// <param name="entityRef2">Second EntityRef to compare.</param>
        /// <returns>true if both EntityRefs are equal; otherwise, false.</returns>
        public static bool operator !=(EntityRef<T> entityRef1, EntityRef<T> entityRef2)
        {
            return !(entityRef1 == entityRef2);
        }

        #endregion
    }
}
