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

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
    /// <summary>
    /// Formats a keyvalue token and displays the dictionary entries value.
    /// </summary>
    public class KeyValueToken : TokenFunction
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="TimeStampToken"/>.
        /// </summary>
        public KeyValueToken() : base("{keyvalue(")
        {
        }

        /// <summary>
        /// Gets the value of a property from the log entry.
        /// </summary>
        /// <param name="tokenTemplate">Dictionary key name.</param>
        /// <param name="log">Log entry containing with extended properties dictionary values.</param>
		/// <returns>The value of the key from the extended properties dictionary, or <see langword="null"/> 
		/// (Nothing in Visual Basic) if there is no entry with that key.</returns>
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            string propertyString = "";
            object propertyObject;

				if (log.ExtendedProperties.TryGetValue(tokenTemplate, out propertyObject))
            {
                propertyString = propertyObject.ToString();
            }

            return propertyString;
        }
    }
}