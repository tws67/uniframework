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
	/// <see cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ReplaceHandlerData"/> instance.
	/// </summary>
	/// <seealso cref="Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionHandlerData"/>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class ReplaceHandlerSetting : ExceptionHandlerSetting
	{
		private String exceptionMessage;
		private String replaceExceptionType;

		internal ReplaceHandlerSetting(String name, String exceptionMessage, String replaceExceptionType)
			: base(name)
		{
			this.exceptionMessage = exceptionMessage;
			this.replaceExceptionType = replaceExceptionType;
		}

		/// <summary>
		/// Gets the message for the new replacing exception.
		/// </summary>
		public String ExceptionMessage
		{
			get { return exceptionMessage; }
			set { exceptionMessage = value; }
		}

		/// <summary>
		/// Gets the name of the type for the new replacing exception.
		/// </summary>
		public String ReplaceExceptionType
		{
			get { return replaceExceptionType; }
			set { replaceExceptionType = value; }
		}
	}
}
