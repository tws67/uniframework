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
using System.Diagnostics;
using System.ComponentModel;
using System.Configuration;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration
{
	/// <summary>
	/// Represents the configuration data for a <see cref="RollingFlatFileTraceListenerData"/>.
	/// </summary>	
	[Assembler(typeof(RollingTraceListenerAssembler))]
	public class RollingFlatFileTraceListenerData : TraceListenerData
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="TraceListenerData"/> class.
		/// </summary>
		public RollingFlatFileTraceListenerData()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RollingFlatFileTraceListenerData"/> class.
		/// </summary>
		/// <param name="name">The name for the configuration object.</param>
		/// <param name="traceOutputOptions">The trace options.</param>
		/// <param name="fileName"></param>
		/// <param name="footer"></param>
		/// <param name="header"></param>
		/// <param name="rollSizeKB"></param>
		/// <param name="timeStampPattern"></param>
		/// <param name="rollFileExistsBehavior"></param>
		/// <param name="rollInterval"></param>
		/// <param name="formatter"></param>
		public RollingFlatFileTraceListenerData(string name,
			string fileName,
			string header,
			string footer,
			int rollSizeKB,
			string timeStampPattern,
			RollFileExistsBehavior rollFileExistsBehavior,
			RollInterval rollInterval,
			TraceOptions traceOutputOptions,
			string formatter)
			: base(name, typeof(RollingFlatFileTraceListener), traceOutputOptions)
		{
			this.FileName = fileName;
			this.Header = header;
			this.Footer = footer;
			this.RollSizeKB = rollSizeKB;
			this.RollFileExistsBehavior = rollFileExistsBehavior;
			this.RollInterval = rollInterval;
			this.TimeStampPattern = timeStampPattern;
			this.Formatter = formatter;
		}

		private const string FileNamePropertyName = "fileName";
		/// <summary>
		/// FileName
		/// </summary>
		[ConfigurationProperty(FileNamePropertyName)]
		public string FileName
		{
			get { return (string)this[FileNamePropertyName]; }
			set { this[FileNamePropertyName] = value; }
		}

		private const string RollSizeKBPropertyName = "rollSizeKB";
		/// <summary>
		/// Roll Size KB 
		/// </summary>
		[ConfigurationProperty(RollSizeKBPropertyName)]
		public int RollSizeKB
		{
			get { return (int)this[RollSizeKBPropertyName]; }
			set { this[RollSizeKBPropertyName] = value; }
		}

		private const string TimeStampPatternPropertyName = "timeStampPattern";
		/// <summary>
		/// Time stamp
		/// </summary>
		[ConfigurationProperty(TimeStampPatternPropertyName)]
		public string TimeStampPattern
		{
			get { return (string)this[TimeStampPatternPropertyName]; }
			set { this[TimeStampPatternPropertyName] = value; }
		}

		private const string RollFileExistsBehaviorPropertyName = "rollFileExistsBehavior";
		/// <summary>
		/// Exists Behavior
		/// </summary>
		[ConfigurationProperty(RollFileExistsBehaviorPropertyName)]
		public RollFileExistsBehavior RollFileExistsBehavior
		{
			get { return (RollFileExistsBehavior)this[RollFileExistsBehaviorPropertyName]; }
			set { this[RollFileExistsBehaviorPropertyName] = value; }
		}

		private const string RollIntervalPropertyName = "rollInterval";
		/// <summary>
		/// Roll Intervall
		/// </summary>
		[ConfigurationProperty(RollIntervalPropertyName)]
		public RollInterval RollInterval
		{
			get { return (RollInterval)this[RollIntervalPropertyName]; }
			set { this[RollIntervalPropertyName] = value; }
		}

		private const string formatterNameProperty = "formatter";

		/// <summary>
		/// Gets and sets the formatter name.
		/// </summary>
		[ConfigurationProperty(formatterNameProperty, IsRequired = false)]
		public string Formatter
		{
			get { return (string)base[formatterNameProperty]; }
			set { base[formatterNameProperty] = value; }
		}

		private const string headerProperty = "header";
		/// <summary>
		/// Gets and sets the header.
		/// </summary>
		[ConfigurationProperty(headerProperty, IsRequired = false)]
		public string Header
		{
			get { return (string)base[headerProperty]; }
			set { base[headerProperty] = value; }
		}

		private const string footerProperty = "footer";
		/// <summary>
		/// Gets and sets the footer.
		/// </summary>
		[ConfigurationProperty(footerProperty, IsRequired = false)]
		public string Footer
		{
			get { return (string)base[footerProperty]; }
			set { base[footerProperty] = value; }
		}
	}

	/// <summary>
	/// This type supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
	/// Represents the process to build a <see cref="RollingFlatFileTraceListener"/> described by a <see cref="RollingFlatFileTraceListenerData"/> configuration object.
	/// </summary>
	/// <remarks>This type is linked to the <see cref="RollingFlatFileTraceListenerData"/> type and it is used by the  Custom Factory
	/// to build the specific <see cref="TraceListener"/> object represented by the configuration object.
	/// </remarks>
	public class RollingTraceListenerAssembler : TraceListenerAsssembler
	{
		/// <summary>
		/// This method supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
		/// Builds a <see cref="FlatFileTraceListener"/> based on an instance of <see cref="FlatFileTraceListenerData"/>.
		/// </summary>
		/// <seealso cref="TraceListenerCustomFactory"/>
		/// <param name="context">The <see cref="IBuilderContext"/> that represents the current building process.</param>
		/// <param name="objectConfiguration">The configuration object that describes the object to build. Must be an instance of <see cref="FlatFileTraceListenerData"/>.</param>
		/// <param name="configurationSource">The source for configuration objects.</param>
		/// <param name="reflectionCache">The cache to use retrieving reflection information.</param>
		/// <returns>A fully initialized instance of <see cref="FlatFileTraceListener"/>.</returns>
		public override TraceListener Assemble(IBuilderContext context, TraceListenerData objectConfiguration, IConfigurationSource configurationSource, ConfigurationReflectionCache reflectionCache)
		{
			RollingFlatFileTraceListenerData castObjectConfiguration
				= (RollingFlatFileTraceListenerData)objectConfiguration;

			ILogFormatter formatter = GetFormatter(context, castObjectConfiguration.Formatter, configurationSource, reflectionCache);

			RollingFlatFileTraceListener createdObject
				= new RollingFlatFileTraceListener(
					castObjectConfiguration.FileName,
					castObjectConfiguration.Header,
					castObjectConfiguration.Footer,
					formatter,
					castObjectConfiguration.RollSizeKB,
					castObjectConfiguration.TimeStampPattern,
					castObjectConfiguration.RollFileExistsBehavior,
					castObjectConfiguration.RollInterval
					);

			return createdObject;
		}
	}
}