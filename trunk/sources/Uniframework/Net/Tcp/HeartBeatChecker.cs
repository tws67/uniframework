// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Uniframework.Net
{
    /// <summary>
    /// 心跳检查器
    /// </summary>
    public abstract class  HeartBeatChecker : DisposableAndStartableBase
    {
        protected Timer checkTimer;
        private int heartBeatPeriod;
        private bool enableCheckHeartBeat;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public HeartBeatChecker()
        {
            heartBeatPeriod = 30000; // 默认时间间隔30秒
        }

        /// <summary>
        /// 是否启动心跳检查功能
        /// </summary>
        public bool EnableCheckHeartBeat
        {
            get { return enableCheckHeartBeat; }
            set { enableCheckHeartBeat = value; }
        }

        /// <summary>
        /// 心跳检查时间间隔(毫秒)
        /// </summary>
        public int HeartBeatPeriod
        {
            get { return heartBeatPeriod; }
            set { heartBeatPeriod = value; }
        }

        /// <summary>
        /// 启动心跳检查功能
        /// </summary>
        protected override void OnStart()
        {
            if (EnableCheckHeartBeat) {
                checkTimer = new Timer(new TimerCallback(CheckHeartBeatCallBack), null,
                    HeartBeatPeriod, HeartBeatPeriod);
                NetDebuger.PrintDebugMessage("Start heartbeat checker, Period:" + HeartBeatPeriod + "(ms)");
            }
        }

        /// <summary>
        /// 停止检查心跳功能
        /// </summary>
        protected override void OnStop()
        {
            if (EnableCheckHeartBeat && checkTimer != null) {
                lock (checkTimer) {
                    if (EnableCheckHeartBeat && checkTimer != null) {
                        NetDebuger.PrintDebugMessage("Stop heartbeat checker");
                        checkTimer.Dispose();
                        checkTimer = null;
                    }
                }
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="dispodedByUser"></param>
        protected override void Free(bool disposing)
        {
            if (disposing) {
                Stop();
            }

            base.Free(disposing);
        }

        /// <summary>
        /// 检查心跳的回调函数
        /// </summary>
        /// <param name="o">参数（未使用）</param>
        protected abstract void CheckHeartBeatCallBack(object obj);
    }
}
