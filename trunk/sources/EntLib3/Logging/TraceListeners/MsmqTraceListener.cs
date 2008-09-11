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
using System.Text;
using System.Messaging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners
{
	/// <summary>
	/// Is a <see cref="TraceListener"/> that delivers the log entries to an Msmq instance.
	/// </summary>
	public class MsmqTraceListener : FormattedTraceListenerBase
	{
		private string queuePath;
		private MessagePriority messagePriority;
		private TimeSpan timeToReachQueue;
		private TimeSpan timeToBeReceived;
		private bool recoverable;
		private bool useAuthentication;
		private bool useDeadLetterQueue;
		private bool useEncryption;
		private MessageQueueTransactionType transactionType;
		private IMsmqSendInterfaceFactory msmqInterfaceFactory;

		/// <summary>
		/// Initializes a new instance of <see cref="MsmqTraceListener"/>.
		/// </summary>
		/// <param name="name">The name of the new instance.</param>
		/// <param name="queuePath">The path to the queue to deliver to.</param>
		/// <param name="formater">The formatter to use.</param>
		/// <param name="messagePriority">The priority for the messages to send.</param>
		/// <param name="recoverable">The recoverable flag for the messages to send.</param>
		/// <param name="timeToReachQueue">The timeToReachQueue for the messages to send.</param>
		/// <param name="timeToBeReceived">The timeToBeReceived for the messages to send.</param>
		/// <param name="useAuthentication">The useAuthentication flag for the messages to send.</param>
		/// <param name="useDeadLetterQueue">The useDeadLetterQueue flag for the messages to send.</param>
		/// <param name="useEncryption">The useEncryption flag for the messages to send.</param>
		/// <param name="transactionType">The <see cref="MessageQueueTransactionType"/> for the message to send.</param>
		public MsmqTraceListener(string name, string queuePath, ILogFormatter formater,
								 MessagePriority messagePriority, bool recoverable,
								 TimeSpan timeToReachQueue, TimeSpan timeToBeReceived,
								 bool useAuthentication, bool useDeadLetterQueue, bool useEncryption,
								 MessageQueueTransactionType transactionType)
			: this(name, queuePath, formater, messagePriority, recoverable,
				   timeToReachQueue, timeToBeReceived,
				   useAuthentication, useDeadLetterQueue, useEncryption,
				   transactionType, new MsmqSendInterfaceFactory())
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="MsmqTraceListener"/>.
		/// </summary>
		/// <param name="name">The name of the new instance.</param>
		/// <param name="queuePath">The path to the queue to deliver to.</param>
		/// <param name="formater">The formatter to use.</param>
		/// <param name="messagePriority">The priority for the messages to send.</param>
		/// <param name="recoverable">The recoverable flag for the messages to send.</param>
		/// <param name="timeToReachQueue">The timeToReachQueue for the messages to send.</param>
		/// <param name="timeToBeReceived">The timeToBeReceived for the messages to send.</param>
		/// <param name="useAuthentication">The useAuthentication flag for the messages to send.</param>
		/// <param name="useDeadLetterQueue">The useDeadLetterQueue flag for the messages to send.</param>
		/// <param name="useEncryption">The useEncryption flag for the messages to send.</param>
		/// <param name="transactionType">The <see cref="MessageQueueTransactionType"/> for the message to send.</param>
		/// <param name="msmqInterfaceFactory">The factory to create the msmq interfaces.</param>
		public MsmqTraceListener(string name, string queuePath, ILogFormatter formater,
								 MessagePriority messagePriority, bool recoverable,
								 TimeSpan timeToReachQueue, TimeSpan timeToBeReceived,
								 bool useAuthentication, bool useDeadLetterQueue, bool useEncryption,
								 MessageQueueTransactionType transactionType,
								 IMsmqSendInterfaceFactory msmqInterfaceFactory)
			: base(formater)
		{
			this.queuePath = queuePath;
			this.messagePriority = messagePriority;
			this.recoverable = recoverable;
			this.timeToReachQueue = timeToReachQueue;
			this.timeToBeReceived = timeToBeReceived;
			this.useAuthentication = useAuthentication;
			this.useDeadLetterQueue = useDeadLetterQueue;
			this.useEncryption = useEncryption;
			this.transactionType = transactionType;
			this.msmqInterfaceFactory = msmqInterfaceFactory;
		}

		/// <summary>
		/// Sends the traced object to its final destination through a <see cref="MessageQueue"/>.
		/// </summary>
		/// <param name="eventCache">The context information provided by <see cref="System.Diagnostics"/>.</param>
		/// <param name="source">The name of the trace source that delivered the trace data.</param>
		/// <param name="eventType">The type of event.</param>
		/// <param name="id">The id of the event.</param>
		/// <param name="data">The data to trace.</param>
		public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
		{
			if (data is LogEntry)
			{
				SendMessageToQueue(data as LogEntry);
				InstrumentationProvider.FireTraceListenerEntryWrittenEvent();
			}
			else if (data is string)
			{
				Write(data as string);
			}
			else
			{
				base.TraceData(eventCache, source, eventType, id, data);
			}
		}

		/// <summary>
		/// Writes the specified message to the message queue.
		/// </summary>
		/// <param name="message"></param>
		public override void WriteLine(string message)
		{
			Write(message);
		}

		/// <summary>
		/// Writes the specified message to the message queue.
		/// </summary>
		/// <param name="message">Message to be written.</param>
		public override void Write(string message)
		{
			SendMessageToQueue(message);
		}

		internal Message CreateMessage(LogEntry logEntry)
		{
			string formattedLogEntry = FormatEntry(logEntry);

			return CreateMessage(formattedLogEntry, logEntry.Title);
		}

		private Message CreateMessage(string messageBody, string messageLabel)
		{
			Message queueMessage = new Message();

			queueMessage.Body = messageBody;
			queueMessage.Label = messageLabel;
			queueMessage.Priority = this.messagePriority;
			queueMessage.TimeToBeReceived = this.timeToBeReceived;
			queueMessage.TimeToReachQueue = this.timeToReachQueue;
			queueMessage.Recoverable = this.recoverable;
			queueMessage.UseAuthentication = this.useAuthentication;
			queueMessage.UseDeadLetterQueue = this.useDeadLetterQueue;
			queueMessage.UseEncryption = this.useEncryption;

			return queueMessage;
		}

		internal string QueuePath
		{
			get { return queuePath; }
		}

		private void SendMessageToQueue(string message)
		{
			using (IMsmqSendInterface messageQueueInterface = this.msmqInterfaceFactory.CreateMsmqInterface(this.queuePath))
			{
				using (Message queueMessage = CreateMessage(message, string.Empty))
				{
					messageQueueInterface.Send(queueMessage, this.transactionType);
					messageQueueInterface.Close();
				}
			}
		}

		private void SendMessageToQueue(LogEntry logEntry)
		{
			using (IMsmqSendInterface messageQueueInterface = this.msmqInterfaceFactory.CreateMsmqInterface(this.queuePath))
			{
				using (Message queueMessage = CreateMessage(logEntry))
				{
					messageQueueInterface.Send(queueMessage, this.transactionType);
					messageQueueInterface.Close();
				}
			}
		}

		private string FormatEntry(LogEntry entry)
		{
			string formattedMessage = Formatter.Format(entry);

			return formattedMessage;
		}
	}
}
