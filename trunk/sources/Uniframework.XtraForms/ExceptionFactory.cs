using System;

namespace Uniframework.XtraForms
{
    /// <summary>
    /// 
    /// </summary>
    internal static class ExceptionFactory
    {
        /// <summary>
        /// Creates the type of the invalid adapter element.
        /// </summary>
        /// <param name="elementType">Type of the element.</param>
        /// <param name="factoryType">Type of the factory.</param>
        /// <returns></returns>
        public static Exception CreateInvalidAdapterElementType(Type elementType, Type factoryType)
        {
            return new ArgumentException(
                string.Format("The specified uielement type '{0}' is not a valid for the '{1}'.",
                              elementType.FullName, factoryType.Name)
                );
        }

        /// <summary>
        /// Creates the invalid element host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static Exception CreateInvalidElementHost(Type host)
        {
            return new ArgumentException(host.Name + " cannot host uielements.");
        }
    }
}