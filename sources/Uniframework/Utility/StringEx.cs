// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Uniframework
{
    /// <summary>
    /// �ַ�����չ��
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// ʹ��ǰ��ָ���������÷ָ������е��ַ���
        /// </summary>
        /// <param name="s">��Ҫ���зָ����ַ���</param>
        /// <param name="frontSeperator">ǰ�ָ���</param>
        /// <param name="backSeperator">��ָ���</param>
        /// <returns>�ָ����</returns>
        public static String[] Split(String s, String frontSeperator, String backSeperator)
        {
            int startIndex = 0;
            ArrayList result = new ArrayList();

            while(true)
            {
                int frontIndex = s.IndexOf(frontSeperator, startIndex);

                if ( (frontIndex + 1 > s.Length) || frontIndex == -1)
                 {
                    break;
                }
                int backIndex = s.IndexOf(backSeperator, frontIndex + 1);
                if(backIndex== -1 )
                {
                    break;
                }

                result.Add(s.Substring(frontIndex + 1, backIndex - frontIndex - 1));
                startIndex = backIndex+1;
            }

            String[] stringArray = new String[result.Count];
            result.CopyTo(stringArray);
            return stringArray;
        }

        /// <summary>
        /// �ָ��ַ���
        /// </summary>
        /// <param name="s">��Ҫ�ָ���ַ���</param>
        /// <param name="seperator">�ָ����</param>
        /// <returns>���طָ�Ľ��(�������ָ��ַ���)��������Ϊ�գ�����0��������</returns>
        public static string[] Split(string s , string seperator)
        {
            List<string> result = new List<string>();
            int startIndex = 0;
            while(true)
            {
                int itemIndex = s.IndexOf(seperator, startIndex);
                if(itemIndex==-1)
                {
                    string last = s.Substring(startIndex);
                    if (!string.IsNullOrEmpty(last))
                        result.Add(last);

                    break;
                }
                string value = s.Substring(startIndex, itemIndex - startIndex);
                if(!string.IsNullOrEmpty(value))
                    result.Add(value);
                startIndex = itemIndex + seperator.Length;
            }

            return result.ToArray();
        }

        /// <summary>
        /// �ָ��ַ���
        /// </summary>
        /// <param name="target">��Ҫ�ָ���ַ���</param>
        /// <param name="seperators">�ָ��������</param>
        /// <param name="length">��ɷָ���ַ�������</param>
        /// <returns>���طָ�Ľ��(�������ָ��ַ���)��������Ϊ�գ�����0��������</returns>
        public static string[] Split(string target, string[] seperators, out int length)
        {
            List<string> result = new List<string>();
            int startIndex = 0; //ÿ�β��ҵ���ʼλ��
            
            while (true)
            {
                int itemIndex =-1;
                int minItemIndex = int.MaxValue;  //����ķָ���ŵ�λ��
                string seperator=null;

                foreach (string item in seperators)
                {
                    itemIndex= target.IndexOf(item, startIndex);
                    if(itemIndex != -1 && itemIndex < minItemIndex)
                    {
                        minItemIndex = itemIndex;
                        seperator = item;
                    }
                }

                if (minItemIndex ==int.MaxValue)
                {
                    //�κεķָ���Ŷ�û���ҵ�
                    break;
                }
                else
                {
                    result.Add(target.Substring(startIndex, minItemIndex - startIndex));
                    startIndex = minItemIndex + seperator.Length;
                }
            }
           
            length = startIndex;
            return result.ToArray();
        }

        /// <summary>
        /// ��16�����ַ���תΪ����
        /// </summary>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public static int CaculateXdigitValue(String valueString)
        {
            int sum =0; 
            for (int i = 0; i < valueString.Length; ++i)
            {
                sum *=16;
                char c = valueString[i];
                c = char.ToUpper(c);

                if(Char.IsNumber(c))
                {
                    sum += c-'0';
                    continue;
                }

                if(c>='A' && c<'G')
                {
                    sum += c - 'A'+10;
                    continue;
                }

                throw new Exception("�Ƿ����ַ�");
            }

            return sum;
        }

        /// <summary>
        /// Trims the end of strings.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="trimStr">The trim STR.</param>
        /// <returns></returns>
        public static string TrimEnd(string target,string trimStr)
        {
            int index = target.LastIndexOf(trimStr);
            if (index != -1)
            {
                return target.Substring(0, index+1);
            }

            return target;
        }
    }
}
