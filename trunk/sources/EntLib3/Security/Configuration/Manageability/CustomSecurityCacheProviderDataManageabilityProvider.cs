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
using System.Collections.Generic;
using System.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Configuration.Manageability
{
	internal class CustomSecurityCacheProviderDataManageabilityProvider 
		: CustomProviderDataManageabilityProvider<CustomSecurityCacheProviderData>
	{
		public CustomSecurityCacheProviderDataManageabilityProvider()
			: base(Resources.SecurityCacheProviderPolicyNameTemplate)
		{ }

		protected override NamedConfigurationSetting CreateSetting(CustomSecurityCacheProviderData data, 
			String[] attributes)
		{
			return new CustomSecurityCacheProviderSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
