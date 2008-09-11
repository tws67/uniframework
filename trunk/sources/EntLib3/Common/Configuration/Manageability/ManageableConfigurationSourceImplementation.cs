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

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal class ManageableConfigurationSourceImplementation : IDisposable
	{
		private ExeConfigurationFileMap fileMap;

		private IManageabilityHelper manageabilityHelper;
		private IGroupPolicyWatcher groupPolicyWatcher;
		private ConfigurationChangeWatcherCoordinator watcherCoordinator;
		private ConfigurationChangeNotificationCoordinator notificationCoordinator;
		private IConfigurationAccessor currentConfigurationAccessor;

		private object configurationUpdateLock = new object();

		public ManageableConfigurationSourceImplementation(string configurationFilePath,
			bool refresh,
			IDictionary<string, ConfigurationSectionManageabilityProvider> manageabilityProviders,
			bool readGroupPolicies,
			bool generateWmiObjects,
			string applicationName)
			: this(configurationFilePath,
				new ManageabilityHelper(manageabilityProviders, readGroupPolicies, generateWmiObjects, applicationName),
				new GroupPolicyWatcher(),
				new ConfigurationChangeWatcherCoordinator(configurationFilePath, refresh),
				new ConfigurationChangeNotificationCoordinator())
		{ }

		internal ManageableConfigurationSourceImplementation(string configurationFilePath,
			IManageabilityHelper manageabilityHelper,
			IGroupPolicyWatcher groupPolicyWatcher,
			ConfigurationChangeWatcherCoordinator watcherCoordinator,
			ConfigurationChangeNotificationCoordinator notificationCoordinator)
		{
			this.fileMap = new ExeConfigurationFileMap();
			this.fileMap.ExeConfigFilename = configurationFilePath;

			this.manageabilityHelper = manageabilityHelper;
			this.notificationCoordinator = notificationCoordinator;
			AttachGroupPolicyWatcher(groupPolicyWatcher);
			AttachWatcherCoordinator(watcherCoordinator);

			InitializeConfiguration();
		}

		private void InitializeConfiguration()
		{
			currentConfigurationAccessor
				= new ConfigurationInstanceConfigurationAccessor(
					ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None));

			manageabilityHelper.UpdateConfigurationManageability(currentConfigurationAccessor);
			foreach (String managedSectionName in currentConfigurationAccessor.GetRequestedSectionNames())
			{
				ConfigurationSection configurationSection = currentConfigurationAccessor.GetSection(managedSectionName);
				if (configurationSection != null)
				{
					watcherCoordinator.SetWatcherForConfigSource(configurationSection.SectionInformation.ConfigSource);
				}
			}
		}

		public ConfigurationSection GetSection(string sectionName)
		{
			// always request to current accessor, no need for lock
			ConfigurationSection section = currentConfigurationAccessor.GetSection(sectionName);

			if (section != null)
			{
				lock (configurationUpdateLock)
				{
					watcherCoordinator.SetWatcherForConfigSource(section.SectionInformation.ConfigSource);
				}
			}

			return section;
		}

		public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
		{
			this.notificationCoordinator.AddSectionChangeHandler(sectionName, handler);
		}

		public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
		{
			this.notificationCoordinator.RemoveSectionChangeHandler(sectionName, handler);
		}

		private void AttachGroupPolicyWatcher(IGroupPolicyWatcher groupPolicyWatcher)
		{
			this.groupPolicyWatcher = groupPolicyWatcher;
			this.groupPolicyWatcher.GroupPolicyUpdated += OnGroupPolicyUpdated;
			this.groupPolicyWatcher.StartWatching();
		}

		private void AttachWatcherCoordinator(ConfigurationChangeWatcherCoordinator watcherCoordinator)
		{
			this.watcherCoordinator = watcherCoordinator;
			this.watcherCoordinator.ConfigurationChanged += OnConfigurationChanged;
		}

		private void OnGroupPolicyUpdated(bool machine)
		{
			UpdateConfiguration(ConfigurationChangeWatcherCoordinator.MainConfigurationFileSource);
		}

		private void OnConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
		{
			UpdateConfiguration(e.SectionName);
		}

		private void UpdateConfiguration(String configSource)
		{
			lock (configurationUpdateLock)
			{
				ConfigurationInstanceConfigurationAccessor updatedConfigurationAccessor
					= new ConfigurationInstanceConfigurationAccessor(ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None));

				manageabilityHelper.UpdateConfigurationManageability(updatedConfigurationAccessor);

				List<String> sectionsToNotify = new List<String>();
				bool notifyAll = ConfigurationChangeWatcherCoordinator.MainConfigurationFileSource.Equals(configSource);

				foreach (String sectionName in currentConfigurationAccessor.GetRequestedSectionNames())
				{
					ConfigurationSection currentSection = currentConfigurationAccessor.GetSection(sectionName);
					ConfigurationSection updatedSection = updatedConfigurationAccessor.GetSection(sectionName);

					if (currentSection != null || updatedSection != null)
					{
						UpdateWatchers(currentSection, updatedSection);

						// notify if:
						// - instructed to notify all
						// - any of the versions is null, or its config source matches the changed source
						if (notifyAll
							|| (updatedSection == null || configSource.Equals(updatedSection.SectionInformation.ConfigSource))
							|| (currentSection == null || configSource.Equals(currentSection.SectionInformation.ConfigSource)))
						{
							sectionsToNotify.Add(sectionName);
						}
					}
				}

				currentConfigurationAccessor = updatedConfigurationAccessor;
				notificationCoordinator.NotifyUpdatedSections(sectionsToNotify);
			}
		}

		private void UpdateWatchers(ConfigurationSection currentSection, ConfigurationSection updatedSection)
		{
			if (currentSection != null)
			{
				// remove the watcher for the 'old' section if it's not the main and it's different from the new, or the section was removed
				if (updatedSection == null
					|| !currentSection.SectionInformation.ConfigSource.Equals(updatedSection.SectionInformation.ConfigSource))
				{
					if (!ConfigurationChangeWatcherCoordinator.MainConfigurationFileSource.Equals(currentSection.SectionInformation.ConfigSource))
					{
						watcherCoordinator.RemoveWatcherForConfigSource(currentSection.SectionInformation.ConfigSource);
					}
				}

				// add the watcher for the new source, if exists
				if (updatedSection != null)
				{
					watcherCoordinator.SetWatcherForConfigSource(updatedSection.SectionInformation.ConfigSource);
				}
			}
			else
			{
				// section restored, just add the watcher on the new source
				watcherCoordinator.SetWatcherForConfigSource(updatedSection.SectionInformation.ConfigSource);
			}
		}

		public void Dispose()
		{
			this.groupPolicyWatcher.Dispose();
			this.watcherCoordinator.Dispose();
		}

		internal IManageabilityHelper ManageabilityHelper
		{
			get { return manageabilityHelper; }
		}
	}
}