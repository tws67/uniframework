using System;

namespace Uniframework.Client.ConnectionManagement
{
	/// <summary>
	/// Exception thrown when the ConnectionManager determines that it is not possible to go online
	/// </summary>
	[Serializable()]
	public class ConnectionUnavailableException : ApplicationException
	{
		/// <summary>
		/// Empty constructor for exception
		/// </summary>
		public ConnectionUnavailableException()
		{
		}

		/// <summary>
		/// Constructor for exception that accepts a message as to why the exception occured
		/// </summary>
		/// <param name="reason">Reason for the exception</param>
		public ConnectionUnavailableException(string reason) : base(reason)
		{	
		}
	}
}
