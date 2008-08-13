using System;

namespace Uniframework.Client.ConnectionManagement
{
	/// <summary>
	/// Interface defining how the connection detection will occur. 
	/// </summary>
	public interface IConnectionDetectionStrategy
	{
		/// <summary>
		/// Returns whether or not the implementing class believe we are connected to a network
		/// </summary>
		/// <returns>True if and only if the appropriate connectivity is detected</returns>
		bool IsConnected();
		
		/// <summary>
		/// Getter property for the polling interval in seconds.
		/// </summary>
		/// <value>Polling interaval in seconds</value>
		int  PollInterval { get; }
	}
}
