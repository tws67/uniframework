using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.Utility;

namespace Microsoft.Practices.CompositeUI.Common
{
    public class AdapterFactoryCatalog<T> : IAdapterFactoryCatalog<T>
    {
        private List<IAdapterFactory<T>> factories = new List<IAdapterFactory<T>>();

        /// <summary>
        /// A list of the contained factories.
        /// </summary>
        /// <value></value>
        public IList<IAdapterFactory<T>> Factories
        {
            get { return factories.AsReadOnly(); }
        }

        /// <summary>
        /// Returns a factory for the passed object element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// This exception is thrown if the object element is not supported by any of the registered factories.
        /// </exception>
        public IAdapterFactory<T> GetFactory(object element)
        {
            for (int i = factories.Count - 1; i >= 0; i--)
            {
                if (factories[i].Supports(element))
                {
                    return factories[i];
                }
            }

            throw new ArgumentException(String.Format("没有为类型 \"{0}\" 注册适配器工厂。",
                element.GetType().ToString()));
        }

        /// <summary>
        /// Registers a new factory in the catalog.
        /// </summary>
        /// <param name="factory"></param>
        public void RegisterFactory(IAdapterFactory<T> factory)
        {
            Guard.ArgumentNotNull(factory, "factory");

            factories.Add(factory);
        }
    }
}
