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
        /// ������
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
        /// ��ͼ׼���÷���������Presenter�г�ʼ����ͼ
        /// </summary>
        public virtual void OnViewReady() { }

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
        /// Creates and shows a smart part on the specified workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to create and show.</typeparam>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The new smart part instance.</returns>
        public virtual TView ShowViewInWorkspace<TView>(string workspaceName)
        {
            TView view = WorkItem.SmartParts.AddNew<TView>();
            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view);
            return view;
        }

        /// <summary>
        /// Shows the view in workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <param name="spi">The spi.</param>
        /// <returns></returns>
        public virtual TView ShowViewInWorkspace<TView>(string workspaceName, ISmartPartInfo spi)
        {
            TView view = WorkItem.SmartParts.AddNew<TView>();
            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view, spi);
            return view;
        }

        /// <summary>
        /// Shows a specific smart part in the workspace. If a smart part with the specified id
        /// is not found in the <see cref="WorkItem.SmartParts"/> collection, a new instance
        /// will be created; otherwise, the existing instance will be re used.
        /// </summary>
        /// <typeparam name="TView">The type of the smart part to show.</typeparam>
        /// <param name="viewId">The id of the smart part in the <see cref="WorkItem.SmartParts"/> collection.</param>
        /// <param name="workspaceName">The name of the workspace in which to show the smart part.</param>
        /// <returns>The smart part instance.</returns>
        public virtual TView ShowViewInWorkspace<TView>(string viewId, string workspaceName)
        {
            TView view = default(TView);
            if (WorkItem.SmartParts.Contains(viewId))
            {
                view = WorkItem.SmartParts.Get<TView>(viewId);
            }
            else
            {
                view = WorkItem.SmartParts.AddNew<TView>(viewId);
            }

            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view);

            return view;
        }

        /// <summary>
        /// Shows the view in workspace.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <param name="viewId">The view id.</param>
        /// <param name="workspaceName">Name of the workspace.</param>
        /// <param name="spi">The spi.</param>
        /// <returns></returns>
        public virtual TView ShowViewInWorkspace<TView>(string viewId, string workspaceName, ISmartPartInfo spi)
        {
            TView view = default(TView);
            if (WorkItem.SmartParts.Contains(viewId))
            {
                view = WorkItem.SmartParts.Get<TView>(viewId);
            }
            else
            {
                view = WorkItem.SmartParts.AddNew<TView>(viewId);
            }

            IWorkspace wp = WorkItem.Workspaces.Get(workspaceName);
            if (wp != null)
                wp.Show(view, spi);

            return view;
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
