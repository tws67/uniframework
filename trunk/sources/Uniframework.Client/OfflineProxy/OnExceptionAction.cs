using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
	/// <summary>
	/// Action to take when an exception occurs during the dispatch of a request.
	/// </summary>
	public enum OnExceptionAction
	{
		Dismiss,
		Retry
	}
}
