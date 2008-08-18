using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 插件节点
    /// </summary>
    public class AddInNode
    {
        private IDictionary<string, AddInNode> childNodes = new Dictionary<string, AddInNode>();
        private IList<AddInElement> addInElements = new List<AddInElement>();
        private string path = string.Empty;
        private string label = string.Empty;

        /// <summary>
        /// 节点路径
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// 节点标题
        /// </summary>
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary>
        /// 子节点
        /// </summary>
        public IDictionary<string, AddInNode> ChildNodes
        {
            get { return childNodes; }
        }

        /// <summary>
        /// 插件单元
        /// </summary>
        public IList<AddInElement> AddInElements
        {
            get { return addInElements; }
        }

        /// <summary>
        /// 构建子节点
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="caller">调用者</param>
        /// <returns>插件单元列表</returns>
        public List<T> BuildChildItems<T>(object caller)
        {
            List<T> items = new List<T>(addInElements.Count);
            foreach (AddInElement element in addInElements)
            {
                ArrayList subItems = null;
                if (childNodes.ContainsKey(element.Id))
                    subItems = childNodes[element.Id].BuildChildItems(caller);
                object result = element.BuildItem(caller, subItems);
                if (result == null)
                    continue;

                IBuilderModifier bm = result as IBuilderModifier;
                if (bm != null)
                    bm.Apply(items);
                else if (result is T)
                    items.Add((T)result);
                else
                    throw new InvalidCastException("The AddInNode <" + element.Name + " id='" + element.Id
                                                   + "' returned an instance of " + result.GetType().FullName
                                                   + " but the type " + typeof(T).FullName + " is expected.");
            }
            return items;
        }

        /// <summary>
        /// 构建子节点
        /// </summary>
        /// <param name="caller">调用者</param>
        /// <returns>插件单元列表</returns>
        public ArrayList BuildChildItems(object caller)
        {
            ArrayList items = new ArrayList(addInElements.Count);
            foreach (AddInElement element in addInElements)
            {
                // 由于CAB中添加子项目依赖于父路径，因此必须先创建当前元素才能再创建子项
                // 此处乃BUG也！！！
                object result = element.BuildItem(caller, null); 
                ArrayList subItems = null;
                if (childNodes.ContainsKey(element.Id))
                {
                    subItems = childNodes[element.Id].BuildChildItems(caller);
                }

                if (result == null)
                    continue;
                IBuilderModifier bm = result as IBuilderModifier;
                if (bm != null)
                    bm.Apply(items);
                else
                    items.Add(result);
            }
            return items;
        }

        /// <summary>
        /// 构建指定标识的插件单元
        /// </summary>
        /// <param name="childId">待创建的插件单元标识</param>
        /// <param name="caller">调用者</param>
        /// <param name="subItems">子对象列表</param>
        /// <returns>返回创建好的插件单元</returns>
        public object BuildChildItem(string childId, object caller, ArrayList subItems)
        {
            foreach (AddInElement element in addInElements)
            {
                if (element.Id == childId)
                    return element.BuildItem(caller, subItems);
            }
            throw new AddInException(String.Format("不存在路径为 \"{0}\" 的插件单元", childId));
        }
    }
}
