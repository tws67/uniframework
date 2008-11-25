using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Uniframework.Switch
{
    /// <summary>
    /// 通道集合类，允许用户通过索引或别名访问通道对象
    /// </summary>
    public class ChannelCollection : ReadOnlyCollectionBase
    {
        public ChannelCollection()
        { 
        }

        public ChannelCollection(IChannel[] channels)
        {
            this.AddRange(channels);
        }

        public ChannelCollection(ChannelCollection channels)
        {
            this.AddRange(channels);
        }

        public IChannel this[int index]
        {
            get 
            {
                //foreach (IChannel chnl in InnerList)
                //{
                //    if (chnl.ChannelID == index)
                //        return chnl;
                //}
                //return null;
                return index >= 0 && index < InnerList.Count ? (IChannel)InnerList[index] : null;
            }
            set { InnerList[index] = value; }
        }

        /// <summary>
        /// 通道别名索引器
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns>返回指定别名的通道，如果不存在则返回null</returns>
        public IChannel this[string alias]
        {
            get
            {
                foreach (IChannel chnl in InnerList)
                {
                    if (chnl.ChannelAlias == alias)
                        return chnl;
                }
                return null;
            }
        }

        public IChannel Add(IChannel value)
        {
            InnerList.Add(value);
            return value;
        }

        public void AddRange(IChannel[] values)
        {
            foreach (IChannel chnl in values)
            {
                InnerList.Add(chnl);
            }
        }

        public void AddRange(ChannelCollection values)
        {
            foreach (IChannel chnl in values)
            {
                InnerList.Add(chnl);
            }
        }

        public void CopyTo(IChannel[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public bool Contains(IChannel value)
        {
            return InnerList.Contains(value);
        }

        public void Remove(IChannel value)
        {
            InnerList.Remove(value);
        }
    }
}
