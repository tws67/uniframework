using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.CompositeUI.Common
{
    /// <summary>
    /// Represents a factory for creating adapters of the type T.
    /// </summary>
    public interface IAdapterFactory<T>
    {
        /// <summary>
        /// Gets an adapter for the passed object element.
        /// </summary>
        T GetAdapter(object element);

        /// <summary>
        /// True, if the factory is able to create an adapter for the object element.
        /// </summary>
        bool Supports(object element);
    }
}
