//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright � Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
	/// Represents the configuration settings that describe an <see cref="SystemConfigurationSource"/>.
    /// </summary>
    public class SystemConfigurationSourceElement : ConfigurationSourceElement
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="SystemConfigurationSourceElement"/> class with default values.
		/// </summary>
		public SystemConfigurationSourceElement()
			: this(Resources.SystemConfigurationSourceName)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SystemConfigurationSourceElement"/> class with a name and an type.
		/// </summary>
		/// <param name="name">The instance name.</param>
        public SystemConfigurationSourceElement(string name)
            : base(name, typeof(SystemConfigurationSource))
		{				
        }

		/// <summary>
		/// Returns a new <see cref="SystemConfigurationSource"/>.
		/// </summary>
		/// <returns>A new configuration source.</returns>
		protected internal override IConfigurationSource CreateSource()
		{
			IConfigurationSource createdObject = new SystemConfigurationSource();

			return createdObject;
		}
	}
}
