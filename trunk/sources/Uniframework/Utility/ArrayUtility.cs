using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// 数组工具类
    /// </summary>
    public static class ArrayUtility
    {
        private static int buffer_size = 100;

        /// <summary>合并两个数组</summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array1">数组1</param>
        /// <param name="array2">数组2</param>
        /// <returns>合并后的数组</returns>
        public static T[] ArrayMerge<T>(T[] array1, T[] array2)
        {
            T[] array = new T[array1.Length + array2.Length];
            array1.CopyTo(array, 0);
            array2.CopyTo(array, array1.Length);
            return array;
        }

        /// <summary>
        /// 从流里读出所有的字节数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadAllBytesFromStream(Stream stream)
        {
            MemoryStream destStream = new MemoryStream();
            byte[] buffer = new byte[100];
            while (true) {
                int bytesRead = stream.Read(buffer, 0, buffer_size);
                if (bytesRead == 0) {
                    break; // 读完了所有数据
                }
                destStream.Write(buffer, 0, bytesRead);
            }
            destStream.Flush();
            return destStream.ToArray();
        }

        /// <summary>
        /// 将数据项添加到数组中去
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="item">要添加的数据项</param>
        /// <returns>处理后的数组</returns>
        public static T[] AddArrayItem<T>(T[] array, T item)
        {
            List<T> operateList = null;
            if (item != null)
            {
                if (array != null)
                    operateList = new List<T>(array);
                else
                    operateList = new List<T>();
                operateList.Add(item);
                array = operateList.ToArray();
            }
            return array;
        }

        /// <summary>
        /// 删除数组中的对应项
        /// (不知道删除后的数组顺序如何)
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <param name="item">要删除的数据项</param>
        /// <returns>处理后的数组</returns>
        public static T[] RemoveArrayItem<T>(T[] array, T item)
        {
            List<T> operateList;
            if (item != null)
            {
                if (array == null)
                    return null;

                operateList = new List<T>(array);
                operateList.Remove(item);
                return operateList.ToArray();
            }
            return array;
        }
    }
}
