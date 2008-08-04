using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Practices.CompositeUI.SmartParts;
using Microsoft.Practices.CompositeUI.Common.Services;

namespace Microsoft.Practices.CompositeUI.Common
{
    public abstract class Presenter<TView> : IDisposable
    {
        private TView view;
        private WorkItem workItem;

        /// <summary>
        /// 工作项，商业用例
        /// </summary>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set { workItem = value; }
        }

        /// <summary>
        /// 视图
        /// </summary>
        public TView View
        {
            get { return view; }
            set
            {
                view = value;
                AttachView(value);
            }
        }

        /// <summary>
        /// 设置视图
        /// </summary>
        /// <param name="view">视图实例</param>
        protected virtual void AttachView(TView view)
        { }

        /// <summary>
        /// 关闭视图
        /// </summary>
        protected virtual void CloseView()
        {
            IWorkspaceLocatorService locator = WorkItem.Services.Get<IWorkspaceLocatorService>();
            if (locator != null)
            {
                IWorkspace wp = locator.FindContainingWorkspace(workItem, view);
                if (wp != null)
                    wp.Close(view);
            }
        }

        /// <summary>
        /// 虚构函数
        /// </summary>
        ~Presenter()
        {
            Dispose(false);
        }

        #region IDisposable 成员

        /// <summary>
        /// 当Presenter被回收或不再使用时调用
        /// </summary>
        /// <param name="disposing">资源清除标志</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (workItem != null)
                    workItem.Items.Remove(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
