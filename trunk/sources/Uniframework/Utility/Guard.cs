using System;
using System.Globalization;
using System.Reflection;

namespace Uniframework
{
    /// <summary>
    /// 
    /// </summary>
	public static class Guard
	{
        /// <summary>
        /// Arguments the not null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="argumentName">Name of the argument.</param>
		public static void ArgumentNotNull(object value, string argumentName)
		{
			if (value == null)
				throw new ArgumentNullException(argumentName);
		}
	}
}
