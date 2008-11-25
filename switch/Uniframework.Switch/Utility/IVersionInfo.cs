using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// ͨ����Ϣ��ʶ�ӿڣ����ڶ԰忨����������������ȵı�ʶ
    /// </summary>
    public interface IVersionInfo
    {
        /// <summary>
        /// ����
        /// </summary>
        string Name {get; set; }
        /// <summary>
        /// ������Ϣ
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// �汾
        /// </summary>
        string Version { get; set; }
        /// <summary>
        /// ��չ������Ϣ
        /// </summary>
        Dictionary<string, object> Properties { get;}
    }
}
