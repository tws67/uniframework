using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Uniframework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class db4oEnlist : IEnlistmentNotification
    {
        IObjectContainer db;
        object oldItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="db4oEnlist"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="item">The item.</param>
        public db4oEnlist(IObjectContainer database, object item)
        {
            db = database;
            oldItem = item;
        }

        #region IEnlistmentNotification ≥…‘±

        public void Commit(Enlistment enlistment)
        {
            db.Commit();
            oldItem = null;
        }

        public void InDoubt(Enlistment enlistment)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        public void Rollback(Enlistment enlistment)
        {
            db.Rollback();
            db.Ext().Refresh(oldItem, int.MaxValue);
        }

        #endregion
    }
}
