//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Exception Handling Application Block
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

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information for a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionPolicyData"/> instance.
	/// </summary>
	/// <remarks>
	/// ExceptionPolicyData do not have any properties of their own that are not collections of other configuration objects.
	/// Instances of <see cref="ExceptionPolicySetting"/> are still published for the sake of completeness.
	/// </remarks>
	/// <seealso cref="ExceptionTypeSetting"/>
	/// <seealso cref="ExceptionHandlerSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class ExceptionPolicySetting : NamedConfigurationSetting
	{
		internal ExceptionPolicySetting(String name)
			: base(name)
		{ }
	}
}
