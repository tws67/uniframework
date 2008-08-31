using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

using Uniframework.Services;
using Uniframework.Services.db4oService;

namespace Uniframework.SmartClient
{
    public class PropertyService : IPropertyService, IDisposable
    {
        private static readonly string PROPERTY_DBFILE = "Uniframework.conf";

        private Properties properties = new Properties();
        //private db4oDatabaseService dbService;
        private IObjectDatabase db;

        public PropertyService()
        {
            string confPath = FileUtility.GetParent(FileUtility.ApplicationRootPath) + @"\Configuration";
            if (!Directory.Exists(confPath))
                Directory.CreateDirectory(confPath);
            dbService = new db4oDatabaseService(confPath);
            db = dbService.OpenDatabase(PROPERTY_DBFILE);

            Properties[] pros = db.Load<Properties>();
            if (pros.Length > 0)
                properties = pros[0];
        }

        [ServiceDependency]
        public db4oDatabaseService dbService
        {
            get;
            internal set;
        }

        #region IPropertyService Members

        public object Get(string property)
        {
            return properties.Get(property);
        }

        public T Get<T>(string property, T defautValue)
        {
            return properties.Get<T>(property, defautValue);
        }

        public void Set<T>(string property, T value)
        {
            properties.Set<T>(property, value);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            db.Save(properties);
        }

        #endregion
    }
}
