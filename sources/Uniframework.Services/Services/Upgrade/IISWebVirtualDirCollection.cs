using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uniframework.Services
{
    /// <summary>
    /// 虚拟目录集合
    /// </summary>
    public class IISWebVirtualDirCollection : CollectionBase
    {
        public IISWebServer Parent = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="IISWebVirtualDirCollection"/> class.
        /// </summary>
        /// <param name="Parent">The parent.</param>
        public IISWebVirtualDirCollection(IISWebServer Parent)
        {
            this.Parent = Parent;
        }

        /// <summary>
        /// Gets the <see cref="Uniframework.Services.IISWebVirtualDir"/> at the specified index.
        /// </summary>
        /// <value></value>
        public IISWebVirtualDir this[int Index]
        {
            get
            {
                return (IISWebVirtualDir)this.List[Index];

            }
        }

        /// <summary>
        /// Gets the <see cref="Uniframework.Services.IISWebVirtualDir"/> with the specified name.
        /// </summary>
        /// <value></value>
        public IISWebVirtualDir this[string Name]
        {
            get
            {
                Name = Name.ToLower();
                IISWebVirtualDir list;
                for (int i = 0; i < this.List.Count; i++)
                {
                    list = (IISWebVirtualDir)this.List[i];
                    if (list.Name.ToLower() == Name)
                        return list;
                }
                return null;
            }
        }


        /// <summary>
        /// Add_s the specified web virtual dir.
        /// </summary>
        /// <param name="WebVirtualDir">The web virtual dir.</param>
        internal void Add_(IISWebVirtualDir WebVirtualDir)
        {
            try
            {
                this.List.Add(WebVirtualDir);
            }
            catch
            {
                throw (new Exception("发生意外错误，可能是某节点将该节点的上级节点作为它自己的子级插入"));
            }
        }

        /// <summary>
        /// Adds the specified web virtual dir.
        /// </summary>
        /// <param name="WebVirtualDir">The web virtual dir.</param>
        public void Add(IISWebVirtualDir WebVirtualDir)
        {
            WebVirtualDir.Parent = this.Parent;
            try
            {
                this.List.Add(WebVirtualDir);
            }
            catch
            {
                throw (new Exception("发生意外错误，可能是某节点将该节点的上级节点作为它自己的子级插入"));
            }
            IISManagement.CreateIISWebVirtualDir(WebVirtualDir, true);

        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="WebVirtualDirs">The web virtual dirs.</param>
        public void AddRange(IISWebVirtualDir[] WebVirtualDirs)
        {
            for (int i = 0; i <= WebVirtualDirs.GetUpperBound(0); i++)
            {
                Add(WebVirtualDirs[i]);
            }
        }

        /// <summary>
        /// Removes the specified web virtual dir.
        /// </summary>
        /// <param name="WebVirtualDir">The web virtual dir.</param>
        public void Remove(IISWebVirtualDir WebVirtualDir)
        {
            for (int i = 0; i < this.List.Count; i++)
            {
                if ((IISWebVirtualDir)this.List[i] == WebVirtualDir)
                {
                    this.List.RemoveAt(i);
                    IISManagement.RemoveIISWebVirtualDir(WebVirtualDir);
                    return;
                }
            }
        }
    }
}
