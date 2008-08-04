using System;
using System.Collections.Generic;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class Set<T>
    {
        private List<T> set;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Set()
        {
            set = new List<T>();
        }

        /// <summary>
        /// 构造函数，使用指定元素初始化集合
        /// </summary>
        /// <param name="flag">初始化元素</param>
        public Set(T flag)
            : this()
        {
            set.Add(flag);
        }

        /// <summary>
        /// 构造函数，使用指定集合初始化集合
        /// </summary>
        /// <param name="flags">初始化元素集合</param>
        public Set(T[] flags)
        {
            set.AddRange(flags);
        }

        /// <summary>
        /// 判断集合中是否包含指定元素
        /// </summary>
        /// <param name="flag">元素</param>
        /// <returns>如果集合中包含指定元素返回true，否则返回false。</returns>
        public bool Contains(T flag)
        {
            return set.Contains(flag);
        }

        /// <summary>
        /// 包含元素
        /// </summary>
        /// <param name="flag">元素</param>
        public void Include(T flag)
        {
            if (!set.Contains(flag))
                set.Add(flag);
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="flag">元素</param>
        public void Exclude(T flag)
        {
            if (set.Contains(flag))
                set.Remove(flag);
        }
    }
}
