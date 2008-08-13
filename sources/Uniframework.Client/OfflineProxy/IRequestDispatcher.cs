using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
	/// <summary>
	/// Valid values for the dispatching process result.
	/// </summary>
	public enum DispatchResult
	{
		Succeeded,
		Failed,
		Expired
	}

	public interface IRequestDispatcher
	{
		/// <summary>
		/// This method's implementations should dispatch a request for the given network name.
		/// </summary>
		/// <param name="request">Request to be dispatched.</param>
		/// <param name="networkName">Current network name.</param>
		/// <returns>DispatchResult with the corresponding result of the dispatching process.</returns>
		DispatchResult Dispatch(Request request, string networkName);
	}
}
