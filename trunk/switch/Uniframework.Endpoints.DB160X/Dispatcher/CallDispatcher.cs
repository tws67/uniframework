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
    /// 呼入事件分配器，主要用于完成对呼入电话的侦听并将相应的呼入事件分配给对应的通道
    /// </summary>
    public class CallDispatcher : EventDispatcher
    {
        private readonly int Defaultdelay = 425; // 线程延迟时隙
        private ICTIDriver driver = null;

        // 构造函数
        public CallDispatcher(Type serviceType, ICTIDriver driver)
            : base(serviceType)
        {
            Guard.ArgumentNotNull(driver, "driver");

            this.driver = driver;
        }

        // 构造函数
        public CallDispatcher(Type serviceType, Type baseType, ICTIDriver driver)
            : base(serviceType, baseType)
        {
            Guard.ArgumentNotNull(driver, "driver");

            this.driver = driver;
        }

        /// <summary>
        /// 侦听板卡上所有外线的呼入事件，在一个循环中进行侦听并且不断的检查是否发出了停止信号(abort)
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
                            case ChannelType.TRUNK: // 监听外线通道的呼入事件
                                if (D160X.RingDetect(i))
                                {
                                    n = i;
                                    //callNumber = GetCallerNumber(i);
                                    if (driver.Channels[i] != null)
                                        driver.Channels[i].Logger.Info(String.Format("Call 事件分配器探测到通道 {0} 有呼入事件发生，呼入电话号码为: ", i, callNumber));
                                }
                                break;

                            case ChannelType.USER:  // 监听内线通道的呼出事件
                                if (D160X.OffHookDetect(i))
                                {
                                    n = i;
                                    callNumber = driver.Channels[i].ChannelAlias;
                                    if (driver.Channels[i] != null)
                                        driver.Channels[i].Logger.Info(String.Format("Call 事件分配器探测到通道 {0} 有提机事件发生，呼出通道别名为: {1}", i, driver.Channels[i].ChannelAlias));
                                }
                                break;
                        }

                        // 分发事件到订阅者组件，此处不检查callNumber是否为空是因为没有来电显示D160X模拟卡
                        // 便无法取得主叫号码。
                        if (n != -1 && Subject.Invocations > 0)
                        {
                            ICallHandler handler = Subject as ICallHandler;
                            handler.Call(n, callNumber);
                        }
                    }
                }
                System.Threading.Thread.Sleep(Interval); // 使事件侦听暂停一段时间
            }
        }

        /// <summary>
        /// 获取呼入的主叫号码
        /// </summary>
        /// <param name="chnlno">通道编号</param>
        /// <returns>返回主叫号码，要取得主叫号码对应的电话线路必须申请来电显示功能，如果获取主叫号码超时则直接返回空字符串。</returns>
        private string GetCallerNumber(int chnlno)
        {
            byte[] callNumber = new byte[32 * sizeof(char)];
            long T = Environment.TickCount;
            int result = -1;

            // 只有当函数返回3或4时才表示接收完毕
            do
            {
                if (Environment.TickCount - T > driver.Timeout * Defaultdelay)
                    return "";
                System.Threading.Thread.Sleep(Defaultdelay);

                // 获取主叫号码
                lock (D160X.SyncObj)
                {
                    result = D160X.GetCallerIDStr(chnlno, callNumber);
                }
            } while (result != 3 && result != 4);

            string Astr = Encoding.UTF8.GetString(callNumber);
            Astr = Astr.Substring(0, Astr.Length - 8); // 去除FSK码
            return Astr;
        }
    }
}
