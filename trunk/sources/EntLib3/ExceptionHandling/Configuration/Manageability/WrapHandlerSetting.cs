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

namespace Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.Manageability
{
	/// <summary>
	/// Represents the configuration information from a 
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.WrapHandlerData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionHandlerData"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class WrapHandlerSetting : ExceptionHandlerSetting
	{
		private String exceptionMessage;
		private String wrapExceptionType;

		internal WrapHandlerSetting(String name, String exceptionMessage, String wrapExceptionType)
			: base(name)
		{
			this.exceptionMessage = exceptionMessage;
			this.wrapExceptionType = wrapExceptionType;
		}

		/// <summary>
		/// Gets the message for the wrapping exception.
		/// </summary>
		public String ExceptionMessage
		{
			get { return exceptionMessage; }
			internal set { exceptionMessage = value; }
		}

		/// <summary>
		/// Gets the name of the type for the wrapping exception.
		/// </summary>
		public String WrapExceptionType
		{
			get { return wrapExceptionType; }
			internal set { wrapExceptionType = value; }
		}
	}
}
