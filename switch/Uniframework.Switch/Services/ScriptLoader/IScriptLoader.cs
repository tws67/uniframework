using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// �ű����ع��߽ӿ�
    /// </summary>
    public interface IScriptLoader
    {
        /// <summary>
        /// �ű������ļ���
        /// </summary>
        string FileName { get; }
        /// <summary>
        /// �ű��������������
        /// </summary>
        string MainPoint { get; set; }
        /// <summary>
        /// ִ�нű����򣬼򻯽ű�����������Ҫ��Ϊ�˷���ϵͳͨ���߳�ִ��
        /// </summary>
        void RunScript();
    }
}
