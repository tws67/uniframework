using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// ���鹤����
    /// </summary>
    public static class ArrayUtility
    {
        private static int buffer_size = 100;

        /// <summary>�ϲ���������</summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="array1">����1</param>
        /// <param name="array2">����2</param>
        /// <returns>�ϲ��������</returns>
        public static T[] ArrayMerge<T>(T[] array1, T[] array2)
        {
            T[] array = new T[array1.Length + array2.Length];
            array1.CopyTo(array, 0);
            array2.CopyTo(array, array1.Length);
            return array;
        }

        /// <summary>
        /// ������������е��ֽ�����
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
                    break; // ��������������
                }
                destStream.Write(buffer, 0, bytesRead);
            }
            destStream.Flush();
            return destStream.ToArray();
        }

        /// <summary>
        /// ����������ӵ�������ȥ
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="array">����</param>
        /// <param name="item">Ҫ��ӵ�������</param>
        /// <returns>����������</returns>
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
        /// ɾ�������еĶ�Ӧ��
        /// (��֪��ɾ���������˳�����)
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="array">����</param>
        /// <param name="item">Ҫɾ����������</param>
        /// <returns>����������</returns>
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
