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
using System.Management.Instrumentation;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.Formatters
{
	/// <summary>
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.CustomFormatterData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomFormatterSetting : FormatterSetting
	{
		private String formatterType;
		private String[] attributes;

		internal CustomFormatterSetting(String name, String formatterType, String[] attributes)
			: base(name)
		{
			this.formatterType = formatterType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the assembly qualified name of the formatter type for the represented configuration element.
		/// </summary>
		public String FormatterType
		{
			get { return formatterType; }
			internal set { formatterType = value; }
		}

		/// <summary>
		/// Gets the attributes for the represented configuration element.
		/// </summary>
		/// <remarks>
		/// The attributes are encoded as an string array of name/value pairs.
		/// </remarks>
		public String[] Attributes
		{
			get { return attributes; }
			internal set { attributes = value; }
		}
	}
}
