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
 * 0.2.1 - Introduction of RuntimeErrors.
 */

#region Namespace imports

using System;
using System.Globalization;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Helper class to handle runtime errors, occurring during query execution and while retrieving results. Methods will throw an exception.
    /// </summary>
    internal static class RuntimeErrors
    {
        public static Exception MissingFieldMappingAttribute(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.MissingFieldMappingAttribute, property));
        }

        public static Exception ListVersionMismatch()
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.ListVersionMismatch));
        }

        public static Exception ConnectionExceptionWs(string url, Exception innerException)
        {
            return new SharePointConnectionException(String.Format(CultureInfo.InvariantCulture, Errors.ConnectionExceptionWs, url), innerException);
        }

        public static Exception InvalidLookupField(string property)
        {
            return new NotSupportedException(String.Format(CultureInfo.InvariantCulture, Errors.InvalidLookupField, property));
        }

        public static Exception LookupFieldPatchError()
        {
            return new InvalidOperationException(Errors.LookupFieldPatchError);
        }

        public static Exception UnrecognizedMappingType(string fieldType)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.UnrecognizedMappingType, fieldType));
        }

        public static Exception TooManyUnknownChoiceValues(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.TooManyUnknownChoiceValues, property));
        }

        public static Exception InvalidOtherChoiceFieldMapping(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.InvalidOtherChoiceFieldMapping, property));
        }

        public static Exception MissingOtherChoiceFieldMapping(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.MissingOtherChoiceFieldMapping, property));
        }

        public static Exception UnsupportedQueryOperator(string queryOperator)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.UnsupportedQueryOperator, queryOperator));
        }

        public static Exception EmptySequence()
        {
            return new InvalidOperationException(Errors.EmptySequence);
        }

        public static Exception FatalError()
        {
            return new InvalidOperationException(Errors.FatalError);
        }

        public static Exception CamlMethodsInvalidUse()
        {
            return new InvalidOperationException(Errors.CamlMethodsInvalidUse);
        }

        public static Exception MissingListAttribute()
        {
            return new InvalidOperationException(Errors.MissingListAttribute);
        }

        public static Exception MoreThanOnePrimaryKey()
        {
            return new InvalidOperationException(Errors.MoreThanOnePrimaryKey);
        }

        public static Exception MissingPrimaryKey()
        {
            return new InvalidOperationException(Errors.MissingPrimaryKey);
        }

        public static Exception StoragePropertyMissingOnReadOnlyField(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.StoragePropertyMissingOnReadOnlyField, property));
        }

        public static Exception InvalidStoragePropertyFieldReference(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.StoragePropertyMissingOnReadOnlyField, property));
        }

        public static Exception NonReadOnlyFieldWithoutSetter(string property)
        {
            return new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, Errors.NonReadOnlyFieldWithoutSetter, property));
        }

        public static Exception DuplicateSharePointListObject()
        {
            return new InvalidOperationException(Errors.DuplicateSharePointListObject);
        }

        public static Exception InvalidUriSpecified()
        {
            return new ArgumentException(Errors.InvalidUriSpecified);
        }
    }
}
