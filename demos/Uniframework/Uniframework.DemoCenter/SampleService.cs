using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Uniframework.DemoCenter
{
    public class SampleService : ISampleService
    {
        Timer timer;
        Timer timerChanged;

        public SampleService()
        {
            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;

            timerChanged = new Timer(5000);
            timerChanged.Elapsed += new ElapsedEventHandler(timerChanged_Elapsed);
            timerChanged.Enabled = true;
        }

        private void timerChanged_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (DataChanged != null) {
                DataChanged(this, new EventArgs());
            }
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SampleEvent != null) {
                SampleEvent(this, new EventArgs<string>(DateTime.Now.ToString()));
            }
        }

        #region ISampleService Members

        public string Hello(string username)
        {
            return "Hello " + username + ", sample test, now is : " + DateTime.Now.ToString();
        }

        public string HelloOffline(string username)
        {
            return "Hello " + username + ", offline test, now is : " + DateTime.Now.ToString();
        }

        public string Hello4Cache(string username)
        {
            return "hello " + username + ", cache test, now is : " + DateTime.Now.ToString();
        }

        public void ThrowException()
        {
            throw new UniframeworkException("Uniframework exception from server side.");
        }

        public event EventHandler<EventArgs<string>> SampleEvent;

        public event EventHandler DataChanged;

        #endregion
    }
}
