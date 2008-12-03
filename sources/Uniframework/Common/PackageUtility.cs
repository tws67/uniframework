using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Uniframework.Services;

namespace Uniframework
{
    /// <summary>
    /// Զ�̵��ð�������
    /// </summary>
    public static class PackageUtility
    {
        private static Serializer serializer = new Serializer();

        /// <summary>
        /// Զ�̷������÷�װ����
        /// </summary>
        public static readonly string METHOD                  = "context_uf_method"; // ������
        public static readonly string METHOD_NAME             = "context_uf_method_name"; // ������
        public static readonly string METHOD_PARAMETERS       = "context_uf_method_parameters"; // ��������
        public static readonly string METHOD_ISGENERIC        = "context_uf_method_isgeneric"; // �Ƿ��Ƿ��ͷ���
        public static readonly string METHOD_INVOKEINFO       = "context_uf_invokeinfo"; // ������������Ϣ�����ڷ����������ɷ��ͷ�����

        public static readonly string SESSION_ID              = "context_uf_session_id"; // �Ự��ʶ
        public static readonly string SESSION_USERNAME        = "context_uf_session_username"; // �û���ʶ
        public static readonly string SESSION_PASSWORD        = "context_uf_session_password"; // �û�����
        public static readonly string SESSION_IPADDRESS       = "context_uf_session_ipaddress"; // �ͻ���IP��ַ
        public static readonly string SESSION_ENCTRYPTKEY     = "context_uf_session_encryptkey"; // ��Կ

        /// <summary>
        /// 
        /// </summary>
        public static readonly string PARAMETERS_NAME = "contex_uf_parameters";


        ///// <summary>
        ///// ����Զ�̹��̵��ð�
        ///// </summary>
        ///// <param name="method">The method.</param>
        ///// <param name="parameters">The parameters.</param>
        ///// <param name="package">The package.</param>
        //public static void EncodeInvoke(MethodInfo method, object parameters, string encryptKey, ref NetworkInvokePackage package)
        //{
        //    package.Context[METHOD] = serializer.Serialize<MethodInfo>(method);
        //    package.Context[METHOD_NAME] = serializer.Serialize<string>(method.Name);
        //    package.Context[METHOD_PARAMETERS] = SecurityUtility.DESEncrypt(serializer.Serialize<object>(parameters), encryptKey);
        //}

        /// <summary>
        /// ����Զ�̹��̵��ð�
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="method">The method.</param>
        /// <param name="parameters">The parameters.</param>
        public static void DecodeInvoke(NetworkInvokePackage package, string encryptKey, out MethodInfo method, out object[] parameters)
        {
            method = serializer.Deserialize<MethodInfo>((byte[])package.Context[METHOD]);
            parameters = serializer.Deserialize<object[]>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_PARAMETERS], encryptKey));
        }

        /// <summary>
        /// ����Զ�̹��̵��ð�
        /// </summary>
        /// <param name="package">The package.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="invokeInfo">The invoke info.</param>
        public static void DecodeInvoke(NetworkInvokePackage package, string encryptKey, out MethodInvokeInfo invokeInfo, out object[] parameters)
        {
            invokeInfo = serializer.Deserialize<MethodInvokeInfo>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_INVOKEINFO], encryptKey));
            parameters = serializer.Deserialize<object[]>(SecurityUtility.DESDecrypt((byte[])package.Context[METHOD_PARAMETERS], encryptKey)); 
        }

        /// <summary>
        /// Encodes the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="encryptKey">The encrypt key.</param>
        /// <param name="package">The package.</param>
        public static void EncodeInvoke(MethodInfo method, object parameters, string encryptKey, ref NetworkInvokePackage package)
        {
            package.Context[METHOD]            = serializer.Serialize<MethodInfo>(method);
            package.Context[METHOD_NAME]       = serializer.Serialize<string>(method.Name);
            package.Context[METHOD_ISGENERIC]  = method.IsGenericMethod;
            package.Context[METHOD_PARAMETERS] = SecurityUtility.DESEncrypt(serializer.Serialize<object>(parameters), encryptKey);
            package.Context[METHOD_INVOKEINFO] = SecurityUtility.DESEncrypt(serializer.Serialize<MethodInvokeInfo>(new MethodInvokeInfo(method)), encryptKey);
        }
    }
}
