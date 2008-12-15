using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Lephone.Data;
using Lephone.Data.Definition;

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
    /// 数据列表基础类
    /// </summary>
    /// <typeparam name="DbEntity">The type of the b entity.</typeparam>
    public abstract class DataListBase<DbEntity> : IDataListViewHandler
    {
        #region Dependency services

        [ServiceDependency]
        public IDatabaseService DatabaseService
        {
            get;
            set;
        }

        #endregion

        #region IDataListViewHandler Members

        public event EventHandler Enter;

        public event EventHandler Leave;

        public abstract void Initilize();

        public abstract bool CanInsert { get; }
        public abstract void Insert();

        public abstract bool CanEdit { get; }
        public abstract void Edit();

        public abstract bool CanDelete { get; }
        public abstract void Delete();

        public abstract bool CanExpand { get; } 
        public abstract void Expand();

        public abstract bool CanCollaspe { get; }
        public abstract void Collaspe();

        public abstract bool CanFilter { get; }
        public abstract void Filter();

        public abstract bool CanRefresh { get;}
        public abstract void Refresh();

        #endregion

        protected void OnEnter(object sender, EventArgs e)
        {
            if (Enter != null)
                Enter(sender, e);
        }

        protected void OnLeave(object sender, EventArgs e)
        {
            if (Leave != null)
                Leave(sender, e);
        }
    }
}
