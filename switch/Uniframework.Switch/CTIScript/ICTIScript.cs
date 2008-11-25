using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// CTI�ű��ӿڣ�������Ҫ֧�����������Ľű�������̳�����ӿ�
    /// </summary>
    public interface ICTIScript
    {
        /// <summary>
        /// �ű������õ�ͨ��
        /// </summary>
        IChannel Channel { get; }
        /// <summary>
        /// �ű���ʼ������
        /// </summary>
        /// <param name="chnl"></param>
        void Initialize(IChannel chnl);
        /// <summary>
        /// �ű�ִ���庯�������е�ҵ���߼����ڴ˴�
        /// </summary>
        void Run();
        /// <summary>
        /// �ű�ִ�������ذ汾��������ִ��ʱ������Ӧ�Ĳ���
        /// </summary>
        /// <param name="args">�����б�</param>
        void Run(object[] args);
        /// <summary>
        /// �ű�����������
        /// </summary>
        void Terminate();
    }
}
