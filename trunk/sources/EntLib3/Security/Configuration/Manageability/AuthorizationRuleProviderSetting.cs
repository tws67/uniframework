//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Security Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.AuthorizationRuleProviderData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.AuthorizationRuleProviderData"/>
	/// <seealso cref="NamedConfigurationSetting"/>
	/// <seealso cref="ConfigurationSetting"/>	
	[InstrumentationClass(InstrumentationType.Instance)]
	public class AuthorizationRuleProviderSetting : AuthorizationProviderSetting
	{
		private String[] rules;

		internal AuthorizationRuleProviderSetting(String name, String[] rules)
			: base(name)
		{
			this.rules = rules;
		}

		/// <summary>
		/// Gets the collection of rules represented as a 
		/// <see cref="String"/> array of key/value pairs for the represented configuration object.
		/// </summary>
		/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.Security.Configuration.AuthorizationRuleProviderData.Rules">
		/// AuthorizationRuleProviderData.Rules</seealso>
		public String[] Rules
		{
			get { return rules; }
			internal set { rules = value; }
		}
	}
}
