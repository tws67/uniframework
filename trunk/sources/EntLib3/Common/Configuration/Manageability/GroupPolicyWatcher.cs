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
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability.Properties;

namespace Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability
{
	/// <devdoc>
	/// The policy watcher can be started and stopped many times. To deal with this, when a watcher thread is started
	/// it is given an 'exit' event that will be signaled when the thread needs to be stopped. Once the thread is started
	/// it own the exit event, and will release it when it terminates. More than one watching thread may be active at the
	/// same time, having different exit events, if the old watching thread doesn't get processing time before the new 
	/// thread is started; when the old thread gets to run it will consume the signaled exit event and finish.
	/// </devdoc>
	internal sealed class GroupPolicyWatcher : IGroupPolicyWatcher
	{
		private object lockObject = new object();
		private AutoResetEvent currentThreadExitEvent;
		private GroupPolicyNotificationRegistrationBuilder registrationBuilder;
		public event GroupPolicyUpdateDelegate GroupPolicyUpdated;

		public GroupPolicyWatcher()
			: this(new GroupPolicyNotificationRegistrationBuilder())
		{ }

		public GroupPolicyWatcher(GroupPolicyNotificationRegistrationBuilder registrationBuilder)
		{
			this.registrationBuilder = registrationBuilder;
		}

		public void StartWatching()
		{
			lock (lockObject)
			{
				if (currentThreadExitEvent == null)
				{
					// this event will be released by the watcher thread on exit.
					currentThreadExitEvent = new AutoResetEvent(false);

					Thread watchingThread = new Thread(new ParameterizedThreadStart(DoWatch));
					watchingThread.IsBackground = true;
					watchingThread.Name = Resources.GroupPolicyWatcherThread;
					watchingThread.Start(currentThreadExitEvent);
				}
			}
		}

		public void StopWatching()
		{
			lock (lockObject)
			{
				if (currentThreadExitEvent != null)
				{
					// this signal will be consumed by the current watcher thread.
					currentThreadExitEvent.Set();

					currentThreadExitEvent = null;
				}
			}
		}

		private void DoWatch(object parameter)
		{
			AutoResetEvent exitEvent = (AutoResetEvent)parameter;

			try
			{
				using (GroupPolicyNotificationRegistration registration = registrationBuilder.CreateRegistration())
				{
					AutoResetEvent[] policyEvents
						= new AutoResetEvent[] { exitEvent, registration.MachinePolicyEvent, registration.UserPolicyEvent };

					bool listening = true;

					while (listening)
					{
						int result = WaitHandle.WaitAny(policyEvents);	// 0 == exit, 1 == machine, 2 == user
						if (result != 0)
						{
							// notification from policy handles, not from exit
							// fire the change notification mechanism
							if (GroupPolicyUpdated != null)
							{
								GroupPolicyUpdated(result == 1);
							}
						}
						else
						{
							// notification from exit
							listening = false;
						}
					}
				}
			}
			finally
			{
				// release the thread's exit event.
				exitEvent.Close();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~GroupPolicyWatcher()
		{
			Dispose(false);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.StopWatching();
			}
		}


		internal class GroupPolicyNotificationRegistrationBuilder
		{
			public virtual GroupPolicyNotificationRegistration CreateRegistration()
			{
				return new GroupPolicyNotificationRegistration();
			}
		}

		internal class GroupPolicyNotificationRegistration : IDisposable
		{
			private AutoResetEvent machinePolicyEvent;
			private AutoResetEvent userPolicyEvent;

			public GroupPolicyNotificationRegistration()
			{
				machinePolicyEvent = new AutoResetEvent(false);
				userPolicyEvent = new AutoResetEvent(false);

				CheckReturnValue(NativeMethods.RegisterGPNotification(machinePolicyEvent.SafeWaitHandle, true));
				CheckReturnValue(NativeMethods.RegisterGPNotification(userPolicyEvent.SafeWaitHandle, false));
			}

			public virtual void Dispose()
			{
				try
				{
					CheckReturnValue(NativeMethods.UnregisterGPNotification(machinePolicyEvent.SafeWaitHandle));
					CheckReturnValue(NativeMethods.UnregisterGPNotification(userPolicyEvent.SafeWaitHandle));
				}
				finally
				{
					machinePolicyEvent.Close();
					userPolicyEvent.Close();
				}
			}

			private void CheckReturnValue(bool returnValue)
			{
				if (!returnValue)
				{
					Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
				}
			}

			public AutoResetEvent MachinePolicyEvent
			{
				get { return machinePolicyEvent; }
			}

			public AutoResetEvent UserPolicyEvent
			{
				get { return userPolicyEvent; }
			}
		}
	}
}
