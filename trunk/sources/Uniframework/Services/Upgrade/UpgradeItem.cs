using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 更新项目
    /// </summary>
    [Serializable]
    public class UpgradeItem
    {
        private string filename;
        private string name;
        private long size;

        public UpgradeItem(string filename)
        {
            this.filename = filename;
            this.name = Path.GetFileName(filename);
            FileInfo fileInfo = new FileInfo(name);
            size = fileInfo.Length;
        }

        public string FileName
        {
            get { return filename; }
        }

        /// <summary>
        /// 名称
        /// </summary>
        public String Name
        {
            get { return name; }
        }

        /// <summary>
        /// 大小
        /// </summary>
        public long Size
        {
            get { return size; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
