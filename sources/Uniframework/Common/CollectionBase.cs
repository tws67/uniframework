// ***************************************************************
//  version:  1.0   date: 12/01/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Uniframework
{
    /// <summary>
    /// 为强类型集合提供基类（泛型）
    /// </summary>
    /// <typeparam name="T">需要实现的集合强类型</typeparam>
    public class CollectionBase<T> : CollectionBase
    {
        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get { return (T)List[index]; }
            set { List[index] = value; }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int Add(T value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, T value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(T value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            return List.Contains(value);
        }

    }
}
