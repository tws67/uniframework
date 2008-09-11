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
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.CustomHandlerData"/> instance.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class CustomHandlerSetting : ExceptionHandlerSetting
	{
		private String handlerType;
		private String[] attributes;

		internal CustomHandlerSetting(String name, String filterType, String[] attributes)
			: base(name)
		{
			this.handlerType = filterType;
			this.attributes = attributes;
		}

		/// <summary>
		/// Gets the name of the type for the custom exception handler.
		/// </summary>
		public String HandlerType
		{
			get { return handlerType; }
			internal set { handlerType = value; }
		}

		/// <summary>
		/// Gets the collection of attributes for the custom exception handler represented as a 
		/// <see cref="String"/> array of key/value pairs.
		/// </summary>
		public String[] Attributes
		{
			get { return attributes; }
			internal set { attributes = value; }
		}
	}
}
