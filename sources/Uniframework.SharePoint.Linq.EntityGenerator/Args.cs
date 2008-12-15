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
 * 0.2.1 - Entity generator creation
 * 0.2.2 - New entity model
 */

#region Namespace imports

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;

#endregion

namespace Uniframework.SharePoint.Linq.EntityGenerator
{
    /// <summary>
    /// Helper class to parse and validate the command-line arguments.
    /// </summary>
    public class EntityGeneratorArgs
    {
        #region Properties

        /// <summary>
        /// Run-mode to run the entity generator in.
        /// </summary>
        public RunModes RunMode { get; set; }

        /// <summary>
        /// Connection parameters to connect to SharePoint.
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Namespace to put code in.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Language to generate entity class code in.
        /// </summary>
        public Language Language { get; set; }

        #endregion
    }

    /// <summary>
    /// Supported code generation languages.
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// Generate C# code.
        /// </summary>
        CSharp,

        /// <summary>
        /// Generate VB.NET code.
        /// </summary>
        VB
    }

    /// <summary>
    /// Specifies the run-mode of the export.
    /// </summary>
    /// <remarks>
    /// Valid combinations:
    /// - Online | CodeGen
    /// - Online | Export
    /// - Offline | CodeGen
    /// </remarks>
    [Flags]
    public enum RunModes
    {
        /// <summary>
        /// Online.
        /// </summary>
        Online = 1,

        /// <summary>
        /// Offline
        /// </summary>
        Offline = 2,

        /// <summary>
        /// Generate code.
        /// </summary>
        CodeGen = 4,

        /// <summary>
        /// Export SPML.
        /// </summary>
        Export = 8
    }

    /// <summary>
    /// Connection parameters for communication with SharePoint.
    /// </summary>
    public class Connection
    {
        #region Properties

        /// <summary>
        /// Url of SharePoint site to connect to.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Indicates whether or not custom authentication is enabled.
        /// </summary>
        public bool CustomAuthentication { get; set; }

        /// <summary>
        /// User name to connect to SharePoint.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Password to connect to SharePoint.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Domain to connect to SharePoint.
        /// </summary>
        public string Domain { get; set; }

        #endregion

        #region Factory methods

        /// <summary>
        /// Creates a Connection definition object from a SPML connection definition.
        /// </summary>
        /// <param name="spml">SharePoint connection definition in SPML.</param>
        /// <returns>Connection definition object for the specified connection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "spml")]
        public static Connection FromSpml(XmlNode spml)
        {
            if (spml == null)
                throw new ArgumentNullException("spml");

            //
            // Connection object.
            //
            Connection conn = new Connection();

            //
            // Set general connection information.
            //
            conn.Url = new Uri(spml.Attributes["Url"].Value);

            XmlAttribute user = spml.Attributes["User"];
            conn.CustomAuthentication = user != null;
            if (conn.CustomAuthentication)
            {
                conn.User = user.Value;

                XmlAttribute password = spml.Attributes["Password"];
                if (password != null)
                    conn.Password = password.Value;

                XmlAttribute domain = spml.Attributes["Domain"];
                if (domain != null)
                    conn.Domain = domain.Value;
            }

            //
            // Return connection object.
            //
            return conn;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the SPML representation for the Connection element.
        /// </summary>
        /// <returns>SPML XML element.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Spml")]
        public XmlNode ToSpml()
        {
            XmlDataDocument doc = new XmlDataDocument();
            XmlElement conn = doc.CreateElement("Connection");

            XmlAttribute url = doc.CreateAttribute("Url");
            url.Value = Url.ToString();
            conn.Attributes.Append(url);

            if (CustomAuthentication)
            {
                XmlAttribute user = doc.CreateAttribute("User");
                user.Value = User ?? "";
                conn.Attributes.Append(user);

                XmlAttribute password = doc.CreateAttribute("Password");
                password.Value = Password ?? "";
                conn.Attributes.Append(password);

                if (Domain != null)
                {
                    XmlAttribute domain = doc.CreateAttribute("Domain");
                    domain.Value = Domain;
                    conn.Attributes.Append(domain);
                }
            }

            return conn;
        }

        #endregion
    }
}
