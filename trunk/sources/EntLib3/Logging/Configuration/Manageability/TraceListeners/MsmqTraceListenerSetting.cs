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
using System.Management.Instrumentation;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Manageability;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Manageability.TraceListeners
{
	/// <summary>
	/// Represents an instance of <see cref="Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.MsmqTraceListenerData"/>
	/// as an instrumentation class.
	/// </summary>
	[InstrumentationClass(InstrumentationType.Instance)]
	public class MsmqTraceListenerSetting : TraceListenerSetting
	{
		private String formatter;
		private String messagePriority;
		private String queuePath;
		private bool recoverable;
		private String timeToBeReceived;
		private String timeToReachQueue;
		private String transactionType;
		private bool useAuthentication;
		private bool useDeadLetterQueue;
		private bool useEncryption;

		internal MsmqTraceListenerSetting(String name,
			String formatter,
			String messagePriority,
			String queuePath,
			bool recoverable,
			String timeToBeReceived,
			String timeToReachQueue,
			String traceOutputOptions,
			String transactionType,
			bool useAuthentication,
			bool useDeadLetterQueue,
			bool useEncryption)
			: base(name, traceOutputOptions)
		{
			this.formatter = formatter;
			this.messagePriority = messagePriority;
			this.queuePath = queuePath;
			this.recoverable = recoverable;
			this.timeToBeReceived = timeToBeReceived;
			this.timeToReachQueue = timeToReachQueue;
			this.transactionType = transactionType;
			this.useAuthentication = useAuthentication;
			this.useDeadLetterQueue = useDeadLetterQueue;
			this.useEncryption = useEncryption;
		}

		/// <summary>
		/// Gets the name of the formatter for the represented configuration element.
		/// </summary>
		public String Formatter
		{
			get { return formatter; }
			internal set { formatter = value; }
		}

		/// <summary>
		/// Gets the name of value of the message priority for the represented configuration element.
		/// </summary>
		public String MessagePriority
		{
			get { return messagePriority; }
			internal set { messagePriority = value; }
		}

		/// <summary>
		/// Gets the queue path for the represented configuration element.
		/// </summary>
		public String QueuePath
		{
			get { return queuePath; }
			internal set { queuePath = value; }
		}

		/// <summary>
		/// Gets the value of the recoverable property for the represented configuration element.
		/// </summary>
		public bool Recoverable
		{
			get { return recoverable; }
			internal set { recoverable = value; }
		}

		/// <summary>
		/// Gets the string representation of the <see cref="TimeSpan"/> value of the time to be received 
		/// for the represented configuration element.
		/// </summary>
		public String TimeToBeReceived
		{
			get { return timeToBeReceived; }
			internal set { timeToBeReceived = value; }
		}

		/// <summary>
		/// Gets the string representation of the <see cref="TimeSpan"/> value of the time to reach queue
		/// for the represented configuration element.
		/// </summary>
		public String TimeToReachQueue
		{
			get { return timeToReachQueue; }
			internal set { timeToReachQueue = value; }
		}

		/// <summary>
		/// Gets the name of value of the transaction type for the represented configuration element.
		/// </summary>
		public String TransactionType
		{
			get { return transactionType; }
			internal set { transactionType = value; }
		}

		/// <summary>
		/// Gets the value of the use authentication property for the represented configuration element.
		/// </summary>
		public bool UseAuthentication
		{
			get { return useAuthentication; }
			internal set { useAuthentication = value; }
		}

		/// <summary>
		/// Gets the value of the use dead letter property for the represented configuration element.
		/// </summary>
		public bool UseDeadLetterQueue
		{
			get { return useDeadLetterQueue; }
			internal set { useDeadLetterQueue = value; }
		}

		/// <summary>
		/// Gets the value of the use encryption property for the represented configuration element.
		/// </summary>
		public bool UseEncryption
		{
			get { return useEncryption; }
			internal set { useEncryption = value; }
		}
	}
}