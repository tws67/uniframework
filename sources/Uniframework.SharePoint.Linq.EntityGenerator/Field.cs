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
 * 0.2.1 - Introduction of Field class
 * 0.2.3 - SPML conversions
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.CodeDom;
using System.ComponentModel;
using System.Drawing.Design;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Represents a SharePoint list field with all the information required by LINQ to SharePoint.
    /// </summary>
    public class Field
    {
        #region Properties

        /// <summary>
        /// Field identifier.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Field identifier.")]
        public Guid Id { get; set; }

        /// <summary>
        /// Field name.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("Field name.")]
        [ParenthesizePropertyName(true)]
        public string Name { get; set; }

        /// <summary>
        /// Field display name.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("Field display name.")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Field description.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Identification")]
        [Description("Field description. Will be used for the comment on the corresponding entity class field property.")]
        public string Description { get; set; }

        /// <summary>
        /// Textual representation of the SharePoint field type.
        /// </summary>
        [Browsable(false)]
        public string SharePointType { get; set; }

        /// <summary>
        /// Indicates whether the field is hidden or not.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Indicates whether the field is hidden or not.")]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Indicates whether the field is read-only or not.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Indicates whether the field is read-only or not.")]
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Indicates whether the field acts as a primary key or not.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Indicates whether the field acts as a primary key or not.")]
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Indicates whether the field is a calculated field or not.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Indicates whether the field is a calculated field or not.")]
        public bool IsCalculated { get; set; }

        /// <summary>
        /// Indicates whether the field is required or not.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Indicates whether the field is required or not.")]
        public bool IsRequired { get; set; }

        /// <summary>
        /// Runtime type to be used in entity classes.
        /// This value will be null in case the field type doesn't have a direct mapping to a runtime type,
        /// e.g. when using Lookup or Choice fields. In such a case, it's up to the entity mapper to cook up
        /// the appropriate types at a later stage.
        /// </summary>
        [Browsable(false)]
        public Type RuntimeType { get; set; }

        /// <summary>
        /// Field type used by LINQ to SharePoint.
        /// </summary>
        /// <seealso cref="SharePointType"/>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("Field type used by LINQ to SharePoint.")]
        public FieldType FieldType { get; set; }

        /// <summary>
        /// For Lookup and LookupMulti fields only. Contains the list used for the lookup.
        /// </summary>
        /// <seealso cref="LookupField"/>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Lookup")]
        [Description("Contains the list used for the lookup.")]
        public string LookupList { get; set; }

        /// <summary>
        /// For Lookup and LookupMulti fields only. Contains the field in the LookupList used for the lookup.
        /// </summary>
        /// <seealso cref="LookupList"/>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Lookup")]
        [Description("Contains the field in the LookupList used for the lookup.")]
        public string LookupField { get; set; }

        /// <summary>
        /// For Choice and MultiChoice fields only. Indicates whether or not a fill-in choice is enabled for this field.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Choices")]
        [Description("Indicates whether or not a fill-in choice is enabled for this field.")]
        public bool FillInChoiceEnabled { get; set; }

        /// <summary>
        /// List of choices for Choice and MultiChoice fields.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Choices")]
        [Editor(typeof(ChoiceEditor), typeof(UITypeEditor))]
        [Description("List of choices for Choice and MultiChoice fields.")]
        public IList<Choice> Choices { get; private set; }

        /// <summary>
        /// Indicates whether the field is included in a list export.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Mapping")]
        [Description("Indicates whether or not the field is included in the export.")]
        public bool Include { get; set; }

        /// <summary>
        /// Mapping alias for the field.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mapping")]
        [Description("Mapping alias for the field.")]
        public string Alias { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a Field definition object from a CAML list definition.
        /// </summary>
        /// <param name="fieldDefinition">SharePoint list field definition.</param>
        /// <returns>Field definition object for the specified list field.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Caml")]
        public static Field FromCaml(XmlNode fieldDefinition)
        {
            if (fieldDefinition == null)
                throw new ArgumentNullException("fieldDefinition");

            //
            // Field object. Include by default.
            //
            Field field = new Field();
            field.Include = true;

            //
            // Field ID (GUID).
            //
            XmlAttribute aID = fieldDefinition.Attributes["ID"];
            if (aID != null)
                field.Id = new Guid(aID.Value);

            //
            // Field name.
            //
            field.Name = (string)fieldDefinition.Attributes["Name"].Value;
            field.DisplayName = (string)fieldDefinition.Attributes["DisplayName"].Value;

            //
            // Field description.
            //
            XmlAttribute aDescription = fieldDefinition.Attributes["Description"];
            if (aDescription != null)
                field.Description = (string)aDescription.Value;

            //
            // Field hidden?
            //
            XmlAttribute aHidden = fieldDefinition.Attributes["Hidden"];
            field.IsHidden = false;
            if (aHidden != null)
                field.IsHidden = bool.Parse(aHidden.Value);

            //
            // Field read-only?
            //
            XmlAttribute aReadOnly = fieldDefinition.Attributes["ReadOnly"];
            field.IsReadOnly = false;
            if (aReadOnly != null)
                field.IsReadOnly = bool.Parse(aReadOnly.Value);

            //
            // Field type.
            //
            string type = (string)fieldDefinition.Attributes["Type"].Value;
            field.IsCalculated = false;

            //
            // Calculated field. Use underlying type for mapping.
            //
            if (type == "Calculated")
            {
                field.SharePointType = (string)fieldDefinition.Attributes["ResultType"].Value;
                field.IsCalculated = true;
            }
            else
                field.SharePointType = type;

            //
            // Primary key field should be imported as well.
            //
            XmlAttribute primaryKey = fieldDefinition.Attributes["PrimaryKey"];
            field.IsPrimaryKey = false;
            if (primaryKey != null)
                field.IsPrimaryKey = bool.Parse(primaryKey.Value);

            //
            // Check whether the field is required or not. Will be used to make value types nullable if not required.
            //
            XmlAttribute aRequired = fieldDefinition.Attributes["Required"];
            field.IsRequired = false;
            if (aRequired != null)
                field.IsRequired = bool.Parse(aRequired.Value);

            //
            // Choices.
            //
            if (field.SharePointType.EndsWith("Choice", StringComparison.Ordinal))
            {
                field.Choices = new List<Choice>();
                foreach (XmlNode choice in fieldDefinition["CHOICES"])
                    field.Choices.Add(Choice.FromCaml(choice));

                //
                // Additional fill-in field needed if FillInChoice is set.
                //
                XmlAttribute fillInChoice = fieldDefinition.Attributes["FillInChoice"];
                field.FillInChoiceEnabled = fillInChoice != null && bool.Parse((string)fillInChoice.Value);
            }

            //
            // Lookup.
            //
            if (field.SharePointType.StartsWith("Lookup", StringComparison.Ordinal))
            {
                field.LookupList = (string)fieldDefinition.Attributes["List"].Value;
                field.LookupField = (string)fieldDefinition.Attributes["ShowField"].Value;
            }

            //
            // Populate type information.
            //
            GetType(field);

            //
            // Return field definition object.
            //
            return field;
        }

        /// <summary>
        /// Creates a Field definition object from a SPML field definition.
        /// </summary>
        /// <param name="spml">SharePoint field definition in SPML.</param>
        /// <returns>Field definition object for the specified field.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spml"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public static Field FromSpml(XmlNode spml)
        {
            if (spml == null)
                throw new ArgumentNullException("spml");

            //
            // Field object.
            //
            Field field = new Field();

            //
            // General field information.
            //
            field.Name = spml.Attributes["Name"].Value;
            field.DisplayName = spml.Attributes["DisplayName"].Value;
            field.Id = new Guid(spml.Attributes["Id"].Value);
            field.SharePointType = spml.Attributes["Type"].Value;
            XmlAttribute description = spml.Attributes["Description"];
            if (description != null)
                field.Description = description.Value;
            XmlAttribute alias = spml.Attributes["Alias"];
            if (alias != null)
                field.Alias = alias.Value;

            //
            // Boolean values.
            //
            XmlAttribute hidden = spml.Attributes["Hidden"];
            if (hidden != null)
                field.IsHidden = Helpers.ParseBool(hidden.Value);
            XmlAttribute readOnly = spml.Attributes["ReadOnly"];
            if (readOnly != null)
                field.IsReadOnly = Helpers.ParseBool(readOnly.Value);
            XmlAttribute primaryKey = spml.Attributes["PrimaryKey"];
            if (primaryKey != null)
                field.IsPrimaryKey = Helpers.ParseBool(primaryKey.Value);
            XmlAttribute calculated = spml.Attributes["Calculated"];
            if (calculated != null)
                field.IsCalculated = Helpers.ParseBool(calculated.Value);
            XmlAttribute required = spml.Attributes["Required"];
            if (required != null)
                field.IsRequired = Helpers.ParseBool(required.Value);

            //
            // Choices.
            //
            XmlAttribute fillInChoice = spml.Attributes["FillInChoice"];
            if (fillInChoice != null)
                field.FillInChoiceEnabled = Helpers.ParseBool(fillInChoice.Value);

            XmlElement choices = spml["Choices"];
            if (choices != null)
            {
                field.Choices = new List<Choice>();
                foreach (XmlNode choice in choices.ChildNodes)
                    field.Choices.Add(Choice.FromSpml(choice));
            }

            //
            // Lookup.
            //
            XmlAttribute lookupList = spml.Attributes["LookupList"];
            XmlAttribute lookupField = spml.Attributes["LookupField"];
            if (lookupList != null)
                field.LookupList = lookupList.Value;
            if (lookupField != null)
                field.LookupField = lookupField.Value;

            //
            // Populate type information.
            //
            GetType(field);

            //
            // Return field definition object.
            //
            return field;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Sets the FieldType and RuntimeType properties on the specified field.
        /// </summary>
        /// <param name="field">Field to set type information for.</param>
        private static void GetType(Field field)
        {
            //
            // Analyze the types.
            //
            switch (field.SharePointType)
            {
                //
                // Boolean == bool or bool?
                //
                case "Boolean":
                    field.FieldType = FieldType.Boolean;
                    field.RuntimeType = field.IsRequired ? typeof(bool) : typeof(bool?);
                    break;
                //
                // Text == string
                //
                case "Text":
                    field.FieldType = FieldType.Text;
                    field.RuntimeType = typeof(string);
                    break;
                //
                // Note == string
                //
                case "Note":
                    field.FieldType = FieldType.Note;
                    field.RuntimeType = typeof(string);
                    break;
                //
                // DateTime == System.DateTime
                //
                case "DateTime":
                    field.FieldType = FieldType.DateTime;
                    field.RuntimeType = field.IsRequired ? typeof(DateTime) : typeof(DateTime?);
                    break;
                //
                // Number == double or double?
                //
                case "Number":
                    field.FieldType = FieldType.Number;
                    field.RuntimeType = field.IsRequired ? typeof(double) : typeof(double?);
                    break;
                //
                // Currency == double or double?
                //
                case "Currency":
                    field.FieldType = FieldType.Currency;
                    field.RuntimeType = field.IsRequired ? typeof(double) : typeof(double?);
                    break;
                //
                // Counter == int (used for primary key field)
                //
                case "Counter":
                    field.FieldType = FieldType.Counter;
                    field.RuntimeType = typeof(int);
                    break;
                //
                // URL == UrlValue (helper object from LINQ to SharePoint)
                //
                case "URL":
                    field.FieldType = FieldType.URL;
                    field.RuntimeType = typeof(Url);
                    break;
                //
                // Choice and MultiChoice are mapped on helper enums, (optionally) together with a helper string field for fill-in choices.
                // The type is made nullable if a value is not required.
                //
                case "Choice":
                case "MultiChoice":
                    field.FieldType = (field.SharePointType == "Choice" ? FieldType.Choice : FieldType.MultiChoice);
                    break;
                //
                // Lookup fields require the generation of another linked entity type.
                //
                case "Lookup":
                    field.FieldType = FieldType.Lookup;
                    break;
                //
                // LookupMulti fields require the generation of another linked entity type.
                //
                case "LookupMulti":
                    field.FieldType = FieldType.LookupMulti;
                    break;
                //
                // Currently no support for User and UserMulti fields.
                //
                case "User":
                case "UserMulti":
                    break;
                //
                // Ignore other types too.
                //
                default:
                    field.FieldType = FieldType.None;
                    field.RuntimeType = null;
                    break;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the SPML representation for the Field element.
        /// </summary>
        /// <returns>SPML XML element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public XmlNode ToSpml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement field = doc.CreateElement("Field");
            field.Attributes.Append(doc.CreateAttribute("Name")).Value = this.Name;
            field.Attributes.Append(doc.CreateAttribute("DisplayName")).Value = this.DisplayName;
            field.Attributes.Append(doc.CreateAttribute("Type")).Value = this.SharePointType;
            if (!string.IsNullOrEmpty(this.Description))
                field.Attributes.Append(doc.CreateAttribute("Description")).Value = this.Description;
            if (!string.IsNullOrEmpty(this.Alias))
                field.Attributes.Append(doc.CreateAttribute("Alias")).Value = this.Alias;
            field.Attributes.Append(doc.CreateAttribute("Id")).Value = this.Id.ToString("D");

            if (this.IsHidden)
                field.Attributes.Append(doc.CreateAttribute("Hidden")).Value = "true";
            if (this.IsReadOnly)
                field.Attributes.Append(doc.CreateAttribute("ReadOnly")).Value = "true";
            if (this.IsPrimaryKey)
                field.Attributes.Append(doc.CreateAttribute("PrimaryKey")).Value = "true";
            if (this.IsCalculated)
                field.Attributes.Append(doc.CreateAttribute("Calculated")).Value = "true";
            if (this.IsRequired)
                field.Attributes.Append(doc.CreateAttribute("Required")).Value = "true";

            if (this.FieldType == FieldType.Choice || this.FieldType == FieldType.MultiChoice)
            {
                if (this.FillInChoiceEnabled)
                    field.Attributes.Append(doc.CreateAttribute("FillInChoice")).Value = "true";

                XmlElement choices = doc.CreateElement("Choices");
                foreach (Choice choice in this.Choices)
                    choices.AppendChild(doc.ImportNode(choice.ToSpml(), true));
                field.AppendChild(choices);
            }

            if (this.FieldType == FieldType.Lookup || this.FieldType == FieldType.LookupMulti)
            {
                field.Attributes.Append(doc.CreateAttribute("LookupList")).Value = this.LookupList;
                field.Attributes.Append(doc.CreateAttribute("LookupField")).Value = this.LookupField;
            }

            return field;
        }

        #endregion
    }
}
