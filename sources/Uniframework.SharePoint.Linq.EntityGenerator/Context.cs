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
 * 0.2.3 - Introduction of Context class + SPML conversions
 * 0.2.4 - Configurable support for SharePoint Object Model data provider
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Represents a SharePointDataContext with all the information required by LINQ to SharePoint.
    /// </summary>
    public class Context
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Context()
        {
            Lists = new List<List>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Friendly name for the data context.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Url to the SharePoint site.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Connection parameters.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Lists contained by the data context.
        /// </summary>
        public IList<List> Lists { get; internal set; }

        /// <summary>
        /// Indicates whether or not the SharePoint Object Model provider is to be supported directly on the generated data context.
        /// </summary>
        public bool EnableObjectModelProvider { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a Context definition object from a SPML data context definition.
        /// </summary>
        /// <param name="spml">SharePoint data context definition in SPML.</param>
        /// <returns>Context definition object for the specified data context.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spml")]
        public static Context FromSpml(XmlNode spml)
        {
            if (spml == null)
                throw new ArgumentNullException("spml");

            //
            // Context object.
            //
            Context context = new Context();

            //
            // Set general context information.
            //
            context.Name = spml.Attributes["Name"].Value;
            context.Url = new Uri(spml.Attributes["Url"].Value);
            context.Connection = Connection.FromSpml(spml["Connection"]);

            //
            // Check SharePoint Object Model support.
            //
            XmlAttribute enableObjectModelProvider = spml.Attributes["EnableObjectModelProvider"];
            if (enableObjectModelProvider != null)
                context.EnableObjectModelProvider = Helpers.ParseBool(enableObjectModelProvider.Value);

            //
            // Get lists.
            //
            if (spml["Lists"] != null)
                foreach (XmlNode c in spml["Lists"].ChildNodes)
                    context.Lists.Add(List.FromSpml(c));

            //
            // Return context definition object.
            //
            return context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the SPML representation for the Context element.
        /// </summary>
        /// <returns>SPML XML element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public XmlNode ToSpml()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement ctx = doc.CreateElement("SharePointDataContext");
            doc.AppendChild(ctx);

            XmlAttribute name = doc.CreateAttribute("Name");
            name.Value = Name ?? "";
            ctx.Attributes.Append(name);

            XmlAttribute url = doc.CreateAttribute("Url");
            url.Value = Url.ToString();
            ctx.Attributes.Append(url);

            if (this.EnableObjectModelProvider)
                ctx.Attributes.Append(doc.CreateAttribute("EnableObjectModelProvider")).Value = "true";

            XmlAttribute ns = doc.CreateAttribute("xmlns");
            ns.Value = "http://www.codeplex.com/LINQtoSharePoint/SPML.xsd";
            ctx.Attributes.Append(ns);

            XmlElement lists = doc.CreateElement("Lists");
            ctx.AppendChild(lists);
            foreach (List list in Lists)
                lists.AppendChild(doc.ImportNode(list.ToSpml(), true));

            ctx.AppendChild(doc.ImportNode(Connection.ToSpml(), true));

            return ctx;
        }

        #endregion
    }
}
