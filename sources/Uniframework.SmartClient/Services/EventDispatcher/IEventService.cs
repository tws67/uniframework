using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// �ͻ����첽�¼��������
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// ��ȡָ�����Ƶ��¼�������
        /// </summary>
        /// <param name="eventName">�¼�����������</param>
        /// <returns>����ָ�����Ƶ��¼������������򷵻�null</returns>
        EventDispatcher GetEventDispatcher(string eventName);
        /// <summary>
        /// ��ϵͳע���¼�������
        /// </summary>
        /// <param name="eventName">�¼�����������</param>
        /// <param name="eventDispatcher">�¼�������</param>
        void RegisterEventDispatcher(string eventName, EventDispatcher eventDispatcher);
        /// <summary>
        /// ��ϵͳע���¼�������
        /// </summary>
        /// <param name="eventName">�¼�����������</param>
        void UnRegisterEventDispatcher(string eventName);
        /// <summary>
        /// ������е��¼�������
        /// </summary>
        void ClearEventDispatcher();
        /// <summary>
        /// ����ָ�����¼�������
        /// </summary>
        /// <param name="eventName">�¼�����������</param>
        void StartEventDispatcher(string eventName);
        /// <summary>
        /// ָֹͣ�����¼�������
        /// </summary>
        /// <param name="eventName">�¼�����������</param>
        void StopEventDispatcher(string eventName);
        /// <summary>
        /// ע��һ���¼�������
        /// </summary>
        /// <param name="subscripter">�¼�������</param>
        void RegisterSubscripter(ISubscripterRegister subscripter);
        /// <summary>
        /// ע���¼�������
        /// </summary>
        /// <param name="subscripterName">�¼�����������</param>
        void UnRegisterSubscripter(string subscripterName);
    }
}
