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
using System.Globalization;
using System.IO;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	/// <devdoc>
	/// Manages the singleton <see cref="ManageableConfigurationSourceImplementation"/> instance for a given 
	/// file name, application name and group policy enablement combination.
	/// </devdoc>
	internal class ManageableConfigurationSourceSingletonHelper : IDisposable
	{
		private object lockObject = new object();
		private IDictionary<ImplementationKey, ManageableConfigurationSourceImplementation> instances;
		internal bool refresh;

		public ManageableConfigurationSourceSingletonHelper()
			: this(true)
		{ }

		internal ManageableConfigurationSourceSingletonHelper(bool refresh)
		{
			this.refresh = refresh;
			this.instances
				= new Dictionary<ImplementationKey, ManageableConfigurationSourceImplementation>(new ImplementationKeyComparer());
		}

		public ManageableConfigurationSourceImplementation GetInstance(String configurationFilePath,
			IDictionary<String, ConfigurationSectionManageabilityProvider> manageabilityProviders,
			bool readGroupPolicies,
			bool generateWmiObjects,
			String applicationName)
		{
			if (String.IsNullOrEmpty(configurationFilePath))
				throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "configurationFilePath");

			String rootedConfigurationFilePath = RootConfigurationFilePath(configurationFilePath);

			if (!File.Exists(rootedConfigurationFilePath))
				throw new FileNotFoundException(String.Format(Resources.Culture, Resources.ExceptionConfigurationLoadFileNotFound, rootedConfigurationFilePath));

			ImplementationKey key = new ImplementationKey(rootedConfigurationFilePath, applicationName, readGroupPolicies);
			ManageableConfigurationSourceImplementation instance;

			lock (lockObject)
			{
				instances.TryGetValue(key, out instance);
				if (instance == null)
				{
					instance = new ManageableConfigurationSourceImplementation(rootedConfigurationFilePath,
						refresh,
						manageabilityProviders,
						readGroupPolicies,
						generateWmiObjects,
						applicationName);
					instances.Add(key, instance);
				}
			}

			return instance;
		}

		public void Dispose()
		{
			foreach (ManageableConfigurationSourceImplementation instance in instances.Values)
			{
				instance.Dispose();
			}

		}

		private static String RootConfigurationFilePath(String configurationFilePath)
		{
			String rootedConfigurationFile = configurationFilePath;
			if (!Path.IsPathRooted(rootedConfigurationFile))
			{
				rootedConfigurationFile
					= Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootedConfigurationFile));
			}
			return rootedConfigurationFile;
		}
	}
}