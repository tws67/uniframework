using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.Utility;
using Uniframework.SmartClient;

namespace Uniframework.Switch.Endpoints.DB160X
{
    public interface ICallHandler
    {
        void Call(int chnlno, string callNumber);
    }

    /// <summary>
    /// �����¼�����������Ҫ������ɶԺ���绰������������Ӧ�ĺ����¼��������Ӧ��ͨ��
    /// </summary>
    public class CallDispatcher : EventDispatcher
    {
        private readonly int Defaultdelay = 425; // �߳��ӳ�ʱ϶
        private ICTIDriver driver = null;

        // ���캯��
        public CallDispatcher(Type serviceType, ICTIDriver driver)
            : base(serviceType)
        {
            Guard.ArgumentNotNull(driver, "driver");

            this.driver = driver;
        }

        // ���캯��
        public CallDispatcher(Type serviceType, Type baseType, ICTIDriver driver)
            : base(serviceType, baseType)
        {
            Guard.ArgumentNotNull(driver, "driver");

            this.driver = driver;
        }

        /// <summary>
        /// �����忨���������ߵĺ����¼�����һ��ѭ���н����������Ҳ��ϵļ���Ƿ񷢳���ֹͣ�ź�(abort)
        /// </summary>
        protected override void Listen()
        {
            while (!abort)
            {
                int n;
                bool chnlRing = false;
                string callNumber;
                for(int i = 0; i < driver.ChannelCount; i++)
                {
                    lock (D160X.SyncObj)
                    {
                        n = -1;
                        callNumber = string.Empty;
                        switch (driver.Channels[i].ChannelType)
                        {
                            case ChannelType.TRUNK: // ��������ͨ���ĺ����¼�
                                if (D160X.RingDetect(i))
                                {
                                    n = i;
                                    //callNumber = GetCallerNumber(i);
                                    if (driver.Channels[i] != null)
                                        driver.Channels[i].Logger.Info(String.Format("Call �¼�������̽�⵽ͨ�� {0} �к����¼�����������绰����Ϊ: ", i, callNumber));
                                }
                                break;

                            case ChannelType.USER:  // ��������ͨ���ĺ����¼�
                                if (D160X.OffHookDetect(i))
                                {
                                    n = i;
                                    callNumber = driver.Channels[i].ChannelAlias;
                                    if (driver.Channels[i] != null)
                                        driver.Channels[i].Logger.Info(String.Format("Call �¼�������̽�⵽ͨ�� {0} ������¼�����������ͨ������Ϊ: {1}", i, driver.Channels[i].ChannelAlias));
                                }
                                break;
                        }

                        // �ַ��¼���������������˴������callNumber�Ƿ�Ϊ������Ϊû��������ʾD160Xģ�⿨
                        // ���޷�ȡ�����к��롣
                        if (n != -1 && Subject.Invocations > 0)
                        {
                            ICallHandler handler = Subject as ICallHandler;
                            handler.Call(n, callNumber);
                        }
                    }
                }
                System.Threading.Thread.Sleep(Interval); // ʹ�¼�������ͣһ��ʱ��
            }
        }

        /// <summary>
        /// ��ȡ��������к���
        /// </summary>
        /// <param name="chnlno">ͨ�����</param>
        /// <returns>�������к��룬Ҫȡ�����к����Ӧ�ĵ绰��·��������������ʾ���ܣ������ȡ���к��볬ʱ��ֱ�ӷ��ؿ��ַ�����</returns>
        private string GetCallerNumber(int chnlno)
        {
            byte[] callNumber = new byte[32 * sizeof(char)];
            long T = Environment.TickCount;
            int result = -1;

            // ֻ�е���������3��4ʱ�ű�ʾ�������
            do
            {
                if (Environment.TickCount - T > driver.Timeout * Defaultdelay)
                    return "";
                System.Threading.Thread.Sleep(Defaultdelay);

                // ��ȡ���к���
                lock (D160X.SyncObj)
                {
                    result = D160X.GetCallerIDStr(chnlno, callNumber);
                }
            } while (result != 3 && result != 4);

            string Astr = Encoding.UTF8.GetString(callNumber);
            Astr = Astr.Substring(0, Astr.Length - 8); // ȥ��FSK��
            return Astr;
        }
    }
}
