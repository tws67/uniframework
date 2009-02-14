using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraNavBar;
using Microsoft.Practices.CompositeUI;
using Uniframework.XtraForms.Workspaces;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 导航栏分组条构建器
    /// </summary>
    public class XtraNavBarGroupBuilder : IBuilder
    {
        #region IBuilder Members

        /// <summary>
        /// 条件处理标志
        /// </summary>
        /// <value></value>
        public bool HandleConditions
        {
            get { return true; }
        }

        public string ClassName
        {
            get { return "XtraNavBarGroup"; }
        }

        /// <summary>
        /// 构建插件单元
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <param name="context">上下文，用于存放在构建时需要的组件</param>
        /// <param name="element">插件单元</param>
        /// <param name="subItems">被构建的子对象列表</param>
        /// <returns>构建好的插件单元</returns>
        public object BuildItem(object caller, WorkItem context, AddInElement element, ArrayList subItems)
        {
            if (element.Configuration.Attributes["label"] == null)
                throw new AddInException(String.Format("没有为类型为 \"{0}\" 的插件单元{1}提供label属性。",
                    element.ClassName, element.Id));
            if(element.Configuration.Attributes["navipane"] == null)
                throw new AddInException(String.Format("没有为类型为 \"{0}\" 的插件单元{1}提供navipane属性。",
                    element.ClassName, element.Id));

            string label = element.Configuration.Attributes["label"];
            string navipane = element.Configuration.Attributes["navipane"];
            NavBarGroup item = new NavBarGroup(BuilderUtility.GetStringRES(context, label));
            item.GroupStyle = NavBarGroupStyle.LargeIconsText;
            NavBarControl naviPane = context.Items.Get<NavBarControl>(navipane);
            if (naviPane == null)
                throw new UniframeworkException("未定义框架外壳的导航栏管理器。");
            naviPane.Groups.Add(item); // 添加分组条到导航栏

            if (element.Configuration.Attributes["tooltip"] != null)
                item.Hint = element.Configuration.Attributes["tooltip"];
            
            // 设置分组栏显示的图像
            if (element.Configuration.Attributes["imagefile"] != null)
            {
                string image = element.Configuration.Attributes["imagefile"];
                item.SmallImage = BuilderUtility.GetBitmap(context, image, 16, 16);
            }
            if (element.Configuration.Attributes["largeimage"] != null)
            {
                string largeImage = element.Configuration.Attributes["largeimage"];
                item.LargeImage = BuilderUtility.GetBitmap(context, largeImage, 32, 32);
            }

            if (element.Configuration.Attributes["register"] != null)
            {
                bool register = bool.Parse(element.Configuration.Attributes["register"]);
                if (register)
                    context.UIExtensionSites.RegisterSite(BuilderUtility.CombinPath(element.Path, element.Id), item);
            }
            return item;
        }

        #endregion
    }
}
