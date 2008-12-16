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
using Uniframework.Database;
using Uniframework.Services;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// �����б����
    /// </summary>
    public abstract partial class DataListBaseView : DevExpress.XtraEditors.XtraUserControl, IDataListHandler
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
        public abstract void Initilize();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�����µ���������
        /// </summary>
        /// <value>����<c>true</c>������Բ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public abstract bool CanInsert { get; }
        /// <summary>
        /// �����µ���������
        /// </summary>
        public abstract void Insert();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�༭ѡ����������
        /// </summary>
        /// <value><c>true</c>������Ա༭�Ļ�; ����Ϊ, <c>false</c>.</value>
        /// ����
        public abstract bool CanEdit { get; }
        /// <summary>
        /// �༭ѡ����������
        /// </summary>
        public abstract void Edit();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�ɷ�ɾ��ѡ����������
        /// </summary>
        /// <value>����<c>true</c>�������ɾ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public abstract bool CanDelete { get; }
        /// <summary>
        /// ɾ��ѡ����������
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ����չ��
        /// </summary>
        /// <value>����<c>true</c>�������չ���Ļ�; ����Ϊ, <c>false</c>.</value>
        public abstract bool CanExpand { get; }
        /// <summary>
        /// չ�������б���ͼ
        /// </summary>
        public abstract void Expand();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ�����۵�
        /// </summary>
        /// <value>����<c>true</c>��������۵��Ļ�; ����Ϊ, <c>false</c>.</value>
        public abstract bool CanCollaspe { get; }
        /// <summary>
        /// �۵������б���ͼ
        /// </summary>
        public abstract void Collaspe();

        /// <summary>
        /// ��ȡһ��ֵ������ǰ�����б��Ƿ����ˢ��
        /// </summary>
        /// <value>����<c>true</c>�������ˢ�µĻ�; ����Ϊ, <c>false</c>.</value>
        public abstract bool CanRefresh { get; }
        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public new abstract void Refresh();

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
