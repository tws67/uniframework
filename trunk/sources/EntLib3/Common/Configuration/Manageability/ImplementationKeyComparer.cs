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
using System.Collections.Generic;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal class ImplementationKeyComparer : IEqualityComparer<ImplementationKey>
	{
		public bool Equals(ImplementationKey x, ImplementationKey y)
		{
			if (x.fileName != y.fileName
				&& (x.fileName == null || !x.fileName.Equals(y.fileName, StringComparison.OrdinalIgnoreCase)))
				return false;
			if (x.applicationName != y.applicationName
				&& (x.applicationName == null || !x.applicationName.Equals(y.applicationName, StringComparison.OrdinalIgnoreCase)))
				return false;
			if (x.enableGroupPolicies != y.enableGroupPolicies)
				return false;

			return true;
		}

		public int GetHashCode(ImplementationKey obj)
		{
			return (obj.fileName == null ? 0 : obj.fileName.GetHashCode())
				^ (obj.applicationName == null ? 0 : obj.applicationName.GetHashCode())
				^ obj.enableGroupPolicies.GetHashCode();
		}
	}
}
