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
 * 0.2.2 - Introduction of EntitySet<T>; replaces lazy loading thunk functionality.
 * 0.2.3 - Lazy loading implementation improvement.
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
    /// Set of entity references. Used to enable lazy loading.
    /// </summary>
    /// <typeparam name="T">Type of the referenced entities.</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class EntitySet<T> : IList<T>, ICollection<T>
        where T : class
    {
        #region Private members

        /// <summary>
        /// List to retrieve the entities from.
        /// </summary>
        private SharePointList<T> _list;

        /// <summary>
        /// Ids of the entities referenced to. Allows deferred loading.
        /// </summary>
        private int[] _ids;

        /// <summary>
        /// Entities referenced to. Will be null if not loaded yet.
        /// </summary>
        private IList<T> _entities;

        /// <summary>
        /// Indicates whether or not the entities have been loaded already.
        /// </summary>
        private bool _loaded;

        /// <summary>
        /// Delegate to the action to be taken when an entity is added to the set. Allows change tracking.
        /// </summary>
        private Action<T> _onAdd;

        /// <summary>
        /// Delegate to the action to be taken when an entity is removed from the set. Allows change tracking.
        /// </summary>
        private Action<T> _onRemove;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a set of entity references for lazy loading.
        /// </summary>
        /// <param name="context">Context used to load the entities.</param>
        /// <param name="ids">Primary keys of the entities to be loaded.</param>
        /// <param name="onAdd">Action to take when an entity is added to the set.</param>
        /// <param name="onRemove">Action to take when an entity is removed from the set.</param>
        internal EntitySet(SharePointDataContext context, int[] ids, Action<T> onAdd, Action<T> onRemove)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (ids == null)
                throw new ArgumentNullException("ids");

            _list = context.GetList<T>();
            _ids = ids;
            _entities = null;
            _loaded = false;
            _onAdd = onAdd;
            _onRemove = onRemove;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether or not the referenced entities have been loaded already.
        /// </summary>
        public bool IsLoaded
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
            _entities = _list.GetEntitiesById(_ids);
            _loaded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public void Assign(IEnumerable<T> source)
        {
            _entities = new List<T>(source); //TODO
        }

        #endregion

        #region Collection support

        /// <summary>
        /// Gets or sets the entity at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the entity to get or set.</param>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentException("Collection index should be positive.");

                //
                // Lazy loading.
                //
                if (!_loaded)
                    this.Load();

                return _entities[index];
            }
            set
            {
                if (index < 0)
                    throw new ArgumentException("Collection index should be positive.");

                //
                // Lazy loading.
                //
                if (!_loaded)
                    this.Load();

                if (_onRemove != null)
                    _onRemove(_entities[index]);

                if (_onAdd != null)
                    _onAdd(value);

                _entities[index] = value;
            }
        }

        /// <summary>
        /// Searches for the specified entity and returns the zero-based index of the first occurrence within the entity list.
        /// </summary>
        /// <param name="entity">The entity to locate in the entity list.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire entity list, if found; otherwise, -1.</returns>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public int IndexOf(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            return _entities.IndexOf(entity);
        }

        /// <summary>
        /// Inserts an entity into the entity list at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="entity">The object to insert.</param>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#")]
        public void Insert(int index, T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (index < 0)
                throw new ArgumentException("Collection index should be positive.");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            if (_onAdd != null)
                _onAdd(entity);

            _entities.Insert(index, entity);
        }

        /// <summary>
        /// Removes the entity at the specified index of the entity list.
        /// </summary>
        /// <param name="index">The zero-based index of the entity to remove.</param>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public void RemoveAt(int index)
        {
            if (index < 0)
                throw new ArgumentException("Collection index should be positive.");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            if (_onRemove != null)
                _onRemove(_entities[index]);

            _entities.RemoveAt(index);
        }

        /// <summary>
        /// Adds an entity to the end of the entity list.
        /// </summary>
        /// <param name="entity">The entity to be added to the end of the entity list.</param>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //
            // Lazy loading. TODO: needed?
            //
            if (!_loaded)
                this.Load();

            if (_onAdd != null)
                _onAdd(entity);

            _entities.Add(entity);
        }

        /// <summary>
        /// Removes all entities from the entity list.
        /// </summary>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public void Clear()
        {
            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            if (_onRemove != null)
            {
                foreach (T entity in _entities)
                    _onRemove(entity);
            }

            _entities.Clear();
        }

        /// <summary>
        /// Determines whether an entity is in the entity list.
        /// </summary>
        /// <param name="entity">The entity to locate in the entity list.</param>
        /// <returns>true if entity is found in the entity list; otherwise, false.</returns>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public bool Contains(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            return _entities.Contains(entity);
        }

        /// <summary>
        /// Copies the entire entity list to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the entities copied from the entity list. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0 || arrayIndex >= array.Length)
                throw new ArgumentException("Invalid array index specified.");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            _entities.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of entities actually contained in the entity list.
        /// </summary>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public int Count
        {
            get
            {
                //
                // Lazy loading.
                //
                if (!_loaded)
                    this.Load();

                return _entities.Count;
            }
        }

        /// <summary>
        /// Gets whether the entity list is read-only. Will always return false in the current implementation.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the specified entity from the entity list.
        /// </summary>
        /// <param name="entity">The entity to remove from the entity list.</param>
        /// <returns>true if entity is successfully removed; otherwise, false. This method also returns false if item was not found in the entity list.</returns>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public bool Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            if (_onRemove != null)
                _onRemove(entity);

            return _entities.Remove(entity);
        }

        #endregion

        #region Enumeration support

        /// <summary>
        /// Returns an enumerator that iterates through the entity list.
        /// </summary>
        /// <returns>An enumerator for the entity list.</returns>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        public IEnumerator<T> GetEnumerator()
        {
            //
            // Lazy loading.
            //
            if (!_loaded)
                this.Load();

            return _entities.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the entity list.
        /// </summary>
        /// <returns>An enumerator for the entity list.</returns>
        /// <remarks>Will trigger deferred loading if the entities are not loaded yet.</remarks>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Equals and equality operators

        /// <summary>
        /// Checks for equality with a given a object instance.
        /// </summary>
        /// <param name="obj">Object to check for equality.</param>
        /// <returns>True if both objects represent the same EntitySet; false otherwise.</returns>
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
            // Can only compare to another EntitySet instance.
            //
            EntitySet<T> e = obj as EntitySet<T>;
            if (e == null)
                return false;

            //
            // Compare both instances.
            //
            return object.ReferenceEquals(e._list, _list)
                && e._ids == _ids
                && e._loaded == _loaded;
        }

        /// <summary>
        /// Returns the hash code for the EntitySet.
        /// </summary>
        /// <returns>Hash code.</returns>
        public override int GetHashCode()
        {
            return _ids.GetHashCode() ^ _loaded.GetHashCode() ^ (_list != null ? _list.GetHashCode() : 0) ^ (_entities != null ? _entities.GetHashCode() : 0);
        }

        /// <summary>
        /// Checks for equality between two EntitySets.
        /// </summary>
        /// <param name="entitySet1">First EntitySet to compare.</param>
        /// <param name="entitySet2">Second EntitySet to compare.</param>
        /// <returns>true if both EntitySets are equal; otherwise, false.</returns>
        public static bool operator ==(EntitySet<T> entitySet1, EntitySet<T> entitySet2)
        {
            if (entitySet1 == null && entitySet2 == null)
                return true;
            else if (entitySet1 == null || entitySet2 == null)
                return false;
            else
                return entitySet1.Equals(entitySet2);
        }

        /// <summary>
        /// Checks for inequality between two EntitySets.
        /// </summary>
        /// <param name="entitySet1">First EntitySet to compare.</param>
        /// <param name="entitySet2">Second EntitySet to compare.</param>
        /// <returns>true if both EntitySets are equal; otherwise, false.</returns>
        public static bool operator !=(EntitySet<T> entitySet1, EntitySet<T> entitySet2)
        {
            return !(entitySet1 == entitySet2);
        }

        #endregion
    }
}
