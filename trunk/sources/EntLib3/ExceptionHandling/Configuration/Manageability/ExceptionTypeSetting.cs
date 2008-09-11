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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionTypeData"/> instance.
	/// </summary>
	/// <remarks>
	/// ExceptionTypeData instances are held by 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionPolicyData"/>, but
	/// the wmi objects that represent them related only by matching values for the 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionTypeSetting.Policy"/>
	/// property.
	/// </remarks>
	/// <seealso cref="ExceptionPolicySetting"/>
	/// <seealso cref="ExceptionHandlerSetting"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class ExceptionTypeSetting : NamedConfigurationSetting
	{
		private String policy;
		private String exceptionTypeName;
		private String postHandlingAction;

		internal ExceptionTypeSetting(String name, String exceptionTypeName, String postHandlingAction)
			: base(name)
		{
			this.exceptionTypeName = exceptionTypeName;
			this.postHandlingAction = postHandlingAction;
		}

		/// <summary>
		/// Gets the name of the exception type to handle.
		/// </summary>
		public String ExceptionTypeName
		{
			get { return exceptionTypeName; }
			internal set { exceptionTypeName = value; }
		}

		/// <summary>
		/// Gets the name of the policy to which the represented 
		/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionTypeData"/> instance
		/// belongs.
		/// </summary>
		public String Policy
		{
			get { return policy; }
			internal set { policy = value; }
		}

		/// <summary>
		/// Gets the name of the value for the for the post handling action.
		/// </summary>
		public String PostHandlingAction
		{
			get { return postHandlingAction; }
			set { postHandlingAction = value; }
		}
	}
}
