/*
 * LINQ to SharePoint
 * http://www.codeplex.com/LINQtoSharePoint
 * 
 * Copyright Bart De Smet (C) 2007
 * info@bartdesmet.net - http://blogs.bartdesmet.net/bart
 * 
 * This project is subject to licensing restrictions. Visit http://www.codeplex.com/LINQtoSharePoint/Project/License.aspx for more information.
 */

#region Namespace imports

using System;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Class with helper methods and other members for CAML queries.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Caml")]
    public static class CamlMethods
    {
        #region Properties

        /// <summary>
        /// Gets the current date/time. Represented by &lt;Now&gt; element in CAML.
        /// </summary>
        public static DateTime Now 
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Gets the current date. Represented by &lt;Today&gt; element in CAML.
        /// </summary>
        public static DateTime Today
        {
            get { return DateTime.Today; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts a &lt;DateRangesOverlap&gt; CAML element in the query.
        /// </summary>
        /// <param name="value">Value to compare with in the DateRangesOverlap CAML query element.</param>
        /// <param name="fields">List of entity fields to participate in the DateRangesOverlap CAML query element.</param>
        /// <returns>Indicates whether or not the specified DateTimes overlap.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fields")]
        public static bool DateRangesOverlap(DateTime value, params DateTime?[] fields)
        {
            throw RuntimeErrors.CamlMethodsInvalidUse();
        }

        #endregion
    }
}
