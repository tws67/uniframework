using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Uniframework.Client.OfflineProxy
{
    /// <summary>
    /// ¿Îœﬂ«Î«Û
    /// </summary>
	public class Request
	{
        private Guid requestId;
		private OfflineBehavior behavior;
        private MethodInfo method;
		private object[] callParameters;
        private object result;
        private DefaultServiceAgent serviceAgent;

		/// <summary>
		/// Default constructor.
		/// It sets a new RequestId, an empty Behavior and an empty CallParameters object array.
		/// </summary>
		public Request()
		{
			requestId = Guid.NewGuid();
			behavior = new OfflineBehavior();
			callParameters = new object[0];
		}

        public Request(MethodInfo method, DefaultServiceAgent serviceAgent)
            : this()
        {
            this.method = method;
            this.serviceAgent = serviceAgent;
        }

        public Request(MethodInfo method, DefaultServiceAgent serviceAgent, params object[] callParameters)
            : this(method, serviceAgent)
        {
            this.callParameters = callParameters;
        }

        /// <summary>
        /// Global unique identifier for the request.
        /// </summary>
        public Guid RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }

		/// <summary>
		/// Offline behavior for the current request.
		/// It contains several options.
		/// </summary>
		public OfflineBehavior Behavior
		{
			get { return behavior; }
			set { behavior = value; }
		}

        /// <summary>
        /// the request of remote method
        /// </summary>
        public MethodInfo Method
        {
            get { return method; }
            set { method = value; }
        }

		/// <summary>
		/// Object array with the parameters to be used dispatching the request.
		/// </summary>
		public object[] CallParameters
		{
			get { return callParameters; }
			set { callParameters = value; }
		}

        /// <summary>
        /// the result of remote call
        /// </summary>
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        public DefaultServiceAgent ServiceAgent
        {
            get { return serviceAgent; }
            set { serviceAgent = value; }
        }
	}
}
