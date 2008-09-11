//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal struct ImplementationKey
	{
		public String fileName;
		public String applicationName;
		public Boolean enableGroupPolicies;

		public ImplementationKey(String fileName, String applicationName, Boolean enableGroupPolicies)
		{
			this.fileName = fileName != null ? fileName.ToLower(CultureInfo.CurrentCulture) : null;
			this.applicationName = applicationName != null ? applicationName.ToLower(CultureInfo.CurrentCulture) : null;
			this.enableGroupPolicies = enableGroupPolicies;
		}
	}
}
