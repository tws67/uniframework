using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Uniframework.Services
{
    /// <summary>
    /// ϵͳע������ӿ�
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// �ж�·���Ƿ����
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
        bool Exists(string path);

        /// <summary>
        /// �ж�ָ��·���е�ĳ���ڵ��Ƿ����
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="item">�ڵ�����</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
        bool Exists(string path, string item);

        /// <summary>
        /// �ж�ָ��·���е��Ƿ�����ӽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����true�����򷵻�false</returns>
        bool HasChildren(string path);

        /// <summary>
        /// ��ȡָ��·���Ľڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
        XmlNode GetItem(string path);

        /// <summary>
        /// ��ȡָ��·����ָ���ڵ����ƵĽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="item">��������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
        XmlNode GetItem(string path, KeyValuePair<string, string> item);

        /// <summary>
        /// ��ȡָ��·���¶�Ӧ�����Ľڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="index">������</param>
        /// <returns>������ڵĻ�����ָ��·���µĽڵ㣬���򷵻�null</returns>
        XmlNode GetItem(string path, int index);

        /// <summary>
        /// ��ȡָ��·���½ڵ��ֵ
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���ڵ��µ�ֵ�����򷵻ؿ��ַ���</returns>
        string GetItemValue(string path);

        /// <summary>
        /// ��ȡָ��·���ڵ������ֵ
        /// </summary>
        /// <param name="path">·������</param>
        /// <param name="attribute">��������</param>
        /// <returns>������ڵĻ�����ָ�����Ե�ֵ�����򷵻ؿ��ַ���</returns>
        string GetItemAttribute(string path, string attribute);

        /// <summary>
        /// ��ȡָ��·���µ������ӽڵ�
        /// </summary>
        /// <param name="path">·������</param>
        /// <returns>������ڵĻ�����ָ��·���µ����нڵ��б����򷵻�null</returns>
        XmlNodeList GetChildren(string path);

        /// <summary>
        /// ��ѯָ�����ƵĽڵ�·��
        /// </summary>
        /// <param name="item">�ڵ�����</param>
        /// <returns>����ҵ��ڵ㷵�ؽڵ��·�������򷵻�null</returns>
        string SearchNode(string item);
    }
}
