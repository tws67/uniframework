using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.EventBroker;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Services;
using Uniframework.Services.db4oService;
using Uniframework.SmartClient.Constants;

namespace Uniframework.SmartClient
{
    public class PropertyService : IPropertyService, IDisposable
    {

        private static readonly string PROPERTY_DBFILE = "Uniframework.conf";
        private IObjectDatabaseService dbService;
        private IObjectDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyService"/> class.
        /// </summary>
        public PropertyService()
        {
            string confPath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Configuration";
            if (!Directory.Exists(confPath))
                Directory.CreateDirectory(confPath);
            dbService = new db4oDatabaseService(confPath);
            db = dbService.OpenDatabase(PROPERTY_DBFILE);
        }

        #region IPropertyService Members

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public object Get(string property)
        {
            Property[] items = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            return items.Length > 0 ? items[0].Data : null;
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
            Property[] items = db.Load<Property>(delegate(Property prop) {
                return prop.Name == property;
            });
            try {
                return items.Length > 0 ? (T)items[0].Data : defaultValue;
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
            Property prop = Get(property) as Property;
            object oldValue = (prop != null && prop.Data != null) ? prop.Data : null;
            if (prop != null)
                db.Delete(prop);

            Property item = new Property();
            item.Name = property;
            item.Data = value;
            db.Save(item);
            OnPropertyChanged(new PropertyChangedEventArgs(item, oldValue)); // 触发事件
        }

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        [EventPublication(EventNames.Uniframework_PropertyChanged, PublicationScope.Global)]
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
            dbService.CloseDatabase(PROPERTY_DBFILE);
        }

        #endregion
    }
}
