using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Microsoft.Practices.CompositeUI;

namespace Uniframework.SmartClient
{
    /// <summary>
    /// 系统插件树，用于管理系统的插件对象
    /// </summary>
    [Service]
    public class AddInTree
    {
        private IList<AddIn> addIns = new List<AddIn>();
        private AddInNode rootNode = new AddInNode();
        private IDictionary<string, IConditionEvaluator> conditionEvaluators = new Dictionary<string, IConditionEvaluator>();
        private object syncObj = new object();
        private WorkItem workItem;

        public AddInTree()
        { 
        }

        /// <summary>
        /// 安装到系统中的插件
        /// </summary>
        public IList<AddIn> AddIns
        {
            get { return addIns; }
        }

        /// <summary>
        /// 插件根节点
        /// </summary>
        public AddInNode RootNode
        {
            get { return rootNode; }
        }

        /// <summary>
        /// 根工作项，组件容器
        /// </summary>
        [ServiceDependency]
        public WorkItem WorkItem
        {
            get { return workItem; }
            set { workItem = value; }
        }

        /// <summary>
        /// 条件计算器
        /// </summary>
        public IDictionary<string, IConditionEvaluator> ConditionEvaluators
        {
            get { return conditionEvaluators; }
        }

        /// <summary>
        /// 增加新的插件
        /// </summary>
        /// <param name="addIn">插件</param>
        public void InsertAddIn(AddIn addIn)
        {
            if (addIn.Enabled)
            {
                foreach (ExtensionPath path in addIn.Paths.Values)
                {
                    AddExtensionPath(path);
                }
            }
            addIns.Add(addIn);
        }
        
        /// <summary>
        /// 缷载插件
        /// </summary>
        /// <param name="addIn">插件</param>
        public void RemoveAddIn(AddIn addIn)
        {
            if (addIn.Enabled)
                throw new AddInException("不能缷载正在使用的插件。");
            addIns.Remove(addIn);
        }

        /// <summary>
        /// 使用插件不可用
        /// </summary>
        /// <param name="addIn">插件</param>
        public void DisableAddIn(AddIn addIn)
        {
            addIn.Enabled = false;
        }

        /// <summary>
        /// 检查是否存在指定路径的插件节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>如果存在返回true，否则返回false</returns>
        public bool Exists(string path)
        {
            if (path == null || path.Length == 0)
            {
                return true;
            }

            string[] splittedPath = path.Split('/');
            AddInNode curPath = rootNode;
            int i = 0;
            while (i < splittedPath.Length)
            {
                // curPath = curPath.ChildNodes[splittedPath[i]] - check if child path exists
                if (!curPath.ChildNodes.TryGetValue(splittedPath[i], out curPath))
                {
                    return false;
                }
                ++i;
            }
            return true;
        }

        /// <summary>
        /// 查找指定路径的插件节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="throwOnNotFound">未找到是否触发异常</param>
        /// <returns>返回找到的插件节点</returns>
        public AddInNode GetChild(string path, bool throwOnNotFound)
        {
            if (path == null || path.Length == 0)
            {
                return rootNode;
            }
            string[] splittedPath = path.Split('/');
            AddInNode curPath = rootNode;
            int i = 0;
            while (i < splittedPath.Length)
            {
                if (!curPath.ChildNodes.TryGetValue(splittedPath[i], out curPath))
                {
                    if (throwOnNotFound)
                        throw new AddInException(String.Format("不存在路径为 \"{0}\" 的插件节点。", path));
                    else
                        return null;
                }
                ++i;
            }
            return curPath;
        }
        
        /// <summary>
        /// 查找指定路径的插件节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>返回找到的插件节点</returns>
        public AddInNode GetChild(string path)
        {
            return GetChild(path, true);
        }

        /// <summary>
        /// 创建指定路径下的插件节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="caller">调用者</param>
        /// <returns>返回创建的插件节点</returns>
        public object BuildItem(object caller, string path)
        {
            int pos = path.LastIndexOf('/');
            string parent = path.Substring(0, pos);
            string child = path.Substring(pos + 1);
            AddInNode node = GetChild(parent);
            return node.BuildChildItem(child, caller, BuildItems(caller, path, false));
        }

        /// <summary>
        /// 创建指定路径下的插件节点列表
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="caller">调用者</param>
        /// <param name="throwOnNotFound">未找到是否触发异常</param>
        /// <returns>返回创建的插件节点列表</returns>
        public ArrayList BuildItems(object caller, string path, bool throwOnNotFound)
        {
            AddInNode node = GetChild(path, throwOnNotFound);
            if (node == null)
                return new ArrayList();
            else
                return node.BuildChildItems(caller);
        }

        /// <summary>
        /// 创建指定路径下的插件节点
        /// </summary>
        /// <typeparam name="T">泛型类型</typeparam>
        /// <param name="path">路径</param>
        /// <param name="caller">调用者</param>
        /// <returns>返回创建的插件节点列表</returns>
        public List<T> BuildItems<T>(object caller, string path)
        {
            return BuildItems<T>(caller, path, true);
        }

        /// <summary>
        /// 创建指定路径下的插件节点
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="caller">调用者</param>
        /// <param name="throwOnNotFound">如果为true，当未找到指定路径的节点时将抛出异常</param>
        public List<T> BuildItems<T>(object caller, string path, bool throwOnNotFound)
        {
            AddInNode node = GetChild(path, throwOnNotFound);
            if (node == null)
                return new List<T>();
            else
                return node.BuildChildItems<T>(caller);
        }

        #region Assistant functions

        /// <summary>
        /// 以指定节点为根路径创建插件节点
        /// </summary>
        /// <param name="localRoot">根节点</param>
        /// <param name="path">路径</param>
        /// <returns>创建好的插件节点</returns>
        private AddInNode CreatePath(AddInNode localRoot, string path)
        {
            if (path == null || path.Length == 0)
            {
                return localRoot;
            }

            string[] splittedPath = path.Split('/');
            AddInNode curPath = localRoot;
            int i = 0;
            while (i < splittedPath.Length)
            {
                if (!curPath.ChildNodes.ContainsKey(splittedPath[i]))
                {
                    curPath.ChildNodes[splittedPath[i]] = new AddInNode();
                }
                curPath = curPath.ChildNodes[splittedPath[i]];
                ++i;
            }

            return curPath;
        }

        /// <summary>
        /// 向插件树中加入扩展路径对象
        /// </summary>
        /// <param name="path">扩展路径</param>
        private void AddExtensionPath(ExtensionPath path)
        {
            AddInNode treePath = CreatePath(rootNode, path.Name);
            treePath.Path = path.Name;
            treePath.Label = path.Label;
            foreach (AddInElement element in path.AddInElements)
            {
                treePath.AddInElements.Add(element);
            }
        }

        #endregion

    }
}
