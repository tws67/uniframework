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
	/// Represents the configuration information for an instance of a concrete subclass of
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionHandlerData"/>.
	/// </summary>
	/// <remarks>
	/// There way to relate the objects representing an <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionTypeData"/> instance
	/// and the <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionHandlerData"/> instances in its
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionTypeData.ExceptionHandlers"/> collection is through
	/// the <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionHandlerSetting.ExceptionType"/> and
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionHandlerSetting.Policy"/> properties. Also,
	/// the order of the handlers in the Handlers collection is represented with the 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionHandlerSetting.Order"/> property as the order 
	/// of the handlers is relevant to the exception handling process.
	/// </remarks>
	/// <seealso cref="ExceptionTypeSetting"/>
	/// <seealso cref="ExceptionPolicySetting"/>
	/// <seealso cref="WrapHandlerSetting"/>
	/// <seealso cref="ReplaceHandlerSetting"/>
	/// <seealso cref="CustomHandlerSetting"/>
	[InstrumentationClass(InstrumentationType.Abstract)]
	public class ExceptionHandlerSetting : NamedConfigurationSetting
	{
		private String policy;
		private String exceptionType;
		private int order;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExceptionHandlerSetting"/> class.
		/// </summary>
		/// <param name="name">The name of the exception handler.</param>
		protected ExceptionHandlerSetting(String name)
			: base(name)
		{ }

		/// <summary>
		/// Gets the type of the exception to which the handler is registered.
		/// </summary>
		/// <remarks>
		/// This property is used toghether with <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionHandlerSetting.Policy"/>
		/// to relate the objects representing exception policies, exception types and exception handlers.
		/// </remarks>
		public String ExceptionType
		{
			get { return exceptionType; }
			internal set { exceptionType = value; }
		}

		/// <summary>
		/// Gets the name of the policy to which the handler is registered.
		/// </summary>
		/// <remarks>
		/// This property is used toghether with <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability.ExceptionHandlerSetting.ExceptionType"/>
		/// to relate the objects representing exception policies, exception types and exception handlers.
		/// </remarks>
		public String Policy
		{
			get { return policy; }
			internal set { policy = value; }
		}

		/// <summary>
		/// Gets the order for the represented hadler among the collection of handlers registered to hande the 
		/// same exception type in the same exception policy.
		/// </summary>
		public int Order
		{
			get { return order; }
			internal set { order = value; }
		}
	}
}