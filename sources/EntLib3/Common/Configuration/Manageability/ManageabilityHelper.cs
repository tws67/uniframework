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
using System.Runtime.InteropServices;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Adm;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal class ManageabilityHelper : IManageabilityHelper
	{
		private IDictionary<string, ConfigurationSectionManageabilityProvider> manageabilityProviders;
		private bool readGroupPolicies;
		private IRegistryAccessor registryAccessor;
		private bool generateWmiObjects;
		private IWmiPublisher wmiPublisher;
		private string applicationName;

		private IDictionary<string, IEnumerable<ConfigurationSetting>> publishedSettingsMapping;

		public ManageabilityHelper(IDictionary<string, ConfigurationSectionManageabilityProvider> manageabilityProviders,
			bool readGroupPolicies,
			bool generateWmiObjects,
			string applicationName)
			: this(manageabilityProviders,
				readGroupPolicies,
				new RegistryAccessor(),
				generateWmiObjects,
				new InstrumentationWmiPublisher(),
				applicationName)
		{ }

		public ManageabilityHelper(IDictionary<string, ConfigurationSectionManageabilityProvider> manageabilityProviders,
			bool readGroupPolicies,
			IRegistryAccessor registryAccessor,
			bool generateWmiObjects,
			IWmiPublisher wmiPublisher,
			string applicationName)
		{
			this.manageabilityProviders = manageabilityProviders;
			this.readGroupPolicies = readGroupPolicies;
			this.registryAccessor = registryAccessor;
			this.generateWmiObjects = generateWmiObjects;
			this.wmiPublisher = wmiPublisher;
			this.applicationName = applicationName;

			this.publishedSettingsMapping = new Dictionary<string, IEnumerable<ConfigurationSetting>>();
		}

		public void UpdateConfigurationManageability(IConfigurationAccessor configurationAccessor)
		{
			using (new GroupPolicyLock())
			{
				foreach (String sectionName in manageabilityProviders.Keys)
				{
					if (publishedSettingsMapping.ContainsKey(sectionName))
					{
						RevokeAll(publishedSettingsMapping[sectionName]);
					}

					ConfigurationSectionManageabilityProvider manageabilityProvider = manageabilityProviders[sectionName];

					ConfigurationSection section = configurationAccessor.GetSection(sectionName);
					if (section != null)
					{
						ICollection<ConfigurationSetting> wmiSettings = new List<ConfigurationSetting>();

						IRegistryKey machineKey = null;
						IRegistryKey userKey = null;

						try
						{
							LoadPolicyRegistryKeys(sectionName, out machineKey, out userKey);

							if (manageabilityProvider
									.OverrideWithGroupPoliciesAndGenerateWmiObjects(section,
										this.readGroupPolicies, machineKey, userKey,
										generateWmiObjects, wmiSettings))
							{
								publishedSettingsMapping[sectionName] = wmiSettings;

								PublishAll(wmiSettings, sectionName);
							}
							else
							{
								configurationAccessor.RemoveSection(sectionName);
							}
						}
						catch (Exception e)
						{
							ManageabilityExtensionsLogger.LogException(e, Resources.ExceptionUnexpectedErrorProcessingSection);
						}
						finally
						{
							ReleasePolicyRegistryKeys(machineKey, userKey);
						}
					}
				}
			}	// locks on group policies are released here
		}

		private void LoadPolicyRegistryKeys(String sectionName, out IRegistryKey machineKey, out IRegistryKey userKey)
		{
			if (readGroupPolicies)
			{
				String sectionKeyName = BuildSectionKeyName(this.applicationName, sectionName);
				machineKey = registryAccessor.LocalMachine.OpenSubKey(sectionKeyName);
				userKey = registryAccessor.CurrentUser.OpenSubKey(sectionKeyName);
			}
			else
			{
				machineKey = null;
				userKey = null;
			}
		}

		private static void ReleasePolicyRegistryKeys(IRegistryKey machineKey, IRegistryKey userKey)
		{
			if (machineKey != null)
			{
				try { machineKey.Close(); }
				catch (Exception) { }
			}

			if (userKey != null)
			{
				try { userKey.Close(); }
				catch (Exception) { }
			}
		}

		private void PublishAll(IEnumerable<ConfigurationSetting> instances, String sectionName)
		{
			foreach (ConfigurationSetting instance in instances)
			{
				instance.ApplicationName = this.applicationName;
				instance.SectionName = sectionName;
				this.wmiPublisher.Publish(instance);
			}
		}

		public void RevokeAll(IEnumerable<ConfigurationSetting> instances)
		{
			foreach (ConfigurationSetting instance in instances)
			{
				this.wmiPublisher.Revoke(instance);
			}
		}

		internal static string BuildSectionKeyName(String applicationName, String sectionName)
		{
			return Path.Combine(Path.Combine(@"Software\Policies\", applicationName), sectionName);
		}

		internal IDictionary<string, ConfigurationSectionManageabilityProvider> ManageabilityProviders
		{
			get { return manageabilityProviders; }
		}

		private class GroupPolicyLock : IDisposable
		{
			private IntPtr machineCriticalSectionHandle;
			private IntPtr userCriticalSectionHandle;

			public GroupPolicyLock()
			{
				// lock policy processing, user first
				// see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/policy/policy/entercriticalpolicysection.asp for details

				userCriticalSectionHandle = NativeMethods.EnterCriticalPolicySection(false);
				if (IntPtr.Zero == userCriticalSectionHandle)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}

				machineCriticalSectionHandle = NativeMethods.EnterCriticalPolicySection(true);
				if (IntPtr.Zero == machineCriticalSectionHandle)
				{
					// save the current call's error
					int hr = Marshal.GetHRForLastWin32Error();

					// release the user policy section first - don't check for errors as an exception will be thrown
					NativeMethods.LeaveCriticalPolicySection(userCriticalSectionHandle);

					Marshal.ThrowExceptionForHR(hr);
				}
			}

			void IDisposable.Dispose()
			{
				// release locks in the reverse order
				// handles shouldn't be null here, as the constructor should have thrown if they were
				// exceptions are not thrown here; critical section locks will be timed out by the O.S.
				NativeMethods.LeaveCriticalPolicySection(machineCriticalSectionHandle);
				NativeMethods.LeaveCriticalPolicySection(userCriticalSectionHandle);
			}
		}
	}
}