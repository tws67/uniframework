using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultServiceCaller : IServiceCaller
    {
        private IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServiceCaller"/> class.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public DefaultServiceCaller(IKernel kernel)
        {
            this.kernel = kernel;
        }

        #region IServiceCaller Members

        /// <summary>
        /// ͨ���������ָ������
        /// </summary>
        /// <param name="method">����</param>
        /// <param name="parameters">��������</param>
        /// <returns></returns>
        public object Invoke(System.Reflection.MethodInfo method, object[] parameters)
        {
            object service = kernel[method.DeclaringType];
            ISystemService system = (ISystemService)kernel[typeof(ISystemService)];
            DynamicInvoker invoker = system.GetInvoker(method);
            return invoker(service, parameters);
        }

        #endregion
    }
}
