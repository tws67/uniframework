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

using System.Globalization;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
    /// <summary>
    /// Formats a timestamp token with a custom date time format string.
    /// </summary>
    public class TimeStampToken : TokenFunction
    {
		private const string LocalStartDelimiter = "local";
		private const string LocalStartDelimiterWithFormat = "local:";

        /// <summary>
        /// Initializes a new instance of a <see cref="TimeStampToken"/>.
        /// </summary>
        public TimeStampToken() : base("{timestamp(")
        {
        }

        /// <summary>
        /// Formats the timestamp property with the specified date time format string.
        /// </summary>
        /// <param name="tokenTemplate">Date time format string.</param>
        /// <param name="log">Log entry containing the timestamp.</param>
        /// <returns>Returns the formatted time stamp.</returns>
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
			string result = null;
			if (tokenTemplate.Equals(LocalStartDelimiter, System.StringComparison.InvariantCultureIgnoreCase))
			{
				System.DateTime localTime = log.TimeStamp.ToLocalTime();
				result = localTime.ToString();
			}
			else if (tokenTemplate.StartsWith(LocalStartDelimiterWithFormat, System.StringComparison.InvariantCultureIgnoreCase))
			{
				string formatTemplate = tokenTemplate.Substring(LocalStartDelimiterWithFormat.Length);
				System.DateTime localTime = log.TimeStamp.ToLocalTime();
				result = localTime.ToString(formatTemplate, CultureInfo.CurrentCulture);
			}
			else
			{
				result = log.TimeStamp.ToString(tokenTemplate, CultureInfo.CurrentCulture);
			}
			return result;
        }
    }
}