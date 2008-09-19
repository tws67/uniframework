//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Data Access Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.Data.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information for an Oracle database connection defined by the Oracle
	/// specific connection information configuration section.
	/// </summary>
	/// <remarks>
	/// The collection of <see cref="Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OraclePackageData"/> instances
	/// defined by an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleConnectionData"/>
	/// is represented as a <see cref="String"/> array contaning key/value pairs.
	/// </remarks>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleConnectionSettings"/>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Data.Oracle.Configuration.OracleConnectionData"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class OracleConnectionSetting : NamedConfigurationSetting
	{
		private String[] packages;

		internal OracleConnectionSetting(String name, String[] packages)
			: base(name)
		{
			this.packages = packages;
		}

		/// <summary>
		/// Gets the package mapping information specified by the represented Oracle connection data object as a
		/// <see cref="String"/> array of key/value pairs.
		/// </summary>
		public String[] Packages
		{
			get { return packages; }
			internal set { packages = value; }
		}
	}
}
