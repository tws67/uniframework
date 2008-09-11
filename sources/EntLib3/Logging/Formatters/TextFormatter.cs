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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
	/// <summary>
	/// Represents a template based formatter for <see cref="LogEntry"/> messages.
	/// </summary>
	[ConfigurationElementType(typeof(TextFormatterData))]
	public class TextFormatter : LogFormatter
	{
		/// <summary>
		/// Message template containing tokens.
		/// </summary>
		private string template;

		/// <summary>
		/// Array of token formatters.
		/// </summary>
		private ArrayList tokenFunctions;

		private const string timeStampToken = "{timestamp}";
		private const string messageToken = "{message}";
		private const string categoryToken = "{category}";
		private const string priorityToken = "{priority}";
		private const string eventIdToken = "{eventid}";
		private const string severityToken = "{severity}";
		private const string titleToken = "{title}";
		private const string errorMessagesToke = "{errorMessages}";

		private const string machineToken = "{machine}";
		private const string appDomainNameToken = "{appDomain}";
		private const string processIdToken = "{processId}";
		private const string processNameToken = "{processName}";
		private const string threadNameToken = "{threadName}";
		private const string win32ThreadIdToken = "{win32ThreadId}";
		private const string activityidToken = "{activity}";

		private const string NewLineToken = "{newline}";
		private const string TabToken = "{tab}";

		/// <summary>
		/// Initializes a new instance of a <see cref="TextFormatter"></see>
		/// </summary>
		/// <param name="template">Template to be used when formatting.</param>
		public TextFormatter(string template)
		{
            if (!string.IsNullOrEmpty(template))
            {
                this.template = template;
            }
            else
            {
                this.template = Resources.DefaultTextFormat;
            }
			RegisterTokenFunctions();
		}

		/// <summary>
		/// Initializes a new instance of a <see cref="TextFormatter"></see> with a default template.
		/// </summary>
		public TextFormatter()
			: this(Resources.DefaultTextFormat)
		{
		}

		/// <summary>
		/// Gets or sets the formatting template.
		/// </summary>
		public string Template
		{
			get { return template; }
			set { template = value; }
		}


		/// <overloads>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values
		/// </overloads>
		/// <summary>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values.
		/// </summary>
		/// <param name="log">Log entry to format.</param>
		/// <returns>Formatted string with tokens replaced with property values.</returns>
		public override string Format(LogEntry log)
		{
			return Format(CreateTemplateBuilder(), log);
		}


		/// <summary>
		/// Formats the <see cref="LogEntry"/> object by replacing tokens with values writing the format result
		/// to a <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="templateBuilder">The <see cref="StringBuilder"/> that holds the formatting result.</param>
		/// <param name="log">Log entry to format.</param>
		/// <returns>Formatted string with tokens replaced with property values.</returns>
		protected virtual string Format(StringBuilder templateBuilder, LogEntry log)
		{
			templateBuilder.Replace(timeStampToken, log.TimeStampString);
			templateBuilder.Replace(titleToken, log.Title);
			templateBuilder.Replace(messageToken, log.Message);
			templateBuilder.Replace(eventIdToken, log.EventId.ToString(Resources.Culture));
			templateBuilder.Replace(priorityToken, log.Priority.ToString(Resources.Culture));
			templateBuilder.Replace(severityToken, log.Severity.ToString());
			templateBuilder.Replace(errorMessagesToke, 
				log.ErrorMessages != null ? log.ErrorMessages.ToString() : null);

			templateBuilder.Replace(machineToken, log.MachineName);
			templateBuilder.Replace(appDomainNameToken, log.AppDomainName);
			templateBuilder.Replace(processIdToken, log.ProcessId);
			templateBuilder.Replace(processNameToken, log.ProcessName);
			templateBuilder.Replace(threadNameToken, log.ManagedThreadName);
			templateBuilder.Replace(win32ThreadIdToken, log.Win32ThreadId);
			templateBuilder.Replace(activityidToken, log.ActivityId.ToString("D", Resources.Culture));

			templateBuilder.Replace(categoryToken, FormatCategoriesCollection(log.Categories));

			FormatTokenFunctions(templateBuilder, log);

			templateBuilder.Replace(NewLineToken, Environment.NewLine);
			templateBuilder.Replace(TabToken, "\t");

			return templateBuilder.ToString();
		}

		/// <summary>
		/// Provides a textual representation of a categories list.
		/// </summary>
		/// <param name="categories">The collection of categories.</param>
		/// <returns>A comma delimited textural representation of the categories.</returns>
		public static string FormatCategoriesCollection(ICollection<string> categories)
		{
			StringBuilder categoriesListBuilder = new StringBuilder();
			int i = 0;
			foreach (String category in categories)
			{
				categoriesListBuilder.Append(category);
				if (++i < categories.Count)
				{
					categoriesListBuilder.Append(", ");
				}
			}
			return categoriesListBuilder.ToString();
		}

		/// <summary>
		/// Creates a new builder to hold the formatting results based on the receiver's template.
		/// </summary>
		/// <returns>The new <see cref="StringBuilder"/>.</returns>
		protected StringBuilder CreateTemplateBuilder()
		{
			StringBuilder templateBuilder =
							new StringBuilder((this.template == null) || (this.template.Length > 0) ? this.template : Resources.DefaultTextFormat);
			return templateBuilder;
		}

		private void FormatTokenFunctions(StringBuilder templateBuilder, LogEntry log)
		{
			foreach (TokenFunction token in tokenFunctions)
			{
				token.Format(templateBuilder, log);
			}
		}

		private void RegisterTokenFunctions()
		{
			tokenFunctions = new ArrayList();
			tokenFunctions.Add(new DictionaryToken());
			tokenFunctions.Add(new KeyValueToken());
			tokenFunctions.Add(new TimeStampToken());
			tokenFunctions.Add(new ReflectedPropertyToken());
		}
	}
}