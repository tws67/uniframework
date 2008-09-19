//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Cryptography Application Block
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
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Security.Cryptography.Configuration.Manageability
{
	internal class CustomSymmetricCryptoProviderDataManageabilityProvider 
		: CustomProviderDataManageabilityProvider<CustomSymmetricCryptoProviderData>
	{
		public CustomSymmetricCryptoProviderDataManageabilityProvider()
			: base(Resources.SymmetricCryptoProviderPolicyNameTemplate)
		{ }

		protected override NamedConfigurationSetting CreateSetting(CustomSymmetricCryptoProviderData data, 
			String[] attributes)
		{
			return new CustomSymmetricCryptoProviderSetting(data.Name, data.Type.AssemblyQualifiedName, attributes);
		}
	}
}
