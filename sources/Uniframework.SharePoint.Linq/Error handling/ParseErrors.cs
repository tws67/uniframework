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
 * 0.2.1 - Introduction of ParseErrors.
 */

#region Namespace imports

using System;
using System.Globalization;
using System.Xml;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Helper class to handle parse errors. Methods will either throw an exception or log the errors on the query object.
    /// </summary>
    internal static class ParseErrors
    {
        public static XmlElement UnsupportedQueryOperator(this QueryParser parser, string queryOperator, int start, int end)
        {
            return KeepOrThrow(parser, 1, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedQueryOperator, queryOperator), start, end);
        }

        public static XmlElement NonBoolConstantValueInPredicate(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 2, Errors.NonBoolConstantValueInPredicate, start, end);
        }

        public static XmlElement AfterProjection(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 3, String.Format(CultureInfo.InvariantCulture, Errors.AfterProjection, method), start, end);
        }

        public static XmlElement DateRangesOverlapInvalidValueArgument(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 4, Errors.DateRangesOverlapInvalidValueArgument, start, end);
        }

        public static XmlElement DateRangesOverlapMissingFieldReferences(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 5, Errors.DateRangesOverlapMissingFieldReferences, start, end);
        }

        //public static XmlElement PredicateContainsNonEntityReference(this QueryParser parser, string member, int start, int end)
        //{
        //    return KeepOrThrow(parser, 6, String.Format(CultureInfo.InvariantCulture, Errors.PredicateContainsNonEntityReference, member), start, end);
        //}

        public static XmlElement PredicateContainsNonEntityMethodCall(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 7, String.Format(CultureInfo.InvariantCulture, Errors.PredicateContainsNonEntityMethodCall, method), start, end);
        }

        public static XmlElement InvalidEntityReference(this QueryParser parser, string member, int start, int end)
        {
            return KeepOrThrow(parser, 8, String.Format(CultureInfo.InvariantCulture, Errors.InvalidEntityReference, member), start, end);
        }

        public static XmlElement UnsupportedStringMethodCall(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 9, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedStringMethodCall, method), start, end);
        }

        public static XmlElement UnsupportedMethodCall(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 10, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedMethodCall, method), start, end);
        }

        public static XmlElement CantNegate(this QueryParser parser, string expression, int start, int end)
        {
            return KeepOrThrow(parser, 11, String.Format(CultureInfo.InvariantCulture, Errors.CantNegate, expression), start, end);
        }

        public static XmlElement UnsupportedUnary(this QueryParser parser, string type, int start, int end)
        {
            return KeepOrThrow(parser, 12, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedUnary, type), start, end);
        }

        public static XmlElement UnsupportedBinary(this QueryParser parser, string type, int start, int end)
        {
            return KeepOrThrow(parser, 13, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedBinary, type), start, end);
        }

        public static XmlElement InvalidNullValuedCondition(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 14, Errors.InvalidNullValuedCondition, start, end);
        }

        public static XmlElement UnrecognizedEnumValue(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 15, Errors.UnrecognizedEnumValue, start, end);
        }

        public static XmlElement UnsupportedOrdering(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 16, Errors.UnsupportedOrdering, start, end);
        }

        public static XmlElement InvalidLookupMultiContainsCall(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 17, Errors.InvalidLookupMultiContainsCall, start, end);
        }

        public static XmlElement SecondProjectionExpression(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 18, Errors.SecondProjectionExpression, start, end);
        }

        public static XmlElement DateRangesOverlapInvalidFieldReferences(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 19, Errors.DateRangesOverlapInvalidFieldReferences, start, end);
        }

        //public static XmlElement NonUniqueLookupField(this QueryParser parser, string property, int start, int end)
        //{
        //    return KeepOrThrow(parser, 20, String.Format(CultureInfo.InvariantCulture, Errors.NonUniqueLookupField, property), start, end);
        //}

        //public static XmlElement MissingLookupFieldSetting(this QueryParser parser, string field, int start, int end)
        //{
        //    return KeepOrThrow(parser, 21, String.Format(CultureInfo.InvariantCulture, Errors.MissingLookupFieldSetting, field), start, end);
        //}

        //public static XmlElement NonExistingLookupField(this QueryParser parser, string field, string lookup, int start, int end)
        //{
        //    return KeepOrThrow(parser, 22, String.Format(CultureInfo.InvariantCulture, Errors.NonExistingLookupField, field, lookup), start, end);
        //}

        //public static XmlElement NullValuedLookupField(this QueryParser parser, string field, string lookup, int start, int end)
        //{
        //    return KeepOrThrow(parser, 23, String.Format(CultureInfo.InvariantCulture, Errors.NullValuedLookupField, field, lookup), start, end);
        //}

        public static XmlElement MultipleEntityReferencesInCondition(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 24, Errors.MultipleEntityReferencesInCondition, start, end);
        }

        public static XmlElement NonAddressPropertyCallOnUrlValue(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 25, Errors.NonAddressPropertyCallOnUrlValue, start, end);
        }

        public static XmlElement MultipleGroupings(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 26, Errors.MultipleGroupings, start, end);
        }

        public static XmlElement UnsupportedGrouping(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 27, Errors.UnsupportedGrouping, start, end);
        }

        public static XmlElement AfterGrouping(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 28, String.Format(CultureInfo.InvariantCulture, Errors.AfterGrouping, method), start, end);
        }

        public static XmlElement AfterTake(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 29, String.Format(CultureInfo.InvariantCulture, Errors.AfterTake, method), start, end);
        }

        public static XmlElement AnyOnNonLookupMultiField(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 30, Errors.AnyOnNonLookupMultiField, start, end);
        }

        public static XmlElement UnsupportedExtensionMethod(this QueryParser parser, string method, int start, int end)
        {
            return KeepOrThrow(parser, 31, String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedExtensionMethod, method), start, end);
        }

        public static XmlElement LookupMultiAnyOnNonPositiveContext(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 32, Errors.LookupMultiAnyOnNonPositiveContext, start, end);
        }

        public static XmlElement UnsupportedQueryExpression(this QueryParser parser, int start, int end)
        {
            return KeepOrThrow(parser, 99, Errors.UnsupportedQueryExpression, start, end);
        }

        public static XmlElement MissingFieldMappingAttribute(this QueryParser parser, string property)
        {
            return KeepOrThrow(parser, 101, String.Format(CultureInfo.InvariantCulture, Errors.MissingFieldMappingAttribute, property), 0, 0);
        }

        //public static XmlElement GeneralError(this QueryParser parser, string message, int start, int end)
        //{
        //    return KeepOrThrow(parser, 9999, message, start, end);
        //}

        //public static void LookupFieldPatchError()
        //{
        //    throw new InvalidOperationException(Errors.LookupFieldPatchError);
        //}

        public static void FatalError()
        {
            throw new InvalidOperationException(Errors.FatalError);
        }

        private static XmlElement KeepOrThrow(QueryParser parser, int errorCode, string message, int start, int end)
        {
            //
            // Error object.
            //
            ParseError error = new ParseError(errorCode, message, start, end);

            //
            // Check run mode.
            //
            if (parser._errors != null)
            {
                //
                // Create error message and add it to the errors collection of the query object.
                //
                int id = parser._errors.Add(error);

                //
                // Generate an error reference tag that will be inserted in the generated CAML on the faulting position.
                //
                return parser._factory.ParseError(id);
            }
            else
            {
                //
                // Runtime mode: throw an exception.
                //
                NotSupportedException ex = new NotSupportedException(error.ToString());
                ex.HelpLink = error.HelpLink;
                throw ex;
            }
        }
    }
}
