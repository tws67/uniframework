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
 * 0.2.1 - Introduction of Helpers.
 * 0.2.2 - Change of GetListAttribute.
 * 0.2.3 - GetEntityProperties
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Globalization;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Class with helper methods used by the query parser.
    /// </summary>
    static internal class Helpers
    {
        #region Helpers for attributes

        /// <summary>
        /// Retrieves the FieldAttribute applied on the specified entity property, if one is set.
        /// </summary>
        /// <param name="member">Entity property to examine for a FieldAttribute.</param>
        /// <returns>The FieldAttribute applied on the specified entity property; null if not set.</returns>
        internal static FieldAttribute GetFieldAttribute(PropertyInfo member)
        {
            Debug.Assert(member != null);

            //
            // Get custom attributes of type FieldAttribute that are applied on the member.
            //
            FieldAttribute[] fa = member.GetCustomAttributes(typeof(FieldAttribute), false) as FieldAttribute[];
            if (fa != null && fa.Length != 0 && fa[0] != null)
                return fa[0];
            else
                return null;
        }

        /// <summary>
        /// Helper method to get the ListAttribute applied for the given entity type. An InvalidOperationException will be thrown if no ListAttribute is found.
        /// </summary>
        /// <param name="type">Type to get the ListAttribute for.</param>
        /// <param name="throwExceptionIfMissing">Indicates whether or not to throw an exception if the list attribute is missing.</param>
        /// <returns>ListAttribute applied on the entity object.</returns>
        internal static ListAttribute GetListAttribute(Type type, bool throwExceptionIfMissing)
        {
            Debug.Assert(type != null);

            ListAttribute[] la = type.GetCustomAttributes(typeof(ListAttribute), false) as ListAttribute[];
            if (la != null && la.Length != 0)
                return la[0];
            else
            {
                if (throwExceptionIfMissing)
                    throw RuntimeErrors.MissingListAttribute();
                else
                    return null;
            }
        }

        /// <summary>
        /// Finds the primary key field on a given entity type.
        /// </summary>
        /// <param name="entityType">Entity type to get the primary key field for.</param>
        /// <param name="pkField">Field attribute for the found primary key. Will be null if no primary key is found and <paramref name="required">required</paramref> is set to false.</param>
        /// <param name="pkProp">Property info for the property acting as the primary key. Will be null if no primary key is found and <paramref name="required">required</paramref> is set to false.</param>
        /// <param name="required">If set to true, an exception will be thrown if no primary key field is found on the entity type.</param>
        /// <exception cref="InvalidOperationException">
        /// If more than one property is found with a primary key field attribute, or,
        /// if <paramref name="required">required</paramref> is set and no primary key is found.
        /// </exception>
        internal static void FindPrimaryKey(Type entityType, out FieldAttribute pkField, out PropertyInfo pkProp, bool required)
        {
            Debug.Assert(entityType != null);

            pkField = null;
            pkProp = null;

            //
            // Find field attribute and corresponding property for the field with PrimaryKey attribute value set to true.
            //
            foreach (PropertyInfo property in entityType.GetProperties())
            {
                FieldAttribute field = Helpers.GetFieldAttribute(property);
                if (field != null && field.PrimaryKey && field.FieldType == FieldType.Counter)
                {
                    if (pkField != null)
                        throw RuntimeErrors.MoreThanOnePrimaryKey();

                    pkField = field;
                    pkProp = property;
                    break;
                }
            }

            //
            // Primary key field should be present in order to make the query.
            //
            if (required && (pkField == null || pkProp == null))
                throw RuntimeErrors.MissingPrimaryKey();
        }

        #endregion

        #region Helpers for logging

        /// <summary>
        /// Helper method to log query information before fetching results.
        /// </summary>
        /// <param name="output">Output to write log information to.</param>
        /// <param name="where">Query predicate.</param>
        /// <param name="order">Ordering clause.</param>
        /// <param name="projection">Projection clause.</param>
        /// <param name="group">Grouping clause.</param>
        internal static void LogTo(TextWriter output, XmlElement where, XmlElement order, XmlElement projection, XmlElement group)
        {
            Debug.Assert(output != null);

            //
            // We'll output XML representing various CAML elements. Output should be indented for natural reading.
            //
            XmlTextWriter xw = new XmlTextWriter(output);
            xw.Formatting = Formatting.Indented;

            //
            // Write the query, including the predicate and the ordering element.
            //
            xw.WriteStartElement("Query");
            if (where != null && where.ChildNodes.Count != 0)
                where.WriteTo(xw);
            if (order != null)
                order.WriteTo(xw);
            if (group != null)
                group.WriteTo(xw);
            xw.WriteEndElement();

            //
            // If a projection clause is present, we'll print that too.
            //
            if (projection != null)
            {
                XmlDocument projectDoc = new XmlDocument();
                projectDoc.LoadXml(projection.OuterXml);
                projectDoc.Save(xw);
            }

            //
            // Flush output.
            //
            output.Flush();
        }

        #endregion

        #region Helpers for entity properties

        /// <summary>
        /// Gets the entity properties for the given type.
        /// </summary>
        /// <param name="type">Type to get entity properties from.</param>
        /// <returns>Sequence of entity properties for the given type.</returns>
        /// <remarks>Properties are considered to be entity properties if these have a FieldAttribute applied.</remarks>
        internal static IEnumerable<PropertyInfo> GetEntityProperties(Type type)
        {
            foreach (PropertyInfo prop in type.GetProperties())
                if (GetFieldAttribute(prop) != null)
                    yield return prop;
        }

        #endregion

        #region Helpers for DateTime

        /// <summary>
        /// Generates an ISO8601 date/time string based on the specified DateTime instance.
        /// </summary>
        /// <param name="val">DateTime instance to convert to ISO8601.</param>
        /// <returns>ISO8601 date/time representation of the specified DateTime instance.</returns>
        public static string CreateISO8601DateTimeFromSystemDateTime(DateTime val)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:yyyy}-{0:MM}-{0:dd}T{0:HH}:{0:mm}:{0:ss}Z", val);
        }

        #endregion

        #region Parse methods for SPField* class replacements

        /// <summary>
        /// Parses a MultiChoice field value string representation to a list of strings representing the individual choices.
        /// </summary>
        /// <param name="fieldValue">MultiChoice field value string representation to be parsed.</param>
        /// <returns>List of strings representing the individual choices in the MultiChoice field.</returns>
        public static List<string> ParseMultiChoiceFieldValue(string fieldValue)
        {
            List<string> results = new List<string>();

            if (!string.IsNullOrEmpty(fieldValue))
            {
                //
                // Value should start with. If not, make sure the field is trailed with delimiters.
                //
                if (!fieldValue.StartsWith(";#", StringComparison.Ordinal))
                    fieldValue = ";#" + fieldValue + ";#";

                //
                // Split the string based on the ;# delimiter.
                //
                for (int i = 2, pos = 0; i < fieldValue.Length; i = pos + 2)
                {
                    //
                    // Search from the current position to an occurrence of ;#.
                    //
                    pos = fieldValue.IndexOf(";#", i, StringComparison.Ordinal);

                    //
                    // None found? We're done.
                    //
                    if (pos < 0)
                        break;
                    //
                    // Something beyond the current position? This is another choice value.
                    //
                    else if (pos > i)
                        results.Add(fieldValue.Substring(i, pos - i));
                }
            }

            return results;
        }

        #endregion
    }

    /// <summary>
    /// Represents the value of a Lookup field.
    /// </summary>
    internal class LookupFieldValue
    {
        #region Private members

        /// <summary>
        /// Id of the referenced list item.
        /// </summary>
        private int _id;

        /// <summary>
        /// Value of the Lookup field; will be the display field's value.
        /// </summary>
        private string _value;

        #endregion

        #region Properties

        /// <summary>
        /// Id of the referenced list item.
        /// </summary>
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                _value = null;
            }
        }

        /// <summary>
        /// Value of the Lookup field; will be the display field's value.
        /// </summary>
        public string Value
        {
            get
            {
                if (!IsSecret)
                    return _value;
                else
                    return "***";
            }
            set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Indicates whether or not the field is considered secret.
        /// </summary>
        public bool IsSecret { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Parses a string representation of a Lookup value to a LookupFieldValue instance.
        /// </summary>
        /// <param name="fieldValue">Field value to parse.</param>
        /// <returns>LookupFieldValue instance representing the specified string value.</returns>
        public static LookupFieldValue Parse(string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue))
                return null;

            LookupFieldValue result = new LookupFieldValue();

            if (fieldValue == "***")
                result.IsSecret = true;
            else
            {
                //
                // Find the delimiter.
                //
                int index = fieldValue.IndexOf(";#", StringComparison.Ordinal);

                //
                // Set the id.
                //
                int id;
                if (!int.TryParse(fieldValue.Substring(0, index), NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
                    throw new ArgumentException("Invalid lookup field identifier.", "fieldValue");
                result._id = id;

                //
                // Set the value.
                //
                if (index >= 0)
                {
                    index += 2;
                    if (index < fieldValue.Length)
                        result._value = fieldValue.Substring(index, fieldValue.Length - index);
                }
                else
                    result._value = "";
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Represents the value of a LookupMulti field.
    /// </summary>
    internal class LookupMultiFieldValue
    {
        #region Private members

        /// <summary>
        /// List of the individual Lookup field values that are contained in the LookupMulti field.
        /// </summary>
        private List<LookupFieldValue> _values = new List<LookupFieldValue>();

        #endregion

        #region Properties

        /// <summary>
        /// List of the individual Lookup field values that are contained in the LookupMulti field.
        /// </summary>
        public List<LookupFieldValue> Values
        {
            get
            {
                return _values;
            }
        }

        #endregion

        #region Factory methods

        /// <summary>
        /// Parses a string representation of a LookupMulti value to a LookupMultiFieldValue instance.
        /// </summary>
        /// <param name="fieldValue">Field value to parse.</param>
        /// <returns>LookupMultiFieldValue instance representing the specified string value.</returns>
        public static LookupMultiFieldValue Parse(string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue))
                return null;

            LookupMultiFieldValue result = new LookupMultiFieldValue();

            //
            // Keep track of found identifiers to eliminate duplicates.
            //
            Dictionary<int, bool> values = new Dictionary<int, bool>();

            //
            // Remove trailing ;# if present.
            //
            if (fieldValue.StartsWith(";#", StringComparison.Ordinal))
                fieldValue = fieldValue.Substring(2);

            //
            // Find all the id;#value sequences and parse them.
            //
            int b = 0;
            int j;
            while (b < fieldValue.Length)
            {
                //
                // Look for next occurrence of ;#.
                //
                int i = fieldValue.IndexOf(";#", b, StringComparison.Ordinal);

                //
                // If the current position (b) and the position where a delimiter is found are the same, there's no id.
                // In this case we'll need to skip the value part of the pair and move on to the next pair.
                //
                bool skip = (i == b);

                //
                // No more occurrences of delimiters, take the remainder of the string.
                //
                if (i < 0)
                    j = fieldValue.Length;
                //
                // Delimiter found. Skip it, and try to find a corresponding value by looking for the next delimiter.
                //
                else
                {
                    i += 2;
                    if (i < fieldValue.Length)
                    {
                        j = fieldValue.IndexOf(";#", i, StringComparison.Ordinal);

                        if (j < 0)
                            j = fieldValue.Length;
                    }
                    else
                        j = fieldValue.Length;
                }

                //
                // If the pair was considered valid, i.e. no absent id part, parse it.
                //
                if (!skip)
                {
                    string t = fieldValue.Substring(b, j - b);
                    if (t != ";#")
                    {
                        try
                        {
                            //
                            // Parse the pair and look for duplicates.
                            //
                            var val = LookupFieldValue.Parse(t);
                            if (!values.ContainsKey(val.Id))
                            {
                                values.Add(val.Id, true);
                                result._values.Add(val);
                            }
                        }
                        catch (ArgumentException) { }
                    }
                }

                //
                // Get rid of the last delimiter and move on to the next pair.
                //
                b = j + 2;
            }

            return result;
        }

        #endregion
    }
}
