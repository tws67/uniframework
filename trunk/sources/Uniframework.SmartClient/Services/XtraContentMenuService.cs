using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraBars;
using DevExpress.XtraEditors;

using Microsoft.Practices.CompositeUI;
using Microsoft.Practices.ObjectBuilder;

namespace Uniframework.SmartClient
{
    [Service]
    public class XtraContentMenuService : IContentMenuService
    {
        private Dictionary<string, BarSubItem> contents = new Dictionary<string, BarSubItem>();
        private BarManager barManager;
        private WorkItem workItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="XtraContentMenuService"/> class.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        /// <param name="barManager">The bar manager.</param>
        public XtraContentMenuService([ServiceDependency]WorkItem workItem, BarManager barManager)
        {
            this.barManager = barManager;
            this.workItem = workItem;
        }

        #region IContentMenuService Members

        /// <summary>
        /// Registers the content menu.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        public void RegisterContentMenu(string name, object content)
        {
            if (content is BarSubItem)
                contents[name] = content as BarSubItem;
        }

        /// <summary>
        /// Unregister content menu.
        /// </summary>
        /// <param name="name">The name.</param>
        public void UnRegisterContentMenu(string name)
        {
            if (contents.ContainsKey(name))
                contents.Remove(name);
        }

        /// <summary>
        /// 获取指定名称的上下文菜单
        /// </summary>
        /// <param name="name">上下文菜单名称</param>
        /// <returns>返回正确的上下文菜单，如果不存在指定名称的上下文菜单则返回空的<see cref="PopupMenu"/>对象</returns>
        public object GetContentMenu(string name)
        {
            PopupMenu content = new PopupMenu();
            content.Manager = barManager;
            if (contents.ContainsKey(name)) {
                AddInTree addInTree = workItem.Services.Get<AddInTree>();
                if(addInTree != null)
                    try
                    {
                        AddInNode addInNode = addInTree.GetChild(name);
                        if (addInNode != null) {
                            addInNode.BuildChildItems(this);
                        }
                    }
                    catch (Exception) {
                        throw new UniframeworkException(String.Format("无法创建指定路径 \"{0} \" 的插件单元。", name));
                    }
            }

            if(contents.ContainsKey(name)) {
                foreach (BarItemLink link in contents[name].ItemLinks) {
                    content.AddItem(link.Item);
                }
            }
                
            return content;
        }

        #endregion
    }
}
