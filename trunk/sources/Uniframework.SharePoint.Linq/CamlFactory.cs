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
 * 0.2.1 - Introduction of CamlFactory.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Factory for CAML fragments.
    /// </summary>
    internal class CamlFactory
    {
        #region Private members

        /// <summary>
        /// XmlDocument object used to build query fragments; acts a the root for all XML elements used while parsing the query.
        /// </summary>
        private XmlDocument _doc = new XmlDocument();

        #endregion

        #region Null checks

        public XmlElement IsNull()
        {
            return _doc.CreateElement("IsNull");
        }

        public XmlElement IsNotNull()
        {
            return _doc.CreateElement("IsNotNull");
        }

        #endregion

        #region Equality check

        public XmlElement Eq()
        {
            return _doc.CreateElement("Eq");
        }

        public XmlElement Neq()
        {
            return _doc.CreateElement("Neq");
        }

        #endregion

        #region DateTime support

        public XmlElement Now()
        {
            return _doc.CreateElement("Now");
        }

        public XmlElement Today()
        {
            return _doc.CreateElement("Today");
        }

        public XmlElement DateRangesOverlap()
        {
            return _doc.CreateElement("DateRangesOverlap");
        }

        #endregion

        #region String operators

        public XmlElement Contains()
        {
            return _doc.CreateElement("Contains");
        }

        public XmlElement BeginsWith()
        {
            return _doc.CreateElement("BeginsWith");
        }

        #endregion

        #region Binary operators

        public XmlElement And()
        {
            return _doc.CreateElement("And");
        }

        public XmlElement Or()
        {
            return _doc.CreateElement("Or");
        }

        public XmlElement And(XmlNode left, XmlNode right)
        {
            XmlElement and = _doc.CreateElement("And");
            and.AppendChild(left);
            and.AppendChild(right);
            return and;
        }

        public XmlElement Or(XmlNode left, XmlNode right)
        {
            XmlElement or = _doc.CreateElement("Or");
            or.AppendChild(left);
            or.AppendChild(right);
            return or;
        }

        #endregion

        #region Core query schema

        public XmlElement Where(XmlNode predicate)
        {
            XmlElement where = _doc.CreateElement("Where");
            where.AppendChild(predicate);
            return where;
        }

        public XmlElement OrderBy()
        {
            return _doc.CreateElement("OrderBy");
        }

        public XmlElement GroupBy()
        {
            return _doc.CreateElement("GroupBy");
        }

        public XmlElement ViewFields()
        {
            return _doc.CreateElement("ViewFields");
        }

        public XmlElement FieldRef(string field)
        {
            XmlElement fieldRef = _doc.CreateElement("FieldRef");
            XmlAttribute fieldName = _doc.CreateAttribute("Name");
            fieldName.Value = XmlConvert.EncodeName(field);
            fieldRef.Attributes.Append(fieldName);
            return fieldRef;
        }

        public XmlElement Value(string type)
        {
            XmlElement valueElement = _doc.CreateElement("Value");
            XmlAttribute ta = _doc.CreateAttribute("Type");
            ta.Value = type;
            valueElement.Attributes.Append(ta);
            return valueElement;
        }

        #endregion

        #region General CreateElement method

        public XmlElement CreateElement(string camlQueryElement)
        {
            return _doc.CreateElement(camlQueryElement);
        }

        #endregion

        #region Helper elements for LINQ to SharePoint parser

        /// <summary>
        /// Gets a Boolean patch for use in query predicates. These patches are invalid CAML elements and should be removed by the query parser prior to query execution.
        /// </summary>
        /// <param name="value">Boolean value of the patch.</param>
        /// <returns>Patch representing the specified Boolean value.</returns>
        public XmlElement BooleanPatch(bool value)
        {
            return _doc.CreateElement(value ? "TRUE" : "FALSE");
        }

        public XmlElement Patch(string field)
        {
            XmlElement p = _doc.CreateElement("Patch");
            XmlAttribute a = _doc.CreateAttribute("Field");
            a.Value = field;
            p.Attributes.Append(a);
            return p;
        }

        public XmlElement ParseError(int id)
        {
            XmlElement errorElement = _doc.CreateElement("ParseError");
            XmlAttribute idAttribute = _doc.CreateAttribute("ID");
            idAttribute.Value = id.ToString(CultureInfo.InvariantCulture.NumberFormat);
            errorElement.Attributes.Append(idAttribute);
            return errorElement;
        }

        #endregion

        #region Helper attributes

        public XmlAttribute LookupAttribute()
        {
            XmlAttribute lookupAttribute = _doc.CreateAttribute("LookupId");
            lookupAttribute.Value = "TRUE";
            return lookupAttribute;
        }

        public XmlAttribute DescendingAttribute()
        {
            XmlAttribute ascending = _doc.CreateAttribute("Ascending");
            ascending.Value = "FALSE";
            return ascending;
        }

        #endregion
    }
}
