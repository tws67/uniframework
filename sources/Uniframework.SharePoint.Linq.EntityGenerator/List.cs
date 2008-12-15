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
 * 0.2.1 - Introduction of List class
 * 0.2.3 - SPML conversions
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Globalization;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Represents a SharePoint list with all the information required by LINQ to SharePoint.
    /// </summary>
    public class List
    {
        #region Static properties

        /// <summary>
        /// Enables/disables auto-pluralization/singularization for English list names into a deducted entity alias.
        /// </summary>
        /// <remarks>Only affects SPML generation (online mode).</remarks>
        public static bool AutoPluralize { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// List identifier.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Metadata")]
        [Description("List identifier.")]
        public Guid Id { get; set; }

        /// <summary>
        /// List name.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("List name.")]
        [ParenthesizePropertyName(true)]
        public string Name { get; set; }

        /// <summary>
        /// List description.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Identification")]
        [Description("List description. Will be used for the comment on the corresponding entity class.")]
        public string Description { get; set; }

        /// <summary>
        /// Version of the list, retrieved at entity generation time.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("Version of the list.")]
        public int Version { get; set; }

        /// <summary>
        /// Relative path to the list on the SharePoint site.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("Relative path to the list on the SharePoint site.")]
        public string Path { get; set; }

        /// <summary>
        /// List of exported list fields.
        /// </summary>
        [Browsable(false)]
        public IList<Field> Fields { get; private set; }

        /// <summary>
        /// Mapping alias for the list.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mapping")]
        [Description("Mapping alias for the list. Will be used as the property name for the list reference in the data context.")]
        public string ListAlias { get; set; }

        /// <summary>
        /// Mapping alias for the entity type.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mapping")]
        [Description("Mapping alias for the list entity. Will be used as the type name for the list entity.")]
        public string EntityAlias { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a List definition object from a CAML list definition.
        /// </summary>
        /// <param name="listDefinition">SharePoint list definition in CAML.</param>
        /// <returns>List definition object for the specified list.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Caml")]
        public static List FromCaml(XmlNode listDefinition)
        {
            if (listDefinition == null)
                throw new ArgumentNullException("listDefinition");

            //
            // List object.
            //
            List list = new List();

            //
            // Set general list information.
            //
            list.Name = (string)listDefinition.Attributes["Title"].Value;
            string listDescription = (string)listDefinition.Attributes["Description"].Value;
            if (!String.IsNullOrEmpty(listDescription))
                list.Description = null;
            list.Id = new Guid((string)listDefinition.Attributes["ID"].Value);
            list.Version = int.Parse(listDefinition.Attributes["Version"].Value, CultureInfo.InvariantCulture.NumberFormat);
            list.Path = (string)listDefinition.Attributes["RootFolder"].Value;

            //
            // Auto-pluralize?
            //
            if (List.AutoPluralize)
                list.EntityAlias = Helpers.Singularize(list.Name);

            //
            // Get fields.
            //
            list.Fields = new List<Field>();
            if (listDefinition["Fields"] != null)
            {
                foreach (XmlNode c in listDefinition["Fields"].ChildNodes)
                    list.Fields.Add(Field.FromCaml(c));
            }

            //
            // Return list definition object.
            //
            return list;
        }

        /// <summary>
        /// Creates a List definition object from a SPML list definition.
        /// </summary>
        /// <param name="spml">SharePoint list definition in SPML.</param>
        /// <returns>List definition object for the specified list.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spml"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public static List FromSpml(XmlNode spml)
        {
            if (spml == null)
                throw new ArgumentNullException("spml");

            //
            // List object.
            //
            List list = new List();

            //
            // Set general list information.
            //
            list.Name = spml.Attributes["Name"].Value;
            string listDescription = (string)spml.Attributes["Description"].Value;
            if (!String.IsNullOrEmpty(listDescription))
                list.Description = null;
            list.Id = new Guid((string)spml.Attributes["Id"].Value);
            list.Version = int.Parse(spml.Attributes["Version"].Value, CultureInfo.InvariantCulture.NumberFormat);
            list.Path = (string)spml.Attributes["Path"].Value;
            XmlAttribute entityAlias = spml.Attributes["EntityAlias"];
            if (entityAlias != null)
                list.EntityAlias = entityAlias.Value;
            XmlAttribute listAlias = spml.Attributes["ListAlias"];
            if (listAlias != null)
                list.ListAlias = listAlias.Value;

            //
            // Get fields.
            //
            list.Fields = new List<Field>();
            if (spml["Fields"] != null)
                foreach (XmlNode c in spml["Fields"].ChildNodes)
                    list.Fields.Add(Field.FromSpml(c));

            //
            // Return list definition object.
            //
            return list;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all of the fields supported by LINQ to SharePoint.
        /// </summary>
        /// <returns>Fields supported by LINQ to SharePoint.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<Field> GetKnownFields()
        {
            //
            // Export only fields that aren't hidden or the primary key field,
            // and for which the underlying type is recognized and supported by the mapper.
            //
            foreach (Field field in this.Fields)
                if (!field.IsHidden || field.IsPrimaryKey)
                    if (field.FieldType != FieldType.None)
                        yield return field;
        }

        /// <summary>
        /// Generates the SPML representation for the List element.
        /// </summary>
        /// <returns>SPML XML element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public XmlNode ToSpml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement list = doc.CreateElement("List");
            list.Attributes.Append(doc.CreateAttribute("Name")).Value = this.Name;
            list.Attributes.Append(doc.CreateAttribute("Description")).Value = this.Description;
            list.Attributes.Append(doc.CreateAttribute("Id")).Value = this.Id.ToString("D");
            list.Attributes.Append(doc.CreateAttribute("Version")).Value = this.Version.ToString(CultureInfo.InvariantCulture);
            list.Attributes.Append(doc.CreateAttribute("Path")).Value = this.Path;
            if (!string.IsNullOrEmpty(this.EntityAlias))
                list.Attributes.Append(doc.CreateAttribute("EntityAlias")).Value = this.EntityAlias;
            if (!string.IsNullOrEmpty(this.ListAlias))
                list.Attributes.Append(doc.CreateAttribute("ListAlias")).Value = this.ListAlias;

            XmlElement fields = doc.CreateElement("Fields");
            foreach (Field field in GetKnownFields())
                if (field.Include)
                    fields.AppendChild(doc.ImportNode(field.ToSpml(), true));
            list.AppendChild(fields);

            return list;
        }

        #endregion
    }
}
