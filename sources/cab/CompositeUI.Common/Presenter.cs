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
        /// �������ҵ����
        /// </summary>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set { workItem = value; }
        }

        /// <summary>
        /// ��ͼ
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
        /// ������ͼ
        /// </summary>
        /// <param name="view">��ͼʵ��</param>
        protected virtual void AttachView(TView view)
        { }

        /// <summary>
        /// �ر���ͼ
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
        /// �鹹����
        /// </summary>
        ~Presenter()
        {
            Dispose(false);
        }

        #region IDisposable ��Ա

        /// <summary>
        /// ��Presenter�����ջ���ʹ��ʱ����
        /// </summary>
        /// <param name="disposing">��Դ�����־</param>
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
