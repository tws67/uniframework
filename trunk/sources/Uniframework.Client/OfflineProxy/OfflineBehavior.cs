using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
	public class OfflineBehavior
	{
		private uint maxRetries;
		private CommandCallback returnCallback;
		private DateTime? expiration;
		private uint stamps;
		private string tag;
		private CommandCallback exceptionCallback;
		private Guid messageId;
		private DateTime? queuedDate;

		/// <summary>
		/// Callback to be invoked if an Exception is thrown during the dispatching process.
		/// </summary>
		public CommandCallback ExceptionCallback
		{
			get { return exceptionCallback; }
			set { exceptionCallback = value; }
		}

		/// <summary>
		/// Maximum datetime to dispatch a request. If the request is expired, it will not be dispatched and
		/// a RequestDispatched event will be fired with the DispatchResult.Expired result in the argument.
		/// </summary>
		public DateTime? Expiration
		{
			get { return expiration; }
			set { expiration = value; }
		}

		/// <summary>
		/// Datetime when the request has been enqueued.
		/// </summary>
		public DateTime? QueuedDate
		{
			get { return queuedDate; }
			set { queuedDate = value; }
		}

		/// <summary>
		/// Maximum number of dispatching retries, before to move a failing dispatch request in the dead letter queue.
		/// </summary>
		public uint MaxRetries
		{
			get { return maxRetries; }
			set { maxRetries = value; }
		}

		/// <summary>
		/// Callback to be invoked when a request is dispatched succesfully. 
		/// </summary>
		public CommandCallback ReturnCallback
		{
			get { return returnCallback; }
			set { returnCallback = value; }
		}

		/// <summary>
		/// Maximum connectivity price to be dispatched.
		/// The request will be dispatched automatically if the connectivity price
		/// is equal or lower than this value.
		/// </summary>
		public uint Stamps
		{
			get { return stamps; }
			set { stamps = value; }
		}

		/// <summary>
		/// String to provide a customizable clasification for the request.
		/// </summary>
		public string Tag
		{
			get { return tag; }
			set { tag = value; }
		}

		/// <summary>
		/// Message Id
		/// </summary>
		public Guid MessageId
		{
			get { return messageId; }
			set { messageId = value; }
		}
	}
}
