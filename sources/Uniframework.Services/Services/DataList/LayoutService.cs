using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uniframework.Db4o;

namespace Uniframework.Services
{
    /// <summary>
    /// 列表布局服务
    /// </summary>
    public class LayoutService : ILayoutService
    {
        private readonly string DEFAULT_USER = "Admin";
        private readonly string LAYOUTDB = "Layouts.yap";

        private IDb4oDatabase db;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutService"/> class.
        /// </summary>
        /// <param name="dbService">The db service.</param>
        public LayoutService(IDb4oDatabaseService dbService)
        {
            db = dbService.Open(LAYOUTDB);
        }

        #region ILayoutService Members

        /// <summary>
        /// 保存列表布局到数据库
        /// </summary>
        /// <param name="layout">布局信息</param>
        public void StoreLayout(Layout layout)
        {
            DeleteLayout(layout.User, layout.Module, layout.AppUri);
            db.Store(layout);
        }

        /// <summary>
        /// 从数据库恢复列表布局信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="module">模块</param>
        /// <param name="appUri">应用程序路径</param>
        /// <returns>如果存在列表布局信息则返回其布局信息否则为空</returns>
        public Layout RestoreLayout(string user, string module, string appUri)
        {
            IList<Layout> layouts = db.Query<Layout>(lay => 
                lay.User == user && lay.Module == module && lay.AppUri == appUri);
            if (layouts.Count > 0)
                return layouts[0];
            else {
                // 如果不存在用户定义的布局信息则加载管理员的布局
                layouts = db.Query<Layout>(lay => 
                    lay.User == DEFAULT_USER && lay.Module == module && lay.AppUri == appUri);
                if (layouts.Count > 0)
                    return layouts[0];
            }
            return null;
        }

        #endregion

        private void DeleteLayout(string user, string module, string appUri)
        {
            IList<Layout> layouts = db.Query<Layout>(lay => 
                lay.User == user && lay.Module == module && lay.AppUri == appUri);
            if (layouts.Count > 0)
                db.Delete(layouts[0]);
        }
    }
}
