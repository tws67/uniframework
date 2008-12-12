using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// ������ýӿ�
    /// </summary>
    public interface IServiceCaller
    {
        /// <summary>
        /// ͨ���������ָ������
        /// </summary>
        /// <param name="method">����</param>
        /// <param name="parameters">��������</param>
        /// <returns></returns>
        object Invoke(MethodInfo method, object[] parameters);
    }
}
