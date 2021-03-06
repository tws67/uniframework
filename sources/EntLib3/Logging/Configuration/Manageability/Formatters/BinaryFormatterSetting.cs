//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
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
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.BinaryLogFormatterData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class BinaryFormatterSetting : FormatterSetting
	{
		internal BinaryFormatterSetting(String name)
			: base(name)
		{ }
	}
}
