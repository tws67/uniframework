using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
	public class RequestDispatchedEventArgs : EventArgs
	{
		private DispatchResult result;
		private Request request;

		/// <summary>
		/// It creates an RequestDispatchedEventArgs for the given request and result.
		/// It is used by the RequestDispatched event, fired during the dispatch process.
		/// </summary>
		/// <param name="request">Request tried to be dispatched.</param>
		/// <param name="result">Dispatch process result.</param>
		public RequestDispatchedEventArgs(Request request, DispatchResult result)
		{
			this.request = request;
			this.result = result;
		}

		/// <summary>
		/// Request tried to be dispatched.
		/// </summary>
		public Request Request
		{
			get { return request; }
		}

		/// <summary>
		/// Result of the dispatch process.
		/// </summary>
		public DispatchResult Result
		{
			get { return result; }
		}
	}
}
