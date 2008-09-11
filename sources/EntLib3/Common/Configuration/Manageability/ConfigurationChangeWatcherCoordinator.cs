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
using System.IO;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Storage;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	/// <summary>
	/// Manages the configuration file watchers for a collection of configuration sections.
	/// </summary>
	internal class ConfigurationChangeWatcherCoordinator : IDisposable
	{
		public const String MainConfigurationFileSource = "";

		private bool refresh;
		private String mainConfigurationFileName;
		private String mainConfigurationFilePath;
		private Dictionary<String, ConfigurationChangeFileWatcher> configSourceWatcherMapping;

		public event ConfigurationChangedEventHandler ConfigurationChanged;

		public ConfigurationChangeWatcherCoordinator(String mainConfigurationFileName,
			bool refresh)
		{
			this.mainConfigurationFileName = mainConfigurationFileName;
			this.mainConfigurationFilePath = Path.GetDirectoryName(mainConfigurationFileName);
			this.refresh = refresh;

			this.configSourceWatcherMapping = new Dictionary<String, ConfigurationChangeFileWatcher>();

			CreateWatcherForConfigSource(MainConfigurationFileSource);
		}

		public bool IsWatchingConfigSource(String configSource)
		{
			return this.configSourceWatcherMapping.ContainsKey(configSource);
		}

		public void SetWatcherForConfigSource(String configSource)
		{
			if (!IsWatchingConfigSource(configSource))
			{
				CreateWatcherForConfigSource(configSource);
			}
		}

		public void RemoveWatcherForConfigSource(String configSource)
		{
			ConfigurationChangeFileWatcher watcher = null;
			configSourceWatcherMapping.TryGetValue(configSource, out watcher);

			if (watcher != null)
			{
				configSourceWatcherMapping.Remove(configSource);
				watcher.Dispose();
			}
		}

		private ConfigurationChangeFileWatcher CreateWatcherForConfigSource(String configSource)
		{
			ConfigurationChangeFileWatcher watcher = null;

			if (MainConfigurationFileSource.Equals(configSource))
			{
				watcher = new ConfigurationChangeFileWatcher(mainConfigurationFileName,
					configSource);
			}
			else
			{
				watcher = new ConfigurationChangeFileWatcher(Path.Combine(mainConfigurationFilePath, configSource),
					configSource);
			}
			watcher.ConfigurationChanged += OnConfigurationChanged;

			configSourceWatcherMapping.Add(configSource, watcher);

			if (refresh)
			{
				watcher.StartWatching();
			}

			return watcher;
		}

		public void Dispose()
		{
			foreach (IDisposable watcher in this.configSourceWatcherMapping.Values)
			{
				watcher.Dispose();
			}
		}

		internal void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs args)
		{
			if (ConfigurationChanged != null)
			{
				ConfigurationChanged(this, args);
			}
		}

		internal ICollection<String> WatchedConfigSources
		{
			get { return this.configSourceWatcherMapping.Keys as ICollection<String>; }
		}

		internal IDictionary<String, ConfigurationChangeFileWatcher> ConfigSourceWatcherMapping
		{
			get { return this.configSourceWatcherMapping; }
		}
	}
}
