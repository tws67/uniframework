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
 * 0.2.2 - New entity model; added Storage property to FieldAttribute
 */

#region Namespace imports

using System;

#endregion

namespace Uniframework.SharePoint.Linq
{
    /// <summary>
    /// Mapping attribute for SharePoint list fields, applied on entity class properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class FieldAttribute : Attribute
    {
        #region Private members

        /// <summary>
        /// Name of the list field in SharePoint.
        /// </summary>
        /// <remarks>No automatic property with private setter because of Microsoft.Design CA1019 violation.</remarks>
        private string _field;

        /// <summary>
        /// List field type in SharePoint.
        /// </summary>
        /// <remarks>No automatic property with private setter because of Microsoft.Design CA1019 violation.</remarks>
        private FieldType _fieldType;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a field mapping to the specified underlying list field in SharePoint.
        /// </summary>
        /// <param name="field">SharePoint list field where the property is mapped to.</param>
        /// <param name="fieldType">SharePoint type of the list field.</param>
        /// <remarks>The field name should not be XML-encoded. This will be done automatically if needed.</remarks>
        public FieldAttribute(string field, FieldType fieldType)
        {
            _field = field;
            _fieldType = fieldType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the list field in SharePoint.
        /// </summary>
        /// <remarks>The field name should not be XML-encoded. This will be done automatically if needed.</remarks>
        public string Field
        {
            get { return _field; }
        }

        /// <summary>
        /// List field type in SharePoint.
        /// </summary>
        public FieldType FieldType
        {
            get { return _fieldType; }
        }

        /// <summary>
        /// String-representation of the GUID that uniquely identifies the list field in SharePoint.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates that the field is mapped to the primary key field of the list.
        /// </summary>
        public bool PrimaryKey
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the field shown in a Lookup field.
        /// </summary>
        public string LookupDisplayField
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the list field in SharePoint is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates whether the list field in SharePoint is calculated.
        /// </summary>
        public bool Calculated
        {
            get;
            set;
        }

        /// <summary>
        /// Points to a string property in the entity type that contains the fill-in choice of a multi-choice list field in SharePoint.
        /// </summary>
        public string OtherChoice
        {
            get;
            set;
        }

        /// <summary>
        /// References the entity class field that holds the value of the list field.
        /// Used to set read-only fields and to bypass the property getters and setters in some cases.
        /// </summary>
        public string Storage
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Attribute holds metadata about the underlying SharePoint list on an entity type class definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ListAttribute : Attribute
    {
        #region Private members

        /// <summary>
        /// SharePoint list where the entity type is mapped to.
        /// </summary>
        /// <remarks>No automatic property with private setter because of Microsoft.Design CA1019 violation.</remarks>
        private string _list;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an entity class mapping to the specified underlying SharePoint list.
        /// </summary>
        /// <param name="list">SharePoint list where the entity type is mapped to.</param>
        public ListAttribute(string list)
        {
            _list = list;
            CheckVersion = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// SharePoint list where the entity type is mapped to.
        /// </summary>
        public string List
        {
            get { return _list; }
        }

        /// <summary>
        /// String-representation of the GUID that uniquely identifies the SharePoint list.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Relative URL path to the SharePoint list on the server.
        /// </summary>
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// Version number of the SharePoint list.
        /// </summary>
        public int Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the actual SharePoint list version should be matched against the list version as indicated by the metadata on the list entity type (default is true).
        /// </summary>
        /// <remarks>This setting can be overridden on the SharePointList&lt;T&gt; and SharePointDataContext level.</remarks>
        public bool CheckVersion
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// Mapping attribute for (multi-)choice values to enum fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ChoiceAttribute : Attribute
    {
        #region Private members

        /// <summary>
        /// Name of the choice value in SharePoint.
        /// </summary>
        /// <remarks>No automatic property with private setter because of Microsoft.Design CA1019 violation.</remarks>
        private string _choice;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an enum field mapping to the specified underlying SharePoint choice value.
        /// </summary>
        /// <param name="choice">SharePoint choice value where the field is mapped to.</param>
        /// <remarks>The choice value should not be XML-encoded. This will be done automatically if needed.</remarks>
        public ChoiceAttribute(string choice)
        {
            _choice = choice;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Name of the choice value in SharePoint.
        /// </summary>
        /// <remarks>The choice value should not be XML-encoded. This will be done automatically if needed.</remarks>
        public string Choice
        {
            get { return _choice; }
        }

        #endregion
    }

    /// <summary>
    /// Supported SharePoint field types for entity property mapping.
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// Not used.
        /// </summary>
        None = SPFieldType.Invalid,

        /// <summary>
        /// Boolean field.
        /// </summary>
        Boolean = SPFieldType.Boolean,

        /// <summary>
        /// Choice field.
        /// </summary>
        Choice = SPFieldType.Choice,

        /// <summary>
        /// Counter field. Used for primary key fields.
        /// </summary>
        Counter = SPFieldType.Counter,

        /// <summary>
        /// Currency field.
        /// </summary>
        Currency = SPFieldType.Currency,

        /// <summary>
        /// DateTime field.
        /// </summary>
        DateTime = SPFieldType.DateTime,

        /// <summary>
        /// Integer field.
        /// </summary>
        Integer = SPFieldType.Integer,

        /// <summary>
        /// Lookup field.
        /// </summary>
        Lookup = SPFieldType.Lookup,

        /// <summary>
        /// Multiple lookup field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        LookupMulti = 101,

        /// <summary>
        /// MultiChoice field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
        MultiChoice = SPFieldType.MultiChoice,

        /// <summary>
        /// Note field.
        /// </summary>
        Note = SPFieldType.Note,

        /// <summary>
        /// Number field.
        /// </summary>
        Number = SPFieldType.Number,

        /// <summary>
        /// Text field.
        /// </summary>
        Text = SPFieldType.Text,

        /// <summary>
        /// URL field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "URL")]
        URL = SPFieldType.URL
    }

    /// <summary>
    /// Microsoft.SharePoint.SPFieldType clone based on metadata export.
    /// </summary>
    internal enum SPFieldType
    {
        Invalid = 0,
        Integer = 1,
        Text = 2,
        Note = 3,
        DateTime = 4,
        Counter = 5,
        Choice = 6,
        Lookup = 7,
        Boolean = 8,
        Number = 9,
        Currency = 10,
        URL = 11,
        Computed = 12,
        Threading = 13,
        Guid = 14,
        MultiChoice = 15,
        GridChoice = 16,
        Calculated = 17,
        File = 18,
        Attachments = 19,
        User = 20,
        Recurrence = 21,
        CrossProjectLink = 22,
        ModStat = 23,
        Error = 24,
        ContentTypeId = 25,
        PageSeparator = 26,
        ThreadIndex = 27,
        WorkflowStatus = 28,
        AllDayEvent = 29,
        WorkflowEventType = 30,
        MaxItems = 31,
    }
}