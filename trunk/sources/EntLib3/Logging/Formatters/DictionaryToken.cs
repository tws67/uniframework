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

using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Formatters
{
    /// <summary>
    /// Formats a dictionary token by iterating through the dictionary and displays 
    /// the key and value for each entry.
    /// </summary>
    public class DictionaryToken : TokenFunction
    {
        private const string DictionaryKeyToken = "{key}";
        private const string DictionaryValueToken = "{value}";

        /// <summary>
        /// Initializes a new instance of a <see cref="DictionaryToken"/>.
        /// </summary>
        public DictionaryToken() : base("{dictionary(")
        {
        }

        /// <summary>
        /// Iterates through each entry in the dictionary and display the key and/or value.
        /// </summary>
        /// <param name="tokenTemplate">Template to repeat for each key/value pair.</param>
        /// <param name="log">Log entry containing the extended properties dictionary.</param>
        /// <returns>Repeated template for each key/value pair.</returns>
        public override string FormatToken(string tokenTemplate, LogEntry log)
        {
            StringBuilder dictionaryBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> entry in log.ExtendedProperties)
            {
                StringBuilder singlePair = new StringBuilder(tokenTemplate);
                string keyName = "";
                if (entry.Key != null)
                {
                    keyName = entry.Key.ToString();
                }
                singlePair.Replace(DictionaryKeyToken, keyName);

                string keyValue = "";
                if (entry.Value != null)
                {
                    keyValue = entry.Value.ToString();
                }
                singlePair.Replace(DictionaryValueToken, keyValue);

                dictionaryBuilder.Append(singlePair.ToString());
            }

            return dictionaryBuilder.ToString();
        }
    }
}