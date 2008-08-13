//===============================================================================
// Microsoft patterns & practices
// Mobile Client Software Factory - July 2006
using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Client.OfflineProxy
{
	public interface IConnectionMonitor
	{
		/// <summary>
		/// Get the current connection price.
		/// It should throw an InvalidOperationException if there is not connection.
		/// </summary>
        uint CurrentConnectionPrice { get;}

		/// <summary>
		/// Get the current network.
		/// </summary>
        string CurrentNetwork { get;}

		/// <summary>
		/// Gets the connection state.
		/// </summary>
		bool IsConnected { get;}

		/// <summary>
		/// Event fired when the device gets connected.
		/// </summary>
		event EventHandler ConnectionStatusChanged;
	}
}
