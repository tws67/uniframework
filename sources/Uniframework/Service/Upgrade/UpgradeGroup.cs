using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 系统更新的组
    /// </summary>
    [Serializable]
    public class UpgradeGroup
    {
        private string name;
        private string target;
        private List<UpgradeItem> items = new List<UpgradeItem>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="target">更新的目标文件夹</param>
        public UpgradeGroup(string name, string target)
        {
            this.name = name;
            this.target = target;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// 更新的目标位置
        /// </summary>
        public string Target
        {
            get { return target; }
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        public IList<UpgradeItem> Items
        {
            get { return items; }
        }
        
        /// <summary>
        /// 大小
        /// </summary>
        public long SubTotalSize
        {
            get { 
                long size = 0;
                foreach (UpgradeItem item in items)
                    size += item.Size;
                return size;
            }
        }
    }
}
