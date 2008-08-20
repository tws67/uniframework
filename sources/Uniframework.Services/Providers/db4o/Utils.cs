using System;

namespace Uniframework.Services.db4oProviders
{
    internal static class Utils
    {
        public static string DefaultIfBlank(string originalValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(originalValue))
                return defaultValue;

            return originalValue;
        }
    }
}