using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 计时器类(是线程安全的)
    /// </summary>
    public class TimeCounter : StartableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeCounter"/> class.
        /// </summary>
        public TimeCounter()
        {
            Reset();
        }

        private long startTime;
        private long spanTicks;
        private object syncObj = new object();

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            lock(syncObj)
                startTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Gets the milliseconds.
        /// </summary>
        /// <value>The milliseconds.</value>
        public long Milliseconds
        {
            get { return (long)(Ticks / 10000); }
        }

        /// <summary>
        /// Gets the seconds.
        /// </summary>
        /// <value>The seconds.</value>
        public long Seconds
        {
            get { return (long)(Ticks / 10000000); }
        }

        /// <summary>
        /// 从计时器启动到停止时的时间间隔，如果计时器还没有停止，就代表是到当前时间的时间间隔
        /// </summary>
        public long Ticks
        {
            get
            {
                lock (syncObj)
                {
                    if (!IsRun)
                    {
                        return spanTicks;
                    }
                    else
                    {
                        return DateTime.Now.Ticks - startTime;
                    }
                }
            }
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        protected override void OnStart()
        {
            Reset();
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        protected override void OnStop()
        {
            lock (syncObj)
                spanTicks = DateTime.Now.Ticks - startTime;
        }
    }
}
