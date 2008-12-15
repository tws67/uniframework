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
 * 0.2.3 - Introduction of Choice class
 */

#region Namespace imports

using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Represents a field value for a Choice or MultiChoice list field.
    /// </summary>
    public class Choice : ICloneable
    {
        #region Properties

        /// <summary>
        /// Choice name.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Identification")]
        [Description("Choice name.")]
        [ParenthesizePropertyName(true)]
        public string Name { get; set; }

        /// <summary>
        /// Choice description.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Identification")]
        [Description("Choice description. Will be used for the comment on the choice's corresponding enum value.")]
        public string Description { get; set; }

        /// <summary>
        /// Mapping alias for the choice.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Mapping")]
        [Description("Mapping alias for the choice. Will be used as the enum value name for the choice.")]
        public string Alias { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a Choice definition object from a CAML choice definition.
        /// </summary>
        /// <param name="choiceDefinition">SharePoint choice definition in CAML.</param>
        /// <returns>Choice definition object for the specified choice.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Caml")]
        public static Choice FromCaml(XmlNode choiceDefinition)
        {
            if (choiceDefinition == null)
                throw new ArgumentNullException("choiceDefinition");

            //
            // Choice object.
            //
            Choice choice = new Choice();

            //
            // Set general choice information.
            //
            choice.Name = choiceDefinition.InnerText;

            //
            // Return choice definition object.
            //
            return choice;
        }

        /// <summary>
        /// Creates a Choice definition object from a SPML choice definition.
        /// </summary>
        /// <param name="spml">SharePoint choice definition in SPML.</param>
        /// <returns>Choice definition object for the specified choice.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spml"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public static Choice FromSpml(XmlNode spml)
        {
            if (spml == null)
                throw new ArgumentNullException("spml");

            //
            // Choice object.
            //
            Choice choice = new Choice();

            //
            // Set general choice information.
            //
            choice.Name = spml.InnerText;
            XmlAttribute description = spml.Attributes["Description"];
            if (description != null)
                choice.Description = description.Value;

            XmlAttribute alias = spml.Attributes["Alias"];
            if (alias != null)
                choice.Alias = alias.Value;

            //
            // Return choice definition object.
            //
            return choice;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the SPML representation for the Choice element.
        /// </summary>
        /// <returns>SPML XML element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public XmlNode ToSpml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement choice = doc.CreateElement("Choice");
            choice.InnerText = this.Name;

            if (!string.IsNullOrEmpty(this.Alias))
            {
                XmlAttribute alias = doc.CreateAttribute("Alias");
                alias.Value = this.Alias;
                choice.Attributes.Append(alias);
            }

            if (!string.IsNullOrEmpty(this.Description))
            {
                XmlAttribute description = doc.CreateAttribute("Description");
                description.Value = this.Description;
                choice.Attributes.Append(description);
            }

            return choice;
        }

        /// <summary>
        /// Clones the Choice object.
        /// </summary>
        /// <returns>Clone of the Choice object.</returns>
        public object Clone()
        {
            Choice clone = new Choice();
            clone.Alias = this.Alias;
            clone.Description = this.Description;
            clone.Name = this.Name;
            return clone;
        }

        #endregion
    }

    /// <summary>
    /// Custom editor for Choice collection editing.
    /// </summary>
    [Serializable]
    public class ChoiceEditor : UITypeEditor
    {
        #region Overrides

        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
                return UITypeEditorEditStyle.Modal;
            else
                return base.GetEditStyle(context);
        }

        [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
                List<Choice> choices = value as List<Choice>;
                if (editorService != null && choices != null)
                {
                    //
                    // Clone the choices.
                    //
                    List<Choice> choices2 = new List<Choice>();
                    foreach (Choice c in choices)
                        choices2.Add((Choice)c.Clone());

                    //
                    // Show dialog and store results if dialog responded OK.
                    //
                    ChoiceEditorForm form = new ChoiceEditorForm(choices2);
                    if (editorService.ShowDialog(form) == DialogResult.OK)
                    {
                        choices.Clear();
                        choices.AddRange(choices2);
                        value = form.Choices;
                    }
                }
            }

            return value;
        }

        #endregion
    }

}
