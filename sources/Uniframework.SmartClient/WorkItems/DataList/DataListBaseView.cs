using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.CompositeUI.Commands;
using Microsoft.Practices.CompositeUI.Common;
using Microsoft.Practices.ObjectBuilder;

using Uniframework;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// �����б����
    /// </summary>
    public partial class DataListBaseView : DevExpress.XtraEditors.XtraUserControl, IDataListHandler
    {
        public DataListBaseView()
        {
            InitializeComponent();
        }

        #region Dependency Services

        [ServiceDependency]
        public WorkItem WorkItem
        {
            get;
            set;
        }

        #endregion

        #region Method && Properties

        /// <summary>
        /// ��ʼ�������б����ֻ�������б��һ�μ���ʱʹ��
        /// </summary>
        public virtual void Initilize() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�����µ���������
        /// </summary>
        /// <value>����<c>true</c>������Բ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public virtual bool CanInsert {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// �����µ���������
        /// </summary>
        public virtual void Insert() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�༭ѡ����������
        /// </summary>
        /// <value><c>true</c>������Ա༭�Ļ�; ����Ϊ, <c>false</c>.</value>
        /// ����
        public virtual bool CanEdit { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// �༭ѡ����������
        /// </summary>
        public virtual void Edit() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�ɾ��ѡ����������
        /// </summary>
        /// <value>����<c>true</c>�������ɾ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public virtual bool CanDelete { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// ɾ��ѡ����������
        /// </summary>
        public virtual void Delete() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ����չ��
        /// </summary>
        /// <value>����<c>true</c>�������չ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public virtual bool CanExpand { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// չ�������б���ͼ
        /// </summary>
        public virtual void Expand() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ�����۵�
        /// </summary>
        /// <value>����<c>true</c>��������۵��Ļ�; ����Ϊ, <c>false</c>.</value>
        public virtual bool CanCollaspe { 
            get { throw new NotImplementedException(); } 
        }
        /// <summary>
        /// �۵������б���ͼ
        /// </summary>
        public virtual void Collaspe() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ����ˢ��
        /// </summary>
        /// <value>����<c>true</c>�������ˢ�µĻ�; ����Ϊ, <c>false</c>.</value>
        public virtual bool CanRefreshDataSource { 
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public virtual void RefreshDataSource() {
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        /// <summary>
        /// ���ݲ���ǰ���¼�
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataInserting;
        /// <summary>
        /// ���ݲ������¼�
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataInserted;

        /// <summary>
        /// ���ݱ༭ǰ���¼�
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataEditing;
        /// <summary>
        /// ���ݱ༭����¼�
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataEdited;

        /// <summary>
        /// ����ɾ��ǰ���¼�
        /// </summary>
        public event EventHandler<CancelDataHandlerEventArgs> DataDeleting;
        /// <summary>
        /// ����ɾ������¼�
        /// </summary>
        public event EventHandler<DataHandlerEventArgs> DataDeleted;

        /// <summary>
        /// �б�ˢ�º���¼�
        /// </summary>
        public event EventHandler<EventArgs> DataRefreshed;

        #endregion

        /// <summary>
        /// ������Դ���б�
        /// </summary>
        protected virtual void BindingDataSource()
        { 

        }
    }
}
