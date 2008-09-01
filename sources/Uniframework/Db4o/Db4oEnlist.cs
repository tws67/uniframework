using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;

namespace Uniframework.Db4o
{
    /// <summary>
    /// 
    /// </summary>
    public class Db4oEnlist : IEnlistmentNotification
    {
        private IObjectContainer container;
        private object oldItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="db4oEnlist"/> class.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="item">The item.</param>
        public Db4oEnlist(IObjectContainer container, object item)
        {
            this.container = container;
            oldItem = item;
        }

        #region IEnlistmentNotification 成员

        /// <summary>
        /// Commits the specified enlistment.
        /// </summary>
        /// <param name="enlistment">The enlistment.</param>
        public void Commit(Enlistment enlistment)
        {
            container.Commit();
            oldItem = null;
        }

        /// <summary>
        /// Ins the doubt.
        /// </summary>
        /// <param name="enlistment">The enlistment.</param>
        public void InDoubt(Enlistment enlistment)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Prepares the specified preparing enlistment.
        /// </summary>
        /// <param name="preparingEnlistment">The preparing enlistment.</param>
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            preparingEnlistment.Prepared();
        }

        /// <summary>
        /// Rollbacks the specified enlistment.
        /// </summary>
        /// <param name="enlistment">The enlistment.</param>
        public void Rollback(Enlistment enlistment)
        {
            container.Rollback();
            container.Ext().Refresh(oldItem, int.MaxValue);
        }

        #endregion
    }
}
