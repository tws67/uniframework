using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.Switch
{
    /// <summary>
    /// Uniframework �����
    /// </summary>
    public interface IVirtualCTI
    {
        /// <summary>
        /// ��������ȫ�ֱ���
        /// </summary>
        Dictionary<String, object> GlobalVars { get; }
        /// <summary>
        /// ������ - CAB����
        /// </summary>
        WorkItem WorkItem { get; }

        /// <summary>
        /// ���������ʱ��
        /// </summary>
        DateTime Initialized { get; }
        /// <summary>
        /// ������Ự��ʶ
        /// </summary>
        String Session_ID { get; }
        /// <summary>
        /// ��ǰ�����֧�ֵ���󲢷��Ự��
        /// </summary>
        Int32 SessionLimit { get; set; }
        /// <summary>
        /// ��������б�ʶ
        /// </summary>
        Boolean Running { get; }

        /// <summary>
        /// ���������
        /// </summary>
        void Start();
        /// <summary>
        /// ֹͣ�����
        /// </summary>
        void Shutdown();
    }
}
