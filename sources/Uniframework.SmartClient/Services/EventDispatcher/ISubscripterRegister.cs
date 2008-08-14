using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// �¼�������ע��ӿڣ�ʵ�ֶ������Լ����¼�������ע���ע������
    /// </summary>
    public interface ISubscripterRegister
    {
        /// <summary>
        /// ����������
        /// </summary>
        string Name { get; }
        /// <summary>
        /// ���¼�������ע���Լ�
        /// </summary>
        void Register();
        /// <summary>
        /// ���¼�������ע���Լ�
        /// </summary>
        void UnRegister();
    }
}
