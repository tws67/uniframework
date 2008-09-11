//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Logging Application Block
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Practices.EnterpriseLibrary.Logging.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
	/// <summary>
	/// Token Function for Adding Reflected Property Values.
	/// </summary>
	public class ReflectedPropertyToken : TokenFunction
	{
		private const string StartDelimiter = "{property(";

		/// <summary>
		/// Constructor that initializes the token with the token name
		/// </summary>
		public ReflectedPropertyToken()
			: base(StartDelimiter)
		{
		}

		/// <summary>
		/// Searches for the reflected property and returns its value as a string
		/// </summary>
		public override string FormatToken(string tokenTemplate, LogEntry log)
		{
			Type logType = log.GetType();
			PropertyInfo property = logType.GetProperty(tokenTemplate);
			if (property != null)
			{
				object value = property.GetValue(log, null);
				return value != null ? value.ToString() : string.Empty;
			}
			else
			{
				return String.Format(Resources.Culture, Resources.ReflectedPropertyTokenNotFound, tokenTemplate);
			}
		}
	}
}
