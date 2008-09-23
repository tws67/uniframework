using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Practices.CompositeUI.Common.Services
{
    /// <summary>
    /// 实体转换接口
    /// </summary>
    public interface IEntityTranslator
    {
        /// <summary>
        /// Determines whether this instance can translate the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can translate the specified target type; otherwise, <c>false</c>.
        /// </returns>
        bool CanTranslate(Type targetType, Type sourceType);
        /// <summary>
        /// Determines whether this instance can translate.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <returns>
        /// 	<c>true</c> if this instance can translate; otherwise, <c>false</c>.
        /// </returns>
        bool CanTranslate<TTarget, TSource>();
        /// <summary>
        /// Translates the specified service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        object Translate(IEntityTranslatorService service, Type targetType, object source);
        /// <summary>
        /// Translates the specified service.
        /// </summary>
        /// <typeparam name="TTarget">The type of the target.</typeparam>
        /// <param name="service">The service.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        TTarget Translate<TTarget>(IEntityTranslatorService service, object source);
    }
}
