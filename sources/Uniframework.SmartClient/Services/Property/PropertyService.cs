using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Db4o;

namespace Uniframework.SmartClient
{
    public class PropertyService : IPropertyService, IDisposable
    {

        private static readonly string PROPERTY_DBFILE = "Uniframework.config";
        private IDb4oDatabaseService dbService;
        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyService"/> class.
        /// </summary>
        public PropertyService()
        {
            dbService = new Db4oDatabaseService();
            db = dbService.Open(PROPERTY_DBFILE);
        }

        #region IPropertyService Members

        /// <summary>
        /// Deletes the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        public void Delete(string property)
        {
            IList<Property> objs = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            foreach (Property obj in objs)
                db.Delete(obj);
        }

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Get(string property)
        {
            IList<Property> items = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            return items.Count > 0 ? items[0].Current : null;
        }

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="defautValue">The defaut value.</param>
        /// <returns></returns>
        public T Get<T>(string property, T defaultValue)
        {
            IList<Property> items = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            try {
                return items.Count > 0 ? (T)items[0].Current : defaultValue;
            }
            catch(InvalidCastException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Sets the specified property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public void Set<T>(string property, T value)
        {
            IList<Property> list = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            object oldValue = null;
            if (list.Count > 0) {
                oldValue = list[0].Current;
                db.Delete(list[0]);
            }

            Property item = new Property { 
                Name = property,
                Current = value
            };
            db.Store(item);
            OnPropertyChanged(new PropertyChangedEventArgs(item, oldValue)); // 触发事件
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        [EventPublication(EventNames.Shell_PropertyChanged, PublicationScope.Global)]
        public event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            dbService.Close(PROPERTY_DBFILE);
        }

        #endregion
    }
}
