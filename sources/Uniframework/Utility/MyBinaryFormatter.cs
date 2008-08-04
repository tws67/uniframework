using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Uniframework
{
    /// <summary>
    /// �Զ���Ķ���������/�����л���ʽ�����������л��Ķ�������ṩһ��Ĭ�ϵĹ��캯����
    /// </summary>
    public class MyBinaryFormatter
    {
        private readonly List<object> BufferList = new List<object>();

        /// <summary>
        /// ���л�ָ������
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The obj.</param>
        public void Serialize(Stream stream, object obj)
        {
            Guard.ArgumentNotNull(stream, "out stream can't be null.");
            Guard.ArgumentNotNull(obj, "Serialize object can't be null.");

            BufferList.Clear();
            WriteObject(obj, stream);
            BufferList.Clear();
        }

        /// <summary>
        /// �����л�
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public object Deserialize(Stream stream)
        {
            Guard.ArgumentNotNull(stream, "input stream can't be null.");

            stream.Position = 0;
            BufferList.Clear();
            object resultObject = ReadObject(stream);
            BufferList.Clear();
            return resultObject;
        }

        #region Assistant functions

        #region �����л���ص�һЩ����

        private void WriteString(object obj, Stream stream)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(obj.ToString());
            int length = valueBytes.Length;
            WriteValue(length, stream);
            stream.Write(valueBytes, 0, length);
        }

        private void WriteValue(object value, Stream stream)
        {
            if (value is bool) stream.Write(BitConverter.GetBytes((bool)value), 0, 1);
            else if (value.GetType().IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(value.GetType());
                WriteValue(Convert.ChangeType(value, underlyingType,
                    System.Globalization.CultureInfo.CurrentCulture), stream);
            }
            else if (value is char) stream.Write(BitConverter.GetBytes((char)value), 0, Marshal.SizeOf(typeof(char)));
            else if (value is double) stream.Write(BitConverter.GetBytes((double)value), 0, Marshal.SizeOf(typeof(double)));
            else if (value is float) stream.Write(BitConverter.GetBytes((float)value), 0, Marshal.SizeOf(typeof(float)));
            else if (value is int) stream.Write(BitConverter.GetBytes((int)value), 0, Marshal.SizeOf(typeof(int)));
            else if (value is long) stream.Write(BitConverter.GetBytes((long)value), 0, Marshal.SizeOf(typeof(long)));
            else if (value is short) stream.Write(BitConverter.GetBytes((short)value), 0, Marshal.SizeOf(typeof(short)));
            else if (value is uint) stream.Write(BitConverter.GetBytes((uint)value), 0, Marshal.SizeOf(typeof(uint)));
            else if (value is ulong) stream.Write(BitConverter.GetBytes((ulong)value), 0, Marshal.SizeOf(typeof(ulong)));
            else if (value is ushort) stream.Write(BitConverter.GetBytes((ushort)value), 0, Marshal.SizeOf(typeof(ushort)));
        }

        ///�������ͼ�ȡ������ڲ��ֶΣ�д����
        private void WriteFields(object obj, Type valueType, Stream stream)
        {
            FieldInfo[] fieldInfos = valueType.GetFields(BindingFlags.NonPublic | BindingFlags.Public
                                                         | BindingFlags.Static | BindingFlags.Instance |
                                                         BindingFlags.DeclaredOnly);
            {
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.DeclaringType == valueType && !fieldInfo.IsLiteral)
                        WriteObject(fieldInfo.GetValue(obj), stream);
                }
            }
        }

        /// <summary>
        /// �����д�뷽ʽ
        /// ��һ����־λ�������Ƿ���ֵ
        /// �ڶ�����־λ����������������ַ���
        /// ��������־λ�������Ƿ�Ϊһ������
        /// ���ĸ���־λ���ο���������־λ�������������־λΪ�棬����ĸ���־Ϊ�����б��е�����
        /// �������־λ�������ʵ������
        /// �����ַ�������û�� 3/4 ��־λ 
        /// </summary>
        private void WriteObject(object obj, Stream stream)
        {
            /// ��һ����־λ�������Ƿ���ֵ
            WriteValue(obj != null, stream);
            if (obj == null) return;

            /// �ڶ�����־λ����������������ַ���
            Type valueType = obj.GetType();
            WriteString(string.Format("{0},{1}", valueType.FullName, valueType.Assembly.FullName), stream);
            //if (!valueType.Assembly.GlobalAssemblyCache)
            //    WriteString(string.Format("{0},{1}", valueType.FullName, valueType.Assembly.FullName), stream);
            //else WriteString(string.Format("{0}", valueType.FullName), stream);

            ///�������Ϊֵ���ͣ���ֱ����ֵ�ķ�ʽд����(�Ӵ��ж���Ϊ�˴�����������е�Ԫ��)
            if (valueType.IsValueType && !valueType.IsGenericType)
            {
                WriteValue(obj, stream);
                return;
            }

            //�ر����ַ����Ķ���
            if (valueType == typeof(string))
            {
                WriteString(obj, stream);
                return;
            }


            int index = BufferList.IndexOf(obj);
            /// ��������־λ�������Ƿ�Ϊһ������
            WriteValue(index >= 0, stream);

            if (index >= 0)
            {
                /// ���ĸ���־λ���ο���������־λ�������������־λΪ�棬����ĸ���־Ϊ�����б��е�����
                WriteValue(index, stream);
                return;
            }

            //�������ΪType���ͣ���д��Type��������Ϣ
            if (valueType.BaseType == typeof(Type))
            {
                WriteObject(string.Format("{0},{1}", ((Type)obj).FullName, ((Type)obj).Assembly.FullName), stream);
                BufferList.Add(obj);
                return;
            }

            if (valueType.IsArray)
            {
                Array arrayValue = (Array)obj;
                ///���������һ�����ݣ���д������ĳ���
                WriteValue(arrayValue.Length, stream);

                ///��ȡ����Ԫ�ص�����
                Type elementType = valueType.GetElementType();
                for (int i = 0; i < arrayValue.Length; i++)
                {
                    ///���������Ԫ�������ж�д�뷽ʽ
                    if (elementType.IsValueType && !elementType.IsGenericType)
                        WriteValue(arrayValue.GetValue(i), stream);
                    else WriteObject(arrayValue.GetValue(i), stream);
                }
                return;
            }

            BufferList.Add(obj);
            WriteFields(obj, valueType, stream);

            ///�ݹ鴦���ϲ���
            while (valueType.BaseType != typeof(object) && valueType.BaseType != null)
            {
                valueType = valueType.BaseType;
                WriteFields(obj, valueType, stream);
            }
        }

        #endregion

        #region �뷴���л���ص�һЩ����

        private string ReadString(Stream stream)
        {
            int length = (int)ReadValue(stream, typeof(int));
            if (length == 0) return string.Empty;
            byte[] stringBytes = new byte[length];
            stream.Read(stringBytes, 0, length);
            return Encoding.UTF8.GetString(stringBytes, 0, stringBytes.Length);
        }

        private object ReadValue(Stream stream, Type valueType)
        {

            if (valueType == typeof(bool))
            {
                byte[] boolBytes = new byte[1];
                stream.Read(boolBytes, 0, 1);
                return BitConverter.ToBoolean(boolBytes, 0);
            }

            if (valueType.IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(valueType);
                return ReadValue(stream, underlyingType);
            }

            int valuelength = Marshal.SizeOf(valueType);
            byte[] valueBytes = new byte[valuelength];
            stream.Read(valueBytes, 0, valuelength);
            if (valueType == typeof(char)) return BitConverter.ToChar(valueBytes, 0);
            if (valueType == typeof(double)) return BitConverter.ToDouble(valueBytes, 0);
            if (valueType == typeof(float)) return BitConverter.ToSingle(valueBytes, 0);
            if (valueType == typeof(int)) return BitConverter.ToInt32(valueBytes, 0);
            if (valueType == typeof(long)) return BitConverter.ToInt64(valueBytes, 0);
            if (valueType == typeof(short)) return BitConverter.ToInt16(valueBytes, 0);
            if (valueType == typeof(uint)) return BitConverter.ToUInt32(valueBytes, 0);
            if (valueType == typeof(ulong)) return BitConverter.ToUInt64(valueBytes, 0);
            if (valueType == typeof(ushort)) return BitConverter.ToUInt16(valueBytes, 0);
            return null;
        }

        ///�������ͼ�ȡ������ڲ��ֶΣ������ж�ȡֵ
        private void ReadFields(object obj, Type valueType, Stream stream)
        {
            FieldInfo[] fieldInfos = valueType.GetFields(BindingFlags.NonPublic | BindingFlags.Public
                                                         | BindingFlags.Static | BindingFlags.Instance |
                                                         BindingFlags.DeclaredOnly);
            {
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.DeclaringType == valueType && !fieldInfo.IsLiteral)
                        fieldInfo.SetValue(obj, ReadObject(stream));
                }
            }
        }

        private object ReadObject(Stream stream)
        {
            //��ȡ��һ����־λ����ѯ�����Ƿ���ֵ
            bool isNotNull = (bool)ReadValue(stream, typeof(bool));
            if (!isNotNull) return null;

            //��ȡ�ڶ�����־λ����ö��������������
            string typeString = ReadString(stream);
            Type valueType = Type.GetType(typeString);
            
//#if DEBUG
//            using (TextWriter tw = new StreamWriter(@"E:\Ufdebug\" + typeString + ".txt"))
//            {
//                tw.WriteLine("Stream length is : " + stream.Length.ToString());
//                tw.WriteLine("object type : " + typeString);
//            }
//#endif

            //���Ϊֵ���ͣ�ֱ�ӷ��ض���
            if (valueType.IsValueType && !valueType.IsGenericType)
                return ReadValue(stream, valueType);

            //�ر����ַ�������
            if (valueType == typeof(string))
                return ReadString(stream);

            //��ȡ��������־λ����ѯ�����Ƿ�Ϊһ������
            bool isRef = (bool)ReadValue(stream, typeof(bool));
            if (isRef)
            {
                //��ȡ���ĸ���־λ����ѯ���õĶ���
                int index = (int)ReadValue(stream, typeof(int));
                return BufferList[index];
            }

            //�������ΪType���ͣ�������������Ϣ���й���
            if (valueType.BaseType == typeof(Type))
            {
                string typestring = ReadObject(stream).ToString();
                object value = Type.GetType(typestring);
                BufferList.Add(value);
                return value;
            }

            if (valueType.IsArray)
            {
                //��ȡ����ĳ���
                int length = (int)ReadValue(stream, typeof(int));

                ///��ȡ����Ԫ�ص�����
                Type elementType = valueType.GetElementType();
                Array arrayValue = Array.CreateInstance(elementType, length);
                for (int i = 0; i < length; i++)
                {
                    object elementValue = null;
                    if (elementType.IsValueType && !elementType.IsGenericType)
                        elementValue = ReadValue(stream, elementType);
                    else elementValue = ReadObject(stream);
                    arrayValue.SetValue(elementValue, i);
                }
                return arrayValue;
            }

            object result = Activator.CreateInstance(valueType);
            BufferList.Add(result);

            ReadFields(result, valueType, stream);
            while (valueType.BaseType != typeof(object) && valueType.BaseType != null)
            {
                valueType = valueType.BaseType;
                ReadFields(result, valueType, stream);
            }
            return result;
        }

        #endregion

        #endregion
    }
}
