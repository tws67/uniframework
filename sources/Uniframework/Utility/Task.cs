using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Uniframework
{
    #region 任务计划接口和一些标准实现
    /// <summary>
    /// 计划的接口
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// 返回最初计划执行时间
        /// </summary>
        DateTime ExecutionTime
        {
            get;
            set;
        }

        /// <summary>
        /// 初始化执行时间于现在时间的时间刻度差
        /// </summary>
        long DueTime
        {
            get;
        }

        /// <summary>
        /// 循环的周期
        /// </summary>
        long Period
        {
            get;
        }
    }

    /// <summary>
    /// 计划立即执行任务
    /// </summary>
    public class ImmediateExecution : ISchedule
    {
        #region ISchedule 成员

        /// <summary>
        /// 返回最初计划执行时间
        /// </summary>
        /// <value></value>
        public DateTime ExecutionTime
        {
            get
            {
                return DateTime.Now;
            }
            set
            {
                ;
            }
        }

        /// <summary>
        /// 初始化执行时间于现在时间的时间刻度差
        /// </summary>
        /// <value></value>
        public long DueTime
        {
            get
            {
                return 0;
            }
        }


        /// <summary>
        /// 循环的周期
        /// </summary>
        /// <value></value>
        public long Period
        {
            get
            {
                return Timeout.Infinite;
            }
        }

        #endregion
    }


    /// <summary>
    /// 计划在某一未来的时间执行一个操作一次，如果这个时间比现在的时间小，就变成了立即执行的方式
    /// </summary>
    public class ScheduleExecutionOnce : ISchedule
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="schedule">计划开始执行的时间</param>
        public ScheduleExecutionOnce(DateTime schedule)
        {
            m_schedule = schedule;
        }

        private DateTime m_schedule;

        #region ISchedule 成员

        /// <summary>
        /// 返回最初计划执行时间
        /// </summary>
        /// <value></value>
        public DateTime ExecutionTime
        {
            get
            {
                return m_schedule;
            }
            set
            {
                m_schedule = value;
            }
        }

        /// <summary>
        /// 得到该计划还有多久才能运行
        /// </summary>
        public long DueTime
        {
            get
            {
                long ms = (m_schedule.Ticks - DateTime.Now.Ticks) / 10000;

                if (ms < 0) ms = 0;
                return ms;
            }
        }

        /// <summary>
        /// 循环的周期
        /// </summary>
        /// <value></value>
        public long Period
        {
            get
            {
                return Timeout.Infinite;
            }
        }

        #endregion
    }


    /// <summary>
    /// 周期性的执行计划
    /// </summary>
    public class CycExecution : ISchedule
    {
        /// <summary>
        /// 构造函数，在一个将来时间开始运行
        /// </summary>
        /// <param name="shedule">计划执行的时间</param>
        /// <param name="period">周期时间</param>
        public CycExecution(DateTime shedule, TimeSpan period)
        {
            m_schedule = shedule;
            m_period = period;
        }

        /// <summary>
        /// 构造函数,马上开始运行
        /// </summary>
        /// <param name="period">周期时间</param>
        public CycExecution(TimeSpan period)
        {
            m_schedule = DateTime.Now;
            m_period = period;
        }

        private DateTime m_schedule;
        private TimeSpan m_period;

        #region ISchedule 成员

        /// <summary>
        /// 初始化执行时间于现在时间的时间刻度差
        /// </summary>
        /// <value></value>
        public long DueTime
        {
            get
            {
                long ms = (m_schedule.Ticks - DateTime.Now.Ticks) / 10000;

                if (ms < 0) ms = 0;
                return ms;
            }
        }

        /// <summary>
        /// 返回最初计划执行时间
        /// </summary>
        /// <value></value>
        public DateTime ExecutionTime
        {
            get
            {
                return m_schedule;
            }
            set
            {
                m_schedule = value;
            }


        }

        /// <summary>
        /// 循环的周期
        /// </summary>
        /// <value></value>
        public long Period
        {
            get
            {
                return m_period.Ticks / 10000;
            }
        }

        #endregion

    }


    #endregion

    #region 任务实现
    /// <summary>
    /// 计划任务基类
    /// 启动的任务会在工作工作线程中完成，调用启动方法后会立即返回。
    /// 
    /// 用法：
    /// (1)如果你要创建自己的任务，需要从这个类继承一个新类，然后重载Execute(object param)方法．
    /// 实现自己的任务,再把任务加入到任务管理中心来启动和停止。
    /// 比如：
    /// TaskCenter center = new TaskCenter();
    /// Task newTask = new Task( new ImmediateExecution());
    /// center.AddTask(newTask);
    /// center.StartAllTask();

    /// (2)直接把自己的任务写入TimerCallBack委托，然后生成一个Task类的实例，
    /// 设置它的Job和JobParam属性，再Start就可以启动该服务了。此时不能够再使用任务管理中心了。
    /// 比如：
    /// Task newTask = new Task( new ImmediateExecution());
    ///	newTask.Job+= new TimerCallback(newTask.Execute);
    ///	newTask.JobParam = "Test immedialte task"; //添加自己的参数
    ///	newTask.Start();
    ///	
    /// </summary>
    public class Task
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="schedule">为每个任务制定一个执行计划</param>
        public Task(ISchedule schedule)
        {
            if (schedule == null)
            {
                throw (new ArgumentNullException("schedule"));
            }

            schedule = schedule;
        }


        /// <summary>
        /// 启动任务
        /// </summary>
        public void Start()
        {
            //启动定时器
            m_timer = new Timer(execTask, param, schedule.DueTime, schedule.Period);
        }


        /// <summary>
        /// 停止任务
        /// </summary>
        public void Stop()
        {
            //停止定时器
            m_timer.Change(Timeout.Infinite, Timeout.Infinite);

        }


        /// <summary>
        /// 任务内容
        /// </summary>
        /// <param name="param">任务函数参数</param>
        public virtual void Execute(object param)
        {
            //你需要重载该函数,但是需要在你的新函数中调用base.Execute();
            lastExecuteTime = DateTime.Now;

            if (schedule.Period == Timeout.Infinite)
            {
                nextExecuteTime = DateTime.MaxValue; //下次运行的时间不存在
            }
            else
            {
                TimeSpan period = new TimeSpan(schedule.Period * 1000);

                nextExecuteTime = lastExecuteTime + period;
            }
        }

        /// <summary>
        /// 任务下执行时间
        /// </summary>
        public DateTime NextExecuteTime
        {
            get
            {
                return nextExecuteTime;
            }
        }

        DateTime nextExecuteTime;
        /// <summary>
        /// 执行任务的计划
        /// </summary>
        public ISchedule Shedule
        {
            get
            {
                return schedule;
            }
        }

        private ISchedule schedule = null;

        /// <summary>
        /// 系统定时器
        /// </summary>
        private Timer m_timer;

        /// <summary>
        /// 任务内容
        /// </summary>
        public TimerCallback Job
        {
            get
            {
                return execTask;
            }
            set
            {
                execTask = value;
            }
        }

        private TimerCallback execTask;

        /// <summary>
        /// 任务参数
        /// </summary>
        public object JobParam
        {
            set
            {
                param = value;
            }
        }
        private object param;

        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        private string name;


        /// <summary>
        /// 任务描述
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
        private string description;

        /// <summary>
        /// 该任务最后一次执行的时间
        /// </summary>
        public DateTime LastExecuteTime
        {
            get
            {
                return lastExecuteTime;
            }
        }
        private DateTime lastExecuteTime;


    }

    #endregion

    #region 启动任务

    /// <summary>
    /// 任务管理中心
    /// 使用它可以管理一个或则多个同时运行的任务
    /// </summary>
    public class TaskCenter
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TaskCenter()
        {
            scheduleTasks = new ArrayList();
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="newTask">新任务</param>
        public void AddTask(Task newTask)
        {
            scheduleTasks.Add(newTask);
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="delTask">将要删除的任务，你可能需要停止掉该任务</param>
        public void DelTask(Task delTask)
        {
            scheduleTasks.Remove(delTask);
        }

        /// <summary>
        /// 启动所有的任务
        /// </summary>
        public void StartAllTask()
        {
            foreach (Task task in ScheduleTasks)
            {
                StartTask(task);
            }
        }

        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="task"></param>
        public void StartTask(Task task)
        {
            //标准启动方法
            if (task.Job == null)
            {
                task.Job += new TimerCallback(task.Execute);
            }

            task.Start();
        }

        /// <summary>
        /// 终止所有的任务
        /// </summary>
        public void TerminateAllTask()
        {
            foreach (Task task in ScheduleTasks)
            {
                TerminateTask(task);
            }
        }

        /// <summary>
        /// 终止一个任务
        /// </summary>
        /// <param name="task"></param>
        public void TerminateTask(Task task)
        {
            task.Stop();
        }

        /// <summary>
        /// 获得所有的
        /// </summary>
        ArrayList ScheduleTasks
        {
            get
            {
                return scheduleTasks;
            }
        }
        private ArrayList scheduleTasks;

    }

    #endregion
}
