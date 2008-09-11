//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal class ConfigurationInstanceConfigurationAccessor : IConfigurationAccessor
	{
		private System.Configuration.Configuration configuration;
		private IDictionary<String, bool> requestedSections;

		public ConfigurationInstanceConfigurationAccessor(System.Configuration.Configuration configuration)
		{
			this.configuration = configuration;
			this.requestedSections = new Dictionary<String, bool>();
		}

		public ConfigurationSection GetSection(string sectionName)
		{
			ConfigurationSection section = configuration.GetSection(sectionName);
			requestedSections[sectionName] = section != null;

			return section;
		}

		public void RemoveSection(string sectionName)
		{
			configuration.Sections.Remove(sectionName);
		}

		public IEnumerable<string> GetRequestedSectionNames()
		{
			String[] requestedSectionNames = new String[requestedSections.Keys.Count];
			requestedSections.Keys.CopyTo(requestedSectionNames, 0);

			return requestedSectionNames;
		}
	}
}
