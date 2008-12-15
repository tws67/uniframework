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
 * 0.2.1 - Creation as part of refactoring
 * 0.2.2 - New entity model
 * 0.2.3 - Pluralization
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Set of helper methods used in code generation.
    /// </summary>
    internal static class Helpers
    {
        #region Static members

        /// <summary>
        /// Depluralization map.
        /// </summary>
        private static Dictionary<string, string> Plurals;

        #endregion

        #region Static constructor

        /// <summary>
        /// Type initializer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Helpers()
        {
            Plurals = new Dictionary<string, string>();
            Plurals.Add("ies", "y");
            Plurals.Add("oes", "o");
            Plurals.Add("ves", "f");
            Plurals.Add("ges", "ge");
            Plurals.Add("ses", "se");
            Plurals.Add("es", "");
            Plurals.Add("s", "");
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Get a friendly name for a field, usable in C# or VB as a property name.
        /// </summary>
        /// <param name="name">Name to create a friendly name for.</param>
        /// <returns>Friendly name for given name.</returns>
        public static string GetFriendlyName(string name)
        {
            //
            // Find parts of the name, separated by a space, dash or underscore, and create a Pascal-cased name.
            //
            StringBuilder sb = new StringBuilder();
            foreach (string s in name.Split(' ', '-', '_'))
            {
                //
                // Keep only letters and digits.
                //
                string s2 = SanitizeString(s);

                //
                // Process string if not empty.
                //
                if (!String.IsNullOrEmpty(s2))
                {
                    //
                    // First letter of a part should be capitalized.
                    //
                    sb.Append(char.ToUpperInvariant(s2[0]));

                    //
                    // Leave original casing for the remainder.
                    //
                    if (s2.Length > 1)
                        sb.Append(s2.Substring(1));
                }
            }

            //
            // Check for starting with digit case.
            //
            string res = sb.ToString();
            if (res.Length != 0 && char.IsDigit(res[0]))
                res = '_' + res;

            return res;
        }

        /// <summary>
        /// Keeps only letters and digits from a given string.
        /// </summary>
        /// <param name="s">String to be sanitized.</param>
        /// <returns>Sanitized string.</returns>
        public static string SanitizeString(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);

            return sb.ToString();
        }

        /// <summary>
        /// Gets the name for a helper property, typically used for (Multi)Choice field types with fill-in choice enabled.
        /// </summary>
        /// <param name="fieldName">Name of the field to create a helper for.</param>
        /// <returns>Helper field name.</returns>
        public static string GetHelperName(string fieldName)
        {
            return GetFriendlyName(fieldName + "Other");
        }

        /// <summary>
        /// Performs English singularization of verbs.
        /// </summary>
        /// <param name="listName">List name to singularize.</param>
        /// <returns>Singularized name.</returns>
        public static string Singularize(string listName)
        {
            foreach (string plural in Plurals.Keys)
                if (listName.EndsWith(plural, StringComparison.Ordinal))
                    listName = listName.Substring(0, listName.Length - plural.Length) + Plurals[plural];
            return listName;
        }

        /// <summary>
        /// Parses a boolean value.
        /// </summary>
        /// <param name="value">A string containing the value to convert.</param>
        /// <returns>Boolean value for the specified string value.</returns>
        /// <remarks>Supports "true", "false", "1", "0"</remarks>
        public static bool ParseBool(string value)
        {
            bool b;
            if (bool.TryParse(value, out b))
                return b;
            else if (value == "true" || value == "1")
                return true;
            else if (value == "false" || value == "0")
                return false;
            else
                throw new FormatException(Strings.InvalidBool);
        }

        #endregion
    }
}
