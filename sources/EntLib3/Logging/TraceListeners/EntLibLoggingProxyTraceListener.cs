//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
	/// <summary>
	/// Proxy listener for writing the log using the <see cref="Logger"/> class.
	/// </summary>
	public class EntLibLoggingProxyTraceListener : TraceListener
	{
		private static Regex XPathRegex
			= new Regex(@"
						(?<path>.*?)			# but non greedy path
						(						# until either
							$					# the string ends
								|				# or
							(?<!\\);			# a non escaped ; is found
						)
						", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);
		private static Regex NamespaceRegex
			= new Regex(@"
						xmlns						# the 'xmlns' prefix
							:(?<prefix>[^'""]+?)	# followed by a mandatory prefix name
						=							# the '=' sign
							(?<quote>[""'])			# a start quote
								(?<uri>.*?)			# the uri
							\<quote>				# the matching end quote
						", RegexOptions.IgnorePatternWhitespace | RegexOptions.ExplicitCapture);


		private const string TraceEventCacheKey = "TraceEventCache";
		private static readonly string[] SupportedAttributes = new string[] { "categoriesXPathQueries", "namespaces" };
		private IList<string> categoriesXPathQueries;
		private XmlNamespaceManager xmlNamespaceManager;

		/// <summary>
		/// Writes trace information, a data object and event information through the Logging Block.
		/// </summary>
		/// <param name="eventCache">A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">An identification of the source of the trace request.</param>
		/// <param name="eventType">One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="data">The trace data to emit.</param>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add(TraceEventCacheKey, eventCache);
			LogEntry logEntry = null;
			if (data is XPathNavigator)
			{
				XPathNavigator xmlData = data as XPathNavigator;
				List<string> categories = new List<string>();
				categories.Add(source);

				foreach (string xpathQuery in this.CategoriesXPathQueries)
				{
					XPathNodeIterator nodeIterator = xmlData.Select(xpathQuery, this.NamespaceManager);
					foreach (object value in nodeIterator)
					{
						categories.Add(((XPathNavigator)value).Value);
					}
				}

				XmlLogEntry xmlLogEntry = new XmlLogEntry(data, categories, int.MaxValue, id, eventType, null, properties);
				xmlLogEntry.Xml = xmlData;
				logEntry = xmlLogEntry;
			}
			else if (data is LogEntry)
			{
				logEntry = data as LogEntry;
			}
			else
			{
				logEntry = new LogEntry(data,
					string.IsNullOrEmpty(source) ? new string[0] : new string[] { source },
					int.MaxValue, id, eventType, null, properties);
			}
			Logger.Write(logEntry);
		}

		/// <summary>
		/// Writes trace information, a formatted array of objects and event information through the Logging Block.
		/// </summary>
		/// <param name="eventCache">A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">An identification of the source of the trace request.</param>
		/// <param name="eventType">One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="format">A format string that contains zero or more format items, which correspond to objects in the <paramref name="args"/> array.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
		{
			if (args != null)
			{
				this.TraceEvent(eventCache, source, eventType, id, string.Format(CultureInfo.InvariantCulture, format, args));
			}
			else
			{
				this.TraceEvent(eventCache, source, eventType, id, format);
			}
		}

		/// <summary>
		/// Writes trace information, a message, and event information through the Logging Block.
		/// </summary>
		/// <param name="eventCache">A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">An identification of the source of the trace request.</param>
		/// <param name="eventType">One of the <see cref="TraceEventType"/> values specifying the type of event that has caused the trace.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">A message to write.</param>
		public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
		{
			this.TraceData(eventCache, source, eventType, id, message);
		}

		/// <summary>
		/// Writes trace information, a message, a related activity identity and event information.
		/// </summary>
		/// <param name="eventCache">A <see cref="TraceEventCache"/> object that contains the current process ID, thread ID, and stack trace information.</param>
		/// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
		/// <param name="id">A numeric identifier for the event.</param>
		/// <param name="message">A message to write.</param>
		/// <param name="relatedActivityId">A <see cref="Guid"/> object identifying a related activity.</param>
		/// <remarks>The <paramref name="relatedActivityId"/> is saved to a <see cref="LogEntry"/> so the logging infrastructure can reconstruct the transfer message.</remarks>
		public override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add(TraceEventCacheKey, eventCache);

			LogEntry logEntry = new LogEntry(string.IsNullOrEmpty(message) ? string.Empty : message,
				string.IsNullOrEmpty(source) ? new string[0] : new string[] { source },
				int.MaxValue, id, TraceEventType.Transfer, null, properties);
			logEntry.RelatedActivityId = relatedActivityId;

			Logger.Write(logEntry);
		}

		/// <summary>
		/// Writes the specified message through the Logging Block.
		/// </summary>
		public override void Write(string message)
		{
			this.WriteLine(message);
		}

		/// <summary>
		/// Writes the specified message through the Logging Block.
		/// </summary>
		public override void WriteLine(string message)
		{
			this.TraceData(new TraceEventCache(), string.Empty, TraceEventType.Information, 0, message);
		}

		/// <summary>
		/// Gets the supported attributes, namely "categoriesXPathQueries" and "namespaces".
		/// </summary>
		protected override string[] GetSupportedAttributes()
		{
			return SupportedAttributes;
		}

		/// <summary>
		/// Gets the value indicating the receiver is thread safe.
		/// </summary>
		public override bool IsThreadSafe
		{
			get
			{
				return true;
			}
		}

		internal IList<string> CategoriesXPathQueries
		{
			get
			{
				if (categoriesXPathQueries == null)
				{
					string attribute = this.Attributes["categoriesXPathQueries"];
					if (!string.IsNullOrEmpty(attribute))
					{
						categoriesXPathQueries = SplitXPathQueriesString(attribute);
					}
					else
					{
						categoriesXPathQueries = new List<string>(0);
					}
				}
				return categoriesXPathQueries;
			}
		}

		internal XmlNamespaceManager NamespaceManager
		{
			get
			{
				if (this.xmlNamespaceManager == null)
				{
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());

					string attribute = this.Attributes["namespaces"];
					if (!string.IsNullOrEmpty(attribute))
					{
						foreach (KeyValuePair<string, string> kvp in SplitNamespacesString(attribute))
						{
							xmlNamespaceManager.AddNamespace(kvp.Key, kvp.Value);
						}
					}

					this.xmlNamespaceManager = xmlNamespaceManager;
				}
				return this.xmlNamespaceManager;
			}
		}

		internal static IList<string> SplitXPathQueriesString(string xpathsStrings)
		{
			List<string> xpaths = new List<string>();

			xpathsStrings = xpathsStrings.Trim();
			if (xpathsStrings.Length > 0)
			{
				Match match = XPathRegex.Match(xpathsStrings);
				while (match.Success)
				{
					string xpath = match.Groups["path"].Value.Replace(@"\;", ";");
					if (xpath.Length > 0)
					{
						xpaths.Add(xpath);
					}
					match = match.NextMatch();
				}
			}

			return xpaths;
		}

		internal static IDictionary<string, string> SplitNamespacesString(string namespacesString)
		{
			Dictionary<string, string> namespaces = new Dictionary<string, string>();

			namespacesString = namespacesString.Trim();
			if (namespacesString.Length > 0)
			{
				Match match = NamespaceRegex.Match(namespacesString);
				while (match.Success)
				{
					string prefix = match.Groups["prefix"].Value;
					string uri = match.Groups["uri"].Value;

					namespaces[prefix] = uri;

					match = match.NextMatch();
				}
			}

			return namespaces;
		}
	}
}