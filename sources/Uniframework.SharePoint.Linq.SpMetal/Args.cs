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
 * 0.2.0 - Restructuring of class files in project
 * 0.2.1 - Namespace support
 *         Refactoring of argument parsing to Args class.
 */

#region Namespace imports

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Uniframework.SharePoint.Linq.EntityGenerator;


#endregion

namespace Uniframework.SharePoint.Linq.SPMetal
{
    /// <summary>
    /// Helper class to parse and validate the command-line arguments.
    /// </summary>
    public class Args
    {
        #region Properties

        /// <summary>
        /// Run-mode to run the entity generator in.
        /// </summary>
        public RunModes RunMode { get; set; }

        #region {online}

        /// <summary>
        /// Url to the SharePoint site.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Name of the list on the SharePoint site to query for.
        /// </summary>
        public string List { get; set; }

        /// <summary>
        /// Name of the SharePoint data context to generate.
        /// </summary>
        public string Context { get; set; }

        /// <summary>
        /// Enables the SharePoint Object Model data provider.
        /// </summary>
        public bool EnableSom { get; set; }

        #endregion

        #region {connect}

        /// <summary>
        /// User name to connect to the SharePoint web services (optional).
        /// </summary>
        /// <remarks>If not specified, integrated authentication will be used using default network credentials.</remarks>
        public string User { get; set; }

        /// <summary>
        /// Password to connect to the SharePoint web services (optional).
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Domain name to connect to the SharePoint web services (optional).
        /// </summary>
        public string Domain { get; set; }

        #endregion

        #region {codegen}

        /// <summary>
        /// Namespace to put code in.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Output file (optional).
        /// </summary>
        /// <remarks>If no output file is specified, the list's name will be used with an extension of .cs.</remarks>
        public string Code { get; set; }

        /// <summary>
        /// Output code language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Auto-(de)pluralization of entity names.
        /// </summary>
        public bool Pluralize { get; set; }

        #endregion

        #region {offline}

        /// <summary>
        /// Input SPML file.
        /// </summary>
        public string In { get; set; }

        #endregion

        #region {export}

        /// <summary>
        /// Output SPML file.
        /// </summary>
        public string Xml { get; set; }

        #endregion

        #endregion

        #region Factory methods

        /// <summary>
        /// Parses the given command-line arguments.
        /// </summary>
        /// <param name="args">Arguments to be parsed.</param>
        /// <returns>An instance of Args containing the argument values; null if the argument list is invalid.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public static Args Parse(string[] args)
        {
            //
            // Find arguments and check for mutual exclusiveness of online/offline run mode.
            //
            Dictionary<string, string> arguments = FindArgs(args);
            if (!(arguments.ContainsKey("url") ^ arguments.ContainsKey("in")))
                return null;

            //
            // Resulting arguments object.
            //
            Args res = new Args();

            //
            // Online mode.
            //
            if (arguments.ContainsKey("url"))
            {
                //
                // Check for SPML "xml" output argument.
                //
                string xml;
                arguments.TryGetValue("xml", out xml);

                //
                // Code (out) can optionally be empty; in that case, take the lists's name suffixed with the language extension.
                // v0.2.0.0 - File name will be inferred from the loaded list name, not the -list parameter which could be a GUID too.
                //
                string code;
                if (!arguments.TryGetValue("code", out code))
                    arguments.TryGetValue("out", out code);

                //
                // Only one mode is supported: CodeGen or Export.
                //
                if (xml != null && code != null)
                    return null;
                //
                // If Export mode, xml shouldn't be null.
                //
                else if (xml != null)
                {
                    res.Xml = xml;
                    res.RunMode = RunModes.Online | RunModes.Export;
                }
                //
                // Code parameter can be null.
                //
                else
                {
                    res.Code = code;
                    res.RunMode = RunModes.Online | RunModes.CodeGen;
                }
            }
            //
            // Offline mode.
            //
            else if (arguments.ContainsKey("in"))
            {
                res.RunMode = RunModes.Offline | RunModes.CodeGen;

                //
                // In shouldn't be empty.
                //
                string input;
                if (!arguments.TryGetValue("in", out input) || input.Length == 0)
                    return null;
                res.In = input;
            }
            else
                Debug.Assert(false);

            if ((res.RunMode & RunModes.Online) == RunModes.Online)
            {
                //
                // Url shouldn't be empty and should start with either http:// or https://.
                //
                string url;
                if (!arguments.TryGetValue("url", out url) || url.Length == 0 || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                    return null;
                res.Url = new Uri(url);
                if (!(url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && url.Length > 7) && !(url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) && url.Length > 8))
                    return null;

                //
                // List shouldn't be empty.
                //
                string list;
                if (!arguments.TryGetValue("list", out list) || list.Length == 0)
                    return null;
                res.List = list;

                //
                // Context can be empty.
                //
                string context;
                if (arguments.TryGetValue("context", out context) && list.Length != 0)
                {
                    res.Context = context;

                    //
                    // Support for SharePoint Object Model data provider.
                    //
                    res.EnableSom = arguments.ContainsKey("enablesom");
                }

                //
                // Only process authentication arguments if a user argument is present.
                //
                string user;
                if (arguments.TryGetValue("user", out user) && user.Length != 0)
                {
                    //
                    // Password required if user has been specified. Password can be the empty string.
                    //
                    string password;
                    if (arguments.TryGetValue("password", out password))
                    {
                        res.User = user;
                        res.Password = password;

                        //
                        // Optional domain name.
                        //
                        string domain;
                        if (arguments.TryGetValue("domain", out domain) && domain.Length != 0)
                            res.Domain = domain;
                    }
                    else
                        return null;
                }
            }

            if ((res.RunMode & RunModes.CodeGen) == RunModes.CodeGen)
            {
                //
                // Output language can be set optionally and should be CS or VB (case-insensitive).
                // We use a string instead of an enum since we'll pass the language name to CodeDOM.
                //
                string language;
                if (arguments.TryGetValue("language", out language) && language.Length != 0)
                {
                    language = language.ToLowerInvariant();

                    switch (language)
                    {
                        case "cs":
                        case "vcs":
                        case "c#":
                        case "csharp":
                        case "vcsharp":
                            res.Language = "CS";
                            break;
                        case "vb":
                        case "visualbasic":
                            res.Language = "VB";
                            break;
                        default:
                            return null;
                    }
                }
                else
                    res.Language = "CS";

                //
                // v0.2.1.0 - Namespace support.
                //
                string ns;
                arguments.TryGetValue("namespace", out ns);
                res.Namespace = ns;

                //
                // Auto-(de)pluralization.
                //
                res.Pluralize = arguments.ContainsKey("pluralize");
            }

            return res;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Finds the command-line arguments and parses the key and value.
        /// </summary>
        /// <param name="args">Argument list to search for key/value argument pairs.</param>
        /// <returns>Dictionary with key/value argument pairs.</returns>
        /// <example>-key:value, /key:value, -key:"value with spaces"</example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private static Dictionary<string, string> FindArgs(string[] args)
        {
            Dictionary<string, string> res = new Dictionary<string, string>();

            Regex r = new Regex(@"[-/](?<option>\w+)(:(""(?<value>.*)""|(?<value>.*)))?", RegexOptions.CultureInvariant | RegexOptions.Compiled);
            foreach (string arg in args)
                foreach (Match m in r.Matches(arg))
                    res.Add(m.Groups["option"].Value.ToLowerInvariant(), m.Groups["value"].Value);

            return res;
        }

        #endregion
    }
}
