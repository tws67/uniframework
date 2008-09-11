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
using System.ComponentModel;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	internal class ConfigurationChangeNotificationCoordinator
	{
		private object eventHandlersLock = new object();
		private EventHandlerList eventHandlers = new EventHandlerList();

		public void AddSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
		{
			lock (eventHandlersLock)
			{
				eventHandlers.AddHandler(CanonicalizeSectionName(sectionName), handler);
			}
		}

		public void RemoveSectionChangeHandler(string sectionName, ConfigurationChangedEventHandler handler)
		{
			lock (eventHandlersLock)
			{
				eventHandlers.RemoveHandler(CanonicalizeSectionName(sectionName), handler);
			}
		}

		public void NotifyUpdatedSections(IEnumerable<string> sectionsToNotify)
		{
			foreach (string rawSectionName in sectionsToNotify)
			{
				String sectionName = CanonicalizeSectionName(rawSectionName);

				Delegate[] invocationList = null;

				lock (eventHandlersLock)
				{
					ConfigurationChangedEventHandler callbacks = (ConfigurationChangedEventHandler)eventHandlers[sectionName];
					if (callbacks == null)
					{
						continue;
					}
					invocationList = callbacks.GetInvocationList();
				}

				ConfigurationChangedEventArgs eventData = new ConfigurationChangedEventArgs(sectionName);
				foreach (ConfigurationChangedEventHandler callback in invocationList)
				{
					try
					{
						if (callback != null)
						{
							callback(this, eventData);
						}
					}
					catch (Exception e)
					{
						ManageabilityExtensionsLogger.LogException(e,
							String.Format(Resources.Culture,
								Resources.ExceptionErrorOnCallbackForSectionUpdate,
								sectionName,
								callback.ToString()));
					}
				}
			}
		}

		private string CanonicalizeSectionName(string sectionName)
		{
			return String.Intern(sectionName);
		}
	}
}
