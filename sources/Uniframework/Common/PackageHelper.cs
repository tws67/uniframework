using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// Զ�̵��ð�������
    /// </summary>
    public static class PackageHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly string METHOD_NAME = "contex_method";
        /// <summary>
        /// 
        /// </summary>
        public static readonly string PARAMETERS_NAME = "contex_parameters";

        /// <summary>
        /// ��װԶ�̹��̵��ð�
        /// </summary>
        /// <param name="package">��ʵ��</param>
        /// <param name="method">������Ϣ</param>
        /// <param name="parameters">������Ϣ</param>
        public static void EncodeInvoke(NetworkInvokePackage package, byte[] method, byte[] parameters)
        {
            package.Context[METHOD_NAME] = method;
            package.Context[PARAMETERS_NAME] = parameters;
        }
    }
}
