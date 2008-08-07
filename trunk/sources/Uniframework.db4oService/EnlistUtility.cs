using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace Uniframework.Services.db4oService
{
    /// <summary>
    /// 分段提交事务处理帮助类
    /// </summary>
    public static class EnlistUtility
    {
        public static bool Enlist(IEnlistmentNotification enlist)
        {
            System.Transactions.Transaction currentTx = System.Transactions.Transaction.Current;
            if (currentTx != null)
            {
                currentTx.EnlistVolatile(enlist, EnlistmentOptions.None);
                return true;
            }
            return false;
        }
    }
}
