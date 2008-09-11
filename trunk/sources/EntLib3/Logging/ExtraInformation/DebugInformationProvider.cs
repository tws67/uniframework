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
using System.Diagnostics;
using System.Security;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation.Helpers;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation
{
    /// <summary>
    /// Provides useful diagostic information from the debug susbsystem.
    /// </summary>
    public class DebugInformationProvider : IExtraInformationProvider
    {
        private IDebugUtils debugUtils;

        /// <summary>
        /// Creates a new instance of <see cref="DebugInformationProvider"/>.
        /// </summary>
        public DebugInformationProvider()
        {
            this.debugUtils = new DebugUtils();
        }

        /// <summary>
        /// Internal constructor used for testing.
        /// </summary>
        /// <param name="debugUtils">Alternative <see cref="IDebugUtils"/> to use for testing.</param>
		internal DebugInformationProvider(IDebugUtils debugUtils)
        {
            this.debugUtils = debugUtils;
        }

        /// <summary>
        /// Populates an <see cref="IDictionary{K,T}"/> with helpful diagnostic information.
        /// </summary>
        /// <param name="dict">Dictionary used to populate the <see cref="DebugInformationProvider"></see></param>
        public void PopulateDictionary(IDictionary<string, object> dict)
        {
            string value;

            try
            {
                value = debugUtils.GetStackTraceWithSourceInfo(new StackTrace(true));
            }
            catch (SecurityException)
            {
					value = String.Format(Properties.Resources.Culture, Properties.Resources.ExtendedPropertyError, Properties.Resources.DebugInfo_StackTraceSecurityException);
            }
            catch
            {
					value = String.Format(Properties.Resources.Culture, Properties.Resources.ExtendedPropertyError, Properties.Resources.DebugInfo_StackTraceException);
            }

            dict.Add(Properties.Resources.DebugInfo_StackTrace, value);
        }
    }
}