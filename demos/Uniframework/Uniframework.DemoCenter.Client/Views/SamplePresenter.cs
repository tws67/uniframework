using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.CompositeUI;

namespace Uniframework.DemoCenter.Client.Views
{
    public class SamplePresenter : Presenter<SampleView>
    {
        #region Services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        [ServiceDependency]
        public ISampleService SampleService
        {
            get;
            set;
        }

        #endregion

        public string Hello(string username)
        {
            return SampleService.Hello(username);
        }

        public string HelloOffline(string username)
        {
            return SampleService.HelloOffline(username);
        }

        public string Hello4Cache(string username)
        {
            return SampleService.Hello4Cache(username);
        }

        [EventSubscriber(Constants.Event_SamplePublisher)]
        public void OnSampleEvent(object sender, EventArgs<string> e)
        {
            View.SetSampleEvent(e.Data);
        }
    }
}
